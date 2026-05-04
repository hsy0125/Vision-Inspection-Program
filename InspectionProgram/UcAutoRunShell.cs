using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InspectionProgram.Common;
using InspectionProgram.Halcon;
using ImageViewerWinForms;

namespace InspectionProgram.GUI
{
    /// <summary>
    /// Auto Run UI shell. 검사(Blob) 흐름은 <see cref="BlobInspectRunFlow"/> + <see cref="UcInspectFlowStrip"/>.
    /// </summary>
    public partial class UcAutoRunShell : UserControl
    {
        private LanguageType _currentLanguage = LanguageType.Kr;
        private readonly ImageViewPanelView1 _viewer;
        private readonly Button _btnToolRoi;
        private readonly ToolTip _flowToolTip;
        private BlobInspectRunFlow _blobFlow;

        private Panel _pnlInspectSummary;
        private Label _lblInspectSummaryLine1;
        private Label _lblInspectSummaryLine2;
        private Label _lblInspectSummaryLine3;

        public UcAutoRunShell()
        {
            InitializeComponent();
            _viewer = new ImageViewPanelView1();
            _viewer.Dock = DockStyle.Fill;
            pnlViewerHost.Controls.Add(_viewer);
            InitializeInspectSummaryPanel();

            _btnToolRoi = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(2, 0, 6, 0),
                Name = "btnToolRoi",
                Size = new Size(52, 24),
                TabIndex = 103,
                TabStop = false,
                Tag = "ADD_ROI",
                Text = "ROI+",
                UseVisualStyleBackColor = true,
            };
            _btnToolRoi.Click += btnViewerTool_Click;
            flpViewerToolbar.Controls.Add(_btnToolRoi);
            flpViewerToolbar.Controls.SetChildIndex(_btnToolRoi, 3);

            _blobFlow = new BlobInspectRunFlow(
                this,
                "AutoRun",
                _viewer,
                ucInspectFlowStrip1,
                txtInspectionLog,
                dgvCount,
                tabCamera,
                btnToolLoad,
                btnToolClear,
                _btnToolRoi,
                TryTrackRoiByTeachingNccForImage,
                false,
                SetAutoRunInspectSummaryLines,
                () => TryApplyTeachingInspectionRecipe(showMissingRecipeMessage: true));
            if (ucInspectFlowStrip1 != null && ucInspectFlowStrip1.TrkThreshold != null)
            {
                ucInspectFlowStrip1.TrkThreshold.ValueChanged += (_, __) =>
                    AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.Threshold", () => _blobFlow.DefaultThresholdPreview());
            }

            ViewerShellToolbarExtras.Append(flpViewerToolbar, btnViewerTool_Click);
            ReorderViewerToolbarToMatchTeaching();
            WireViewerToolbarStateRefresh();
            WireImageLoadedForFlow();

            _flowToolTip = new ToolTip { ShowAlways = false };
            _flowToolTip.SetToolTip(btnToolLoad, "1) 이미지·폴더 선택 (같은 폴더 ←/→, Cross 끄면)");
            _flowToolTip.SetToolTip(_btnToolRoi, "1) 사각 ROI — 3) Run inspect 전에 검사 영역을 지정");
            if (ucInspectFlowStrip1 != null)
            {
                if (ucInspectFlowStrip1.LblThresholdTitle != null)
                {
                    _flowToolTip.SetToolTip(ucInspectFlowStrip1.LblThresholdTitle, "2) 밝기 하한(이진화) — Blob 밝기 구간 미리보기");
                }

                if (ucInspectFlowStrip1.TrkThreshold != null)
                {
                    _flowToolTip.SetToolTip(ucInspectFlowStrip1.TrkThreshold, "2) 임계값 드래그");
                }

                if (ucInspectFlowStrip1.BtnRunInspection != null)
                {
                    _flowToolTip.SetToolTip(ucInspectFlowStrip1.BtnRunInspection, "3) 티칭 레시피 적용 후 검사 — NCC 모델 있으면 전역 패턴 카운트, 없으면 Blob");
                }

                if (ucInspectFlowStrip1.ChkAutoCycle != null)
                {
                    _flowToolTip.SetToolTip(ucInspectFlowStrip1.ChkAutoCycle, "체크: 목록에 있는 이미지를 Run 한 번에 순서대로 검사");
                }

                if (ucInspectFlowStrip1.BtnStopBatch != null)
                {
                    _flowToolTip.SetToolTip(ucInspectFlowStrip1.BtnStopBatch, "자동 사이클 중단");
                }

                if (ucInspectFlowStrip1.BtnSaveCsv != null)
                {
                    _flowToolTip.SetToolTip(ucInspectFlowStrip1.BtnSaveCsv, "4) 세션 검사 결과를 CSV로 저장 (NG/OK)");
                }
            }

