using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InspectionProgram.Common;
using InspectionProgram.Halcon;
using ImageViewerWinForms;

namespace InspectionProgram.GUI
{
    /// <summary>Blob demo 검사(폴더/단일, 자동 사이클, CSV, DGV) — Auto Run / Teaching 공유.</summary>
    public sealed class BlobInspectRunFlow : IDisposable
    {
        private readonly Control _host;
        private readonly string _caption;
        private readonly ImageViewPanelView1 _viewer;
        private readonly UcInspectFlowStrip _strip;
        private readonly TextBox _log;
        private readonly DataGridView _dgv;
        private readonly TabControl _tabCamera;
        private readonly Button _btnLoad;
        private readonly Button _btnClear;
        private readonly Button _btnRoi;
        private readonly bool _alwaysEnableControls;
        /// <summary>
        /// (옵션) 매 이미지 검사 전에 실행할 전처리(예: NCC로 ROI 이동).
        /// null/빈 문자열이면 OK, 메시지가 있으면 그 사유로 이번 이미지는 NG 처리합니다.
        /// </summary>
        private readonly Func<string, string> _beforeInspectPerImage;

        /// <summary>자동 실행 등에서 좌상단 OK/일치율·개수 패널을 갱신합니다. null이면 무시.</summary>
        private readonly Action<string, string, string> _setInspectSummaryLines;

        /// <summary>
        /// «3) Run inspect» 클릭 직전에만 호출됩니다. 티칭 레시피·NCC 모델을 뷰어에 올리려면 true를 반환하세요.
        /// null이면 호출하지 않습니다(티칭 화면 등).
        /// </summary>
        private readonly Func<bool> _prepareTeachingRecipeBeforeRunInspect;

        private List<string> _imageFolderBatchPaths;
        private int _imageFolderBatchIndex;
        private readonly List<BlobInspectRow> _sessionRows = new List<BlobInspectRow>();
        private bool _autoBatchRunning;
        private CancellationTokenSource _autoBatchCts;
        private bool _disposed;
        /// <summary>
        /// 기본 목표: UI 스레드에서 NCC/Blob 처리가 수백 ms 걸릴 때 0.2s/장은 목표 지연 없이 풀가동되어
        /// 화면은 첫/마지막 프레임만 갱신되는 경우가 많습니다. 여유를 두려면 이 값을 처리시간+α보다 크게 둡니다.
        /// </summary>
        private double _cycleSecondsPerImage = 0.9;

        /// <summary>자동 사이클(폴더 전체 배치) 실행 중이면 Teaching/Viewer의 ImageLoaded 미리보기 등을 끌 수 있습니다.</summary>
        public bool IsAutoBatchRunning => _autoBatchRunning;

        /// <summary>
        /// Blob 개수 기대값. 0 이하면 개수 비교를 하지 않습니다.
        /// (기본 0: 개수 비교 비활성 — ROI 내 Blob이 1개 이상이면 OK)
        /// </summary>
        public int ExpectedBlobCount { get; set; } = 0;

        /// <summary>ROI 내부 전경 픽셀 수 하한(0이면 비활성).</summary>
        public int ForegroundPixelMin { get; set; } = 0;

        /// <summary>ROI 내부 전경 픽셀 수 상한(0이면 비활성).</summary>
        public int ForegroundPixelMax { get; set; } = 0;

        /// <summary>티칭 레시피의 NCC 스코어 필터. 둘 다 유효하면 전역 NCC 카운트에 사용합니다.</summary>
        public double NccFilterMinScore { get; set; } = double.NaN;

        public double NccFilterMaxScore { get; set; } = double.NaN;

        /// <summary>레시피에 포함된 NCC 모델 경로(<see cref="NccSharedModelState"/> 없을 때 보조).</summary>
        public string RecipeNccModelPath { get; set; } = string.Empty;

        public double RecipeNccTemplateWidth { get; set; }

        public double RecipeNccTemplateHeight { get; set; }

        public double RecipeNccMinScore { get; set; } = double.NaN;

        private void PumpUiOnceBestEffort()
        {
            try
            {
                if (_host == null || _host.IsDisposed)
                    return;
                if (_host.InvokeRequired)
                {
                    _host.BeginInvoke((MethodInvoker)PumpUiOnceBestEffort);
                    return;
                }

                _viewer?.CanvasControl?.Invalidate();
                _viewer?.CanvasControl?.Refresh();
                Application.DoEvents();
            }
            catch
            {
            }
        }

        private void InvokeInspectSummary(string line1, string line2, string line3)
        {
            if (_setInspectSummaryLines == null)
                return;

            void Apply()
            {
                try
                {
                    _setInspectSummaryLines(line1 ?? string.Empty, line2 ?? string.Empty, line3 ?? string.Empty);
                }
                catch
                {
                }
            }

            try
            {
                if (_host == null || _host.IsDisposed)
                    return;
                if (_host.InvokeRequired)
                    _host.BeginInvoke((MethodInvoker)Apply);
                else
                    Apply();
            }
            catch
            {
            }
        }

        public ImageViewPanelView1 Viewer
        {
            get { return _viewer; }
        }

        /// <summary>
        /// 자동 사이클에서 1장 처리 목표 시간(초). 예: 0.9면 약 1.1장/초. 처리(로드+NCC+검사) 시간보다 작으면 대기 없이 연속 처리됩니다.
        /// 0 이하면 속도 제한 없음.
        /// </summary>
        public double CycleSecondsPerImage
        {
            get { return _cycleSecondsPerImage; }
            set { _cycleSecondsPerImage = value; }
        }

