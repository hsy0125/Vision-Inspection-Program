using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImageViewerWinForms
{
    public partial class ImageViewPanelBase : UserControl
    {
        private sealed class BlackMenuColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground { get { return Color.Black; } }
            public override Color MenuBorder { get { return Color.FromArgb(80, 80, 80); } }
            public override Color MenuItemBorder { get { return Color.FromArgb(120, 120, 120); } }
            public override Color MenuItemSelected { get { return Color.FromArgb(35, 35, 35); } }
            public override Color MenuItemSelectedGradientBegin { get { return Color.FromArgb(35, 35, 35); } }
            public override Color MenuItemSelectedGradientEnd { get { return Color.FromArgb(35, 35, 35); } }
            public override Color MenuItemPressedGradientBegin { get { return Color.FromArgb(28, 28, 28); } }
            public override Color MenuItemPressedGradientMiddle { get { return Color.FromArgb(28, 28, 28); } }
            public override Color MenuItemPressedGradientEnd { get { return Color.FromArgb(28, 28, 28); } }
            public override Color ImageMarginGradientBegin { get { return Color.Black; } }
            public override Color ImageMarginGradientMiddle { get { return Color.Black; } }
            public override Color ImageMarginGradientEnd { get { return Color.Black; } }
        }

        private const string LegacyIconFilePrefix = "260325_13-44-10_";

        private ToolTip _toolTip;
        private ContextMenuStrip _contextMenu;
        private ToolStripMenuItem _miClearDisplay;
        private ToolStripMenuItem _miClearOverlay;
        private ToolStripMenuItem _miCenterCross;
        private ToolStripMenuItem _miZoomMode;
        private ToolStripMenuItem _miZoomIn;
        private ToolStripMenuItem _miZoomOut;
        private ToolStripMenuItem _miZoomFit;
        private ToolStripMenuItem _miGrayValue;
        private ToolStripMenuItem _miAverageGray;
        private ToolStripMenuItem _miSync;
        private ToolStripMenuItem _miFileLoad;
        private ToolStripMenuItem _miFileSave;
        private ToolStripMenuItem _miMiniMap;
        private string _iconBasePath = string.Empty;
        private int _toolbarHeight;
        private string _viewTitle = "Display View";

        [Category("Action")]
        [Description("Occurs when the view is selected.")]
        public event EventHandler ViewSelected;
        [Category("Action")]
        [Description("Occurs when an external file load is requested.")]
        public event EventHandler FileLoadRequested;

        [Category("Action")]
        [Description("Occurs when an external clear display is requested.")]
        public event EventHandler ClearDisplayRequested;

        [Category("Action")]
        [Description("Occurs when the internal canvas sync state changes.")]
        public event EventHandler SyncStateChanged;

        [Category("Action")]
        [Description("Occurs when the ROI collection changes.")]
        public event EventHandler RoiCollectionChanged;

        [Category("Action")]
        [Description("Occurs when the selected ROI changes.")]
        public event EventHandler SelectedRoiChanged;

        [Browsable(false)]
        public ImageCanvasControl CanvasControl { get; private set; }

        [DefaultValue(0)]
        public int ToolbarHeight
        {
            get { return _toolbarHeight; }
            set
            {
                try
                {
                    _toolbarHeight = value < 0 ? 0 : value;
                    if (tlpRoot != null && tlpRoot.RowStyles.Count > 0)
                        tlpRoot.RowStyles[0].Height = _toolbarHeight;
                    if (tlpToolbar != null)
                        tlpToolbar.Height = _toolbarHeight;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        [DefaultValue("Display View")]
        public string ViewTitle
        {
            get { return _viewTitle; }
            set
            {
                try
                {
                    _viewTitle = string.IsNullOrWhiteSpace(value) ? "Display View" : value;
                    if (CanvasControl != null)
                        CanvasControl.ViewTitle = _viewTitle;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        [Browsable(false)]
        public bool HasImage
        {
            get { return CanvasControl != null && CanvasControl.HasImage; }
        }

        [Browsable(false)]
        public bool IsTargetView
        {
            get { return CanvasControl != null && CanvasControl.IsTargetSelected; }
            set
            {
                try
                {
                    if (CanvasControl != null)
                        CanvasControl.IsTargetSelected = value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        [Browsable(false)]
        public bool SyncEnabled
        {
            get { return ViewSyncManager.IsSyncEnabled; }
            set { ViewSyncManager.IsSyncEnabled = value; }
        }


        /// <summary>
        /// 기본 아이콘 툴바 행 표시 여부. 셸 페이지는 false, 단독 <c>ViewerView</c> 등은 true로 둘 수 있습니다.
        /// </summary>
        public void SetBuiltInToolbarVisible(bool visible)
        {
            try
            {
                int h = visible ? 30 : 0;
                _toolbarHeight = h;
                if (tlpRoot != null && tlpRoot.RowStyles.Count > 0)
                    tlpRoot.RowStyles[0].Height = h;
                if (tlpToolbar != null)
                {
                    tlpToolbar.Height = h;
                    tlpToolbar.Visible = visible;
                }

                Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void AttachToolbarToHost(Control host)
        {
            try
            {
                if (host == null || tlpToolbar == null)
                    return;

                host.SuspendLayout();
                host.Controls.Clear();

                TableLayoutPanel tableHost = host as TableLayoutPanel;
                if (tableHost != null)
                {
                    tableHost.ColumnStyles.Clear();
                    tableHost.RowStyles.Clear();
                    tableHost.ColumnCount = 1;
                    tableHost.RowCount = 1;
                    tableHost.Padding = new Padding(0);
                    tableHost.Margin = new Padding(0);
                    tableHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    tableHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                }

                tlpToolbar.Parent = null;
                tlpToolbar.Dock = DockStyle.Fill;
                tlpToolbar.Margin = new Padding(0);
                tlpToolbar.Visible = true;
                host.Controls.Add(tlpToolbar);
                tlpToolbar.BringToFront();

                _toolbarHeight = 0;
                if (tlpRoot != null && tlpRoot.RowStyles.Count > 0)
                    tlpRoot.RowStyles[0].Height = 0F;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                if (host != null)
                    host.ResumeLayout();
            }
        }

        private bool IsDesignerHosted()
        {
            try
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;

                if (Site != null && Site.DesignMode)
                    return true;

                if (DesignMode)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public ImageViewPanelBase()
        {
            try
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw, true);

                BackColor = Color.FromArgb(45, 45, 45);
                Dock = DockStyle.Fill;
                Margin = new Padding(0);
                Padding = new Padding(0);

                InitializeComponent();

                if (_toolTip == null)
                    _toolTip = new ToolTip();

                InitializeToolbarButtons();

                if (IsDesignerHosted())
                    return;

                SetBuiltInToolbarVisible(false);

                InitializeRuntimeObjects();
                InitializeContextMenu();
                ApplyIconPaths();
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    ViewSyncManager.SyncEnabledChanged -= ViewSyncManager_SyncEnabledChanged;
                }
            }
            catch
            {
            }

            base.Dispose(disposing);
        }

        #region Initialize

        private void InitializeRuntimeObjects()
        {
            if (_toolTip == null)
                _toolTip = new ToolTip();

            CanvasControl = new ImageCanvasControl();
            CanvasControl.Dock = DockStyle.Fill;
            CanvasControl.Margin = new Padding(0);
            CanvasControl.ViewTitle = ViewTitle;
            CanvasControl.ViewSelected += CanvasControl_ViewSelected;
            CanvasControl.ViewStateChanged += CanvasControl_ViewStateChanged;
            CanvasControl.SyncStateChanged += CanvasControl_SyncStateChanged;
            CanvasControl.RoiCollectionChanged += CanvasControl_RoiCollectionChanged;
            CanvasControl.SelectedRoiChanged += CanvasControl_SelectedRoiChanged;
            pnlCanvasHost.Controls.Clear();
            pnlCanvasHost.Controls.Add(CanvasControl);

            ViewSyncManager.SyncEnabledChanged += ViewSyncManager_SyncEnabledChanged;
        }

        private void InitializeToolbarButtons()
        {
            ConfigureButton(btnClearDisplay, "CD", false, "Clear Display");
            ConfigureButton(btnClearOverlay, "CO", false, "Clear Overlay");
            ConfigureButton(btnCenterCross, "CC", true, "Image Center Cross");
            ConfigureButton(btnZoomMode, "ZM", true, "Zoom Active");
            ConfigureButton(btnZoomIn, "+", false, "Zoom In");
            ConfigureButton(btnZoomOut, "-", false, "Zoom Out");
            ConfigureButton(btnZoomFit, "F", false, "Zoom Fit");
            ConfigureButton(btnGrayValue, "G", true, "Gray Value On / Off");
            ConfigureButton(btnAverageGray, "AVG", true, "Average Gray On / Off");
            ConfigureButton(btnSync, "SY", true, "Sync View 1 / View 2");
            ConfigureButton(btnFileLoad, "L", false, "File Load");
            ConfigureButton(btnFileSave, "S", false, "File Save");
            ConfigureButton(btnMiniMap, "MM", true, "Mini Map On / Off");
        }

        private void ConfigureButton(ToolbarIconButton button, string symbol, bool toggleMode, string toolTipText)
        {
            if (button == null)
                return;

            button.Symbol = symbol;
            button.ToggleMode = toggleMode;
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(1);
            button.BackColor = ViewerUiStyle.ToolbarButtonBackColor;
            if (_toolTip != null)
                _toolTip.SetToolTip(button, toolTipText);
        }

        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Renderer = new ToolStripProfessionalRenderer(new BlackMenuColorTable());
            _contextMenu.BackColor = Color.Black;
            _contextMenu.ForeColor = Color.White;
            _contextMenu.ShowImageMargin = true;

            _miClearDisplay = CreateContextMenuItem("Clear Display", btnClearDisplay_Click);
            _miClearOverlay = CreateContextMenuItem("Clear Overlay", btnClearOverlay_Click);
            _miCenterCross = CreateContextMenuItem("Image Center Cross", btnCenterCross_Click);
            _miZoomMode = CreateContextMenuItem("Zoom Active", btnZoomMode_Click);
            _miZoomIn = CreateContextMenuItem("Zoom In", btnZoomIn_Click);
            _miZoomOut = CreateContextMenuItem("Zoom Out", btnZoomOut_Click);
            _miZoomFit = CreateContextMenuItem("Zoom Fit", btnZoomFit_Click);
            _miGrayValue = CreateContextMenuItem("Gray Value", btnGrayValue_Click);
            _miAverageGray = CreateContextMenuItem("Average Gray", btnAverageGray_Click);
            _miSync = CreateContextMenuItem("Sync", btnSync_Click);
            _miFileLoad = CreateContextMenuItem("File Load", btnFileLoad_Click);
            _miFileSave = CreateContextMenuItem("File Save", btnFileSave_Click);
            _miMiniMap = CreateContextMenuItem("Mini Map", btnMiniMap_Click);

            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                _miClearDisplay,
                _miClearOverlay,
                new ToolStripSeparator(),
                _miCenterCross,
                _miZoomMode,
                _miZoomIn,
                _miZoomOut,
                _miZoomFit,
                new ToolStripSeparator(),
                _miGrayValue,
                _miAverageGray,
                _miSync,
                new ToolStripSeparator(),
                _miFileLoad,
                _miFileSave,
                _miMiniMap
            });

            _contextMenu.Opening += ContextMenu_Opening;
            CanvasControl.LongPressContextMenuStrip = _contextMenu;
        }

        private void ContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                UpdateMenuItemIcons();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Public Method

        public void LoadImage(string filePath)
        {
            CanvasControl.LoadImage(filePath);
            UpdateButtonStates();
        }

        public void ClearDisplay()
        {
            CanvasControl.ClearDisplay();
            UpdateButtonStates();
        }

        public void ClearOverlay()
        {
            CanvasControl.ClearOverlay();
            UpdateButtonStates();
        }

        public void ToggleCenterCross()
        {
            CanvasControl.ToggleCenterCross();
            UpdateButtonStates();
        }

        public void ToggleZoomMode()
        {
            CanvasControl.ToggleZoomMode();
            UpdateButtonStates();
        }

        public void ZoomIn()
        {
            CanvasControl.ZoomIn();
            UpdateButtonStates();
        }

        public void ZoomOut()
        {
            CanvasControl.ZoomOut();
            UpdateButtonStates();
        }

        public void ZoomFit()
        {
            CanvasControl.FitToWindow();
            UpdateButtonStates();
        }

        public void ToggleGrayValue()
        {
            CanvasControl.TogglePixelGrayValue();
            UpdateButtonStates();
        }

        public void ToggleAverageGray()
        {
            CanvasControl.ToggleAverageGrayMode();
            UpdateButtonStates();
        }

        public void ToggleMiniMap()
        {
            CanvasControl.ToggleMiniMap();
            UpdateButtonStates();
        }

        public void ToggleSync()
        {
            ViewSyncManager.IsSyncEnabled = !ViewSyncManager.IsSyncEnabled;
        }

        public void LoadImageFromDialog()
        {
            try
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All Files|*.*";
                    if (dlg.ShowDialog() == DialogResult.OK)
                        LoadImage(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ViewTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveImageFromDialog()
        {
            try
            {
                if (HasImage == false)
                    return;

                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpg|TIFF|*.tif";
                    dlg.FileName = GetDefaultSaveFileName();
                    if (dlg.ShowDialog() == DialogResult.OK)
                        CanvasControl.SaveRenderedImage(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ViewTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Event

        protected virtual string GetDefaultSaveFileName()
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(CanvasControl.ImagePath);
                if (string.IsNullOrWhiteSpace(fileName))
                    fileName = ViewTitle.Replace(" ", "_");
                return fileName + "_Result.png";
            }
            catch
            {
                return "Result.png";
            }
        }

        private void CanvasControl_SyncStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (SyncStateChanged != null)
                    SyncStateChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void CanvasControl_RoiCollectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (RoiCollectionChanged != null)
                    RoiCollectionChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void CanvasControl_SelectedRoiChanged(object sender, EventArgs e)
        {
            try
            {
                if (SelectedRoiChanged != null)
                    SelectedRoiChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void CanvasControl_ViewSelected(object sender, EventArgs e)
        {
            EventHandler handler = ViewSelected;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void CanvasControl_ViewStateChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void ViewSyncManager_SyncEnabledChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void btnClearDisplay_Click(object sender, EventArgs e)
        {
            if (ClearDisplayRequested != null)
            {
                ClearDisplayRequested(this, EventArgs.Empty);
                return;
            }

            ExecuteActionSafe(ClearDisplay);
        }
        private void btnClearOverlay_Click(object sender, EventArgs e) { ExecuteActionSafe(ClearOverlay); }
        private void btnCenterCross_Click(object sender, EventArgs e) { ExecuteActionSafe(ToggleCenterCross); }
        private void btnZoomMode_Click(object sender, EventArgs e) { ExecuteActionSafe(ToggleZoomMode); }
        private void btnZoomIn_Click(object sender, EventArgs e) { ExecuteActionSafe(ZoomIn); }
        private void btnZoomOut_Click(object sender, EventArgs e) { ExecuteActionSafe(ZoomOut); }
        private void btnZoomFit_Click(object sender, EventArgs e) { ExecuteActionSafe(ZoomFit); }
        private void btnGrayValue_Click(object sender, EventArgs e) { ExecuteActionSafe(ToggleGrayValue); }
        private void btnAverageGray_Click(object sender, EventArgs e) { ExecuteActionSafe(ToggleAverageGray); }
        private void btnSync_Click(object sender, EventArgs e) { ExecuteActionSafe(ToggleSync); }
        private void btnFileLoad_Click(object sender, EventArgs e)
        {
            if (FileLoadRequested != null)
            {
                FileLoadRequested(this, EventArgs.Empty);
                return;
            }

            ExecuteActionSafe(LoadImageFromDialog);
        }
        private void btnFileSave_Click(object sender, EventArgs e) { ExecuteActionSafe(SaveImageFromDialog); }
        private void btnMiniMap_Click(object sender, EventArgs e) { ExecuteActionSafe(ToggleMiniMap); }

        #endregion

        #region Private Method

        private void ExecuteActionSafe(Action action)
        {
            try
            {
                if (action != null)
                    action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ViewTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ToolStripMenuItem CreateContextMenuItem(string text, EventHandler clickHandler)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text, null, clickHandler);
            item.BackColor = Color.Black;
            item.ForeColor = Color.White;
            item.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            return item;
        }

        private void ApplyIconPaths()
        {
            try
            {
                _iconBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Icons");

                SetButtonIcon(
                    btnClearDisplay,
                    new[] { "Icon_DisplayClear.png" },
                    null,
                    "clear_display.png",
                    null);

                SetButtonIcon(
                    btnClearOverlay,
                    new[] { "Icon_OverlayClear.png" },
                    null,
                    "clear_overlay.png",
                    null);

                SetButtonIcon(
                    btnCenterCross,
                    new[] { "Icon_CenterLine_On.png" },
                    new[] { "Icon_CenterLine_Off.png" },
                    "center_cross_on.png",
                    "center_cross_off.png");

                SetButtonIcon(
                    btnZoomMode,
                    new[] { "Icon_Zoom_On.png" },
                    new[] { "Icon_Zoom_Off.png" },
                    "zoom_mode_on.png",
                    "zoom_mode_off.png");

                SetButtonIcon(
                    btnZoomIn,
                    new[] { "Icon_Zoom_In.png" },
                    null,
                    "zoom_in.png",
                    null);

                SetButtonIcon(
                    btnZoomOut,
                    new[] { "Icon_Zoom_Out.png" },
                    null,
                    "zoom_out.png",
                    null);

                SetButtonIcon(
                    btnZoomFit,
                    new[] { "Icon_Zoom_Fit.png" },
                    null,
                    "zoom_fit.png",
                    null);

                SetButtonIcon(
                    btnGrayValue,
                    new[] { "Icon_GrayValue_On.png" },
                    new[] { "Icon_GrayValue_Off.png" },
                    "gray_value_on.png",
                    "gray_value_off.png");

                SetButtonIcon(
                    btnAverageGray,
                    new[] { "Icon_AverageGray_On.png", "Icon_Avg_On.png" },
                    new[] { "Icon_AverageGray_Off.png", "Icon_Avg_Off.png" },
                    "avg_on.png",
                    "avg_off.png");

                SetButtonIcon(
                    btnSync,
                    new[] { "Icon_Sync_On.png" },
                    new[] { "Icon_Sync_Off.png" },
                    "sync_on.png",
                    "sync_off.png");

                SetButtonIcon(
                    btnFileLoad,
                    new[] { "Icon_ImgOpen.png", "Icon_FileLoad.png" },
                    null,
                    "file_load.png",
                    null);

                SetButtonIcon(
                    btnFileSave,
                    new[] { "Icon_ImgSave.png", "Icon_FileSave.png" },
                    null,
                    "file_save.png",
                    null);

                SetButtonIcon(
                    btnMiniMap,
                    new[] { "Icon_MiniMap_On.png" },
                    new[] { "Icon_MiniMap_Off.png" },
                    "minimap_on.png",
                    "minimap_off.png");

                UpdateMenuItemIcons();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SetButtonIcon(
            ToolbarIconButton button,
            string[] onCandidates,
            string[] offCandidates,
            string legacyOnFileName,
            string legacyOffFileName)
        {
            if (button == null)
                return;

            button.IconPathOn = ResolveExistingIconPath(onCandidates, legacyOnFileName);
            button.IconPathOff = ResolveExistingIconPath(offCandidates, legacyOffFileName);
        }

        private string ResolveExistingIconPath(string[] candidateFileNames, string legacyFileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_iconBasePath))
                    return string.Empty;

                if (candidateFileNames != null)
                {
                    foreach (string candidateFileName in candidateFileNames)
                    {
                        if (string.IsNullOrWhiteSpace(candidateFileName))
                            continue;

                        string candidatePath = Path.Combine(_iconBasePath, candidateFileName);
                        if (File.Exists(candidatePath))
                            return candidatePath;
                    }
                }

                if (string.IsNullOrWhiteSpace(legacyFileName) == false)
                {
                    string legacyPrefixedPath = Path.Combine(_iconBasePath, LegacyIconFilePrefix + legacyFileName);
                    if (File.Exists(legacyPrefixedPath))
                        return legacyPrefixedPath;

                    string legacyPlainPath = Path.Combine(_iconBasePath, legacyFileName);
                    if (File.Exists(legacyPlainPath))
                        return legacyPlainPath;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return string.Empty;
            }
        }

        private void UpdateButtonStates()
        {
            try
            {
                bool hasImage = HasImage;
                btnClearDisplay.Enabled = hasImage;
                btnClearOverlay.Enabled = hasImage;
                btnCenterCross.Enabled = hasImage;
                btnZoomMode.Enabled = hasImage;
                btnZoomIn.Enabled = hasImage;
                btnZoomOut.Enabled = hasImage;
                btnZoomFit.Enabled = hasImage;
                btnGrayValue.Enabled = hasImage;
                btnAverageGray.Enabled = hasImage;
                btnSync.Enabled = true;
                btnFileLoad.Enabled = true;
                btnFileSave.Enabled = hasImage;
                btnMiniMap.Enabled = hasImage;

                if (CanvasControl != null)
                {
                    btnCenterCross.CheckedState = CanvasControl.ShowCenterCross;
                    btnZoomMode.CheckedState = CanvasControl.ZoomModeEnabled;
                    btnMiniMap.CheckedState = CanvasControl.ShowMiniMap;
                    btnGrayValue.CheckedState = CanvasControl.ShowPixelGrayValue;
                    btnAverageGray.CheckedState = CanvasControl.AvgModeEnabled;
                }
                btnSync.CheckedState = ViewSyncManager.IsSyncEnabled;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void UpdateMenuItemIcons()
        {
            try
            {
                SetMenuItemIcon(_miClearDisplay, btnClearDisplay.IconPathOn);
                SetMenuItemIcon(_miClearOverlay, btnClearOverlay.IconPathOn);
                SetMenuItemIcon(_miCenterCross, btnCenterCross.CheckedState ? btnCenterCross.IconPathOn : btnCenterCross.IconPathOff);
                SetMenuItemIcon(_miZoomMode, btnZoomMode.CheckedState ? btnZoomMode.IconPathOn : btnZoomMode.IconPathOff);
                SetMenuItemIcon(_miZoomIn, btnZoomIn.IconPathOn);
                SetMenuItemIcon(_miZoomOut, btnZoomOut.IconPathOn);
                SetMenuItemIcon(_miZoomFit, btnZoomFit.IconPathOn);
                SetMenuItemIcon(_miGrayValue, btnGrayValue.CheckedState ? btnGrayValue.IconPathOn : btnGrayValue.IconPathOff);
                SetMenuItemIcon(_miAverageGray, btnAverageGray.CheckedState ? btnAverageGray.IconPathOn : btnAverageGray.IconPathOff);
                SetMenuItemIcon(_miSync, btnSync.CheckedState ? btnSync.IconPathOn : btnSync.IconPathOff);
                SetMenuItemIcon(_miFileLoad, btnFileLoad.IconPathOn);
                SetMenuItemIcon(_miFileSave, btnFileSave.IconPathOn);
                SetMenuItemIcon(_miMiniMap, btnMiniMap.CheckedState ? btnMiniMap.IconPathOn : btnMiniMap.IconPathOff);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SetMenuItemIcon(ToolStripMenuItem item, string filePath)
        {
            try
            {
                if (item == null)
                    return;

                if (item.Image != null)
                {
                    Image oldImage = item.Image;
                    item.Image = null;
                    oldImage.Dispose();
                }

                if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
                    return;

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (Image src = Image.FromStream(fs))
                {
                    item.Image = new Bitmap(src);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}
