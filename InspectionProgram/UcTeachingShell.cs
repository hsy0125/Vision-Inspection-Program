using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using InspectionProgram.Common;
using InspectionProgram.Camera;
using InspectionProgram.Halcon;
using ImageViewerWinForms;

namespace InspectionProgram.GUI
{
    /// <summary>Teaching UI shell. 탭 구조만 잡아두고 기능은 나중에 연결합니다.</summary>
    public partial class UcTeachingShell : UserControl
    {
        private readonly ImageViewPanelView1 _viewer;
        private int _teachingBlobJob;
        private IFrameSource _liveSource;
        private CancellationTokenSource _liveCts;
        private volatile bool _liveEnabled;
        private LanguageType _currentLanguage = LanguageType.Kr;

        private Panel _pnlNccTopInfo;
        private Label _lblNccTopLine1;
        private Label _lblNccTopLine2;
        private Label _lblNccTopLine3;
        private string _lastNccModelPath = string.Empty;
        private double _lastNccTemplateW;
        private double _lastNccTemplateH;
        private BlobInspectRunFlow _blobFlow;
        private string _nccTrackModelPath = string.Empty;
        private TabControl _rightTabControl;
        private TabPage _tabPattern;
        private TabPage _tabCount;
        private FlowLayoutPanel _flpPattern;
        private FlowLayoutPanel _flpCount;