        /// <summary>자동 사이클 목표 속도(장/초). 예: 5면 초당 5장. 0 이하면 제한 없음.</summary>
        public double CycleImagesPerSecond
        {
            get
            {
                if (_cycleSecondsPerImage <= 0.0)
                    return 0.0;
                return 1.0 / _cycleSecondsPerImage;
            }
            set
            {
                if (value <= 0.0)
                {
                    _cycleSecondsPerImage = 0.0;
                    return;
                }
                _cycleSecondsPerImage = 1.0 / value;
            }
        }

        public BlobInspectRunFlow(
            Control host,
            string messageBoxCaption,
            ImageViewPanelView1 viewer,
            UcInspectFlowStrip strip,
            TextBox inspectionLog,
            DataGridView dgvCount,
            TabControl tabCamera,
            Button btnToolLoad,
            Button btnToolClear,
            Button btnToolRoi,
            Func<string, string> beforeInspectPerImage = null,
            bool alwaysEnableControls = false,
            Action<string, string, string> setInspectSummaryLines = null,
            Func<bool> prepareTeachingRecipeBeforeRunInspect = null)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _caption = messageBoxCaption ?? "Inspect";
            _viewer = viewer;
            _strip = strip;
            _log = inspectionLog;
            _dgv = dgvCount;
            _tabCamera = tabCamera;
            _btnLoad = btnToolLoad;
            _btnClear = btnToolClear;
            _btnRoi = btnToolRoi;
            _beforeInspectPerImage = beforeInspectPerImage;
            _alwaysEnableControls = alwaysEnableControls;
            _setInspectSummaryLines = setInspectSummaryLines;
            _prepareTeachingRecipeBeforeRunInspect = prepareTeachingRecipeBeforeRunInspect;
            if (_strip != null)
            {
                _strip.BtnRunInspection.Click += OnInspectFlowButtonClick;
                _strip.BtnStopBatch.Click += OnInspectFlowButtonClick;
                _strip.BtnSaveCsv.Click += OnInspectFlowButtonClick;
                _strip.BtnPrevImage.Click += OnInspectFlowButtonClick;
                _strip.BtnNextImage.Click += OnInspectFlowButtonClick;
            }
        }

        public void ApplyFlowControlStates()
        {
            try
            {
                bool on = _viewer != null && _viewer.HasImage;
                bool batch = _autoBatchRunning;
                bool canSave = _sessionRows.Count > 0;
                if (_strip == null)
                    return;

                if (_alwaysEnableControls)
                {
                    if (_strip.TrkThreshold != null) _strip.TrkThreshold.Enabled = true;
                    if (_strip.ChkAutoCycle != null) _strip.ChkAutoCycle.Enabled = true;
                    if (_strip.BtnRunInspection != null) _strip.BtnRunInspection.Enabled = true;
                    if (_strip.BtnStopBatch != null) _strip.BtnStopBatch.Enabled = true;
                    if (_strip.BtnSaveCsv != null) _strip.BtnSaveCsv.Enabled = true;
                    if (_strip.BtnPrevImage != null) _strip.BtnPrevImage.Enabled = true;
                    if (_strip.BtnNextImage != null) _strip.BtnNextImage.Enabled = true;
                    if (_btnLoad != null) _btnLoad.Enabled = true;
                    if (_btnClear != null) _btnClear.Enabled = true;
                    if (_btnRoi != null) _btnRoi.Enabled = true;
                    return;
                }

                if (_strip.TrkThreshold != null)
                {
                    _strip.TrkThreshold.Enabled = on && !batch;
                }

                if (_strip.ChkAutoCycle != null)
                {
                    _strip.ChkAutoCycle.Enabled = !batch;
                }

                if (_strip.BtnRunInspection != null)
                {
                    _strip.BtnRunInspection.Enabled = on && !batch;
                }

                if (_strip.BtnStopBatch != null)
                {
                    _strip.BtnStopBatch.Enabled = batch;
                }

                if (_strip.BtnSaveCsv != null)
                {
                    _strip.BtnSaveCsv.Enabled = canSave;
                }

                if (_strip.BtnPrevImage != null)
                {
                    _strip.BtnPrevImage.Enabled = on && !batch && _imageFolderBatchPaths != null && _imageFolderBatchPaths.Count > 1;
                }

                if (_strip.BtnNextImage != null)
                {
                    _strip.BtnNextImage.Enabled = on && !batch && _imageFolderBatchPaths != null && _imageFolderBatchPaths.Count > 1;
                }

                if (_btnLoad != null)
                {
                    _btnLoad.Enabled = !batch;
                }

                if (_btnClear != null)
                {
                    _btnClear.Enabled = !batch;
                }

                if (_btnRoi != null)
                {
                    _btnRoi.Enabled = on && !batch;
                }
            }
            catch
            {
            }
        }

        public void OnImageLoaded()
        {
            ApplyFlowControlStates();
        }

        public void CancelAutoBatchIfAny()
        {
            try
            {
                _autoBatchCts?.Cancel();
            }
            catch
            {
            }
        }

        public void ClearSessionAndFolder()
        {
            ClearImageFolderBatch();
            _sessionRows.Clear();
        }

        public void DefaultThresholdPreview()
        {
            if (_strip?.TrkThreshold == null)
                return;
            int v = _strip.TrkThreshold.Value;
            if (_strip.LblThresholdValue != null)
            {
                _strip.LblThresholdValue.Text = v.ToString(CultureInfo.InvariantCulture);
            }

            if (_viewer?.CanvasControl != null && _viewer.HasImage)
                _viewer.CanvasControl.ShowBlob_Bright(true, v, 255);
        }

