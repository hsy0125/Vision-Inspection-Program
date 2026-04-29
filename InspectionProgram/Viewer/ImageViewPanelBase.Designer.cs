namespace ImageViewerWinForms
{
    partial class ImageViewPanelBase
    {
        private System.ComponentModel.IContainer components = null;
        public System.Windows.Forms.TableLayoutPanel tlpRoot;
        public System.Windows.Forms.TableLayoutPanel tlpToolbar;
        public System.Windows.Forms.Panel pnlCanvasHost;
        public ToolbarIconButton btnClearDisplay;
        public ToolbarIconButton btnClearOverlay;
        public ToolbarIconButton btnCenterCross;
        public ToolbarIconButton btnZoomMode;
        public ToolbarIconButton btnZoomIn;
        public ToolbarIconButton btnZoomOut;
        public ToolbarIconButton btnZoomFit;
        public ToolbarIconButton btnGrayValue;
        public ToolbarIconButton btnAverageGray;
        public ToolbarIconButton btnSync;
        public ToolbarIconButton btnFileLoad;
        public ToolbarIconButton btnFileSave;
        public ToolbarIconButton btnMiniMap;
        private System.Windows.Forms.Panel pnlToolbarSpacer;

        private void InitializeComponent()
        {
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();
            this.tlpToolbar = new System.Windows.Forms.TableLayoutPanel();
            this.btnClearDisplay = new ImageViewerWinForms.ToolbarIconButton();
            this.btnClearOverlay = new ImageViewerWinForms.ToolbarIconButton();
            this.btnCenterCross = new ImageViewerWinForms.ToolbarIconButton();
            this.btnZoomMode = new ImageViewerWinForms.ToolbarIconButton();
            this.btnZoomIn = new ImageViewerWinForms.ToolbarIconButton();
            this.btnZoomOut = new ImageViewerWinForms.ToolbarIconButton();
            this.btnZoomFit = new ImageViewerWinForms.ToolbarIconButton();
            this.btnGrayValue = new ImageViewerWinForms.ToolbarIconButton();
            this.btnAverageGray = new ImageViewerWinForms.ToolbarIconButton();
            this.btnSync = new ImageViewerWinForms.ToolbarIconButton();
            this.btnFileLoad = new ImageViewerWinForms.ToolbarIconButton();
            this.btnFileSave = new ImageViewerWinForms.ToolbarIconButton();
            this.btnMiniMap = new ImageViewerWinForms.ToolbarIconButton();
            this.pnlToolbarSpacer = new System.Windows.Forms.Panel();
            this.pnlCanvasHost = new System.Windows.Forms.Panel();
            this.tlpRoot.SuspendLayout();
            this.tlpToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Controls.Add(this.tlpToolbar, 0, 0);
            this.tlpRoot.Controls.Add(this.pnlCanvasHost, 0, 1);
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Location = new System.Drawing.Point(0, 0);
            this.tlpRoot.Margin = new System.Windows.Forms.Padding(0);
            this.tlpRoot.Name = "tlpRoot";
            this.tlpRoot.RowCount = 2;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Size = new System.Drawing.Size(400, 300);
            this.tlpRoot.TabIndex = 0;
            // 
            // tlpToolbar
            // 
            this.tlpToolbar.BackColor = System.Drawing.Color.Black;
            this.tlpToolbar.ColumnCount = 14;
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpToolbar.Controls.Add(this.btnClearDisplay, 0, 0);
            this.tlpToolbar.Controls.Add(this.btnClearOverlay, 1, 0);
            this.tlpToolbar.Controls.Add(this.btnCenterCross, 2, 0);
            this.tlpToolbar.Controls.Add(this.btnZoomMode, 3, 0);
            this.tlpToolbar.Controls.Add(this.btnZoomIn, 4, 0);
            this.tlpToolbar.Controls.Add(this.btnZoomOut, 5, 0);
            this.tlpToolbar.Controls.Add(this.btnZoomFit, 6, 0);
            this.tlpToolbar.Controls.Add(this.btnGrayValue, 7, 0);
            this.tlpToolbar.Controls.Add(this.btnAverageGray, 8, 0);
            this.tlpToolbar.Controls.Add(this.btnSync, 9, 0);
            this.tlpToolbar.Controls.Add(this.btnFileLoad, 10, 0);
            this.tlpToolbar.Controls.Add(this.btnFileSave, 11, 0);
            this.tlpToolbar.Controls.Add(this.btnMiniMap, 12, 0);
            this.tlpToolbar.Controls.Add(this.pnlToolbarSpacer, 13, 0);
            this.tlpToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpToolbar.Location = new System.Drawing.Point(0, 0);
            this.tlpToolbar.Margin = new System.Windows.Forms.Padding(0);
            this.tlpToolbar.Name = "tlpToolbar";
            this.tlpToolbar.Padding = new System.Windows.Forms.Padding(1);
            this.tlpToolbar.RowCount = 1;
            this.tlpToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpToolbar.Size = new System.Drawing.Size(400, 30);
            this.tlpToolbar.TabIndex = 0;
            // 
            // btnClearDisplay
            // 
            this.btnClearDisplay.Name = "btnClearDisplay";
            this.btnClearDisplay.TabIndex = 0;
            this.btnClearDisplay.Click += new System.EventHandler(this.btnClearDisplay_Click);
            // 
            // btnClearOverlay
            // 
            this.btnClearOverlay.Name = "btnClearOverlay";
            this.btnClearOverlay.TabIndex = 1;
            this.btnClearOverlay.Click += new System.EventHandler(this.btnClearOverlay_Click);
            // 
            // btnCenterCross
            // 
            this.btnCenterCross.Name = "btnCenterCross";
            this.btnCenterCross.TabIndex = 2;
            this.btnCenterCross.Click += new System.EventHandler(this.btnCenterCross_Click);
            // 
            // btnZoomMode
            // 
            this.btnZoomMode.Name = "btnZoomMode";
            this.btnZoomMode.TabIndex = 3;
            this.btnZoomMode.Click += new System.EventHandler(this.btnZoomMode_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.TabIndex = 4;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.TabIndex = 5;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomFit
            // 
            this.btnZoomFit.Name = "btnZoomFit";
            this.btnZoomFit.TabIndex = 6;
            this.btnZoomFit.Click += new System.EventHandler(this.btnZoomFit_Click);
            // 
            // btnGrayValue
            // 
            this.btnGrayValue.Name = "btnGrayValue";
            this.btnGrayValue.TabIndex = 7;
            this.btnGrayValue.Click += new System.EventHandler(this.btnGrayValue_Click);
            // 
            // btnAverageGray
            // 
            this.btnAverageGray.Name = "btnAverageGray";
            this.btnAverageGray.TabIndex = 8;
            this.btnAverageGray.Click += new System.EventHandler(this.btnAverageGray_Click);
            // 
            // btnSync
            // 
            this.btnSync.Name = "btnSync";
            this.btnSync.TabIndex = 9;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnFileLoad
            // 
            this.btnFileLoad.Name = "btnFileLoad";
            this.btnFileLoad.TabIndex = 10;
            this.btnFileLoad.Click += new System.EventHandler(this.btnFileLoad_Click);
            // 
            // btnFileSave
            // 
            this.btnFileSave.Name = "btnFileSave";
            this.btnFileSave.TabIndex = 11;
            this.btnFileSave.Click += new System.EventHandler(this.btnFileSave_Click);
            // 
            // btnMiniMap
            // 
            this.btnMiniMap.Name = "btnMiniMap";
            this.btnMiniMap.TabIndex = 12;
            this.btnMiniMap.Click += new System.EventHandler(this.btnMiniMap_Click);
            // 
            // pnlToolbarSpacer
            // 
            this.pnlToolbarSpacer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlToolbarSpacer.Location = new System.Drawing.Point(392, 2);
            this.pnlToolbarSpacer.Margin = new System.Windows.Forms.Padding(1);
            this.pnlToolbarSpacer.Name = "pnlToolbarSpacer";
            this.pnlToolbarSpacer.Size = new System.Drawing.Size(6, 26);
            this.pnlToolbarSpacer.TabIndex = 13;
            // 
            // pnlCanvasHost
            // 
            this.pnlCanvasHost.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this.pnlCanvasHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCanvasHost.Location = new System.Drawing.Point(0, 30);
            this.pnlCanvasHost.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCanvasHost.Name = "pnlCanvasHost";
            this.pnlCanvasHost.Size = new System.Drawing.Size(400, 270);
            this.pnlCanvasHost.TabIndex = 1;
            // 
            // ImageViewPanelBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this.Controls.Add(this.tlpRoot);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ImageViewPanelBase";
            this.Size = new System.Drawing.Size(400, 300);
            this.tlpRoot.ResumeLayout(false);
            this.tlpToolbar.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