            ApplyTheme();
            InitializeFakeDataForDesign();
            _blobFlow.ApplyFlowControlStates();
            ApplyLanguage(_currentLanguage);
            this.Disposed += UcAutoRunShell_Disposed;
        }

        /// <summary>
        /// 메인 폼 상단 «검사(Inspection)» 또는 «개수(Count)» 메뉴: 티칭 레시피를 적용한 뒤 검사를 실행합니다(NCC 모델 있으면 전역 패턴 카운트 등 «3) Run inspect»와 동일).
        /// </summary>
        public void RunInspectFromMainToolbarCountMenu()
        {
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.RunInspectFromMainToolbarCountMenu", () =>
            {
                if (!TryApplyTeachingInspectionRecipe(showMissingRecipeMessage: true))
                    return;
                _blobFlow?.ExecuteSameAsRunInspectButton();
            });
        }

        /// <summary>
        /// Inspection Log 창에 표시된 내용 전체를 UTF-8 텍스트 파일(.txt)로 저장합니다.
        /// </summary>
        public void SaveInspectionLogToTextFileWithDialog()
        {
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.SaveInspectionLogToTextFileWithDialog", () =>
            {
                if (txtInspectionLog == null)
                    return;

                string content = txtInspectionLog.Text ?? string.Empty;

                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "텍스트 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
                    dlg.Title = "Inspection Log 저장";
                    InspectionResultLogPaths.EnsureDirectoryExists();
                    dlg.InitialDirectory = InspectionResultLogPaths.GetLogDirectory();
                    dlg.FileName = InspectionResultLogPaths.BuildInspectionLogTxtFileName(DateTime.Now);
                    Form owner = FindForm();
                    if (dlg.ShowDialog(owner) != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.FileName))
                        return;

                    string path = dlg.FileName;
                    try
                    {
                        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "파일을 저장하지 못했습니다: " + ex.Message,
                            "Inspection Log",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    MessageBox.Show(
                        "Inspection Log를 저장했습니다.\r\n\r\n" + path,
                        "Inspection Log",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    try
                    {
                        AppLogger.Write("INSPECT", "Inspection Log txt saved: " + path);
                    }
                    catch
                    {
                    }
                }
            });
        }

        /// <summary>
        /// 티칭에서 저장한 <see cref="InspectionRecipe"/>를 자동 실행 뷰어에 적용합니다.
        /// </summary>
        /// <returns>레시피가 있고 현재 이미지에 ROI를 하나 이상 만들었으면 true.</returns>
        public bool TryApplyTeachingInspectionRecipe(bool showMissingRecipeMessage)
        {
            try
            {
                if (!TeachingInspectionRecipeStore.TryGet(out InspectionRecipe r) || r == null || r.IsEmpty)
                {
                    if (showMissingRecipeMessage)
                    {
                        MessageBox.Show(
                            "티칭에서 «검사 레시피 저장»을 누르거나, NCC 모델 저장 시 함께 저장된 레시피가 필요합니다.",
                            "Auto Run",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    return false;
                }

                if (_viewer?.CanvasControl == null)
                    return false;

                if (!_viewer.HasImage)
                {
                    if (showMissingRecipeMessage)
                    {
                        MessageBox.Show(
                            "먼저 Load로 검사할 이미지를 불러오세요.",
                            "Auto Run",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    return false;
                }

                Size sz = _viewer.CanvasControl.ImagePixelSize;
                if (sz.Width <= 0 || sz.Height <= 0)
                    return false;

                Rectangle imgBounds = new Rectangle(0, 0, sz.Width, sz.Height);
                NccSharedModelState.BeginProgrammaticRoiUpdate();
                try
                {
                    _viewer.CanvasControl.ClearROI();

                    int added = 0;
                    foreach (Rectangle rect in r.RoiRectangles)
                    {
                        Rectangle inter = Rectangle.Intersect(rect, imgBounds);
                        if (inter.Width < 4 || inter.Height < 4)
                            continue;

                        var roi = new ROIRectangle(inter, Color.LimeGreen, 2);
                        _viewer.CanvasControl.AddROI(roi);
                        added++;
                    }

                    if (added == 0)
                    {
                        MessageBox.Show(
                            "저장된 레시피 ROI를 현재 이미지 크기 안에 맞출 수 없습니다. 이미지 해상도가 티칭 때와 다른지 확인하세요.",
                            "Auto Run",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return false;
                    }

                    if (_viewer.CanvasControl.ROIItems != null && _viewer.CanvasControl.ROIItems.Count > 0)
                        _viewer.CanvasControl.SelectROIByName(_viewer.CanvasControl.ROIItems[0].Name);

                    if (ucInspectFlowStrip1?.TrkThreshold != null)
                    {
                        int tv = r.Threshold;
                        tv = Math.Max(ucInspectFlowStrip1.TrkThreshold.Minimum, Math.Min(ucInspectFlowStrip1.TrkThreshold.Maximum, tv));
                        ucInspectFlowStrip1.TrkThreshold.Value = tv;
                        if (ucInspectFlowStrip1.LblThresholdValue != null)
                            ucInspectFlowStrip1.LblThresholdValue.Text = tv.ToString(CultureInfo.InvariantCulture);
                    }

                    _viewer.CanvasControl.SetBlobOptions(true, r.Threshold, 255, r.MinArea, 0, 0, 0, 0, false);
                    _viewer.CanvasControl.ShowBlob_Bright(true, r.Threshold, 255);

                    if (_blobFlow != null)
                    {
                        _blobFlow.ExpectedBlobCount = r.ExpectedBlobCount;
                        _blobFlow.ForegroundPixelMin = r.ForegroundPixelMin;
                        _blobFlow.ForegroundPixelMax = r.ForegroundPixelMax;
                        _blobFlow.NccFilterMinScore = r.NccFilterMinScore;
                        _blobFlow.NccFilterMaxScore = r.NccFilterMaxScore;
                        _blobFlow.RecipeNccModelPath = r.NccModelPath ?? string.Empty;
                        _blobFlow.RecipeNccTemplateWidth = r.NccTemplateWidth;
                        _blobFlow.RecipeNccTemplateHeight = r.NccTemplateHeight;
                        _blobFlow.RecipeNccMinScore = r.NccMinScore;
                    }

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(r.NccModelPath) && System.IO.File.Exists(r.NccModelPath))
                        {
                            double ms = (!double.IsNaN(r.NccMinScore) && r.NccMinScore >= 0.0 && r.NccMinScore <= 1.0)
                                ? r.NccMinScore
                                : 0.75;
                            NccSharedModelState.Set(r.NccModelPath, r.NccTemplateWidth, r.NccTemplateHeight, ms);
                        }
                    }
                    catch
                    {
                    }

                    if (!string.IsNullOrWhiteSpace(r.NccModelPath) && System.IO.File.Exists(r.NccModelPath))
                        NccSharedModelState.SetPatternTeachRoiCountFallback(added);

                    _blobFlow?.DefaultThresholdPreview();
                    _blobFlow?.ApplyFlowControlStates();
                    if (_viewer?.CanvasControl != null)
                        _viewer.CanvasControl.ShowUserRoiOverlay = true;
                    return true;
                }
                finally
                {
                    NccSharedModelState.EndProgrammaticRoiUpdate();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    AppLogger.Write("EX", "TryApplyTeachingInspectionRecipe: " + ex.Message);
                }
                catch
                {
                }

                return false;
            }
        }

        public void ApplyLanguage(LanguageType language)
        {
            try
            {
                _currentLanguage = language;

                if (grpViewer != null)
                    grpViewer.Text = LocalizationService.GetText("AutoRun", language);
                if (grpCount != null)
                    grpCount.Text = LocalizationService.GetText("CountSummary", language);
                if (grpInspectionLog != null)
                    grpInspectionLog.Text = LocalizationService.GetText("InspectionLog", language);
                if (grpSystemLog != null)
                    grpSystemLog.Text = LocalizationService.GetText("SystemLog", language);
                if (lblCurrentDeviceTitle != null)
                    lblCurrentDeviceTitle.Text = LocalizationService.GetText("CurrentDevice", language);

                if (btnToolLoad != null)
                    btnToolLoad.Text = LocalizationService.GetText("OpenImage", language);
                if (btnToolClear != null)
                    btnToolClear.Text = LocalizationService.GetText("ClearImage", language);
                if (btnToolSave != null)
                    btnToolSave.Text = LocalizationService.GetText("SaveImage", language);
                if (btnToolZm != null)
                    btnToolZm.Text = LocalizationService.GetText("ZoomMode", language);
                if (btnToolZIn != null)
                    btnToolZIn.Text = LocalizationService.GetText("ZoomIn", language);
                if (btnToolZOut != null)
                    btnToolZOut.Text = LocalizationService.GetText("ZoomOut", language);
                if (_btnToolRoi != null)
                    _btnToolRoi.Text = LocalizationService.GetText("RoiAdd", language);

                if (ucInspectFlowStrip1 != null)
                    ucInspectFlowStrip1.ApplyLanguage(language);

                if (txtInspectionLog != null)
                    txtInspectionLog.Text = BuildAutoRunFlowHelpText(language);
                if (txtSystemLog != null)
                    txtSystemLog.Text = LocalizationService.GetText("SystemReady", language);

                if (dgvCount != null && dgvCount.Columns.Count >= 5)
                {
                    dgvCount.Columns[0].HeaderText = LocalizationService.GetText("Camera", language);
                    dgvCount.Columns[1].HeaderText = LocalizationService.GetText("Total", language);
                    dgvCount.Columns[2].HeaderText = LocalizationService.GetText("Good", language);
                    dgvCount.Columns[3].HeaderText = LocalizationService.GetText("Reject", language);
                    dgvCount.Columns[4].HeaderText = LocalizationService.GetText("Yield", language);
                }

                for (int i = 0; i < tabCamera.TabPages.Count; i++)
                {
                    tabCamera.TabPages[i].Text =
                        LocalizationService.GetText("Camera", language) + " " + (i + 1).ToString("00", CultureInfo.InvariantCulture);
                }

                if (dgvCount != null)
                {
                    for (int r = 0; r < dgvCount.Rows.Count; r++)
                    {
                        if (dgvCount.Columns.Count > 0 && dgvCount.Rows[r].Cells[0] != null)
                        {
                            dgvCount.Rows[r].Cells[0].Value =
                                LocalizationService.GetText("Camera", language) + " " + (r + 1).ToString("00", CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (lblCurrentDeviceValue != null && tabCamera.SelectedIndex >= 0)
                {
                    int idx = tabCamera.SelectedIndex;
                    lblCurrentDeviceValue.Text =
                        LocalizationService.GetText("Camera", language) + " " + (idx + 1).ToString("00", CultureInfo.InvariantCulture);
                }

                ViewerShellToolbarExtras.ApplyToolbarLanguage(flpViewerToolbar, language);
                _blobFlow?.SetUiLanguage(language);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Teaching 툴바 버튼 순서를 기준으로 AutoRun 툴바 버튼을 재배열합니다.
        /// (AutoRun에 없는 버튼은 무시)
        /// </summary>
        private void ReorderViewerToolbarToMatchTeaching()
        {
            try
            {
                if (flpViewerToolbar == null || flpViewerToolbar.Controls == null || flpViewerToolbar.Controls.Count == 0)
                    return;

                // Teaching 기준 순서: Load, Save, Clear, ZM, +, -, Fit, OvClr, Cross, Gray, Avg, Sync, Map, Light, ROI+
                string[] order =
                {
                    "LOAD",
                    "SAVE",
                    "CLEAR",
                    "ZM",
                    "Z+",
                    "Z-",
                    "FIT",
                    "CLR_OVR",
                    "CROSS",
                    "GRAY",
                    "AVG",
                    "SYNC",
                    "MAP",
                    "LIGHT",
                    "ADD_ROI",
                };

                var buttons = flpViewerToolbar.Controls.OfType<Button>().ToList();
                int idx = 0;
                for (int i = 0; i < order.Length; i++)
                {
                    string tag = order[i];
                    Button b = buttons.FirstOrDefault(x => string.Equals(x.Tag as string, tag, StringComparison.OrdinalIgnoreCase));
                    if (b == null)
                        continue;
                    flpViewerToolbar.Controls.SetChildIndex(b, idx);
                    idx++;
                }
            }
            catch
            {
            }
        }

        private void UcAutoRunShell_Disposed(object sender, EventArgs e)
        {
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

        private void ApplyTheme()
        {
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.ApplyTheme", () =>
            {
                BackColor = AppColors.Background;
                ForeColor = AppColors.Foreground;
                Font = AppFontHelper.Create(9F);

                grpViewer.BackColor = AppColors.Surface;
                grpViewer.ForeColor = AppColors.Foreground;
                grpCount.BackColor = AppColors.Surface;
                grpCount.ForeColor = AppColors.Foreground;
                grpInspectionLog.BackColor = AppColors.Surface;
                grpInspectionLog.ForeColor = AppColors.Foreground;
                grpSystemLog.BackColor = AppColors.Surface;
                grpSystemLog.ForeColor = AppColors.Foreground;

                flpViewerToolbar.BackColor = AppColors.SurfaceDark;
                foreach (Control c in flpViewerToolbar.Controls)
                {
                    Button b = c as Button;
                    if (b == null) continue;
                    b.UseVisualStyleBackColor = false;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = AppColors.Border;
                    b.BackColor = AppColors.SurfaceLight;
                    b.ForeColor = AppColors.Foreground;
                    b.Font = AppFontHelper.Create(8.25F);
                }

                if (ucInspectFlowStrip1 != null)
                {
                    ucInspectFlowStrip1.ApplyAppTheme();
                }

                txtInspectionLog.BackColor = AppColors.SurfaceDark;
                txtInspectionLog.ForeColor = AppColors.Foreground;
                txtSystemLog.BackColor = AppColors.SurfaceDark;
                txtSystemLog.ForeColor = AppColors.Foreground;

                dgvCount.BackgroundColor = AppColors.SurfaceDark;
                dgvCount.BorderStyle = BorderStyle.FixedSingle;
                dgvCount.GridColor = AppColors.Border;
                dgvCount.EnableHeadersVisualStyles = false;
                dgvCount.ColumnHeadersDefaultCellStyle.BackColor = AppColors.SurfaceLight;
                dgvCount.ColumnHeadersDefaultCellStyle.ForeColor = AppColors.Foreground;
                dgvCount.DefaultCellStyle.BackColor = AppColors.SurfaceDark;
                dgvCount.DefaultCellStyle.ForeColor = AppColors.Foreground;
                dgvCount.DefaultCellStyle.SelectionBackColor = AppColors.SelectionHighlight;
                dgvCount.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvCount.RowHeadersVisible = false;
                dgvCount.AllowUserToAddRows = false;
                dgvCount.AllowUserToDeleteRows = false;
                dgvCount.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                tabCamera.DrawMode = TabDrawMode.OwnerDrawFixed;
            });
        }

        private void InitializeFakeDataForDesign()
        {
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.InitializeFakeDataForDesign", () =>
            {
                if (dgvCount.Columns.Count == 0)
                {
                    dgvCount.Columns.Add("Camera", "Camera");
                    dgvCount.Columns.Add("Total", "Total");
                    dgvCount.Columns.Add("Good", "Good");
                    dgvCount.Columns.Add("Reject", "Reject");
                    dgvCount.Columns.Add("Yield", "Yield");
                }

                if (tabCamera.TabPages.Count == 0)
                {
                    for (int c = 1; c <= 4; c++)
                    {
                        tabCamera.TabPages.Add(
                            new TabPage(
                                LocalizationService.GetText("Camera", _currentLanguage) + " " + c.ToString("00", CultureInfo.InvariantCulture)));
                    }
                }

                if (dgvCount.Rows.Count == 0)
                {
                    string cam(LanguageType lang, int n) =>
                        LocalizationService.GetText("Camera", lang) + " " + n.ToString("00", CultureInfo.InvariantCulture);
                    dgvCount.Rows.Add(cam(_currentLanguage, 1), "1000", "950", "50", "95.00%");
                    dgvCount.Rows.Add(cam(_currentLanguage, 2), "1000", "930", "70", "93.00%");
                    dgvCount.Rows.Add(cam(_currentLanguage, 3), "1000", "980", "20", "98.00%");
                    dgvCount.Rows.Add(cam(_currentLanguage, 4), "1000", "970", "30", "97.00%");
                    dgvCount.Rows[0].Selected = true;
                }

                lblCurrentDeviceValue.Text =
                    LocalizationService.GetText("Camera", _currentLanguage) + " " + (1).ToString("00", CultureInfo.InvariantCulture);
                txtInspectionLog.Text = BuildAutoRunFlowHelpText(_currentLanguage);
                txtSystemLog.Text = LocalizationService.GetText("SystemReady", _currentLanguage);
            });
        }

        private static string BuildAutoRunFlowHelpText(LanguageType language)
        {
            return LocalizationService.GetText("InspectionUiFlow", language);
        }

        private static bool IsInspectSummaryDesignTime()
        {
            try
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
            catch
            {
                return false;
            }
        }

        private void InitializeInspectSummaryPanel()
        {
            try
            {
                if (pnlViewerHost == null)
                    return;

                bool design = IsInspectSummaryDesignTime();
                Color back = design ? Color.FromArgb(40, 40, 40) : AppColors.SurfaceDark;
                Color fore = Color.LimeGreen;

                _pnlInspectSummary = new Panel
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

                _lblInspectSummaryLine1 = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = fore,
                    BackColor = back,
                    Font = design ? new Font("Segoe UI", 19f, FontStyle.Regular, GraphicsUnit.Point) : AppFontHelper.Create(19f),
                    Text = string.Empty,
                };
                _lblInspectSummaryLine2 = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = fore,
                    BackColor = back,
                    Font = design ? new Font("Segoe UI", 16f, FontStyle.Regular, GraphicsUnit.Point) : AppFontHelper.Create(16f),
                    Text = string.Empty,
                };
                _lblInspectSummaryLine3 = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = fore,
                    BackColor = back,
                    Font = design ? new Font("Segoe UI", 16f, FontStyle.Regular, GraphicsUnit.Point) : AppFontHelper.Create(16f),
                    Text = string.Empty,
                };

                tlp.Controls.Add(_lblInspectSummaryLine1, 0, 0);
                tlp.Controls.Add(_lblInspectSummaryLine2, 0, 1);
                tlp.Controls.Add(_lblInspectSummaryLine3, 0, 2);
                _pnlInspectSummary.Controls.Add(tlp);

                _pnlInspectSummary.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                pnlViewerHost.Controls.Add(_pnlInspectSummary);
                _pnlInspectSummary.BringToFront();

                pnlViewerHost.Resize -= PnlViewerHost_InspectSummaryResize;
                pnlViewerHost.Resize += PnlViewerHost_InspectSummaryResize;
                UpdateInspectSummaryPanelLayout();

                if (design)
                {
                    SetAutoRunInspectSummaryLines("OK", "일치율 = 96.50%", "개수 = 22");
                }
            }
            catch
            {
            }
        }

        private void PnlViewerHost_InspectSummaryResize(object sender, EventArgs e)
        {
            UpdateInspectSummaryPanelLayout();
        }

        private void UpdateInspectSummaryPanelLayout()
        {
            try
            {
                if (pnlViewerHost == null || _pnlInspectSummary == null)
                    return;

                const int marginLeft = 10;
                const int marginTop = 10;
                _pnlInspectSummary.Location = new Point(marginLeft, marginTop);
                _pnlInspectSummary.BringToFront();
            }
            catch
            {
            }
        }

        private void SetAutoRunInspectSummaryLines(string line1, string line2, string line3)
        {
            try
            {
                if (_lblInspectSummaryLine1 == null || _lblInspectSummaryLine2 == null || _lblInspectSummaryLine3 == null)
                    return;

                Color c = Color.LimeGreen;
                _lblInspectSummaryLine1.ForeColor = c;
                _lblInspectSummaryLine2.ForeColor = c;
                _lblInspectSummaryLine3.ForeColor = c;

                _lblInspectSummaryLine1.Text = line1 ?? string.Empty;
                _lblInspectSummaryLine2.Text = line2 ?? string.Empty;
                _lblInspectSummaryLine3.Text = line3 ?? string.Empty;
            }
            catch
            {
            }
        }

        private void ClearAutoRunInspectSummaryOverlay()
        {
            SetAutoRunInspectSummaryLines(string.Empty, string.Empty, string.Empty);
        }

        private void WireImageLoadedForFlow()
        {
            if (_viewer?.CanvasControl == null)
                return;
            _viewer.RoiCollectionChanged += (_, __) =>
                AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.RoiInv", () =>
                    NccSharedModelState.NotifyUserRoiCollectionChangedMayInvalidatePatternSnapshot());
            _viewer.CanvasControl.ImageLoaded += (_, __) =>
                AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.ImageLoaded", () =>
                {
                    if (_blobFlow == null)
                        return;
                    _blobFlow.ApplyFlowControlStates();
                    // 배치 중에는 사이클 루프에서 이미 DefaultThresholdPreview/검사를 수행 — ImageLoaded 미리보기 이중 호출 방지
                    if (!_blobFlow.IsAutoBatchRunning)
                        _blobFlow.DefaultThresholdPreview();

                    // 티칭 레시피가 있고 아직 ROI가 없으면 자동 적용(폴더 넘김 등으로 ROI가 비었을 때)
                    if (_viewer.CanvasControl.RoiItemCount == 0)
                        TryApplyTeachingInspectionRecipe(showMissingRecipeMessage: false);
                });
        }

        /// <summary>뷰어 «지우기»: 이미지·ROI·세션과 공유 NCC/티칭 레시피 메모리를 비워 두 검사 모드가 남은 상태에 묶이지 않게 합니다.</summary>
        private void ApplyAutoRunFullViewerReset()
        {
            ClearAutoRunInspectSummaryOverlay();
            NccSharedModelState.ResetSession();
            TeachingInspectionRecipeStore.Clear();
            _blobFlow.CancelAutoBatchIfAny();
            _blobFlow.ResetInspectionParametersToDefaults();
            if (ucInspectFlowStrip1?.TrkThreshold != null)
            {
                ucInspectFlowStrip1.TrkThreshold.Value = 128;
                if (ucInspectFlowStrip1.LblThresholdValue != null)
                    ucInspectFlowStrip1.LblThresholdValue.Text = "128";
            }

            _viewer.ClearDisplay();
            _blobFlow.ClearSessionAndFolder();
            _blobFlow.ApplyFlowControlStates();
        }

        private void btnViewerTool_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            string id = b != null ? (b.Tag as string ?? b.Text) : "?";
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.ViewerToolClick", () =>
            {
                if (_blobFlow == null)
                    return;
                switch (id)
                {
                    case "LOAD":
                        _blobFlow.LoadImageOrFolderFromUserChoice();
                        break;
                    case "ADD_ROI":
                        if (_viewer?.CanvasControl == null || !_viewer.HasImage)
                        {
                            MessageBox.Show(
                                LocalizationService.GetText("MsgLoadImageFirst", _currentLanguage),
                                LocalizationService.GetText("AutoRun", _currentLanguage),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            break;
                        }

                        {
                            Size sz = _viewer.CanvasControl.ImagePixelSize;
                            if (sz.Width > 0 && sz.Height > 0)
                            {
                                int w = Math.Max(50, (int)Math.Round(sz.Width * 0.25));
                                int h = Math.Max(50, (int)Math.Round(sz.Height * 0.25));
                                int left = Math.Max(0, (sz.Width - w) / 2);
                                int top = Math.Max(0, (sz.Height - h) / 2);
                                var roi = new ROIRectangle(new Rectangle(left, top, w, h), Color.LimeGreen, 2);
                                _viewer.CanvasControl.AddROI(roi);
                                _viewer.CanvasControl.SelectROIByName(roi.Name);
                            }
                        }

                        break;
                    case "CLEAR":
                        ApplyAutoRunFullViewerReset();
                        break;
                    case "SAVE":
                        _viewer.SaveImageFromDialog();
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

        private string TryTrackRoiByTeachingNccForImage(string imagePath)
        {
            try
            {
                if (_viewer?.CanvasControl == null || _viewer.HasImage == false)
                    return "뷰어 없음";
                if (string.IsNullOrWhiteSpace(imagePath) || System.IO.File.Exists(imagePath) == false)
                    return "이미지 경로 없음";

                // NCC 모델이 없어도 Blob 카운트(Run inspect / 개수 메뉴)는 화면의 ROI로 진행 가능합니다.
                // 모델이 있을 때만 아래에서 패턴 매칭으로 ROI 위치·크기를 맞춥니다.
                if (NccSharedModelState.TryGet(
                        out string model,
                        out double tW,
                        out double tH,
                        out double minScore,
                        out bool hasRefPose,
                        out double refRow,
                        out double refCol,
                        out double refAngle) == false)
                {
                    return null;
                }

                var r = NccPatternInspector.MatchFromModelFile(imagePath, model, 0.5, tW, tH);
                r?.EvaluateJudgment(1, minScore);
                if (r == null || r.Count <= 0)
                    return "결과 없음";
                if (r.IsOk == false)
                    return r.JudgmentSummary;

                NccSharedModelState.BeginProgrammaticRoiUpdate();
                try
                {
                    double row = r.Row[0].D;
                    double col = r.Column[0].D;
                    double angle = r.Angle[0].D;
                    AppLogger.Write(
                        "NCC",
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "AutoRun NCC: {0} row={1:0.00} col={2:0.00} ang={3:0.000} hasRef={4}",
                            System.IO.Path.GetFileName(imagePath), row, col, angle, hasRefPose));

                    if (hasRefPose && NccSharedModelState.AlignImagesEnabled)
                    {
                        Bitmap aligned = NccImageAlignment.AlignImageFileToReferenceBitmap(
                            imagePath,
                            model,
                            tW,
                            tH,
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
                            AppLogger.Write("ALIGN", "AutoRun applied alignment to viewer: " + System.IO.Path.GetFileName(imagePath));
                        }
                        else
                        {
                            AppLogger.Write("ALIGN", "AutoRun alignment skipped: " + (alignEx != null ? alignEx.Message : "aligned=null"));
                        }
                    }

                    var roi = _viewer.CanvasControl.SelectedRoi;
                    if (roi == null && _viewer.CanvasControl.ROIItems != null && _viewer.CanvasControl.ROIItems.Count > 0)
                        roi = _viewer.CanvasControl.ROIItems[0];

                    if (roi == null)
                    {
                        Size sz = _viewer.CanvasControl.ImagePixelSize;
                        int w = (int)System.Math.Round(tW > 0.1 ? tW : System.Math.Max(50, sz.Width * 0.25));
                        int h = (int)System.Math.Round(tH > 0.1 ? tH : System.Math.Max(50, sz.Height * 0.25));
                        int left = System.Math.Max(0, (int)System.Math.Round(col - (w / 2.0)));
                        int top = System.Math.Max(0, (int)System.Math.Round(row - (h / 2.0)));
                        var rect = new ImageViewerWinForms.ROIRectangle(new Rectangle(left, top, w, h), Color.LimeGreen, 2);
                        _viewer.CanvasControl.AddROI(rect);
                        _viewer.CanvasControl.SelectROIByName(rect.Name);
                        roi = rect;
                    }

                    // 이동은 누적 오차를 줄이기 위해 "중심 기준 재설정" 우선
                    if (roi is ImageViewerWinForms.ROIRectangle rr)
                    {
                        Rectangle b0 = rr.GetBounds();
                        int w0 = b0.Width;
                        int h0 = b0.Height;
                        int w = (int)System.Math.Round(tW > 0.1 ? tW : w0);
                        int h = (int)System.Math.Round(tH > 0.1 ? tH : h0);
                        w = System.Math.Max(4, w);
                        h = System.Math.Max(4, h);
                        int left = System.Math.Max(0, (int)System.Math.Round(col - (w / 2.0)));
                        int top = System.Math.Max(0, (int)System.Math.Round(row - (h / 2.0)));
                        rr.Rect = new Rectangle(left, top, w, h);
                        _viewer.CanvasControl.SelectROIByName(rr.Name);
                    }
                    else
                    {
                        Rectangle b = roi.GetBounds();
                        double cx = b.Left + (b.Width / 2.0);
                        double cy = b.Top + (b.Height / 2.0);
                        int dx = (int)System.Math.Round(col - cx);
                        int dy = (int)System.Math.Round(row - cy);
                        if (dx != 0 || dy != 0)
                            roi.Move(dx, dy);
                        _viewer.CanvasControl.SelectROIByName(roi.Name);
                    }

                    _viewer.CanvasControl.Invalidate();
                    return null;
                }
                finally
                {
                    NccSharedModelState.EndProgrammaticRoiUpdate();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void WireViewerToolbarStateRefresh()
        {
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.WireViewerToolbarStateRefresh", () =>
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

        /// <summary>
        /// 십자(Cross)가 켜져 있을 때 포커스가 툴바·그리드 등에 있어도 화살표로 기준점을 이동할 수 있게 합니다.
        /// Cross가 꺼져 있으면 같은 폴더의 이전/다음 이미지로 ← / → 를 사용합니다.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (_viewer?.CanvasControl != null && _viewer.CanvasControl.TryProcessCenterCrossArrowKeys(keyData))
                    return true;
                if (_blobFlow != null)
                {
                    if (keyData == Keys.Left && _blobFlow.TryNavigateImageFolderBatch(-1))
                        return true;
                    if (keyData == Keys.Right && _blobFlow.TryNavigateImageFolderBatch(1))
                        return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void tabCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.CameraTabChanged", () =>
            {
                int idx = tabCamera.SelectedIndex < 0 ? 0 : tabCamera.SelectedIndex;
                lblCurrentDeviceValue.Text =
                    LocalizationService.GetText("Camera", _currentLanguage) + " " + (idx + 1).ToString("00", CultureInfo.InvariantCulture);
            });
        }

        private void tabCamera_DrawItem(object sender, DrawItemEventArgs e)
        {
            AppExceptionHandler.ExecuteBestEffort("UcAutoRunShell.CameraTabDraw", () =>
            {
                if (e.Index < 0 || e.Index >= tabCamera.TabPages.Count)
                    return;

                bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                Color backColor = selected ? AppColors.SelectionHighlight : AppColors.SurfaceDark;
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                Color textColor = selected ? Color.White : AppColors.Foreground;
                TextRenderer.DrawText(e.Graphics, tabCamera.TabPages[e.Index].Text, AppFontHelper.Create(9F), e.Bounds, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            });
        }
    }
}