        /// <summary>
        /// 메인 툴바 «개수(Count)» 등 외부에서 <c>3) Run inspect</c> 버튼과 동일한 검사를 실행합니다.
        /// 자동 사이클이 꺼져 있으면 1회, 켜져 있으면 목록 전체를 순차 검사합니다.
        /// Auto Run에서는 «3) Run» 클릭 시 티칭 레시피 적용 훅이 먼저 호출됩니다(이 메서드에서는 호출하지 않음).
        /// 티칭에서 저장한 NCC 모델이 있으면 호스트에서 넘긴 전처리로 ROI를 맞춘 뒤 Blob 카운트합니다.
        /// </summary>
        public void ExecuteSameAsRunInspectButton()
        {
            if (_host == null || _host.IsDisposed)
                return;

            async void RunAsync()
            {
                try
                {
                    await RunInspectionOrBatchAsync().ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    try
                    {
                        AppLogger.Write("EX", "ExecuteSameAsRunInspectButton: " + ex);
                        MessageBox.Show(ex.Message, _caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch
                    {
                    }
                }
            }

            if (_host.InvokeRequired)
                _host.BeginInvoke((MethodInvoker)(() => RunAsync()));
            else
                RunAsync();
        }

        public bool TryNavigateImageFolderBatch(int delta)
        {
            if (_imageFolderBatchPaths == null || _imageFolderBatchPaths.Count <= 1)
                return false;
            if (_viewer == null)
                return false;
            int next = _imageFolderBatchIndex + delta;
            if (next < 0 || next >= _imageFolderBatchPaths.Count)
                return false;
            string path = _imageFolderBatchPaths[next];
            if (string.IsNullOrEmpty(path) || File.Exists(path) == false)
                return false;
            try
            {
                _viewer.LoadImage(path);
                ClearOverlaysBestEffort();
                _imageFolderBatchIndex = next;
                _viewer.ZoomFit();
                AppExceptionHandler.ExecuteBestEffort("BlobInspectRunFlow.FolderNav", () =>
                {
                    ApplyFlowControlStates();
                    DefaultThresholdPreview();
                    TryRunBeforeInspectHookForCurrentImage();
                });
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void LoadImageOrFolderFromUserChoice()
        {
            DialogResult r = MessageBox.Show(
                "이미지 한 장을 열까요, 폴더 전체(목록)를 열까요?\n\n[예] 폴더\n[아니오] 이미지 파일\n[취소] 취소",
                "1) Load",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);
            if (r == DialogResult.Cancel)
                return;
            if (r == DialogResult.Yes)
            {
                if (TryLoadAllImagesFromFolder() == false)
                    return;
            }
            else
            {
                _viewer.LoadImageFromDialog();
                if (_viewer.HasImage == false)
                    return;
                _sessionRows.Clear();
                RefreshImageFolderBatchFromPath(_viewer.CanvasControl.ImagePath);
                _viewer.ZoomFit();
            }

            AppExceptionHandler.ExecuteBestEffort("BlobInspectRunFlow.AfterLoad", () =>
            {
                ApplyFlowControlStates();
                DefaultThresholdPreview();
            });
        }

        private async void OnInspectFlowButtonClick(object sender, EventArgs e)
        {
            var b = sender as Button;
            string id = b != null ? (b.Tag as string ?? string.Empty) : string.Empty;
            if (id == "PREV")
            {
                TryNavigateImageFolderBatch(-1);
                return;
            }

            if (id == "NEXT")
            {
                TryNavigateImageFolderBatch(1);
                return;
            }
            if (id == "STOP")
            {
                _autoBatchCts?.Cancel();
                return;
            }

            if (id == "RUN")
            {
                if (_prepareTeachingRecipeBeforeRunInspect != null)
                {
                    try
                    {
                        if (_prepareTeachingRecipeBeforeRunInspect() == false)
                            return;
                    }
                    catch (Exception prepEx)
                    {
                        try
                        {
                            AppLogger.Write("EX", "RunInspection prepare: " + prepEx);
                            MessageBox.Show(prepEx.Message, _caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch
                        {
                        }

                        return;
                    }
                }

                try
                {
                    await RunInspectionOrBatchAsync().ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    try
                    {
                        AppLogger.Write("EX", "RunInspection: " + ex);
                        MessageBox.Show(ex.Message, _caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch
                    {
                    }
                }

                return;
            }

            AppExceptionHandler.ExecuteBestEffort("BlobInspectRunFlow.InspectionFlow", () =>
            {
                if (id == "CSV")
                {
                    SaveSessionCsvToFileWithDialog();
                }
            });
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            try
            {
                _autoBatchCts?.Cancel();
            }
            catch
            {
            }

            if (_strip != null)
            {
                _strip.BtnRunInspection.Click -= OnInspectFlowButtonClick;
                _strip.BtnStopBatch.Click -= OnInspectFlowButtonClick;
                _strip.BtnSaveCsv.Click -= OnInspectFlowButtonClick;
                _strip.BtnPrevImage.Click -= OnInspectFlowButtonClick;
                _strip.BtnNextImage.Click -= OnInspectFlowButtonClick;
            }
        }

        private void ClearImageFolderBatch()
        {
            _imageFolderBatchPaths = null;
            _imageFolderBatchIndex = 0;
        }

        private static bool IsSupportedImageExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return false;
            switch (ext.ToLowerInvariant())
            {
                case ".bmp":
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".tif":
                case ".tiff":
                    return true;
                default:
                    return false;
            }
        }

        private void RefreshImageFolderBatchFromPath(string currentFilePath)
        {
            ClearImageFolderBatch();
            if (string.IsNullOrWhiteSpace(currentFilePath) || File.Exists(currentFilePath) == false)
                return;

            string dir = Path.GetDirectoryName(currentFilePath);
            if (string.IsNullOrEmpty(dir) || Directory.Exists(dir) == false)
                return;

            try
            {
                string currentFull = Path.GetFullPath(currentFilePath);
                var list = new List<string>();
                foreach (string f in Directory.GetFiles(dir))
                {
                    if (IsSupportedImageExtension(Path.GetExtension(f)))
                        list.Add(Path.GetFullPath(f));
                }

                if (list.Count == 0)
                    return;
                list.Sort(StringComparer.OrdinalIgnoreCase);
                _imageFolderBatchPaths = list;
                int at = _imageFolderBatchPaths.FindIndex(p =>
                    string.Equals(p, currentFull, StringComparison.OrdinalIgnoreCase));
                _imageFolderBatchIndex = at >= 0 ? at : 0;
            }
            catch
            {
                ClearImageFolderBatch();
            }
        }

        private bool TryLoadAllImagesFromFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "검사에 사용할 이미지가 있는 폴더";
                fbd.ShowNewFolderButton = false;
                Form owner = _host.FindForm();
                if (fbd.ShowDialog(owner) != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return false;

                var list = new List<string>();
                try
                {
                    foreach (string f in Directory.GetFiles(fbd.SelectedPath))
                    {
                        if (IsSupportedImageExtension(Path.GetExtension(f)))
                            list.Add(Path.GetFullPath(f));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("폴더를 읽을 수 없습니다: " + ex.Message, _caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (list.Count == 0)
                {
                    MessageBox.Show("이 폴더에 지원하는 이미지가 없습니다.", _caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                list.Sort(StringComparer.OrdinalIgnoreCase);
                ClearImageFolderBatch();
                _sessionRows.Clear();
                _imageFolderBatchPaths = list;
                _imageFolderBatchIndex = 0;
                _viewer.LoadImage(_imageFolderBatchPaths[0]);
                _viewer.ZoomFit();
            }

            return true;
        }

        private async Task RunInspectionOrBatchAsync()
        {
            if (_viewer?.CanvasControl == null || !_viewer.HasImage)
            {
                MessageBox.Show("먼저 Load로 이미지를 불러오세요.", _caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_autoBatchRunning)
                return;

            if (_strip == null || _strip.ChkAutoCycle == null || _strip.ChkAutoCycle.Checked == false)
            {
                RunSingleInspectionSyncAndRecord();
                return;
            }

            if (_imageFolderBatchPaths == null || _imageFolderBatchPaths.Count == 0)
            {
                MessageBox.Show("이미지 목록이 없습니다. (파일/폴더 Load로 이미지를 열어 주세요.)", _caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await RunAutoCyclePassAllAsync().ConfigureAwait(true);
        }

        private async Task RunAutoCyclePassAllAsync()
        {
            var paths = _imageFolderBatchPaths;
            if (paths == null || paths.Count == 0)
                return;

            if (_autoBatchCts != null)
            {
                _autoBatchCts.Dispose();
                _autoBatchCts = null;
            }

            _autoBatchCts = new CancellationTokenSource();
            _autoBatchRunning = true;
            ApplyFlowControlStates();
            try
            {
                AppendLine("자동 사이클 시작: " + paths.Count + "장");
                var swBatch = Stopwatch.StartNew();
                int processed = 0;
                for (int i = 0; i < paths.Count; i++)
                {
                    if (_autoBatchCts != null && _autoBatchCts.IsCancellationRequested)
                    {
                        AppendLine("자동 사이클 취소됨");
                        break;
                    }

                    var swOne = Stopwatch.StartNew();
                    await LoadImageAndWaitAsync(paths[i], 2500).ConfigureAwait(true);
                    ClearOverlaysBestEffort();
                    _imageFolderBatchIndex = i;
                    _viewer.ZoomFit();
                    if (_strip?.TrkThreshold != null)
                    {
                        DefaultThresholdPreview();
                    }
                    PumpUiOnceBestEffort();

                    RunSingleInspectionSyncAndRecord();
                    // UI가 매 장마다 갱신되도록 한 번 양보
                    await Task.Yield();
                    PumpUiOnceBestEffort();
                    swOne.Stop();
                    processed++;

                    // 목표 속도(초/장)를 맞추기 위한 대기
                    try
                    {
                        if (_cycleSecondsPerImage > 0.0)
                        {
                            int targetMs = (int)Math.Round(_cycleSecondsPerImage * 1000.0);
                            // 화면에 "매 장" 보이게 최소 페인트 여유 확보(처리시간과 무관하게 너무 촘촘하면 첫/마지막만 보이기 쉬움)
                            int visibleMinMs = 250;
                            if (targetMs < visibleMinMs)
                                targetMs = visibleMinMs;
                            int remain = targetMs - (int)swOne.ElapsedMilliseconds;
                            if (remain > 0)
                            {
                                CancellationToken token = _autoBatchCts != null ? _autoBatchCts.Token : CancellationToken.None;
                                await Task.Delay(remain, token).ConfigureAwait(true);
                            }
                        }
                    }
                    catch
                    {
                    }
                    PumpUiOnceBestEffort();

                    // 너무 스팸하지 않게 10장마다 속도 로그
                    if (processed % 10 == 0)
                    {
                        double sec = Math.Max(0.001, swBatch.Elapsed.TotalSeconds);
                        double ips = processed / sec;
                        AppendLine(string.Format(CultureInfo.InvariantCulture, "속도: {0:0.0} 장/초 (avg {1:0} ms/장)", ips, 1000.0 / ips));
                    }
                }

                swBatch.Stop();
                if (processed > 0)
                {
                    double sec = Math.Max(0.001, swBatch.Elapsed.TotalSeconds);
                    double ips = processed / sec;
                    AppendLine(string.Format(CultureInfo.InvariantCulture, "자동 사이클 완료: {0}장, {1:0.00}s, {2:0.0} 장/초 (avg {3:0} ms/장)", processed, sec, ips, 1000.0 / ips));
                }
            }
            finally
            {
                _autoBatchRunning = false;
                if (_autoBatchCts != null)
                {
                    _autoBatchCts.Dispose();
                    _autoBatchCts = null;
                }

                ApplyFlowControlStates();
            }
        }

        private void RunSingleInspectionSyncAndRecord()
        {
            if (_viewer?.CanvasControl == null || !_viewer.HasImage)
                return;

            Size sz = _viewer.CanvasControl.ImagePixelSize;
            if (sz.Width <= 0 || sz.Height <= 0)
                return;

            if (TryFullImageNccCountInspect())
                return;

            _viewer.CanvasControl.ShowUserRoiOverlay = true;

            // (옵션) NCC 등으로 ROI를 먼저 업데이트
            string beforeNgNote = null;
            try
            {
                if (_beforeInspectPerImage != null)
                {
                    string ip = _viewer.CanvasControl.ImagePath ?? string.Empty;
                    beforeNgNote = _beforeInspectPerImage(ip);
                }
            }
            catch (Exception ex)
            {
                beforeNgNote = ex.Message;
            }

            _viewer.CanvasControl.ClearDrawOverlays();
            int th = _strip != null && _strip.TrkThreshold != null ? _strip.TrkThreshold.Value : 128;
            int roiCount = _viewer.CanvasControl.RoiItemCount;
            string fileName = Path.GetFileName(_viewer.CanvasControl.ImagePath ?? "unknown");

            bool hasPixelRange = ForegroundPixelMin > 0 || ForegroundPixelMax > 0;
            if (hasPixelRange)
            {
                int fgCount;
                string fgErr;
                if (_viewer.CanvasControl.TryCountForegroundPixelsInSelectedRectRoiUsingMask(th, 255, out fgCount, out fgErr))
                {
                    string minText = ForegroundPixelMin > 0 ? ForegroundPixelMin.ToString(CultureInfo.InvariantCulture) : "-";
                    string maxText = ForegroundPixelMax > 0 ? ForegroundPixelMax.ToString(CultureInfo.InvariantCulture) : "-";
                    AppendLine(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "ROI 전경 픽셀(mask): count={0}, range=[{1},{2}]",
                            fgCount,
                            minText,
                            maxText));

                    bool below = ForegroundPixelMin > 0 && fgCount < ForegroundPixelMin;
                    bool above = ForegroundPixelMax > 0 && fgCount > ForegroundPixelMax;
                    if (below || above)
                    {
                        string reason = string.Format(
                            CultureInfo.InvariantCulture,
                            "ROI 전경 픽셀 수 범위 벗어남: count={0}, range=[{1},{2}]",
                            fgCount,
                            minText,
                            maxText);
                        RecordForcedNg(fileName, th, roiCount, reason);
                        return;
                    }
                }
                else
                {
                    AppendLine("ROI 전경 픽셀(mask) 계산 실패: " + fgErr);
                }
            }

            if (!string.IsNullOrWhiteSpace(beforeNgNote))
            {
                RecordForcedNg(Path.GetFileName(_viewer.CanvasControl.ImagePath ?? "unknown"), th, roiCount, "NCC " + beforeNgNote);
                return;
            }

            // 티칭과 동일: 밝기 구간을 맞춘 뒤 ROI 안에서 Blob 검출 → 핑크 사각형 오버레이
            _viewer.CanvasControl.ShowBlob_Bright(true, th, 255);

            _viewer.CanvasControl.FindBlobObjectsInSelectedRoi();
            int blobCount = _viewer.CanvasControl.BlobResultCount;
            int minAreaEff = _viewer.CanvasControl.BlobAreaMinEffective;
            bool isNg;
            string note;
            EvaluateBlobNg(roiCount, blobCount, ExpectedBlobCount, out isNg, out note);
            DateTime at = DateTime.Now;
            string tIso = at.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            _sessionRows.Add(new BlobInspectRow
            {
                TimeLocalIso = tIso,
                FileName = fileName,
                Result = isNg ? "NG" : "OK",
                RoiCount = roiCount,
                BlobCount = blobCount,
                Th = th,
                Note = note
            });

            string verdictForTeachingStyleLog = isNg ? (string.IsNullOrEmpty(note) ? "NG" : note) : "OK";
            AppendLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "볼 검출(Blob): threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}",
                    th,
                    minAreaEff,
                    blobCount,
                    ExpectedBlobCount,
                    verdictForTeachingStyleLog));

            if (isNg)
                AppendNgFocusedLine(fileName, at, roiCount, blobCount, th, note);

            try
            {
                InvokeInspectSummary(
                    isNg ? "NG" : "OK",
                    "평균=—",
                    string.Format(CultureInfo.InvariantCulture, "개수(n)={0}", blobCount));
            }
            catch
            {
            }

            PumpUiOnceBestEffort();

            if (isNg)
            {
                UpdateDgvCountForSelectedCamera(1, 0, 1);
            }
            else
            {
                UpdateDgvCountForSelectedCamera(1, 1, 0);
            }

            AppLogger.Write(
                "INSPECT",
                string.Format(CultureInfo.InvariantCulture, "Run {0} ROI={1} Blob={2} th={3} {4}", fileName, roiCount, blobCount, th, isNg ? "NG" : "OK"));
        }

        /// <summary>
        /// 티칭에서 공유된 NCC 모델이 있으면 이미지 전체에서 동일 패턴을 찾아 오버레이·로그합니다(Blob ROI 내 카운트와 배타적).
        /// </summary>
        private bool TryResolveNccModel(out string model, out double tW, out double tH, out double sharedMin)
        {
            if (NccSharedModelState.TryGet(
                    out model,
                    out tW,
                    out tH,
                    out sharedMin,
                    out _,
                    out _,
                    out _,
                    out _))
                return true;

            if (string.IsNullOrWhiteSpace(RecipeNccModelPath) || File.Exists(RecipeNccModelPath) == false)
                return false;

            model = RecipeNccModelPath;
            tW = RecipeNccTemplateWidth;
            tH = RecipeNccTemplateHeight;
            sharedMin = (!double.IsNaN(RecipeNccMinScore) && RecipeNccMinScore >= 0.0 && RecipeNccMinScore <= 1.0)
                ? RecipeNccMinScore
                : 0.75;
            return true;
        }

        private bool TryFullImageNccCountInspect()
        {
            if (!TryResolveNccModel(out string model, out double tW, out double tH, out double sharedMin))
                return false;

            string tempCleanup = null;
            try
            {
                string pathForHalcon;
                string logicalFileName;
                string diskPath = _viewer.CanvasControl.ImagePath ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(diskPath) && File.Exists(diskPath))
                {
                    pathForHalcon = diskPath;
                    logicalFileName = Path.GetFileName(diskPath);
                }
                else
                {
                    tempCleanup = Path.Combine(Path.GetTempPath(), "VisionInspectEx_ncc_src_" + Guid.NewGuid().ToString("N") + ".png");
                    if (!_viewer.CanvasControl.TrySaveSourceImageToFile(tempCleanup))
                        return false;
                    pathForHalcon = tempCleanup;
                    logicalFileName = "live_frame.png";
                }

                _viewer.CanvasControl.ShowUserRoiOverlay = false;

                double sMin;
                double sMax;
                if (!double.IsNaN(NccFilterMinScore) && !double.IsNaN(NccFilterMaxScore))
                {
                    sMin = NccFilterMinScore;
                    sMax = NccFilterMaxScore;
                }
                else
                {
                    sMin = sharedMin;
                    sMax = 1.0;
                    const double judgeMin = 0.80;
                    if (sMin < judgeMin)
                        sMin = judgeMin;
                }

                if (sMin > sMax)
                {
                    double t = sMin;
                    sMin = sMax;
                    sMax = t;
                }

                double findFloor = sMin;
                if (findFloor > 0.45)
                    findFloor = 0.45;
                if (findFloor < 0.3)
                    findFloor = 0.3;

                HalconResult raw;
                try
                {
                    raw = NccPatternInspector.MatchFromModelFile(pathForHalcon, model, findFloor, tW, tH, findFloor);
                }
                catch (Exception ex)
                {
                    int th = _strip != null && _strip.TrkThreshold != null ? _strip.TrkThreshold.Value : 128;
                    int roiCount = _viewer.CanvasControl.RoiItemCount;
                    RecordForcedNg(logicalFileName, th, roiCount, "NCC " + ex.Message);
                    PumpUiOnceBestEffort();
                    return true;
                }

                int rawCount = raw != null ? raw.Count : 0;
                HalconResult filtered = null;
                if (raw != null)
                {
                    try
                    {
                        filtered = raw.FilterByScoreRange(sMin, sMax);
                    }
                    catch
                    {
                    }
                }

                int th2 = _strip != null && _strip.TrkThreshold != null ? _strip.TrkThreshold.Value : 128;
                if (filtered != null && filtered.Count > 0 && (ForegroundPixelMin > 0 || ForegroundPixelMax > 0))
                {
                    var keep = new List<int>();
                    for (int i = 0; i < filtered.Count; i++)
                    {
                        try
                        {
                            RectangleF boxF = NccPatternInspector.GetMatchImageRectFromCenteredTemplate(
                                filtered.Row[i].D,
                                filtered.Column[i].D,
                                filtered.TemplateWidth > 0.1 ? filtered.TemplateWidth : 1.0,
                                filtered.TemplateHeight > 0.1 ? filtered.TemplateHeight : 1.0);
                            Rectangle box = Rectangle.Ceiling(boxF);
                            int fg;
                            string fgErr;
                            if (_viewer.CanvasControl.TryCountForegroundPixelsInRectUsingMask(box, th2, 255, out fg, out fgErr))
                            {
                                bool below = ForegroundPixelMin > 0 && fg < ForegroundPixelMin;
                                bool above = ForegroundPixelMax > 0 && fg > ForegroundPixelMax;
                                if (!below && !above)
                                    keep.Add(i);
                            }
                            else
                            {
                                keep.Add(i);
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (keep.Count != filtered.Count)
                    {
                        int before = filtered.Count;
                        filtered = filtered.FilterByIndices(keep);
                        AppendLine(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "NCC 픽셀 필터: kept={0}/{1}, range=[{2},{3}]",
                                filtered.Count,
                                before,
                                ForegroundPixelMin > 0 ? ForegroundPixelMin.ToString(CultureInfo.InvariantCulture) : "-",
                                ForegroundPixelMax > 0 ? ForegroundPixelMax.ToString(CultureInfo.InvariantCulture) : "-"));
                    }
                }

                int matchCount = filtered != null ? filtered.Count : 0;
                int roiCount2 = _viewer.CanvasControl.RoiItemCount;
                string fileName = logicalFileName;

                bool isNg;
                string note;
                if (matchCount == 0)
                {
                    isNg = true;
                    note = rawCount <= 0
                        ? "NCC 매칭 없음"
                        : "NCC 필터 후 매칭 0 (raw=" + rawCount.ToString(CultureInfo.InvariantCulture) + ")";
                }
                else if (ExpectedBlobCount > 0 && matchCount != ExpectedBlobCount)
                {
                    isNg = true;
                    note = "개수 불일치: expected=" + ExpectedBlobCount.ToString(CultureInfo.InvariantCulture)
                        + ", actual=" + matchCount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    isNg = false;
                    note = string.Empty;
                }

                DateTime at = DateTime.Now;
                string tIso = at.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                _sessionRows.Add(new BlobInspectRow
                {
                    TimeLocalIso = tIso,
                    FileName = fileName,
                    Result = isNg ? "NG" : "OK",
                    RoiCount = roiCount2,
                    BlobCount = matchCount,
                    Th = th2,
                    Note = note
                });

                _viewer.CanvasControl.ClearDrawOverlays();
                Color boxColor = Color.Cyan;
                _viewer.CanvasControl.BeginOverlayUpdate();
                try
                {
                    if (filtered != null && filtered.Count > 0)
                    {
                        for (int i = 0; i < filtered.Count; i++)
                        {
                            try
                            {
                                double row = filtered.Row[i].D;
                                double col = filtered.Column[i].D;
                                double scI = 0.0;
                                if (filtered.Score != null && filtered.Score.TupleLength() > i)
                                    scI = filtered.Score[i].D;
                                if (filtered.TemplateWidth > 0.1 && filtered.TemplateHeight > 0.1)
                                {
                                    RectangleF box = NccPatternInspector.GetMatchImageRectFromCenteredTemplate(
                                        row,
                                        col,
                                        filtered.TemplateWidth,
                                        filtered.TemplateHeight);
                                    _viewer.CanvasControl.DrawRectOverlay(box, boxColor, 2.5f, false);
                                    DrawNccScoreCaption(_viewer.CanvasControl, box, col, row, scI, boxColor, true);
                                }
                                else
                                {
                                    _viewer.CanvasControl.DrawCrossLineOverlay(new PointF((float)col, (float)row), 32f, boxColor, 2f);
                                    DrawNccScoreCaption(_viewer.CanvasControl, RectangleF.Empty, col, row, scI, boxColor, false);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    string matchLine = "일치율 = —";
                    if (TryComputeNccScoreAverage(filtered, out double avgDraw))
                        matchLine = string.Format(CultureInfo.InvariantCulture, "일치율 = {0:F2}%", avgDraw * 100.0);

                    InvokeInspectSummary(
                        isNg ? "NG" : "OK",
                        matchLine,
                        string.Format(CultureInfo.InvariantCulture, "개수 = {0}", matchCount));
                }
                finally
                {
                    _viewer.CanvasControl.EndOverlayUpdate();
                }

                string logDetail = "filtered=" + matchCount + ", raw=" + rawCount
                    + string.Format(CultureInfo.InvariantCulture, ", score[{0:F2},{1:F2}]", sMin, sMax);
                if (TryComputeNccScoreAverage(filtered, out double avgLog))
                    logDetail += "  avgScore=" + avgLog.ToString("F3", CultureInfo.InvariantCulture);

                AppendLine("NCC 카운트  " + logDetail);

                if (isNg)
                    AppendNgFocusedLine(fileName, at, roiCount2, matchCount, th2, note);

                PumpUiOnceBestEffort();

                if (isNg)
                    UpdateDgvCountForSelectedCamera(1, 0, 1);
                else
                    UpdateDgvCountForSelectedCamera(1, 1, 0);

                AppLogger.Write(
                    "INSPECT",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "NCC Count {0} match={1} raw={2} {3}",
                        fileName,
                        matchCount,
                        rawCount,
                        isNg ? "NG" : "OK"));

                return true;
            }
            finally
            {
                if (tempCleanup != null)
                {
                    try
                    {
                        if (File.Exists(tempCleanup))
                            File.Delete(tempCleanup);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void DrawNccScoreCaption(
            ImageCanvasControl canvas,
            RectangleF boxOrEmpty,
            double col,
            double row,
            double score,
            Color color,
            bool hasBox)
        {
            if (canvas == null)
                return;
            string t = score.ToString("F3", CultureInfo.InvariantCulture);
            if (hasBox && boxOrEmpty.Width > 0.5f && boxOrEmpty.Height > 0.5f)
            {
                float cx = boxOrEmpty.X + boxOrEmpty.Width * 0.5f;
                float cy = boxOrEmpty.Bottom + 12f;
                canvas.DrawString(new PointF(cx, cy), t, color, 9);
            }
            else
            {
                canvas.DrawString(new PointF((float)col, (float)row + 22f), t, color, 9);
            }
        }

        private static bool TryComputeNccScoreAverage(HalconResult r, out double average)
        {
            average = 0.0;
            if (r == null || r.Count <= 0 || r.Score == null)
                return false;
            if (r.Score.TupleLength() < r.Count)
                return false;
            double sum = 0.0;
            for (int i = 0; i < r.Count; i++)
                sum += r.Score[i].D;
            average = sum / r.Count;
            return true;
        }

        private void RecordForcedNg(string fileName, int th, int roiCount, string note)
        {
            InvokeInspectSummary("NG", "일치율 = —", "");

            DateTime at = DateTime.Now;
            string tIso = at.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            _sessionRows.Add(new BlobInspectRow
            {
                TimeLocalIso = tIso,
                FileName = fileName,
                Result = "NG",
                RoiCount = roiCount,
                BlobCount = 0,
                Th = th,
                Note = note ?? string.Empty
            });

            AppendNgFocusedLine(fileName, at, roiCount, 0, th, note ?? string.Empty);
            UpdateDgvCountForSelectedCamera(1, 0, 1);
            try
            {
                AppLogger.Write("INSPECT", "Run " + fileName + " " + (note ?? "NG"));
            }
            catch
            {
            }
        }

        private void ClearOverlaysBestEffort()
        {
            try
            {
                if (_viewer?.CanvasControl != null)
                {
                    _viewer.CanvasControl.ClearDrawOverlays();
                }
            }
            catch
            {
            }
        }

        private async Task LoadImageAndWaitAsync(string path, int timeoutMs)
        {
            if (_viewer?.CanvasControl == null)
            {
                _viewer.LoadImage(path);
                return;
            }

            // ImageLoaded가 LoadImage() 내부에서 동기적으로 발생할 수 있어 핸들러를 먼저 달아둡니다.
            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = null;
            handler = (_, __) =>
            {
                try { _viewer.CanvasControl.ImageLoaded -= handler; } catch { }
                tcs.TrySetResult(true);
            };

            _viewer.CanvasControl.ImageLoaded += handler;
            try
            {
                _viewer.LoadImage(path);
                await Task.WhenAny(tcs.Task, Task.Delay(Math.Max(50, timeoutMs))).ConfigureAwait(true);
                // paint/message pump 한번 확보
                await Task.Yield();
            }
            finally
            {
                try { _viewer.CanvasControl.ImageLoaded -= handler; } catch { }
            }
        }

        private void TryRunBeforeInspectHookForCurrentImage()
        {
            try
            {
                if (_beforeInspectPerImage == null || _viewer?.CanvasControl == null || !_viewer.HasImage)
                    return;
                string ip = _viewer.CanvasControl.ImagePath ?? string.Empty;
                _beforeInspectPerImage(ip);
            }
            catch
            {
            }
        }

        private static void EvaluateBlobNg(int roiCount, int blobCount, int expectedBlobCount, out bool isNg, out string note)
        {
            if (roiCount == 0)
            {
                isNg = true;
                note = "ROI 없음";
                return;
            }

            if (blobCount == 0)
            {
                isNg = true;
                note = "ROI 내 Blob 0";
                return;
            }

            if (expectedBlobCount > 0 && blobCount != expectedBlobCount)
            {
                isNg = true;
                note = "Blob 개수 불일치: expected=" + expectedBlobCount.ToString(CultureInfo.InvariantCulture)
                    + ", actual=" + blobCount.ToString(CultureInfo.InvariantCulture);
                return;
            }

            isNg = false;
            note = string.Empty;
        }

        private void AppendNgFocusedLine(string fileName, DateTime at, int roi, int blob, int th, string note)
        {
            string s = string.Format(
                CultureInfo.InvariantCulture,
                "NG  {0:yyyy-MM-dd HH:mm:ss.fff}  {1}  ROI={2}  Blob={3}  th={4}  {5}",
                at,
                fileName,
                roi,
                blob,
                th,
                note);
            if (_log != null)
            {
                _log.AppendText(s + Environment.NewLine);
            }
        }

        private void AppendLine(string line)
        {
            if (_log == null)
                return;
            string ts = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            _log.AppendText(ts + "  " + line + Environment.NewLine);
        }

        private void SaveSessionCsvToFileWithDialog()
        {
            if (_sessionRows == null || _sessionRows.Count == 0)
            {
                MessageBox.Show("저장할 세션 행이 없습니다. (먼저 Run inspect 를 실행하세요.)", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV|*.csv|모든 파일|*.*";
                dlg.Title = "검사 세션 CSV 저장";
                InspectionResultLogPaths.EnsureDirectoryExists();
                dlg.InitialDirectory = InspectionResultLogPaths.GetLogDirectory();
                dlg.FileName = Path.GetFileName(InspectionResultLogPaths.BuildCsvFilePath(DateTime.Now));
                Form owner = _host.FindForm();
                if (dlg.ShowDialog(owner) != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.FileName))
                    return;
                string path = dlg.FileName;
                try
                {
                    var sb = new StringBuilder();
                    sb.Append("Time,FileName,Result,RoiCount,BlobCount,Threshold,Note");
                    sb.AppendLine();
                    for (int i = 0; i < _sessionRows.Count; i++)
                    {
                        var r = _sessionRows[i];
                        sb.AppendLine(
                            string.Join(
                                ",",
                                CsvEscape(r.TimeLocalIso),
                                CsvEscape(r.FileName),
                                CsvEscape(r.Result),
                                r.RoiCount.ToString(CultureInfo.InvariantCulture),
                                r.BlobCount.ToString(CultureInfo.InvariantCulture),
                                r.Th.ToString(CultureInfo.InvariantCulture),
                                CsvEscape(r.Note ?? string.Empty)));
                    }

                    File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CSV를 쓰지 못했습니다: " + ex.Message, "CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("CSV를 저장했습니다.\r\n\r\n" + path, "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    AppLogger.Write("INSPECT", "CSV saved: " + path);
                }
                catch
                {
                }
            }
        }

        private static string CsvEscape(string s)
        {
            if (s == null)
                return string.Empty;
            if (s.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void UpdateDgvCountForSelectedCamera(int total, int good, int reject)
        {
            if (_dgv == null || _tabCamera == null)
                return;
            try
            {
                if (_dgv.Rows.Count == 0)
                    return;

                int idx = _tabCamera.SelectedIndex >= 0 ? _tabCamera.SelectedIndex : 0;
                if (idx < 0 || idx >= _dgv.Rows.Count)
                    return;

                if (good + reject != total && total > 0)
                    good = Math.Max(0, total - reject);

                double yield = total > 0 ? (100.0 * good / total) : 0.0;
                _dgv.Rows[idx].Cells[1].Value = total.ToString(CultureInfo.InvariantCulture);
                _dgv.Rows[idx].Cells[2].Value = good.ToString(CultureInfo.InvariantCulture);
                _dgv.Rows[idx].Cells[3].Value = reject.ToString(CultureInfo.InvariantCulture);
                _dgv.Rows[idx].Cells[4].Value = yield.ToString("0.00", CultureInfo.InvariantCulture) + "%";
            }
            catch (Exception ex)
            {
                AppLogger.Write("EX", "UpdateDgvCountForSelectedCamera: " + ex.Message);
            }
        }

        private sealed class BlobInspectRow
        {
            public string TimeLocalIso;
            public string FileName;
            public string Result;
            public int RoiCount;
            public int BlobCount;
            public int Th;
            public string Note;
        }
    }
}