        public UcTeachingShell()
        {
            InitializeComponent();
            _viewer = new ImageViewPanelView1();
            _viewer.Dock = DockStyle.Fill;
            InitializeNccTopInfoPanel();
            pnlViewerHost.Controls.Add(_viewer);
            Disposed += (_, __) => AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.Disposed", OnTeachingShellDisposed);

            UpdateNccCountTrackLabels();

            _blobFlow = new BlobInspectRunFlow(
                this,
                "Teaching",
                _viewer,
                ucInspectFlowStrip1,
                txtTeachingLog,
                null,
                null,
                btnToolLoad,
                btnToolClear,
                btnAddRoiRect,
                TryTrackRoiByNccForImage,
                alwaysEnableControls: true);
            _blobFlow.ExpectedBlobCount = numExpected != null ? (int)numExpected.Value : 0;
            _blobFlow.ForegroundPixelMin = numFgPixelMin != null ? (int)numFgPixelMin.Value : 0;
            _blobFlow.ForegroundPixelMax = numFgPixelMax != null ? (int)numFgPixelMax.Value : 0;
            if (ucInspectFlowStrip1 != null && ucInspectFlowStrip1.TrkThreshold != null)
            {
                ucInspectFlowStrip1.TrkThreshold.ValueChanged += (_, __) =>
                    AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.Thresh", UcInspectFlowStripThresholdChanged);
            }

            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.BlobOverlayStyle", () =>
            {
                _viewer.CanvasControl.SetBlobResultOverlayColors(Color.LimeGreen, Color.FromArgb(85, 0, 220, 0));
            });

            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.WireImageLoaded", () =>
            {
                _viewer.CanvasControl.ImageLoaded += (_, __) =>
                    AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.BallDetectOnImageLoad", () =>
                    {
                        if (_liveEnabled)
                            return;
                        if (_blobFlow != null)
                        {
                            _blobFlow.ApplyFlowControlStates();
                            if (ucInspectFlowStrip1 != null && ucInspectFlowStrip1.LblThresholdValue != null
                                && ucInspectFlowStrip1.TrkThreshold != null)
                            {
                                ucInspectFlowStrip1.LblThresholdValue.Text = ucInspectFlowStrip1.TrkThreshold.Value
                                    .ToString(CultureInfo.InvariantCulture);
                            }
                        }

                        // ROI가 있을 때만 Blob 미리보기(없으면 전체 경로는 즉시 실패하지만, 불필요한 작업·로그·화면 갱신을 막기 위함)
                        // 자동 사이클/배치 중에는 ImageLoaded 기반 Blob 미리보기를 끄고(중복·ROI 없음 로그 감소), Run inspect 경로만 사용
                        if ((_blobFlow == null || !_blobFlow.IsAutoBatchRunning)
                            && _viewer.CanvasControl.RoiItemCount > 0)
                            RefreshTeachingBlobOverlays(true);
                    });
            });

            WireViewerToolbarStateRefresh();

            ApplyTheme();
            InitializeCamerasForDesign();
            if (_blobFlow != null)
            {
                _blobFlow.ApplyFlowControlStates();
            }

            ApplyLanguage(_currentLanguage);
        }

        private string L(string key) => LocalizationService.GetText(key, _currentLanguage);

        public void ApplyLanguage(LanguageType language)
        {
            try
            {
                _currentLanguage = language;

                if (grpViewer != null) grpViewer.Text = L("Teaching");
                if (grpTeachingLog != null) grpTeachingLog.Text = L("Teaching") + " " + L("Log");

                if (btnToolLoad != null) btnToolLoad.Text = L("OpenImage");
                if (btnToolSave != null) btnToolSave.Text = L("SaveImage");
                if (btnToolClear != null) btnToolClear.Text = L("ClearImage");
                if (btnToolZm != null) btnToolZm.Text = L("ZoomMode");
                if (btnToolZIn != null) btnToolZIn.Text = L("ZoomIn");
                if (btnToolZOut != null) btnToolZOut.Text = L("ZoomOut");
                if (btnToolFit != null) btnToolFit.Text = L("FitImage");
                if (btnToolOvClr != null) btnToolOvClr.Text = L("OverlayClear");
                if (btnToolCross != null) btnToolCross.Text = L("Cross");
                if (btnToolGray != null) btnToolGray.Text = L("Gray");
                if (btnToolAvg != null) btnToolAvg.Text = L("Average");
                if (btnToolSync != null) btnToolSync.Text = L("SyncShort");
                if (btnToolMap != null) btnToolMap.Text = L("MapShort");
                if (btnAddRoiRect != null) btnAddRoiRect.Text = L("RoiAdd");
                if (btnDrawString != null) btnDrawString.Text = L("RoiSave");

                if (ucInspectFlowStrip1 != null) ucInspectFlowStrip1.ApplyLanguage(language);

                if (lblNccHeader != null) lblNccHeader.Text = "패턴 매칭 (NCC)";
                if (btnNccSaveModel != null) btnNccSaveModel.Text = "모델 저장";
                if (btnNccRunInspect != null) btnNccRunInspect.Text = "패턴 검사";
                if (btnNccCount != null) btnNccCount.Text = "카운트";
                if (btnSaveInspectionRecipe != null) btnSaveInspectionRecipe.Text = "검사 레시피 저장";

                if (lblNccMinTitle != null) lblNccMinTitle.Text = L("NccMinScore");
                if (lblNccCountMinTitle != null) lblNccCountMinTitle.Text = L("NccCountMin");
                if (lblNccCountMaxTitle != null) lblNccCountMaxTitle.Text = L("NccCountMax");
                if (lblNccCountJudgeMinTitle != null) lblNccCountJudgeMinTitle.Text = L("NccCountJudgeMin");

                if (lblMinAreaTitle != null) lblMinAreaTitle.Text = L("MinArea");
                if (lblExpectedTitle != null) lblExpectedTitle.Text = L("Expected");
                if (lblFgPixelRangeTitle != null) lblFgPixelRangeTitle.Text = "ROI 전경 픽셀 범위";
                if (_tabPattern != null) _tabPattern.Text = "패턴";
                if (_tabCount != null) _tabCount.Text = "개수";
            }
            catch
            {
            }
        }

        private FlowLayoutPanel CreateRightStackPanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10, 10, 10, 10),
                Margin = new Padding(0),
            };
        }

        private void BuildRightTabs()
        {
            try
            {
                if (pnlTeachingRightHost == null || flpTeachingRightStack == null)
                    return;

                _rightTabControl = new TabControl
                {
                    Dock = DockStyle.Fill,
                };
                _tabPattern = new TabPage("패턴");
                _tabCount = new TabPage("개수");
                _flpPattern = CreateRightStackPanel();
                _flpCount = CreateRightStackPanel();
                _tabPattern.Controls.Add(_flpPattern);
                _tabCount.Controls.Add(_flpCount);
                _rightTabControl.TabPages.Add(_tabPattern);
                _rightTabControl.TabPages.Add(_tabCount);

                pnlTeachingRightHost.Controls.Clear();
                pnlTeachingRightHost.Controls.Add(_rightTabControl);

                // 패턴 탭
                if (flowRowNccTitle != null) _flpPattern.Controls.Add(flowRowNccTitle);
                if (lblNccResult != null) _flpPattern.Controls.Add(lblNccResult);
                if (flowRowNccBtn != null) _flpPattern.Controls.Add(flowRowNccBtn);
                if (flowRowNccMin != null) _flpPattern.Controls.Add(flowRowNccMin);
                if (flowRowNccCountBtn != null) _flpPattern.Controls.Add(flowRowNccCountBtn);
                if (flowRowNccCMin != null) _flpPattern.Controls.Add(flowRowNccCMin);
                if (flowRowNccCMax != null) _flpPattern.Controls.Add(flowRowNccCMax);
                if (flowRowNccCountJudgeMin != null) _flpPattern.Controls.Add(flowRowNccCountJudgeMin);

                // 개수 탭
                if (flowRowRecipeSave != null) _flpCount.Controls.Add(flowRowRecipeSave);
                if (flowRowBlobCount != null) _flpCount.Controls.Add(flowRowBlobCount);
                if (flowRowMinArea != null) _flpCount.Controls.Add(flowRowMinArea);
                if (flowRowExpected != null) _flpCount.Controls.Add(flowRowExpected);
                if (flowRowFgPixelRange != null) _flpCount.Controls.Add(flowRowFgPixelRange);
            }
            catch
            {
            }
        }

        private bool IsDesignTime()
        {
            try
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
            }
            catch
            {
            }

            try
            {
                if (Site != null && Site.DesignMode)
                    return true;
            }
            catch
            {
            }

            return false;
        }

        private static Font CreateUiFont(float size)
        {
            try
            {
                return new Font("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Point);
            }
            catch
            {
                return SystemFonts.DefaultFont;
            }
        }

        private void InitializeNccTopInfoPanel()
        {
            try
            {
                if (pnlViewerHost == null)
                    return;

                bool design = IsDesignTime();
                Color back = design ? Color.FromArgb(40, 40, 40) : AppColors.SurfaceDark;
                Color fore = Color.LimeGreen;

                _pnlNccTopInfo = new Panel
                {
                    Dock = DockStyle.None,
                    Size = new Size(260, 96),
                    Padding = new Padding(10, 6, 10, 6),
                    BackColor = back,
                };

                var tlp = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    BackColor = back,
                };
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));

                _lblNccTopLine1 = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = fore,
                    BackColor = back,
                    Font = design ? CreateUiFont(19f) : AppFontHelper.Create(19f),
                    Text = string.Empty,
                };
                _lblNccTopLine2 = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = fore,
                    BackColor = back,
                    Font = design ? CreateUiFont(16f) : AppFontHelper.Create(16f),
                    Text = string.Empty,
                };
                _lblNccTopLine3 = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = fore,
                    BackColor = back,
                    Font = design ? CreateUiFont(16f) : AppFontHelper.Create(16f),
                    Text = string.Empty,
                };

                tlp.Controls.Add(_lblNccTopLine1, 0, 0);
                tlp.Controls.Add(_lblNccTopLine2, 0, 1);
                tlp.Controls.Add(_lblNccTopLine3, 0, 2);
                _pnlNccTopInfo.Controls.Add(tlp);

                // 뷰어 레이아웃을 방해하지 않도록 "떠 있는" 패널로 추가합니다.
                // 사용자 요청: 좌상단(흰색 박스 영역) 쪽
                _pnlNccTopInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                pnlViewerHost.Controls.Add(_pnlNccTopInfo);
                _pnlNccTopInfo.BringToFront();

                pnlViewerHost.Resize -= PnlViewerHost_Resize;
                pnlViewerHost.Resize += PnlViewerHost_Resize;
                UpdateNccTopInfoPanelLayout();

                // 디자이너에서도 형태가 보이도록 더미 텍스트
                if (design)
                {
                    SetNccTopInfo(L("JudgeOK"), "Match=95.00%", "개수(n)=1234", Color.LimeGreen);
                }
            }
            catch
            {
            }
        }

        private void PnlViewerHost_Resize(object sender, EventArgs e)
        {
            UpdateNccTopInfoPanelLayout();
        }

        private void UpdateNccTopInfoPanelLayout()
        {
            try
            {
                if (pnlViewerHost == null || _pnlNccTopInfo == null)
                    return;

                // 사용자 요청: 좌상단 흰색 박스 영역으로 이동
                const int marginLeft = 10;
                const int marginTop = 10;
                int x = marginLeft;
                int y = marginTop;
                _pnlNccTopInfo.Location = new Point(x, y);
                _pnlNccTopInfo.BringToFront();
            }
            catch
            {
            }
        }

        private void SetNccTopInfo(string line1, string line2, string line3, Color color)
        {
            try
            {
                if (_lblNccTopLine1 == null || _lblNccTopLine2 == null || _lblNccTopLine3 == null)
                    return;

                // 요청: 결과 표시 색상은 초록색 고정
                Color c = Color.LimeGreen;
                _lblNccTopLine1.ForeColor = c;
                _lblNccTopLine2.ForeColor = c;
                _lblNccTopLine3.ForeColor = c;

                _lblNccTopLine1.Text = line1 ?? string.Empty;
                _lblNccTopLine2.Text = line2 ?? string.Empty;
                _lblNccTopLine3.Text = line3 ?? string.Empty;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 현재 이미지에서 필터된 매치들의 NCC 상관(0~1) 기반 일치율(백분율)을 좌상단 패널용 문자열로 변환합니다.
        /// (매치가 여러 개면 해당 장에서의 점수 평균, 한 개면 그 매치의 일치율과 동일합니다.)
        /// </summary>
        private static string FormatNccMatchPercentLine(double correlation01)
        {
            return string.Format(CultureInfo.InvariantCulture, "Match={0:F2}%", correlation01 * 100.0);
        }

        /// <summary>우측 NCC 결과 라벨 한 줄 — 해당 검사 이미지 기준 일치율입니다.</summary>
        private static string FormatNccMatchPercentResultLabel(double correlation01)
        {
            return string.Format(CultureInfo.InvariantCulture, "Match (일치율): {0:F2}%", correlation01 * 100.0);
        }

        private void ClearTeachingNccSummaryUi()
        {
            try
            {
                SetNccTopInfo(string.Empty, string.Empty, string.Empty, Color.LimeGreen);
                if (lblNccResult != null)
                {
                    lblNccResult.Text = string.Empty;
                    lblNccResult.ForeColor = AppColors.Foreground;
                }
            }
            catch
            {
            }
        }

        private void OnTeachingShellDisposed()
        {
            try
            {
                StopLiveInternal();
            }
            catch
            {
            }

            try
            {
                if (_blobFlow != null)
                {
                    _blobFlow.Dispose();
                    _blobFlow = null;
                }
            }
            catch
            {
            }
        }

        private string TryTrackRoiByNccForImage(string imagePath)
        {
            try
            {
                if (_viewer?.CanvasControl == null || _viewer.HasImage == false)
                    return "뷰어 없음";
                if (string.IsNullOrWhiteSpace(imagePath) || File.Exists(imagePath) == false)
                    return "이미지 경로 없음";

                string model = !string.IsNullOrWhiteSpace(_nccTrackModelPath) ? _nccTrackModelPath : _lastNccModelPath;
                // 자동 사이클 중 팝업을 띄우지 않기 위해, 모델이 없으면 그냥 추적을 건너뜁니다.
                // 모델은 (1) NCC 모델 저장 또는 (2) NCC 검사에서 선택된 파일을 사용합니다.
                if (string.IsNullOrWhiteSpace(model) || File.Exists(model) == false)
                    return "모델 없음";
                _nccTrackModelPath = model;

                bool sessionTemplates = !string.IsNullOrEmpty(_lastNccModelPath) &&
                    string.Equals(Path.GetFullPath(model), Path.GetFullPath(_lastNccModelPath), StringComparison.OrdinalIgnoreCase);
                double tW = sessionTemplates ? _lastNccTemplateW : 0.0;
                double tH = sessionTemplates ? _lastNccTemplateH : 0.0;

                double jMin = (double)(nudNccMinScore != null ? nudNccMinScore.Value : 0.75M);
                if (jMin < 0.0 || jMin > 1.0)
                    jMin = 0.75;

                HalconResult r = NccPatternInspector.MatchFromModelFile(imagePath, model, 0.5, tW, tH);
                r?.EvaluateJudgment(1, jMin);
                if (r == null || r.Count <= 0)
                    return "결과 없음";

                double row = r.Row[0].D;
                double col = r.Column[0].D;
                double angle = r.Angle[0].D;

                if (NccSharedModelState.TryGet(
                        out string sharedModel,
                        out double _stw,
                        out double _sth,
                        out double _sms,
                        out bool hasRefPose,
                        out double refRow,
                        out double refCol,
                        out double refAngle)
                    && hasRefPose
                    && NccSharedModelState.AlignImagesEnabled
                    && string.Equals(Path.GetFullPath(model), Path.GetFullPath(sharedModel), StringComparison.OrdinalIgnoreCase))
                {
                    Bitmap aligned = NccImageAlignment.AlignImageFileToReferenceBitmap(
                        imagePath,
                        sharedModel,
                        _stw,
                        _sth,
                        row,
                        col,
                        angle,
                        refRow,
                        refCol,
                        refAngle,
                        out Exception alignEx);
                    if (aligned != null && alignEx == null)
                    {
                        _viewer.CanvasControl.SetImage(aligned, imagePath);
                        row = refRow;
                        col = refCol;
                    }
                }

                ROI_Manager roi = _viewer.CanvasControl.SelectedRoi;
                if (roi == null && _viewer.CanvasControl.ROIItems != null && _viewer.CanvasControl.ROIItems.Count > 0)
                    roi = _viewer.CanvasControl.ROIItems[0];

                // ROI가 없으면 하나 만든 뒤, 매칭 중심으로 이동
                if (roi == null)
                {
                    Size sz = _viewer.CanvasControl.ImagePixelSize;
                    int w = (int)Math.Round(tW > 0.1 ? tW : Math.Max(50, sz.Width * 0.25));
                    int h = (int)Math.Round(tH > 0.1 ? tH : Math.Max(50, sz.Height * 0.25));
                    int left = Math.Max(0, (int)Math.Round(col - (w / 2.0)));
                    int top = Math.Max(0, (int)Math.Round(row - (h / 2.0)));
                    var rect = new ROIRectangle(new Rectangle(left, top, w, h), Color.LimeGreen, 2);
                    _viewer.CanvasControl.AddROI(rect);
                    _viewer.CanvasControl.SelectROIByName(rect.Name);
                    roi = rect;
                }

                if (roi is ROIRectangle rr)
                {
                    Rectangle b0 = rr.GetBounds();
                    int w0 = b0.Width;
                    int h0 = b0.Height;
                    int w = (int)Math.Round(tW > 0.1 ? tW : w0);
                    int h = (int)Math.Round(tH > 0.1 ? tH : h0);
                    w = Math.Max(4, w);
                    h = Math.Max(4, h);
                    int left = Math.Max(0, (int)Math.Round(col - (w / 2.0)));
                    int top = Math.Max(0, (int)Math.Round(row - (h / 2.0)));
                    rr.Rect = new Rectangle(left, top, w, h);
                    _viewer.CanvasControl.SelectROIByName(rr.Name);
                }
                else
                {
                    Rectangle b = roi.GetBounds();
                    double cx = b.Left + (b.Width / 2.0);
                    double cy = b.Top + (b.Height / 2.0);
                    int dx = (int)Math.Round(col - cx);
                    int dy = (int)Math.Round(row - cy);
                    if (dx != 0 || dy != 0)
                        roi.Move(dx, dy);
                    _viewer.CanvasControl.SelectROIByName(roi.Name);
                }

                _viewer.CanvasControl.Invalidate();
                return null;
            }
            catch
            {
                return "예외";
            }
        }

        private void btnNccSaveModel_Click(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.NccSave", () => OnNccSaveModelClicked());
        }

        private void btnNccRunInspect_Click(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.NccRun", () => OnNccRunInspectClicked());
        }

        private void btnNccCount_Click(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.NccCount", () => OnNccCountClicked());
        }

        private void btnSaveInspectionRecipe_Click(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.RecipeSave", () => OnSaveInspectionRecipeClicked());
        }

        private void btnBlobCount_Click(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.BlobCount", () =>
            {
                if (_viewer == null || _viewer.CanvasControl == null || !_viewer.HasImage)
                {
                    MessageBox.Show("먼저 이미지를 불러와 주세요.", "개수 카운트", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _viewer.CanvasControl.ShowUserRoiOverlay = true;
                RefreshTeachingBlobOverlays(true);
            });
        }

        private void ApplyTheme()
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.ApplyTheme", () =>
            {
                BackColor = AppColors.Background;
                ForeColor = AppColors.Foreground;
                Font = AppFontHelper.Create(9F);

                grpViewer.BackColor = AppColors.Surface;
                grpViewer.ForeColor = AppColors.Foreground;
                grpTeachingLog.BackColor = AppColors.Surface;
                grpTeachingLog.ForeColor = AppColors.Foreground;

                flpViewerToolbar.BackColor = AppColors.SurfaceDark;
                foreach (Control c in flpViewerToolbar.Controls)
                {
                    Button b = c as Button;
                    if (b == null) continue;
                    b.UseVisualStyleBackColor = false;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = AppColors.Border;
                    b.BackColor = AppColors.SurfaceLight;
                    b.ForeColor = AppColors.Foreground;
                    b.Font = AppFontHelper.Create(9F);
                }

                txtTeachingLog.BackColor = AppColors.SurfaceDark;
                txtTeachingLog.ForeColor = AppColors.Foreground;

                tabCamera.DrawMode = TabDrawMode.OwnerDrawFixed;

                if (ucInspectFlowStrip1 != null)
                {
                    ucInspectFlowStrip1.ApplyAppTheme();
                }

                if (flpTeachingRightStack != null)
                {
                    flpTeachingRightStack.BackColor = AppColors.Surface;
                }
                if (_flpPattern != null)
                {
                    _flpPattern.BackColor = AppColors.Surface;
                }
                if (_flpCount != null)
                {
                    _flpCount.BackColor = AppColors.Surface;
                }

                if (lblNccHeader != null)
                {
                    lblNccHeader.BackColor = AppColors.Surface;
                    lblNccHeader.ForeColor = AppColors.Foreground;
                    try
                    {
                        using (Font f = AppFontHelper.Create(9.5f))
                        {
                            lblNccHeader.Font = new Font(f, FontStyle.Bold);
                        }
                    }
                    catch
                    {
                        lblNccHeader.Font = AppFontHelper.Create(9.5f);
                    }
                }

                if (lblNccResult != null)
                {
                    lblNccResult.BackColor = AppColors.SurfaceDark;
                    lblNccResult.ForeColor = AppColors.Foreground;
                }

                if (btnNccSaveModel != null)
                {
                    btnNccSaveModel.UseVisualStyleBackColor = false;
                    btnNccSaveModel.FlatStyle = FlatStyle.System;
                    btnNccSaveModel.Font = AppFontHelper.Create(9F);
                }

                if (btnNccRunInspect != null)
                {
                    btnNccRunInspect.UseVisualStyleBackColor = false;
                    btnNccRunInspect.FlatStyle = FlatStyle.System;
                    btnNccRunInspect.Font = AppFontHelper.Create(9F);
                }

                if (btnNccCount != null)
                {
                    btnNccCount.UseVisualStyleBackColor = false;
                    btnNccCount.FlatStyle = FlatStyle.System;
                    btnNccCount.Font = AppFontHelper.Create(9F);
                }

                if (btnSaveInspectionRecipe != null)
                {
                    btnSaveInspectionRecipe.UseVisualStyleBackColor = false;
                    btnSaveInspectionRecipe.FlatStyle = FlatStyle.System;
                    btnSaveInspectionRecipe.Font = AppFontHelper.Create(9F);
                }

                if (btnBlobCount != null)
                {
                    btnBlobCount.UseVisualStyleBackColor = false;
                    btnBlobCount.FlatStyle = FlatStyle.System;
                    btnBlobCount.Font = AppFontHelper.Create(9F);
                }
            });
        }

        private void DemoOptions_Changed(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.DemoOptionsChanged", () =>
            {
                if (lblMinArea != null) lblMinArea.Text = trkMinArea.Value.ToString(CultureInfo.InvariantCulture);

                if (_blobFlow != null)
                {
                    _blobFlow.ExpectedBlobCount = numExpected != null ? (int)numExpected.Value : 0;
                    _blobFlow.ForegroundPixelMin = numFgPixelMin != null ? (int)numFgPixelMin.Value : 0;
                    _blobFlow.ForegroundPixelMax = numFgPixelMax != null ? (int)numFgPixelMax.Value : 0;
                }

                if (_viewer?.CanvasControl != null && _viewer.HasImage)
                    RefreshTeachingBlobOverlays(false);
            });
        }

        private void InitializeCamerasForDesign()
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.InitializeCamerasForDesign", () =>
            {
                if (tabCamera.TabPages.Count > 0)
                    return;

                tabCamera.TabPages.Add(new TabPage("CAM 01"));
                tabCamera.TabPages.Add(new TabPage("CAM 02"));
                tabCamera.TabPages.Add(new TabPage("CAM 03"));
                tabCamera.TabPages.Add(new TabPage("CAM 04"));
            });
        }

      

        // Live 버튼은 MainForm의 Menu1에서 실행

        private void StartLive()
        {
            if (_viewer?.CanvasControl == null)
                return;

            if (_liveEnabled)
                return;

            _liveEnabled = true;

            _liveCts = new CancellationTokenSource();
            _liveSource = new OpenCvWebcamFrameSource(deviceIndex: 0, targetFps: 30);

            txtTeachingLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " Live: 웹캠 스트리밍 시작" + Environment.NewLine);

            // Fire-and-forget. 예외는 내부에서 UI 로그로 남깁니다.
            _ = RunLiveLoopAsync(_liveSource, _liveCts.Token);
        }

        private void StopLiveInternal()
        {
            _liveEnabled = false;

            try { _liveCts?.Cancel(); } catch { }
            try { _liveSource?.Stop(); } catch { }
            try { _liveSource?.Dispose(); } catch { }
            try { _liveCts?.Dispose(); } catch { }
            _liveSource = null;
            _liveCts = null;
        }

        private void StopLive()
        {
            if (_liveEnabled == false)
                return;

            StopLiveInternal();
            txtTeachingLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " Live: 중지" + Environment.NewLine);
        }

        private async Task RunLiveLoopAsync(IFrameSource source, CancellationToken token)
        {
            try
            {
                await source.StartAsync(frame =>
                {
                    // frame는 source에서 생성한 Bitmap. UI에서 사용 후 Dispose 필요.
                    if (IsDisposed || _viewer?.CanvasControl == null || _liveEnabled == false)
                    {
                        frame.Dispose();
                        return;
                    }

                    try
                    {
                        if (_viewer.CanvasControl.InvokeRequired)
                        {
                            _viewer.CanvasControl.BeginInvoke((MethodInvoker)(() =>
                            {
                                // UpdateImageFrame이 내부에서 이전 프레임을 Dispose하므로,
                                // 여기서 frame을 Dispose하면 캔버스가 사용하는 이미지가 바로 폐기됩니다.
                                _viewer.CanvasControl.UpdateImageFrame(frame);
                            }));
                        }
                        else
                        {
                            _viewer.CanvasControl.UpdateImageFrame(frame);
                        }
                    }
                    catch
                    {
                        // UI 반영에 실패하면 누수 방지를 위해 Dispose
                        try { frame.Dispose(); } catch { }
                    }
                }, token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (IsDisposed)
                    return;
                try
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        txtTeachingLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " Live 오류: " + ex.Message + Environment.NewLine);
                        StopLiveInternal();
                    }));
                }
                catch
                {
                }
            }
        }

        /// <summary>MainForm 상단 Menu1(Live)에서 호출합니다.</summary>
        public void ToggleLiveFromMenu()
        {
            try
            {
                if (_liveEnabled)
                    StopLive();
                else
                    StartLive();
            }
            catch (Exception ex)
            {
                try { txtTeachingLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " Live 오류: " + ex.Message + Environment.NewLine); } catch { }
                try { MessageBox.Show(ex.Message, "Live", MessageBoxButtons.OK, MessageBoxIcon.Warning); } catch { }
            }
        }

        /// <summary>MainForm 상단 Menu2(Snap)에서 호출합니다.</summary>
        public void SnapFromMenu()
        {
            try
            {
                // 데모용: Live 중이면 중지해서 현재 프레임을 고정(캡처)합니다.
                if (_liveEnabled)
                {
                    StopLive();
                    txtTeachingLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " Snap: 현재 프레임을 고정했습니다." + Environment.NewLine);
                    return;
                }

                txtTeachingLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " Snap: Live가 꺼져 있습니다." + Environment.NewLine);
            }
            catch (Exception ex)
            {
                try { txtTeachingLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " Snap 오류: " + ex.Message + Environment.NewLine); } catch { }
                try { MessageBox.Show(ex.Message, "Snap", MessageBoxButtons.OK, MessageBoxIcon.Warning); } catch { }
            }
        }

        private void btnToolFit_Click(object sender, EventArgs e) => btnViewerTool_Click(sender, e);

        private void btnToolOvClr_Click(object sender, EventArgs e) => btnViewerTool_Click(sender, e);

        private void btnToolCross_Click(object sender, EventArgs e) => btnViewerTool_Click(sender, e);

        private void btnToolGray_Click(object sender, EventArgs e) => btnViewerTool_Click(sender, e);

        private void btnToolAvg_Click(object sender, EventArgs e) => btnViewerTool_Click(sender, e);

        private void btnToolSync_Click(object sender, EventArgs e) => btnViewerTool_Click(sender, e);

        private void btnToolMap_Click(object sender, EventArgs e) => btnViewerTool_Click(sender, e);

        private void btnViewerTool_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            string id = b != null ? (b.Tag as string ?? b.Text) : "?";
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.ViewerToolClick", () =>
            {
                switch (id)
                {
                    case "LOAD":
                        if (_blobFlow != null)
                        {
                            _blobFlow.LoadImageOrFolderFromUserChoice();
                        }

                        break;
                    case "SAVE":
                        _viewer.SaveImageFromDialog();
                        break;
                    case "CLEAR":
                        ClearTeachingNccSummaryUi();
                        if (_blobFlow != null)
                        {
                            _blobFlow.CancelAutoBatchIfAny();
                            _viewer.ClearDisplay();
                            _blobFlow.ClearSessionAndFolder();
                            _blobFlow.ApplyFlowControlStates();
                        }
                        else
                        {
                            _viewer.ClearDisplay();
                        }

                        break;
                    case "Z+":
                        _viewer.ZoomIn();
                        break;
                    case "Z-":
                        _viewer.ZoomOut();
                        break;
                    case "ZM":
                        _viewer.ToggleZoomMode();
                        break;
                    case "FIT":
                        _viewer.ZoomFit();
                        break;
                    case "CLR_OVR":
                        _viewer.ClearOverlay();
                        break;
                    case "CROSS":
                        _viewer.ToggleCenterCross();
                        break;
                    case "GRAY":
                        _viewer.ToggleGrayValue();
                        break;
                    case "AVG":
                        _viewer.ToggleAverageGray();
                        break;
                    case "SYNC":
                        _viewer.ToggleSync();
                        break;
                    case "MAP":
                        _viewer.ToggleMiniMap();
                        break;
                    default:
                        break;
                }

                ViewerShellToolbarExtras.RefreshToggleButtonColors(_viewer, flpViewerToolbar);
            });
        }

        private void WireViewerToolbarStateRefresh()
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.WireViewerToolbarStateRefresh", () =>
            {
                if (_viewer?.CanvasControl == null)
                    return;

                _viewer.CanvasControl.ViewStateChanged += (_, __) =>
                    ViewerShellToolbarExtras.RefreshToggleButtonColors(_viewer, flpViewerToolbar);
                ViewSyncManager.SyncEnabledChanged += (_, __) =>
                    ViewerShellToolbarExtras.RefreshToggleButtonColors(_viewer, flpViewerToolbar);
                ViewerShellToolbarExtras.RefreshToggleButtonColors(_viewer, flpViewerToolbar);
            });
        }

        /// <summary>Cross 켜짐: 십자 이동. Cross 끄면 ←/→로 같은 폴더 이전·다음 이미지 (Auto Run과 동일).</summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (_viewer?.CanvasControl != null && _viewer.CanvasControl.TryProcessCenterCrossArrowKeys(keyData))
                {
                    return true;
                }

                if (_blobFlow != null)
                {
                    if (keyData == Keys.Left && _blobFlow.TryNavigateImageFolderBatch(-1))
                    {
                        return true;
                    }

                    if (keyData == Keys.Right && _blobFlow.TryNavigateImageFolderBatch(1))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnDrawString_Click(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.DrawString", () =>
            {
                // 이미지 로드 확인
                if (_viewer == null || _viewer.CanvasControl == null || !_viewer.HasImage)
                {
                    MessageBox.Show("먼저 이미지를 불러와 주세요.", "Teaching", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                RefreshTeachingBlobOverlays(true);
            });
        }

        private void btnAddRoiRect_Click(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.AddRoiRect", () =>
            {
                if (_viewer?.CanvasControl == null || !_viewer.HasImage)
                {
                    MessageBox.Show("먼저 이미지를 불러와 주세요.", "Teaching", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Size sz = _viewer.CanvasControl.ImagePixelSize;
                if (sz.Width <= 0 || sz.Height <= 0)
                    return;

                int w = Math.Max(50, (int)Math.Round(sz.Width * 0.25));
                int h = Math.Max(50, (int)Math.Round(sz.Height * 0.25));
                int left = Math.Max(0, (sz.Width - w) / 2);
                int top = Math.Max(0, (sz.Height - h) / 2);

                // NCC 카운트에서 ROI 표시를 숨긴 상태였더라도 ROI+ 누르면 즉시 보이게 복구
                _viewer.CanvasControl.ShowUserRoiOverlay = true;

                // 검사 영역 ROI는 사용자가 움직이기 쉬우라고 눈에 띄는 색으로 표시
                var roi = new ROIRectangle(new Rectangle(left, top, w, h), Color.LimeGreen, 2);
                _viewer.CanvasControl.AddROI(roi);
                _viewer.CanvasControl.SelectROIByName(roi.Name);

                txtTeachingLog.AppendText(
                    DateTime.Now.ToString("HH:mm:ss")
                    + " ROI 추가: 사각형 검사영역을 생성했습니다. (마우스로 이동/크기조절 가능)"
                    + Environment.NewLine);

                RefreshTeachingBlobOverlays(true);
            });
        }

        private void UcInspectFlowStripThresholdChanged()
        {
            if (ucInspectFlowStrip1 == null || ucInspectFlowStrip1.TrkThreshold == null)
                return;
            int v = ucInspectFlowStrip1.TrkThreshold.Value;
            if (ucInspectFlowStrip1.LblThresholdValue != null)
            {
                ucInspectFlowStrip1.LblThresholdValue.Text = v.ToString(CultureInfo.InvariantCulture);
            }

            if (_viewer?.CanvasControl != null && _viewer.HasImage)
            {
                RefreshTeachingBlobOverlays(false);
            }
        }

        /// <summary>
        /// Auto Run과 동일하게 밝기 하한~255 구간으로 이진화한 뒤, 이미지 전체에서 Blob을 찾아 박스를 그립니다.
        /// </summary>
        /// <param name="appendLog">true이면 Teaching Log에 검출 개수 한 줄을 추가합니다.</param>
        private void RefreshTeachingBlobOverlays(bool appendLog)
        {
            if (_viewer == null || _viewer.CanvasControl == null || !_viewer.HasImage)
                return;

            _viewer.CanvasControl.ClearDrawOverlays();

            int th = ucInspectFlowStrip1 != null && ucInspectFlowStrip1.TrkThreshold != null
                ? ucInspectFlowStrip1.TrkThreshold.Value
                : 128;
            int minArea = trkMinArea != null ? trkMinArea.Value : 0;
            _viewer.CanvasControl.SetBlobOptions(true, th, 255, minArea, 0, 0, 0, 0, false);
            _viewer.CanvasControl.ShowBlob_Bright(true, th, 255);

            int job = Interlocked.Increment(ref _teachingBlobJob);
            _viewer.CanvasControl.BeginFindBlobObjectsInTargetRoi(25000, (count, ex) =>
            {
                if (IsDisposed || job != Volatile.Read(ref _teachingBlobJob))
                    return;

                AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.BlobAsyncDone", () =>
                {
                    if (ex != null)
                    {
                        txtTeachingLog.AppendText(
                            DateTime.Now.ToString("HH:mm:ss")
                            + " Blob 검출: "
                            + ex.Message
                            + Environment.NewLine);
                        return;
                    }

                    int expected = numExpected != null ? (int)numExpected.Value : 0;
                    string judgeText = "OK";
                    Color judgeColor = Color.LimeGreen;
                    if (expected > 0 && count != expected)
                    {
                        int diff = count - expected;
                        if (diff < 0)
                            judgeText = "NG  Missing: " + (-diff).ToString(CultureInfo.InvariantCulture);
                        else
                            judgeText = "NG  Extra: " + diff.ToString(CultureInfo.InvariantCulture);
                        judgeColor = Color.Red;
                    }

                    // 화면에 판정 텍스트 표시(ROI 좌상단)
                    ROI_Manager roi = _viewer.CanvasControl.SelectedRoi;
                    if (roi == null && _viewer.CanvasControl.ROIItems.Count > 0)
                        roi = _viewer.CanvasControl.ROIItems[0];
                    if (roi != null)
                    {
                        Rectangle b = roi.GetBounds();
                        PointF pt = new PointF(b.Left + 6, b.Top + 6);
                        _viewer.CanvasControl.DrawString(pt, judgeText, judgeColor, 18);
                    }

                    if (appendLog)
                    {
                        txtTeachingLog.AppendText(
                            DateTime.Now.ToString("HH:mm:ss")
                            + string.Format(CultureInfo.InvariantCulture, " 볼 검출(Blob): threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}",
                                th, minArea, count, expected, judgeText)
                            + Environment.NewLine);
                    }
                });
            });
        }

        private void tabCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            // UI shell: later wire per-camera viewer binding.
        }

        private void tabCamera_DrawItem(object sender, DrawItemEventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcTeachingShell.CameraTabDraw", () =>
            {
                if (e.Index < 0 || e.Index >= tabCamera.TabPages.Count)
                    return;

                bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                Color backColor = selected ? AppColors.SelectionHighlight : AppColors.SurfaceDark;
                using (SolidBrush brush = new SolidBrush(backColor))
                    e.Graphics.FillRectangle(brush, e.Bounds);

                Color textColor = selected ? Color.White : AppColors.Foreground;
                TextRenderer.DrawText(e.Graphics, tabCamera.TabPages[e.Index].Text, AppFontHelper.Create(9F), e.Bounds, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            });
        }

        private static void HalconRowColFromRect(Rectangle b, out double row1, out double col1, out double row2, out double col2)
        {
            row1 = b.Top;
            col1 = b.Left;
            row2 = b.Bottom - 1;
            col2 = b.Right - 1;
        }

        private static ROIRectangle TryGetFirstRectRoi(ImageCanvasControl canvas)
        {
            if (canvas == null || canvas.ROIItems == null)
                return null;
            for (int i = 0; i < canvas.ROIItems.Count; i++)
            {
                if (canvas.ROIItems[i] is ROIRectangle r)
                    return r;
            }

            return null;
        }

        private static void DrawNccScoreUnderMatch(
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

        private static double NccCountTrackToScore(int track0to100)
        {
            if (track0to100 < 0)
                track0to100 = 0;
            if (track0to100 > 100)
                track0to100 = 100;
            return 0.5 + (track0to100 / 100.0) * 0.5;
        }

        private void UpdateNccCountTrackLabels()
        {
            if (trkNccCountMin == null || trkNccCountMax == null)
                return;
            if (lblNccCountMinVal != null)
            {
                lblNccCountMinVal.Text = NccCountTrackToScore(trkNccCountMin.Value).ToString("F2", CultureInfo.InvariantCulture);
            }

            if (lblNccCountMaxVal != null)
            {
                lblNccCountMaxVal.Text = NccCountTrackToScore(trkNccCountMax.Value).ToString("F2", CultureInfo.InvariantCulture);
            }
        }

        private void NccCountTracks_Changed(object sender, EventArgs e)
        {
            if (trkNccCountMin == null || trkNccCountMax == null)
                return;
            if (trkNccCountMin.Value > trkNccCountMax.Value)
            {
                if (sender == trkNccCountMin)
                    trkNccCountMax.Value = trkNccCountMin.Value;
                else
                    trkNccCountMin.Value = trkNccCountMax.Value;
            }

            UpdateNccCountTrackLabels();
        }

        private void OnNccCountClicked()
        {
            if (_viewer == null || _viewer.CanvasControl == null || _viewer.HasImage == false)
            {
                MessageBox.Show("검사할 이미지를 먼저 불러와 주세요.", "NCC 카운트", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = _viewer.CanvasControl.ImagePath;
            if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
            {
                MessageBox.Show("검사 이미지에 파일 경로가 없습니다. 이미지 파일(LOAD)로 열어 주세요.", "NCC 카운트", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int th = ucInspectFlowStrip1 != null && ucInspectFlowStrip1.TrkThreshold != null
                    ? ucInspectFlowStrip1.TrkThreshold.Value
                    : 128;
                int fgCount;
                string fgErr;
                if (_viewer.CanvasControl.TryCountForegroundPixelsInSelectedRectRoiUsingMask(th, 255, out fgCount, out fgErr))
                {
                    int fgMin = numFgPixelMin != null ? (int)numFgPixelMin.Value : 0;
                    int fgMax = numFgPixelMax != null ? (int)numFgPixelMax.Value : 0;
                    txtTeachingLog?.AppendText(
                        DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                        + " ROI 전경 픽셀(mask): count="
                        + fgCount.ToString(CultureInfo.InvariantCulture)
                        + ", range=["
                        + fgMin.ToString(CultureInfo.InvariantCulture)
                        + ","
                        + fgMax.ToString(CultureInfo.InvariantCulture)
                        + "]"
                        + Environment.NewLine);
                }
                else
                {
                    txtTeachingLog?.AppendText(
                        DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                        + " ROI 전경 픽셀(mask) 계산 실패: "
                        + fgErr
                        + Environment.NewLine);
                }
            }
            catch
            {
            }

            string model;
            double tW;
            double tH;
            bool useTempModel = false;
            if (!string.IsNullOrWhiteSpace(_lastNccModelPath) && File.Exists(_lastNccModelPath))
            {
                model = _lastNccModelPath;
                tW = _lastNccTemplateW;
                tH = _lastNccTemplateH;
                _nccTrackModelPath = model;
                try
                {
                    double ms = (double)(nudNccMinScore != null ? nudNccMinScore.Value : 0.75M);
                    NccSharedModelState.Set(model, tW, tH, ms);
                }
                catch
                {
                }
            }
            else
            {
                ROIRectangle rectRoi = TryGetFirstRectRoi(_viewer.CanvasControl);
                Rectangle b = rectRoi != null ? rectRoi.GetBounds() : Rectangle.Empty;
                if (rectRoi == null || b.Width < 4 || b.Height < 4)
                {
                    MessageBox.Show(
                        "NCC 모델이 없습니다. 볼이 포함되도록 사각형 ROI를 그리거나, [NCC 모델 저장]으로 모델을 만든 뒤 카운트하세요.",
                        "NCC 카운트",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                string tempModel = Path.Combine(
                    Path.GetTempPath(),
                    "VisionInspectEx_ncc_count_" + Guid.NewGuid().ToString("N") + "." + NccModelFileNaming.NccFileExtension);
                try
                {
                    HalconRowColFromRect(b, out double row1, out double col1, out double row2, out double col2);
                    NccPatternInspector.CreateAndWriteModel(
                        path,
                        row1,
                        col1,
                        row2,
                        col2,
                        tempModel,
                        out tW,
                        out tH);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ROI로부터 임시 NCC 모델을 만들지 못했습니다.\r\n" + ex.Message, "NCC 카운트", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    try
                    {
                        if (File.Exists(tempModel))
                            File.Delete(tempModel);
                    }
                    catch
                    {
                    }

                    return;
                }

                model = tempModel;
                useTempModel = true;
            }

            double sMin = NccCountTrackToScore(trkNccCountMin != null ? trkNccCountMin.Value : 50);
            double sMax = NccCountTrackToScore(trkNccCountMax != null ? trkNccCountMax.Value : 100);
            if (sMin > sMax)
            {
                double t = sMin;
                sMin = sMax;
                sMax = t;
            }

            double judgeMin = (double)(nudNccCountJudgeMin != null ? nudNccCountJudgeMin.Value : 0.80M);
            if (judgeMin < 0.0 || judgeMin > 1.0)
                judgeMin = 0.80;

            // 카운트 판정 최소 점수는 추가 하한으로 적용(트랙바 Min보다 크면 그 값이 우선)
            if (sMin < judgeMin)
                sMin = judgeMin;

            double findFloor = sMin;
            if (findFloor > 0.45)
                findFloor = 0.45;
            if (findFloor < 0.3)
                findFloor = 0.3;

            if (lblNccResult != null)
            {
                lblNccResult.Text = "NCC: " + L("NccCounting") + "…";
                lblNccResult.ForeColor = AppColors.Foreground;
            }

            string pathCopy = path;
            string modelCopy = model;
            double tWc = tW;
            double tHc = tH;
            double sMinCopy = sMin;
            double sMaxCopy = sMax;
            double findFloorC = findFloor;
            bool deleteModelAfter = useTempModel;
            _ = Task.Run(() =>
            {
                HalconResult raw = null;
                Exception ex = null;
                try
                {
                    try
                    {
                        raw = NccPatternInspector.MatchFromModelFile(
                            pathCopy,
                            modelCopy,
                            findFloorC,
                            tWc,
                            tHc,
                            findFloorC);
                    }
                    catch (Exception x)
                    {
                        ex = x;
                    }

                    HalconResult filtered = null;
                    if (ex == null && raw != null)
                    {
                        try
                        {
                            filtered = raw.FilterByScoreRange(sMinCopy, sMaxCopy);
                        }
                        catch
                        {
                        }
                    }

                    int rawCount = raw != null ? raw.Count : 0;

                    if (IsDisposed)
                        return;

                    try
                    {
                        if (InvokeRequired)
                        {
                            BeginInvoke((MethodInvoker)(() => NccCountOnUi(filtered, rawCount, sMinCopy, sMaxCopy, ex)));
                        }
                        else
                        {
                            NccCountOnUi(filtered, rawCount, sMinCopy, sMaxCopy, ex);
                        }
                    }
                    catch
                    {
                    }
                }
                finally
                {
                    if (deleteModelAfter && !string.IsNullOrWhiteSpace(modelCopy))
                    {
                        try
                        {
                            if (File.Exists(modelCopy))
                                File.Delete(modelCopy);
                        }
                        catch
                        {
                        }
                    }
                }
            });
        }

        private void NccCountOnUi(HalconResult filtered, int rawCount, double sMin, double sMax, Exception ex)
        {
            if (_viewer?.CanvasControl == null)
                return;

            if (ex != null)
            {
                SetNccTopInfo(L("JudgeNG"), ex.Message, string.Empty, Color.OrangeRed);
                if (lblNccResult != null)
                {
                    lblNccResult.Text = "NCC: " + L("NccCountError") + "  " + ex.Message;
                    lblNccResult.ForeColor = Color.OrangeRed;
                }

                txtTeachingLog?.AppendText(
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                    + " NCC 카운트 오류: " + ex.Message
                    + Environment.NewLine);
                return;
            }

            if (filtered == null)
            {
                SetNccTopInfo(L("JudgeNG"), L("NccCountFail"), string.Empty, Color.OrangeRed);
                if (lblNccResult != null)
                {
                    lblNccResult.Text = "NCC: " + L("NccCountFail");
                    lblNccResult.ForeColor = Color.OrangeRed;
                }

                return;
            }

            if (filtered.Count > 0 && ((numFgPixelMin != null && numFgPixelMin.Value > 0) || (numFgPixelMax != null && numFgPixelMax.Value > 0)))
            {
                int th = ucInspectFlowStrip1 != null && ucInspectFlowStrip1.TrkThreshold != null
                    ? ucInspectFlowStrip1.TrkThreshold.Value
                    : 128;
                int fgMin = numFgPixelMin != null ? (int)numFgPixelMin.Value : 0;
                int fgMax = numFgPixelMax != null ? (int)numFgPixelMax.Value : 0;
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
                        int fgCount;
                        string fgErr;
                        if (_viewer.CanvasControl.TryCountForegroundPixelsInRectUsingMask(box, th, 255, out fgCount, out fgErr))
                        {
                            bool below = fgMin > 0 && fgCount < fgMin;
                            bool above = fgMax > 0 && fgCount > fgMax;
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
                    txtTeachingLog?.AppendText(
                        DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                        + " NCC 픽셀 필터: kept="
                        + filtered.Count.ToString(CultureInfo.InvariantCulture)
                        + "/"
                        + before.ToString(CultureInfo.InvariantCulture)
                        + ", range=["
                        + (fgMin > 0 ? fgMin.ToString(CultureInfo.InvariantCulture) : "-")
                        + ","
                        + (fgMax > 0 ? fgMax.ToString(CultureInfo.InvariantCulture) : "-")
                        + "]"
                        + Environment.NewLine);
                }
            }

            Color blobScoreColor = Color.Cyan;
            _viewer.CanvasControl.BeginOverlayUpdate();
            try
            {
                _viewer.CanvasControl.ClearDrawOverlays();
                for (int i = 0; i < filtered.Count; i++)
                {
                    try
                    {
                        double row = filtered.Row[i].D;
                        double col = filtered.Column[i].D;
                        double scI = 0.0;
                        if (filtered.Score != null && i < filtered.Score.TupleLength())
                            scI = filtered.Score[i].D;
                        if (filtered.TemplateWidth > 0.1 && filtered.TemplateHeight > 0.1)
                        {
                            RectangleF box = NccPatternInspector.GetMatchImageRectFromCenteredTemplate(
                                row,
                                col,
                                filtered.TemplateWidth,
                                filtered.TemplateHeight);
                            _viewer.CanvasControl.DrawRectOverlay(
                                box,
                                blobScoreColor,
                                2.5f,
                                false);
                            DrawNccScoreUnderMatch(
                                _viewer.CanvasControl,
                                box,
                                col,
                                row,
                                scI,
                                blobScoreColor,
                                hasBox: true);
                        }
                        else
                        {
                            _viewer.CanvasControl.DrawCrossLineOverlay(
                                new PointF((float)col, (float)row),
                                32f,
                                blobScoreColor,
                                2f);
                            DrawNccScoreUnderMatch(
                                _viewer.CanvasControl,
                                RectangleF.Empty,
                                col,
                                row,
                                scI,
                                blobScoreColor,
                                hasBox: false);
                        }
                    }
                    catch
                    {
                    }
                }

                string matchLine = "Match=—";
                if (TryComputeNccScoreAverage(filtered, out double avgDraw))
                    matchLine = FormatNccMatchPercentLine(avgDraw);
                SetNccTopInfo(
                    "OK",
                    matchLine,
                    string.Format(CultureInfo.InvariantCulture, "개수(n)={0}", filtered.Count),
                    Color.Cyan);
            }
            finally
            {
                _viewer.CanvasControl.EndOverlayUpdate();
            }

            string line1 = string.Format(
                CultureInfo.InvariantCulture,
                "NCC: 카운트 {0}  (범위 {1:F2}~{2:F2})",
                filtered.Count,
                sMin,
                sMax);
            if (lblNccResult != null)
            {
                if (TryComputeNccScoreAverage(filtered, out double avgUi))
                {
                    lblNccResult.Text = line1
                        + Environment.NewLine
                        + FormatNccMatchPercentResultLabel(avgUi);
                }
                else
                {
                    lblNccResult.Text = line1 + Environment.NewLine + L("AvgNccScore") + ": —";
                }

                lblNccResult.ForeColor = AppColors.Foreground;
            }

            string logDetail = "filtered=" + filtered.Count + ", raw=" + rawCount
                + string.Format(CultureInfo.InvariantCulture, ", score[{0:F2},{1:F2}]", sMin, sMax);
            if (TryComputeNccScoreAverage(filtered, out double avgLog))
            {
                logDetail += "  avgScore=" + avgLog.ToString("F3", CultureInfo.InvariantCulture);
            }
            else if (filtered.Score != null && filtered.Score.TupleLength() > 0)
            {
                logDetail += "  ex0=" + filtered.Score[0].D.ToString("F3", CultureInfo.InvariantCulture);
            }

            txtTeachingLog?.AppendText(
                DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                + " NCC 카운트  " + logDetail
                + Environment.NewLine);
        }

        private bool TryBuildInspectionRecipeFromCanvas(out InspectionRecipe recipe)
        {
            recipe = new InspectionRecipe { SavedUtc = DateTime.UtcNow };
            if (_viewer?.CanvasControl == null)
                return false;

            IList<ROI_Manager> items = _viewer.CanvasControl.ROIItems;
            if (items == null)
                return false;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is ROIRectangle rr)
                    recipe.RoiRectangles.Add(rr.GetBounds());
            }

            if (recipe.RoiRectangles.Count == 0)
                return false;

            int th = ucInspectFlowStrip1 != null && ucInspectFlowStrip1.TrkThreshold != null
                ? ucInspectFlowStrip1.TrkThreshold.Value
                : 128;
            recipe.Threshold = th;
            recipe.MinArea = trkMinArea != null ? trkMinArea.Value : 0;
            recipe.ExpectedBlobCount = numExpected != null ? (int)numExpected.Value : 0;
            recipe.ForegroundPixelMin = numFgPixelMin != null ? (int)numFgPixelMin.Value : 0;
            recipe.ForegroundPixelMax = numFgPixelMax != null ? (int)numFgPixelMax.Value : 0;

            try
            {
                if (trkNccCountMin != null && trkNccCountMax != null)
                {
                    double sMin = NccCountTrackToScore(trkNccCountMin.Value);
                    double sMax = NccCountTrackToScore(trkNccCountMax.Value);
                    if (sMin > sMax)
                    {
                        double t = sMin;
                        sMin = sMax;
                        sMax = t;
                    }

                    double judgeMin = (double)(nudNccCountJudgeMin != null ? nudNccCountJudgeMin.Value : 0.80M);
                    if (judgeMin < 0.0 || judgeMin > 1.0)
                        judgeMin = 0.80;
                    if (sMin < judgeMin)
                        sMin = judgeMin;
                    recipe.NccFilterMinScore = sMin;
                    recipe.NccFilterMaxScore = sMax;
                }
            }
            catch
            {
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(_lastNccModelPath) && File.Exists(_lastNccModelPath))
                {
                    recipe.NccModelPath = _lastNccModelPath;
                    recipe.NccTemplateWidth = _lastNccTemplateW;
                    recipe.NccTemplateHeight = _lastNccTemplateH;
                    recipe.NccMinScore = (double)(nudNccMinScore != null ? nudNccMinScore.Value : 0.75M);
                }
            }
            catch
            {
            }

            return true;
        }

        private void OnSaveInspectionRecipeClicked()
        {
            if (!TryBuildInspectionRecipeFromCanvas(out InspectionRecipe r))
            {
                MessageBox.Show(
                    "저장할 사각형 ROI가 없습니다. ROI+ 로 검사 영역을 만든 뒤 다시 시도하세요.",
                    "Teaching",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            TeachingInspectionRecipeStore.Set(r);
            txtTeachingLog?.AppendText(
                DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                + " 검사 레시피 저장: ROI="
                + r.RoiRectangles.Count.ToString(CultureInfo.InvariantCulture)
                + ", th="
                + r.Threshold.ToString(CultureInfo.InvariantCulture)
                + ", MinArea="
                + r.MinArea.ToString(CultureInfo.InvariantCulture)
                + ", Expected="
                + r.ExpectedBlobCount.ToString(CultureInfo.InvariantCulture)
                + ", FgPxRange=["
                + r.ForegroundPixelMin.ToString(CultureInfo.InvariantCulture)
                + ","
                + r.ForegroundPixelMax.ToString(CultureInfo.InvariantCulture)
                + "]"
                + Environment.NewLine);
            try
            {
                AppLogger.Write("RECIPE", "Teaching saved ROI count=" + r.RoiRectangles.Count.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {
            }

            MessageBox.Show(
                "자동 실행 화면에서 이미지를 연 뒤 상단 «개수» 또는 «3) Run inspect»로 검사합니다.\r\n"
                + "NCC 모델이 세션에 있으면 이미지 전체에서 동일 패턴을 찾아 카운트하고, 없으면 ROI 내 Blob 검사로 동작합니다.",
                "Teaching",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>NCC 모델 저장 성공 시 레시피도 함께 저장합니다(ROI가 있을 때만).</summary>
        private void TryPersistTeachingInspectionRecipeQuiet()
        {
            if (!TryBuildInspectionRecipeFromCanvas(out InspectionRecipe r))
                return;

            TeachingInspectionRecipeStore.Set(r);
            txtTeachingLog?.AppendText(
                DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                + " 검사 레시피 동반 저장 (NCC 모델 저장 시): ROI="
                + r.RoiRectangles.Count.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine);
            try
            {
                AppLogger.Write("RECIPE", "Co-saved with NCC model ROI count=" + r.RoiRectangles.Count.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {
            }
        }

        private void OnNccSaveModelClicked()
        {
            if (_viewer == null || _viewer.CanvasControl == null || _viewer.HasImage == false)
            {
                MessageBox.Show("이미지를 먼저 파일에서 불러와 주세요. (NCC 모델은 골든 이미지가 필요합니다.)", "NCC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = _viewer.CanvasControl.ImagePath;
            if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
            {
                MessageBox.Show("이미지가 파일 경로와 연결되어 있지 않습니다. 메뉴의 불러오기(LOAD)로 이미지 파일을 지정한 뒤 다시 시도하세요.", "NCC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ROIRectangle rect = TryGetFirstRectRoi(_viewer.CanvasControl);
            if (rect == null)
            {
                MessageBox.Show("사각형(ROI) 검사 영역이 필요합니다. \"ROI+\"로 ROI를 만든 뒤 패턴을 덮으세요.", "NCC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Rectangle b = rect.GetBounds();
            if (b.Width < 4 || b.Height < 4)
            {
                MessageBox.Show("ROI가 너무 작습니다(가로/세로 최소 4픽셀 이상).", "NCC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HalconRowColFromRect(b, out double row1, out double col1, out double row2, out double col2);
            string modelPath = NccModelFileNaming.GetNextModelFilePath(path);

            NccPatternInspector.CreateAndWriteModel(
                path,
                row1,
                col1,
                row2,
                col2,
                modelPath,
                out double tw,
                out double th);

            _lastNccModelPath = modelPath;
            _nccTrackModelPath = modelPath;
            _lastNccTemplateW = tw;
            _lastNccTemplateH = th;
            try
            {
                double ms = (double)(nudNccMinScore != null ? nudNccMinScore.Value : 0.75M);
                HalconResult rGold = NccPatternInspector.MatchFromModelFile(path, modelPath, 0.5, tw, th);
                if (rGold != null && rGold.Count > 0)
                {
                    // 참조 자세는 "골든 이미지에서의 매칭 자세"를 그대로 사용합니다.
                    // (refAngle=0 으로 고정하면 모든 이미지를 '각도 0'으로 강제 정렬하게 되어
                    //  ROI가 예상과 다르게 보이거나 과도한 회전이 발생할 수 있습니다.)
                    NccSharedModelState.SetWithReferencePose(
                        modelPath,
                        tw,
                        th,
                        ms,
                        rGold.Row[0].D,
                        rGold.Column[0].D,
                        rGold.Angle[0].D,
                        path);
                }
                else
                {
                    NccSharedModelState.Set(modelPath, tw, th, ms);
                }
            }
            catch
            {
                try
                {
                    double ms = (double)(nudNccMinScore != null ? nudNccMinScore.Value : 0.75M);
                    NccSharedModelState.Set(modelPath, tw, th, ms);
                }
                catch
                {
                }
            }
            if (lblNccResult != null)
            {
                lblNccResult.Text = "NCC: (" + L("NccModelSaved") + ")";
                lblNccResult.ForeColor = AppColors.Foreground;
            }

            txtTeachingLog?.AppendText(
                DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                + " NCC 모델 저장: " + modelPath
                + Environment.NewLine);
            try
            {
                AppLogger.Write("NCC", "Saved " + modelPath);
            }
            catch
            {
            }

            TryPersistTeachingInspectionRecipeQuiet();
        }

        private void OnNccRunInspectClicked()
        {
            if (_viewer == null || _viewer.CanvasControl == null || _viewer.HasImage == false)
            {
                MessageBox.Show("검사할 이미지를 먼저 불러와 주세요.", "NCC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = _viewer.CanvasControl.ImagePath;
            if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
            {
                MessageBox.Show("검사 이미지에 파일 경로가 없습니다. 이미지 파일(LOAD)로 열어 주세요.", "NCC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string model = _lastNccModelPath;
            if (string.IsNullOrWhiteSpace(model) || File.Exists(model) == false)
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Filter = "NCC 모델|*.ncm|모든 파일|*.*";
                    dlg.Title = "NCC 모델 파일 선택";
                    if (dlg.ShowDialog(FindForm()) != DialogResult.OK)
                        return;
                    model = dlg.FileName;
                }
            }

            if (string.IsNullOrWhiteSpace(model) || File.Exists(model) == false)
                return;
            _nccTrackModelPath = model;

            bool sessionTemplates = !string.IsNullOrEmpty(_lastNccModelPath) &&
                string.Equals(Path.GetFullPath(model), Path.GetFullPath(_lastNccModelPath), StringComparison.OrdinalIgnoreCase);
            double tW = sessionTemplates ? _lastNccTemplateW : 0.0;
            double tH = sessionTemplates ? _lastNccTemplateH : 0.0;
            try
            {
                double ms = (double)(nudNccMinScore != null ? nudNccMinScore.Value : 0.75M);
                NccSharedModelState.Set(model, tW, tH, ms);
            }
            catch
            {
            }

            double jMin = (double)(nudNccMinScore != null ? nudNccMinScore.Value : 0.75M);
            if (jMin < 0.0 || jMin > 1.0)
                jMin = 0.75;

            if (lblNccResult != null)
            {
                lblNccResult.Text = "NCC: " + L("NccInspecting") + "…";
                lblNccResult.ForeColor = AppColors.Foreground;
            }

            string pathCopy = path;
            string modelCopy = model;
            double tWc = tW, tHc = tH;
            double jMinC = jMin;
            _ = Task.Run(() =>
            {
                HalconResult r = null;
                Exception ex = null;
                try
                {
                    r = NccPatternInspector.MatchFromModelFile(
                        pathCopy,
                        modelCopy,
                        0.5,
                        tWc,
                        tHc);
                    r?.EvaluateJudgment(1, jMinC);
                }
                catch (Exception x)
                {
                    ex = x;
                }

                if (IsDisposed)
                    return;

                try
                {
                    if (InvokeRequired)
                    {
                        BeginInvoke((MethodInvoker)(() => NccRunInspectOnUi(r, ex)));
                    }
                    else
                    {
                        NccRunInspectOnUi(r, ex);
                    }
                }
                catch
                {
                }
            });
        }

        private void NccRunInspectOnUi(HalconResult r, Exception ex)
        {
            if (_viewer?.CanvasControl == null)
                return;

            if (ex != null)
            {
                if (lblNccResult != null)
                {
                    lblNccResult.Text = "NCC: " + L("NccError") + "  " + ex.Message;
                    lblNccResult.ForeColor = Color.OrangeRed;
                }

                txtTeachingLog?.AppendText(
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                    + " NCC 검사 오류: " + ex.Message
                    + Environment.NewLine);
                return;
            }

            if (r == null)
            {
                SetNccTopInfo(L("JudgeNG"), L("NccNoResult"), string.Empty, Color.OrangeRed);
                if (lblNccResult != null)
                {
                            lblNccResult.Text = "NCC: " + L("JudgeNG") + "  " + L("NccNoResult");
                    lblNccResult.ForeColor = Color.OrangeRed;
                }

                return;
            }

            Color matchColor = (r.JudgmentEvaluated && r.IsOk) ? Color.Lime : Color.Orange;
            string oneLine = string.Empty;
            _viewer.CanvasControl.BeginOverlayUpdate();
            try
            {
                _viewer.CanvasControl.ClearDrawOverlays();
                for (int i = 0; i < r.Count; i++)
                {
                    try
                    {
                        double row = r.Row[i].D;
                        double col = r.Column[i].D;
                        double scI = 0.0;
                        if (r.Score != null && i < r.Score.TupleLength())
                            scI = r.Score[i].D;
                        if (r.TemplateWidth > 0.1 && r.TemplateHeight > 0.1)
                        {
                            RectangleF box = NccPatternInspector.GetMatchImageRectFromCenteredTemplate(
                                row,
                                col,
                                r.TemplateWidth,
                                r.TemplateHeight);
                            _viewer.CanvasControl.DrawRectOverlay(
                                box,
                                matchColor,
                                2.5f,
                                false);
                            DrawNccScoreUnderMatch(
                                _viewer.CanvasControl,
                                box,
                                col,
                                row,
                                scI,
                                matchColor,
                                hasBox: true);
                        }
                        else
                        {
                            _viewer.CanvasControl.DrawCrossLineOverlay(
                                new PointF((float)col, (float)row),
                                32f,
                                matchColor,
                                2f);
                            DrawNccScoreUnderMatch(
                                _viewer.CanvasControl,
                                RectangleF.Empty,
                                col,
                                row,
                                scI,
                                matchColor,
                                hasBox: false);
                        }
                    }
                    catch
                    {
                    }
                }

                if (r.JudgmentEvaluated)
                {
                    if (r.IsOk)
                    {
                        oneLine = "OK  " + r.JudgmentSummary;
                        if (lblNccResult != null)
                        {
                            if (TryComputeNccScoreAverage(r, out double avgOk))
                            {
                                lblNccResult.Text = "NCC: OK" + Environment.NewLine
                                    + FormatNccMatchPercentResultLabel(avgOk);
                            }
                            else
                            {
                                lblNccResult.Text = "NCC: " + L("JudgeOK") + Environment.NewLine + L("AvgNccScore") + ": —";
                            }

                            lblNccResult.ForeColor = Color.LimeGreen;
                        }

                    string matchLine = "Match=—";
                    if (TryComputeNccScoreAverage(r, out double avgOkDraw))
                        matchLine = FormatNccMatchPercentLine(avgOkDraw);
                    SetNccTopInfo(L("JudgeOK"), matchLine, string.Empty, Color.LimeGreen);
                    }
                    else
                    {
                        oneLine = "NG  " + r.JudgmentSummary;
                        if (lblNccResult != null)
                        {
                            if (TryComputeNccScoreAverage(r, out double avgNg))
                            {
                                lblNccResult.Text = "NCC: NG" + Environment.NewLine
                                    + FormatNccMatchPercentResultLabel(avgNg);
                            }
                            else
                            {
                                lblNccResult.Text = "NCC: " + L("JudgeNG") + Environment.NewLine + L("AvgNccScore") + ": —";
                            }

                            lblNccResult.ForeColor = Color.OrangeRed;
                        }

                    string matchLine = "Match=—";
                    if (TryComputeNccScoreAverage(r, out double avgNgDraw))
                        matchLine = FormatNccMatchPercentLine(avgNgDraw);
                    SetNccTopInfo(L("JudgeNG"), matchLine, string.Empty, Color.OrangeRed);
                    }
                }
                else
                {
                    oneLine = "NG  (판정 미수행)";
                    if (lblNccResult != null)
                    {
                        if (TryComputeNccScoreAverage(r, out double avgU))
                        {
                            lblNccResult.Text = "NCC: NG" + Environment.NewLine
                                + FormatNccMatchPercentResultLabel(avgU);
                        }
                        else
                        {
                            lblNccResult.Text = "NCC: " + L("JudgeNG") + Environment.NewLine + L("AvgNccScore") + ": —";
                        }

                        lblNccResult.ForeColor = Color.OrangeRed;
                    }
                }
            }
            finally
            {
                _viewer.CanvasControl.EndOverlayUpdate();
            }

            string detail = "Count=" + r.Count;
            if (TryComputeNccScoreAverage(r, out double avgD))
            {
                detail += "  avgScore=" + avgD.ToString("F3", CultureInfo.InvariantCulture);
            }
            else if (r.Score != null && r.Score.TupleLength() > 0)
            {
                detail += "  Score0=" + r.Score[0].D.ToString("F3", CultureInfo.InvariantCulture);
            }

            detail += "  " + r.JudgmentSummary;

            txtTeachingLog?.AppendText(
                DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                + " NCC 검사  " + oneLine
                + "  (" + detail + ")"
                + Environment.NewLine);
        }
    }
}

