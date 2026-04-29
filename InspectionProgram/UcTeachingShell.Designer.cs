namespace InspectionProgram.GUI
{
    partial class UcTeachingShell
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpViewer;
        private System.Windows.Forms.SplitContainer splitViewerToolbar;
        private System.Windows.Forms.TableLayoutPanel tlpViewerRoot;
        private System.Windows.Forms.FlowLayoutPanel flpViewerToolbar;
        private System.Windows.Forms.Button btnToolLoad;
        private System.Windows.Forms.Button btnToolSave;
        private System.Windows.Forms.Button btnToolClear;
        private System.Windows.Forms.Button btnToolZm;
        private System.Windows.Forms.Button btnToolZIn;
        private System.Windows.Forms.Button btnToolZOut;
        private System.Windows.Forms.Button btnToolFit;
        private System.Windows.Forms.Button btnToolOvClr;
        private System.Windows.Forms.Button btnToolCross;
        private System.Windows.Forms.Button btnToolGray;
        private System.Windows.Forms.Button btnToolAvg;
        private System.Windows.Forms.Button btnToolSync;
        private System.Windows.Forms.Button btnToolMap;
        private System.Windows.Forms.Button btnAddRoiRect;
        private System.Windows.Forms.Panel pnlViewerHost;
        private System.Windows.Forms.Panel pnlViewerBottom;
        private UcInspectFlowStrip ucInspectFlowStrip1;
        private System.Windows.Forms.TabControl tabCamera;
        private System.Windows.Forms.Button btnDrawString;
        private System.Windows.Forms.Panel pnlTeachingRightHost;
        private System.Windows.Forms.FlowLayoutPanel flpTeachingRightStack;
        private System.Windows.Forms.FlowLayoutPanel flowRowRecipeSave;
        private System.Windows.Forms.FlowLayoutPanel flowRowNccTitle;
        private System.Windows.Forms.FlowLayoutPanel flowRowNccBtn;
        private System.Windows.Forms.FlowLayoutPanel flowRowNccMin;
        private System.Windows.Forms.FlowLayoutPanel flowRowNccCountBtn;
        private System.Windows.Forms.FlowLayoutPanel flowRowNccCMin;
        private System.Windows.Forms.FlowLayoutPanel flowRowNccCMax;
        private System.Windows.Forms.FlowLayoutPanel flowRowNccCountJudgeMin;
        private System.Windows.Forms.FlowLayoutPanel flowRowBlobCount;
        private System.Windows.Forms.FlowLayoutPanel flowRowMinArea;
        private System.Windows.Forms.FlowLayoutPanel flowRowExpected;
        private System.Windows.Forms.Label lblNccHeader;
        private System.Windows.Forms.Label lblNccResult;
        private System.Windows.Forms.Button btnNccSaveModel;
        private System.Windows.Forms.Button btnNccRunInspect;
        private System.Windows.Forms.Button btnNccCount;
        private System.Windows.Forms.Button btnSaveInspectionRecipe;
        private System.Windows.Forms.NumericUpDown nudNccMinScore;
        private System.Windows.Forms.Label lblNccMinTitle;
        private System.Windows.Forms.Label lblNccCountMinTitle;
        private System.Windows.Forms.Label lblNccCountMaxTitle;
        private System.Windows.Forms.Label lblNccCountJudgeMinTitle;
        private System.Windows.Forms.Label lblNccCountMinVal;
        private System.Windows.Forms.Label lblNccCountMaxVal;
        private System.Windows.Forms.TrackBar trkNccCountMin;
        private System.Windows.Forms.TrackBar trkNccCountMax;
        private System.Windows.Forms.NumericUpDown nudNccCountJudgeMin;
        private System.Windows.Forms.Button btnBlobCount;
        private System.Windows.Forms.FlowLayoutPanel flowRowFgPixelRange;
        private System.Windows.Forms.Label lblFgPixelRangeTitle;
        private System.Windows.Forms.NumericUpDown numFgPixelMin;
        private System.Windows.Forms.Label lblFgPixelRangeSep;
        private System.Windows.Forms.NumericUpDown numFgPixelMax;
        private System.Windows.Forms.Label lblMinAreaTitle;
        private System.Windows.Forms.TrackBar trkMinArea;
        private System.Windows.Forms.Label lblMinArea;
        private System.Windows.Forms.Label lblExpectedTitle;
        private System.Windows.Forms.NumericUpDown numExpected;
        private System.Windows.Forms.GroupBox grpTeachingLog;
        private System.Windows.Forms.TextBox txtTeachingLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.grpViewer = new System.Windows.Forms.GroupBox();
            this.splitViewerToolbar = new System.Windows.Forms.SplitContainer();
            this.flpViewerToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnToolLoad = new System.Windows.Forms.Button();
            this.btnToolSave = new System.Windows.Forms.Button();
            this.btnToolClear = new System.Windows.Forms.Button();
            this.btnToolZm = new System.Windows.Forms.Button();
            this.btnToolZIn = new System.Windows.Forms.Button();
            this.btnToolZOut = new System.Windows.Forms.Button();
            this.btnToolFit = new System.Windows.Forms.Button();
            this.btnToolOvClr = new System.Windows.Forms.Button();
            this.btnToolCross = new System.Windows.Forms.Button();
            this.btnToolGray = new System.Windows.Forms.Button();
            this.btnToolAvg = new System.Windows.Forms.Button();
            this.btnToolSync = new System.Windows.Forms.Button();
            this.btnToolMap = new System.Windows.Forms.Button();
            this.btnAddRoiRect = new System.Windows.Forms.Button();
            this.tlpViewerRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pnlViewerHost = new System.Windows.Forms.Panel();
            this.pnlViewerBottom = new System.Windows.Forms.Panel();
            this.btnDrawString = new System.Windows.Forms.Button();
            this.tabCamera = new System.Windows.Forms.TabControl();
            this.pnlTeachingRightHost = new System.Windows.Forms.Panel();
            this.flpTeachingRightStack = new System.Windows.Forms.FlowLayoutPanel();
            this.flowRowRecipeSave = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSaveInspectionRecipe = new System.Windows.Forms.Button();
            this.flowRowNccTitle = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNccHeader = new System.Windows.Forms.Label();
            this.lblNccResult = new System.Windows.Forms.Label();
            this.flowRowNccBtn = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNccSaveModel = new System.Windows.Forms.Button();
            this.btnNccRunInspect = new System.Windows.Forms.Button();
            this.flowRowNccMin = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNccMinTitle = new System.Windows.Forms.Label();
            this.nudNccMinScore = new System.Windows.Forms.NumericUpDown();
            this.flowRowNccCountBtn = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNccCount = new System.Windows.Forms.Button();
            this.flowRowNccCMin = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNccCountMinTitle = new System.Windows.Forms.Label();
            this.trkNccCountMin = new System.Windows.Forms.TrackBar();
            this.lblNccCountMinVal = new System.Windows.Forms.Label();
            this.flowRowNccCMax = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNccCountMaxTitle = new System.Windows.Forms.Label();
            this.trkNccCountMax = new System.Windows.Forms.TrackBar();
            this.lblNccCountMaxVal = new System.Windows.Forms.Label();
            this.flowRowNccCountJudgeMin = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNccCountJudgeMinTitle = new System.Windows.Forms.Label();
            this.nudNccCountJudgeMin = new System.Windows.Forms.NumericUpDown();
            this.flowRowBlobCount = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBlobCount = new System.Windows.Forms.Button();
            this.flowRowMinArea = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMinAreaTitle = new System.Windows.Forms.Label();
            this.trkMinArea = new System.Windows.Forms.TrackBar();
            this.lblMinArea = new System.Windows.Forms.Label();
            this.flowRowExpected = new System.Windows.Forms.FlowLayoutPanel();
            this.lblExpectedTitle = new System.Windows.Forms.Label();
            this.numExpected = new System.Windows.Forms.NumericUpDown();
            this.flowRowFgPixelRange = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFgPixelRangeTitle = new System.Windows.Forms.Label();
            this.numFgPixelMin = new System.Windows.Forms.NumericUpDown();
            this.lblFgPixelRangeSep = new System.Windows.Forms.Label();
            this.numFgPixelMax = new System.Windows.Forms.NumericUpDown();
            this.grpTeachingLog = new System.Windows.Forms.GroupBox();
            this.txtTeachingLog = new System.Windows.Forms.TextBox();
            this.ucInspectFlowStrip1 = new InspectionProgram.GUI.UcInspectFlowStrip();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitViewerToolbar)).BeginInit();
            this.splitViewerToolbar.Panel1.SuspendLayout();
            this.splitViewerToolbar.Panel2.SuspendLayout();
            this.splitViewerToolbar.SuspendLayout();
            this.flpViewerToolbar.SuspendLayout();
            this.tlpViewerRoot.SuspendLayout();
            this.pnlViewerBottom.SuspendLayout();
            this.pnlTeachingRightHost.SuspendLayout();
            this.flpTeachingRightStack.SuspendLayout();
            this.flowRowRecipeSave.SuspendLayout();
            this.flowRowNccTitle.SuspendLayout();
            this.flowRowNccBtn.SuspendLayout();
            this.flowRowNccMin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNccMinScore)).BeginInit();
            this.flowRowNccCountBtn.SuspendLayout();
            this.flowRowNccCMin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkNccCountMin)).BeginInit();
            this.flowRowNccCMax.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkNccCountMax)).BeginInit();
            this.flowRowNccCountJudgeMin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNccCountJudgeMin)).BeginInit();
            this.flowRowBlobCount.SuspendLayout();
            this.flowRowMinArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkMinArea)).BeginInit();
            this.flowRowExpected.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExpected)).BeginInit();
            this.flowRowFgPixelRange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFgPixelMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFgPixelMax)).BeginInit();
            this.grpTeachingLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.grpViewer);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pnlTeachingRightHost);
            this.splitMain.Panel2.Controls.Add(this.grpTeachingLog);
            this.splitMain.Size = new System.Drawing.Size(1500, 820);
            this.splitMain.SplitterDistance = 977;
            this.splitMain.TabIndex = 0;
            // 
            // grpViewer
            // 
            this.grpViewer.Controls.Add(this.splitViewerToolbar);
            this.grpViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpViewer.Location = new System.Drawing.Point(0, 0);
            this.grpViewer.Name = "grpViewer";
            this.grpViewer.Size = new System.Drawing.Size(977, 820);
            this.grpViewer.TabIndex = 0;
            this.grpViewer.TabStop = false;
            this.grpViewer.Text = "Teaching";
            // 
            // splitViewerToolbar
            // 
            this.splitViewerToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitViewerToolbar.Location = new System.Drawing.Point(3, 17);
            this.splitViewerToolbar.Name = "splitViewerToolbar";
            this.splitViewerToolbar.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitViewerToolbar.Panel1
            // 
            this.splitViewerToolbar.Panel1.Controls.Add(this.flpViewerToolbar);
            this.splitViewerToolbar.Panel1MinSize = 24;
            // 
            // splitViewerToolbar.Panel2
            // 
            this.splitViewerToolbar.Panel2.Controls.Add(this.tlpViewerRoot);
            this.splitViewerToolbar.Panel2MinSize = 160;
            this.splitViewerToolbar.Size = new System.Drawing.Size(971, 800);
            this.splitViewerToolbar.SplitterDistance = 32;
            this.splitViewerToolbar.SplitterWidth = 6;
            this.splitViewerToolbar.TabIndex = 1;
            // 
            // flpViewerToolbar
            // 
            this.flpViewerToolbar.AutoScroll = true;
            this.flpViewerToolbar.Controls.Add(this.btnToolLoad);
            this.flpViewerToolbar.Controls.Add(this.btnToolSave);
            this.flpViewerToolbar.Controls.Add(this.btnToolClear);
            this.flpViewerToolbar.Controls.Add(this.btnToolZm);
            this.flpViewerToolbar.Controls.Add(this.btnToolZIn);
            this.flpViewerToolbar.Controls.Add(this.btnToolZOut);
            this.flpViewerToolbar.Controls.Add(this.btnToolFit);
            this.flpViewerToolbar.Controls.Add(this.btnToolOvClr);
            this.flpViewerToolbar.Controls.Add(this.btnToolCross);
            this.flpViewerToolbar.Controls.Add(this.btnToolGray);
            this.flpViewerToolbar.Controls.Add(this.btnToolAvg);
            this.flpViewerToolbar.Controls.Add(this.btnToolSync);
            this.flpViewerToolbar.Controls.Add(this.btnToolMap);
            this.flpViewerToolbar.Controls.Add(this.btnAddRoiRect);
            this.flpViewerToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpViewerToolbar.Location = new System.Drawing.Point(0, 0);
            this.flpViewerToolbar.Margin = new System.Windows.Forms.Padding(0);
            this.flpViewerToolbar.Name = "flpViewerToolbar";
            this.flpViewerToolbar.Padding = new System.Windows.Forms.Padding(6, 6, 6, 4);
            this.flpViewerToolbar.Size = new System.Drawing.Size(971, 32);
            this.flpViewerToolbar.TabIndex = 0;
            this.flpViewerToolbar.WrapContents = false;
            // 
            // btnToolLoad
            // 
            this.btnToolLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolLoad.Location = new System.Drawing.Point(8, 6);
            this.btnToolLoad.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolLoad.Name = "btnToolLoad";
            this.btnToolLoad.Size = new System.Drawing.Size(83, 24);
            this.btnToolLoad.TabIndex = 10;
            this.btnToolLoad.TabStop = false;
            this.btnToolLoad.Tag = "LOAD";
            this.btnToolLoad.Text = "Load";
            this.btnToolLoad.UseVisualStyleBackColor = true;
            this.btnToolLoad.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolSave
            // 
            this.btnToolSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolSave.Location = new System.Drawing.Point(99, 6);
            this.btnToolSave.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolSave.Name = "btnToolSave";
            this.btnToolSave.Size = new System.Drawing.Size(70, 24);
            this.btnToolSave.TabIndex = 11;
            this.btnToolSave.TabStop = false;
            this.btnToolSave.Tag = "SAVE";
            this.btnToolSave.Text = "Save";
            this.btnToolSave.UseVisualStyleBackColor = true;
            this.btnToolSave.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolClear
            // 
            this.btnToolClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolClear.Location = new System.Drawing.Point(177, 6);
            this.btnToolClear.Margin = new System.Windows.Forms.Padding(2, 0, 12, 0);
            this.btnToolClear.Name = "btnToolClear";
            this.btnToolClear.Size = new System.Drawing.Size(76, 24);
            this.btnToolClear.TabIndex = 12;
            this.btnToolClear.TabStop = false;
            this.btnToolClear.Tag = "CLEAR";
            this.btnToolClear.Text = "Clear";
            this.btnToolClear.UseVisualStyleBackColor = true;
            this.btnToolClear.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolZm
            // 
            this.btnToolZm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolZm.Location = new System.Drawing.Point(267, 6);
            this.btnToolZm.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolZm.Name = "btnToolZm";
            this.btnToolZm.Size = new System.Drawing.Size(40, 24);
            this.btnToolZm.TabIndex = 13;
            this.btnToolZm.TabStop = false;
            this.btnToolZm.Tag = "ZM";
            this.btnToolZm.Text = "ZM";
            this.btnToolZm.UseVisualStyleBackColor = true;
            this.btnToolZm.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolZIn
            // 
            this.btnToolZIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolZIn.Location = new System.Drawing.Point(315, 6);
            this.btnToolZIn.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolZIn.Name = "btnToolZIn";
            this.btnToolZIn.Size = new System.Drawing.Size(40, 24);
            this.btnToolZIn.TabIndex = 14;
            this.btnToolZIn.TabStop = false;
            this.btnToolZIn.Tag = "Z+";
            this.btnToolZIn.Text = "+";
            this.btnToolZIn.UseVisualStyleBackColor = true;
            this.btnToolZIn.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolZOut
            // 
            this.btnToolZOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolZOut.Location = new System.Drawing.Point(363, 6);
            this.btnToolZOut.Margin = new System.Windows.Forms.Padding(2, 0, 12, 0);
            this.btnToolZOut.Name = "btnToolZOut";
            this.btnToolZOut.Size = new System.Drawing.Size(40, 24);
            this.btnToolZOut.TabIndex = 15;
            this.btnToolZOut.TabStop = false;
            this.btnToolZOut.Tag = "Z-";
            this.btnToolZOut.Text = "-";
            this.btnToolZOut.UseVisualStyleBackColor = true;
            this.btnToolZOut.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolFit
            // 
            this.btnToolFit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolFit.Location = new System.Drawing.Point(417, 6);
            this.btnToolFit.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolFit.Name = "btnToolFit";
            this.btnToolFit.Size = new System.Drawing.Size(40, 24);
            this.btnToolFit.TabIndex = 16;
            this.btnToolFit.TabStop = false;
            this.btnToolFit.Tag = "FIT";
            this.btnToolFit.Text = "Fit";
            this.btnToolFit.UseVisualStyleBackColor = true;
            this.btnToolFit.Click += new System.EventHandler(this.btnToolFit_Click);
            // 
            // btnToolOvClr
            // 
            this.btnToolOvClr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolOvClr.Location = new System.Drawing.Point(465, 6);
            this.btnToolOvClr.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolOvClr.Name = "btnToolOvClr";
            this.btnToolOvClr.Size = new System.Drawing.Size(65, 24);
            this.btnToolOvClr.TabIndex = 17;
            this.btnToolOvClr.TabStop = false;
            this.btnToolOvClr.Tag = "CLR_OVR";
            this.btnToolOvClr.Text = "OvClr";
            this.btnToolOvClr.UseVisualStyleBackColor = true;
            this.btnToolOvClr.Click += new System.EventHandler(this.btnToolOvClr_Click);
            // 
            // btnToolCross
            // 
            this.btnToolCross.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolCross.Location = new System.Drawing.Point(538, 6);
            this.btnToolCross.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolCross.Name = "btnToolCross";
            this.btnToolCross.Size = new System.Drawing.Size(62, 24);
            this.btnToolCross.TabIndex = 18;
            this.btnToolCross.TabStop = false;
            this.btnToolCross.Tag = "CROSS";
            this.btnToolCross.Text = "Cross";
            this.btnToolCross.UseVisualStyleBackColor = true;
            this.btnToolCross.Click += new System.EventHandler(this.btnToolCross_Click);
            // 
            // btnToolGray
            // 
            this.btnToolGray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolGray.Location = new System.Drawing.Point(608, 6);
            this.btnToolGray.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolGray.Name = "btnToolGray";
            this.btnToolGray.Size = new System.Drawing.Size(44, 24);
            this.btnToolGray.TabIndex = 19;
            this.btnToolGray.TabStop = false;
            this.btnToolGray.Tag = "GRAY";
            this.btnToolGray.Text = "Gray";
            this.btnToolGray.UseVisualStyleBackColor = true;
            this.btnToolGray.Click += new System.EventHandler(this.btnToolGray_Click);
            // 
            // btnToolAvg
            // 
            this.btnToolAvg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolAvg.Location = new System.Drawing.Point(660, 6);
            this.btnToolAvg.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolAvg.Name = "btnToolAvg";
            this.btnToolAvg.Size = new System.Drawing.Size(40, 24);
            this.btnToolAvg.TabIndex = 20;
            this.btnToolAvg.TabStop = false;
            this.btnToolAvg.Tag = "AVG";
            this.btnToolAvg.Text = "Avg";
            this.btnToolAvg.UseVisualStyleBackColor = true;
            this.btnToolAvg.Click += new System.EventHandler(this.btnToolAvg_Click);
            // 
            // btnToolSync
            // 
            this.btnToolSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolSync.Location = new System.Drawing.Point(708, 6);
            this.btnToolSync.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnToolSync.Name = "btnToolSync";
            this.btnToolSync.Size = new System.Drawing.Size(44, 24);
            this.btnToolSync.TabIndex = 21;
            this.btnToolSync.TabStop = false;
            this.btnToolSync.Tag = "SYNC";
            this.btnToolSync.Text = "Sync";
            this.btnToolSync.UseVisualStyleBackColor = true;
            this.btnToolSync.Click += new System.EventHandler(this.btnToolSync_Click);
            // 
            // btnToolMap
            // 
            this.btnToolMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolMap.Location = new System.Drawing.Point(760, 6);
            this.btnToolMap.Margin = new System.Windows.Forms.Padding(2, 0, 12, 0);
            this.btnToolMap.Name = "btnToolMap";
            this.btnToolMap.Size = new System.Drawing.Size(40, 24);
            this.btnToolMap.TabIndex = 22;
            this.btnToolMap.TabStop = false;
            this.btnToolMap.Tag = "MAP";
            this.btnToolMap.Text = "Map";
            this.btnToolMap.UseVisualStyleBackColor = true;
            this.btnToolMap.Click += new System.EventHandler(this.btnToolMap_Click);
            // 
            // btnAddRoiRect
            // 
            this.btnAddRoiRect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddRoiRect.Location = new System.Drawing.Point(814, 6);
            this.btnAddRoiRect.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);
            this.btnAddRoiRect.Name = "btnAddRoiRect";
            this.btnAddRoiRect.Size = new System.Drawing.Size(60, 24);
            this.btnAddRoiRect.TabIndex = 21;
            this.btnAddRoiRect.Text = "ROI+";
            this.btnAddRoiRect.UseVisualStyleBackColor = true;
            this.btnAddRoiRect.Click += new System.EventHandler(this.btnAddRoiRect_Click);
            // 
            // tlpViewerRoot
            // 
            this.tlpViewerRoot.ColumnCount = 1;
            this.tlpViewerRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpViewerRoot.Controls.Add(this.ucInspectFlowStrip1, 0, 0);
            this.tlpViewerRoot.Controls.Add(this.pnlViewerHost, 0, 1);
            this.tlpViewerRoot.Controls.Add(this.pnlViewerBottom, 0, 2);
            this.tlpViewerRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpViewerRoot.Location = new System.Drawing.Point(0, 0);
            this.tlpViewerRoot.Name = "tlpViewerRoot";
            this.tlpViewerRoot.RowCount = 3;
            this.tlpViewerRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpViewerRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpViewerRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tlpViewerRoot.Size = new System.Drawing.Size(971, 762);
            this.tlpViewerRoot.TabIndex = 0;
            // 
            // pnlViewerHost
            // 
            this.pnlViewerHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewerHost.Location = new System.Drawing.Point(3, 43);
            this.pnlViewerHost.Name = "pnlViewerHost";
            this.pnlViewerHost.Size = new System.Drawing.Size(965, 678);
            this.pnlViewerHost.TabIndex = 1;
            // 
            // pnlViewerBottom
            // 
            this.pnlViewerBottom.Controls.Add(this.btnDrawString);
            this.pnlViewerBottom.Controls.Add(this.tabCamera);
            this.pnlViewerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewerBottom.Location = new System.Drawing.Point(0, 724);
            this.pnlViewerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.pnlViewerBottom.Name = "pnlViewerBottom";
            this.pnlViewerBottom.Size = new System.Drawing.Size(971, 38);
            this.pnlViewerBottom.TabIndex = 2;
            // 
            // btnDrawString
            // 
            this.btnDrawString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDrawString.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDrawString.Location = new System.Drawing.Point(854, 6);
            this.btnDrawString.Name = "btnDrawString";
            this.btnDrawString.Size = new System.Drawing.Size(100, 26);
            this.btnDrawString.TabIndex = 1;
            this.btnDrawString.Text = "ROI Save";
            this.btnDrawString.UseVisualStyleBackColor = true;
            this.btnDrawString.Click += new System.EventHandler(this.btnDrawString_Click);
            // 
            // tabCamera
            // 
            this.tabCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCamera.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabCamera.Location = new System.Drawing.Point(0, 0);
            this.tabCamera.Name = "tabCamera";
            this.tabCamera.SelectedIndex = 0;
            this.tabCamera.Size = new System.Drawing.Size(971, 38);
            this.tabCamera.TabIndex = 0;
            this.tabCamera.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabCamera_DrawItem);
            this.tabCamera.SelectedIndexChanged += new System.EventHandler(this.tabCamera_SelectedIndexChanged);
            // 
            // pnlTeachingRightHost
            // 
            this.pnlTeachingRightHost.Controls.Add(this.flpTeachingRightStack);
            this.pnlTeachingRightHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTeachingRightHost.Location = new System.Drawing.Point(0, 0);
            this.pnlTeachingRightHost.Name = "pnlTeachingRightHost";
            this.pnlTeachingRightHost.Size = new System.Drawing.Size(519, 700);
            this.pnlTeachingRightHost.TabIndex = 1;
            // 
            // flpTeachingRightStack
            // 
            this.flpTeachingRightStack.AutoScroll = true;
            this.flpTeachingRightStack.Controls.Add(this.flowRowRecipeSave);
            this.flpTeachingRightStack.Controls.Add(this.flowRowNccTitle);
            this.flpTeachingRightStack.Controls.Add(this.lblNccResult);
            this.flpTeachingRightStack.Controls.Add(this.flowRowNccBtn);
            this.flpTeachingRightStack.Controls.Add(this.flowRowNccMin);
            this.flpTeachingRightStack.Controls.Add(this.flowRowNccCountBtn);
            this.flpTeachingRightStack.Controls.Add(this.flowRowNccCMin);
            this.flpTeachingRightStack.Controls.Add(this.flowRowNccCMax);
            this.flpTeachingRightStack.Controls.Add(this.flowRowNccCountJudgeMin);
            this.flpTeachingRightStack.Controls.Add(this.flowRowBlobCount);
            this.flpTeachingRightStack.Controls.Add(this.flowRowMinArea);
            this.flpTeachingRightStack.Controls.Add(this.flowRowExpected);
            this.flpTeachingRightStack.Controls.Add(this.flowRowFgPixelRange);
            this.flpTeachingRightStack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTeachingRightStack.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpTeachingRightStack.Location = new System.Drawing.Point(0, 0);
            this.flpTeachingRightStack.Margin = new System.Windows.Forms.Padding(0);
            this.flpTeachingRightStack.Name = "flpTeachingRightStack";
            this.flpTeachingRightStack.Padding = new System.Windows.Forms.Padding(10);
            this.flpTeachingRightStack.Size = new System.Drawing.Size(519, 700);
            this.flpTeachingRightStack.TabIndex = 0;
            this.flpTeachingRightStack.WrapContents = false;
            // 
            // flowRowRecipeSave
            // 
            this.flowRowRecipeSave.AutoSize = true;
            this.flowRowRecipeSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowRecipeSave.Controls.Add(this.btnSaveInspectionRecipe);
            this.flowRowRecipeSave.Location = new System.Drawing.Point(10, 10);
            this.flowRowRecipeSave.Margin = new System.Windows.Forms.Padding(0, 0, 0, 14);
            this.flowRowRecipeSave.Name = "flowRowRecipeSave";
            this.flowRowRecipeSave.Size = new System.Drawing.Size(107, 25);
            this.flowRowRecipeSave.TabIndex = 0;
            this.flowRowRecipeSave.WrapContents = false;
            // 
            // btnSaveInspectionRecipe
            // 
            this.btnSaveInspectionRecipe.AutoSize = true;
            this.btnSaveInspectionRecipe.Location = new System.Drawing.Point(0, 0);
            this.btnSaveInspectionRecipe.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnSaveInspectionRecipe.Name = "btnSaveInspectionRecipe";
            this.btnSaveInspectionRecipe.Size = new System.Drawing.Size(107, 23);
            this.btnSaveInspectionRecipe.TabIndex = 0;
            this.btnSaveInspectionRecipe.Text = "검사 레시피 저장";
            this.btnSaveInspectionRecipe.UseVisualStyleBackColor = true;
            this.btnSaveInspectionRecipe.Click += new System.EventHandler(this.btnSaveInspectionRecipe_Click);
            // 
            // flowRowNccTitle
            // 
            this.flowRowNccTitle.AutoSize = true;
            this.flowRowNccTitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowNccTitle.Controls.Add(this.lblNccHeader);
            this.flowRowNccTitle.Location = new System.Drawing.Point(10, 53);
            this.flowRowNccTitle.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.flowRowNccTitle.Name = "flowRowNccTitle";
            this.flowRowNccTitle.Size = new System.Drawing.Size(88, 16);
            this.flowRowNccTitle.TabIndex = 1;
            this.flowRowNccTitle.WrapContents = false;
            // 
            // lblNccHeader
            // 
            this.lblNccHeader.AutoSize = true;
            this.lblNccHeader.Location = new System.Drawing.Point(0, 0);
            this.lblNccHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblNccHeader.Name = "lblNccHeader";
            this.lblNccHeader.Size = new System.Drawing.Size(88, 12);
            this.lblNccHeader.TabIndex = 0;
            this.lblNccHeader.Text = "패턴 매칭";
            // 
            // lblNccResult
            // 
            this.lblNccResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNccResult.Font = new System.Drawing.Font("Segoe UI", 12.5F, System.Drawing.FontStyle.Bold);
            this.lblNccResult.Location = new System.Drawing.Point(10, 71);
            this.lblNccResult.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.lblNccResult.Name = "lblNccResult";
            this.lblNccResult.Size = new System.Drawing.Size(500, 58);
            this.lblNccResult.TabIndex = 1;
            this.lblNccResult.Text = "NCC: —";
            this.lblNccResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowRowNccBtn
            // 
            this.flowRowNccBtn.AutoSize = true;
            this.flowRowNccBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowNccBtn.Controls.Add(this.btnNccSaveModel);
            this.flowRowNccBtn.Controls.Add(this.btnNccRunInspect);
            this.flowRowNccBtn.Location = new System.Drawing.Point(10, 135);
            this.flowRowNccBtn.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.flowRowNccBtn.Name = "flowRowNccBtn";
            this.flowRowNccBtn.Size = new System.Drawing.Size(181, 25);
            this.flowRowNccBtn.TabIndex = 2;
            // 
            // btnNccSaveModel
            // 
            this.btnNccSaveModel.AutoSize = true;
            this.btnNccSaveModel.Location = new System.Drawing.Point(0, 2);
            this.btnNccSaveModel.Margin = new System.Windows.Forms.Padding(0, 2, 8, 0);
            this.btnNccSaveModel.Name = "btnNccSaveModel";
            this.btnNccSaveModel.Size = new System.Drawing.Size(98, 23);
            this.btnNccSaveModel.TabIndex = 0;
            this.btnNccSaveModel.Text = "모델 저장";
            this.btnNccSaveModel.UseVisualStyleBackColor = true;
            this.btnNccSaveModel.Click += new System.EventHandler(this.btnNccSaveModel_Click);
            // 
            // btnNccRunInspect
            // 
            this.btnNccRunInspect.AutoSize = true;
            this.btnNccRunInspect.Location = new System.Drawing.Point(106, 2);
            this.btnNccRunInspect.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.btnNccRunInspect.Name = "btnNccRunInspect";
            this.btnNccRunInspect.Size = new System.Drawing.Size(75, 23);
            this.btnNccRunInspect.TabIndex = 1;
            this.btnNccRunInspect.Text = "검사";
            this.btnNccRunInspect.UseVisualStyleBackColor = true;
            this.btnNccRunInspect.Click += new System.EventHandler(this.btnNccRunInspect_Click);
            // 
            // flowRowNccMin
            // 
            this.flowRowNccMin.AutoSize = true;
            this.flowRowNccMin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowNccMin.Controls.Add(this.lblNccMinTitle);
            this.flowRowNccMin.Controls.Add(this.nudNccMinScore);
            this.flowRowNccMin.Location = new System.Drawing.Point(10, 164);
            this.flowRowNccMin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.flowRowNccMin.Name = "flowRowNccMin";
            this.flowRowNccMin.Size = new System.Drawing.Size(196, 21);
            this.flowRowNccMin.TabIndex = 3;
            this.flowRowNccMin.WrapContents = false;
            // 
            // lblNccMinTitle
            // 
            this.lblNccMinTitle.AutoSize = true;
            this.lblNccMinTitle.Location = new System.Drawing.Point(0, 6);
            this.lblNccMinTitle.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.lblNccMinTitle.Name = "lblNccMinTitle";
            this.lblNccMinTitle.Size = new System.Drawing.Size(126, 12);
            this.lblNccMinTitle.TabIndex = 0;
            this.lblNccMinTitle.Text = "판정 최소 점수 (NCC)";
            // 
            // nudNccMinScore
            // 
            this.nudNccMinScore.DecimalPlaces = 2;
            this.nudNccMinScore.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudNccMinScore.Location = new System.Drawing.Point(132, 0);
            this.nudNccMinScore.Margin = new System.Windows.Forms.Padding(0);
            this.nudNccMinScore.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            this.nudNccMinScore.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            this.nudNccMinScore.Name = "nudNccMinScore";
            this.nudNccMinScore.Size = new System.Drawing.Size(64, 21);
            this.nudNccMinScore.TabIndex = 1;
            this.nudNccMinScore.Value = new decimal(new int[] {
            75,
            0,
            0,
            131072});
            // 
            // flowRowNccCountBtn
            // 
            this.flowRowNccCountBtn.AutoSize = true;
            this.flowRowNccCountBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowNccCountBtn.Controls.Add(this.btnNccCount);
            this.flowRowNccCountBtn.Location = new System.Drawing.Point(10, 195);
            this.flowRowNccCountBtn.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowNccCountBtn.Name = "flowRowNccCountBtn";
            this.flowRowNccCountBtn.Size = new System.Drawing.Size(57, 28);
            this.flowRowNccCountBtn.TabIndex = 4;
            this.flowRowNccCountBtn.WrapContents = false;
            // 
            // btnNccCount
            // 
            this.btnNccCount.AutoSize = true;
            this.btnNccCount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnNccCount.Location = new System.Drawing.Point(3, 3);
            this.btnNccCount.Name = "btnNccCount";
            this.btnNccCount.Size = new System.Drawing.Size(51, 22);
            this.btnNccCount.TabIndex = 0;
            this.btnNccCount.Text = "카운트";
            this.btnNccCount.UseVisualStyleBackColor = true;
            this.btnNccCount.Click += new System.EventHandler(this.btnNccCount_Click);
            // 
            // flowRowNccCMin
            // 
            this.flowRowNccCMin.AutoSize = true;
            this.flowRowNccCMin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowNccCMin.Controls.Add(this.lblNccCountMinTitle);
            this.flowRowNccCMin.Controls.Add(this.trkNccCountMin);
            this.flowRowNccCMin.Controls.Add(this.lblNccCountMinVal);
            this.flowRowNccCMin.Location = new System.Drawing.Point(10, 231);
            this.flowRowNccCMin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowNccCMin.Name = "flowRowNccCMin";
            this.flowRowNccCMin.Size = new System.Drawing.Size(387, 28);
            this.flowRowNccCMin.TabIndex = 5;
            this.flowRowNccCMin.WrapContents = false;
            // 
            // lblNccCountMinTitle
            // 
            this.lblNccCountMinTitle.AutoSize = true;
            this.lblNccCountMinTitle.Location = new System.Drawing.Point(0, 6);
            this.lblNccCountMinTitle.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.lblNccCountMinTitle.Name = "lblNccCountMinTitle";
            this.lblNccCountMinTitle.Size = new System.Drawing.Size(135, 12);
            this.lblNccCountMinTitle.TabIndex = 0;
            this.lblNccCountMinTitle.Text = "카운트 Min (NCC 점수)";
            // 
            // trkNccCountMin
            // 
            this.trkNccCountMin.AutoSize = false;
            this.trkNccCountMin.Location = new System.Drawing.Point(141, 0);
            this.trkNccCountMin.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.trkNccCountMin.Maximum = 100;
            this.trkNccCountMin.Name = "trkNccCountMin";
            this.trkNccCountMin.Size = new System.Drawing.Size(200, 28);
            this.trkNccCountMin.TabIndex = 1;
            this.trkNccCountMin.TickFrequency = 10;
            this.trkNccCountMin.Value = 50;
            this.trkNccCountMin.ValueChanged += new System.EventHandler(this.NccCountTracks_Changed);
            // 
            // lblNccCountMinVal
            // 
            this.lblNccCountMinVal.AutoSize = true;
            this.lblNccCountMinVal.Location = new System.Drawing.Point(347, 6);
            this.lblNccCountMinVal.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblNccCountMinVal.MinimumSize = new System.Drawing.Size(40, 0);
            this.lblNccCountMinVal.Name = "lblNccCountMinVal";
            this.lblNccCountMinVal.Size = new System.Drawing.Size(40, 12);
            this.lblNccCountMinVal.TabIndex = 2;
            this.lblNccCountMinVal.Text = "0.75";
            // 
            // flowRowNccCMax
            // 
            this.flowRowNccCMax.AutoSize = true;
            this.flowRowNccCMax.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowNccCMax.Controls.Add(this.lblNccCountMaxTitle);
            this.flowRowNccCMax.Controls.Add(this.trkNccCountMax);
            this.flowRowNccCMax.Controls.Add(this.lblNccCountMaxVal);
            this.flowRowNccCMax.Location = new System.Drawing.Point(10, 267);
            this.flowRowNccCMax.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowNccCMax.Name = "flowRowNccCMax";
            this.flowRowNccCMax.Size = new System.Drawing.Size(391, 28);
            this.flowRowNccCMax.TabIndex = 6;
            this.flowRowNccCMax.WrapContents = false;
            // 
            // lblNccCountMaxTitle
            // 
            this.lblNccCountMaxTitle.AutoSize = true;
            this.lblNccCountMaxTitle.Location = new System.Drawing.Point(0, 6);
            this.lblNccCountMaxTitle.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.lblNccCountMaxTitle.Name = "lblNccCountMaxTitle";
            this.lblNccCountMaxTitle.Size = new System.Drawing.Size(139, 12);
            this.lblNccCountMaxTitle.TabIndex = 0;
            this.lblNccCountMaxTitle.Text = "카운트 Max (NCC 점수)";
            // 
            // trkNccCountMax
            // 
            this.trkNccCountMax.AutoSize = false;
            this.trkNccCountMax.Location = new System.Drawing.Point(145, 0);
            this.trkNccCountMax.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.trkNccCountMax.Maximum = 100;
            this.trkNccCountMax.Name = "trkNccCountMax";
            this.trkNccCountMax.Size = new System.Drawing.Size(200, 28);
            this.trkNccCountMax.TabIndex = 1;
            this.trkNccCountMax.TickFrequency = 10;
            this.trkNccCountMax.Value = 100;
            this.trkNccCountMax.ValueChanged += new System.EventHandler(this.NccCountTracks_Changed);
            // 
            // lblNccCountMaxVal
            // 
            this.lblNccCountMaxVal.AutoSize = true;
            this.lblNccCountMaxVal.Location = new System.Drawing.Point(351, 6);
            this.lblNccCountMaxVal.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblNccCountMaxVal.MinimumSize = new System.Drawing.Size(40, 0);
            this.lblNccCountMaxVal.Name = "lblNccCountMaxVal";
            this.lblNccCountMaxVal.Size = new System.Drawing.Size(40, 12);
            this.lblNccCountMaxVal.TabIndex = 2;
            this.lblNccCountMaxVal.Text = "1.00";
            // 
            // flowRowNccCountJudgeMin
            // 
            this.flowRowNccCountJudgeMin.AutoSize = true;
            this.flowRowNccCountJudgeMin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowNccCountJudgeMin.Controls.Add(this.lblNccCountJudgeMinTitle);
            this.flowRowNccCountJudgeMin.Controls.Add(this.nudNccCountJudgeMin);
            this.flowRowNccCountJudgeMin.Location = new System.Drawing.Point(10, 303);
            this.flowRowNccCountJudgeMin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowNccCountJudgeMin.Name = "flowRowNccCountJudgeMin";
            this.flowRowNccCountJudgeMin.Size = new System.Drawing.Size(205, 21);
            this.flowRowNccCountJudgeMin.TabIndex = 7;
            this.flowRowNccCountJudgeMin.WrapContents = false;
            // 
            // lblNccCountJudgeMinTitle
            // 
            this.lblNccCountJudgeMinTitle.AutoSize = true;
            this.lblNccCountJudgeMinTitle.Location = new System.Drawing.Point(0, 6);
            this.lblNccCountJudgeMinTitle.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.lblNccCountJudgeMinTitle.Name = "lblNccCountJudgeMinTitle";
            this.lblNccCountJudgeMinTitle.Size = new System.Drawing.Size(135, 12);
            this.lblNccCountJudgeMinTitle.TabIndex = 0;
            this.lblNccCountJudgeMinTitle.Text = "판정 최소 점수 (카운트)";
            // 
            // nudNccCountJudgeMin
            // 
            this.nudNccCountJudgeMin.DecimalPlaces = 2;
            this.nudNccCountJudgeMin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudNccCountJudgeMin.Location = new System.Drawing.Point(141, 0);
            this.nudNccCountJudgeMin.Margin = new System.Windows.Forms.Padding(0);
            this.nudNccCountJudgeMin.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            this.nudNccCountJudgeMin.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            this.nudNccCountJudgeMin.Name = "nudNccCountJudgeMin";
            this.nudNccCountJudgeMin.Size = new System.Drawing.Size(64, 21);
            this.nudNccCountJudgeMin.TabIndex = 1;
            this.nudNccCountJudgeMin.Value = new decimal(new int[] {
            80,
            0,
            0,
            131072});
            // 
            // flowRowMinArea
            // 
            this.flowRowMinArea.AutoSize = true;
            this.flowRowMinArea.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowMinArea.Controls.Add(this.lblMinAreaTitle);
            this.flowRowMinArea.Controls.Add(this.trkMinArea);
            this.flowRowMinArea.Controls.Add(this.lblMinArea);
            this.flowRowMinArea.Location = new System.Drawing.Point(10, 332);
            this.flowRowMinArea.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowMinArea.Name = "flowRowMinArea";
            this.flowRowMinArea.Size = new System.Drawing.Size(324, 28);
            this.flowRowMinArea.TabIndex = 8;
            this.flowRowMinArea.WrapContents = false;
            // 
            // lblMinAreaTitle
            // 
            this.lblMinAreaTitle.AutoSize = true;
            this.lblMinAreaTitle.Location = new System.Drawing.Point(0, 6);
            this.lblMinAreaTitle.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.lblMinAreaTitle.Name = "lblMinAreaTitle";
            this.lblMinAreaTitle.Size = new System.Drawing.Size(52, 12);
            this.lblMinAreaTitle.TabIndex = 0;
            this.lblMinAreaTitle.Text = "MinArea";
            // 
            // trkMinArea
            // 
            this.trkMinArea.AutoSize = false;
            this.trkMinArea.Location = new System.Drawing.Point(58, 0);
            this.trkMinArea.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.trkMinArea.Maximum = 5000;
            this.trkMinArea.Name = "trkMinArea";
            this.trkMinArea.Size = new System.Drawing.Size(220, 28);
            this.trkMinArea.TabIndex = 1;
            this.trkMinArea.TickFrequency = 500;
            this.trkMinArea.Value = 20;
            this.trkMinArea.ValueChanged += new System.EventHandler(this.DemoOptions_Changed);
            // 
            // lblMinArea
            // 
            this.lblMinArea.AutoSize = true;
            this.lblMinArea.Location = new System.Drawing.Point(284, 6);
            this.lblMinArea.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblMinArea.MinimumSize = new System.Drawing.Size(40, 0);
            this.lblMinArea.Name = "lblMinArea";
            this.lblMinArea.Size = new System.Drawing.Size(40, 12);
            this.lblMinArea.TabIndex = 2;
            this.lblMinArea.Text = "20";
            // 
            // flowRowExpected
            // 
            this.flowRowExpected.AutoSize = true;
            this.flowRowExpected.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowExpected.Controls.Add(this.lblExpectedTitle);
            this.flowRowExpected.Controls.Add(this.numExpected);
            this.flowRowExpected.Location = new System.Drawing.Point(10, 368);
            this.flowRowExpected.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowExpected.Name = "flowRowExpected";
            this.flowRowExpected.Size = new System.Drawing.Size(184, 23);
            this.flowRowExpected.TabIndex = 9;
            this.flowRowExpected.WrapContents = false;
            // 
            // lblExpectedTitle
            // 
            this.lblExpectedTitle.AutoSize = true;
            this.lblExpectedTitle.Location = new System.Drawing.Point(0, 6);
            this.lblExpectedTitle.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.lblExpectedTitle.Name = "lblExpectedTitle";
            this.lblExpectedTitle.Size = new System.Drawing.Size(58, 12);
            this.lblExpectedTitle.TabIndex = 0;
            this.lblExpectedTitle.Text = "Expected";
            // 
            // numExpected
            // 
            this.numExpected.Location = new System.Drawing.Point(64, 2);
            this.numExpected.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.numExpected.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numExpected.Name = "numExpected";
            this.numExpected.Size = new System.Drawing.Size(120, 21);
            this.numExpected.TabIndex = 1;
            this.numExpected.ValueChanged += new System.EventHandler(this.DemoOptions_Changed);
            // 
            // grpTeachingLog
            // 
            this.grpTeachingLog.Controls.Add(this.txtTeachingLog);
            this.grpTeachingLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpTeachingLog.Location = new System.Drawing.Point(0, 700);
            this.grpTeachingLog.Name = "grpTeachingLog";
            this.grpTeachingLog.Size = new System.Drawing.Size(519, 120);
            this.grpTeachingLog.TabIndex = 0;
            this.grpTeachingLog.TabStop = false;
            this.grpTeachingLog.Text = "Teaching Log";
            // 
            // txtTeachingLog
            // 
            this.txtTeachingLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTeachingLog.Location = new System.Drawing.Point(3, 17);
            this.txtTeachingLog.Multiline = true;
            this.txtTeachingLog.Name = "txtTeachingLog";
            this.txtTeachingLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTeachingLog.Size = new System.Drawing.Size(513, 100);
            this.txtTeachingLog.TabIndex = 0;
            // 
            // flowRowBlobCount
            // 
            this.flowRowBlobCount.AutoSize = true;
            this.flowRowBlobCount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowBlobCount.Controls.Add(this.btnBlobCount);
            this.flowRowBlobCount.Location = new System.Drawing.Point(0, 0);
            this.flowRowBlobCount.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowBlobCount.Name = "flowRowBlobCount";
            this.flowRowBlobCount.Size = new System.Drawing.Size(200, 100);
            this.flowRowBlobCount.TabIndex = 0;
            this.flowRowBlobCount.WrapContents = false;
            // 
            // btnBlobCount
            // 
            this.btnBlobCount.AutoSize = true;
            this.btnBlobCount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBlobCount.Location = new System.Drawing.Point(0, 0);
            this.btnBlobCount.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btnBlobCount.Name = "btnBlobCount";
            this.btnBlobCount.Size = new System.Drawing.Size(63, 22);
            this.btnBlobCount.TabIndex = 0;
            this.btnBlobCount.Text = "개수 카운트";
            this.btnBlobCount.UseVisualStyleBackColor = true;
            this.btnBlobCount.Click += new System.EventHandler(this.btnBlobCount_Click);
            // 
            // flowRowFgPixelRange
            // 
            this.flowRowFgPixelRange.AutoSize = true;
            this.flowRowFgPixelRange.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowRowFgPixelRange.Controls.Add(this.lblFgPixelRangeTitle);
            this.flowRowFgPixelRange.Controls.Add(this.numFgPixelMin);
            this.flowRowFgPixelRange.Controls.Add(this.lblFgPixelRangeSep);
            this.flowRowFgPixelRange.Controls.Add(this.numFgPixelMax);
            this.flowRowFgPixelRange.Location = new System.Drawing.Point(0, 0);
            this.flowRowFgPixelRange.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowRowFgPixelRange.Name = "flowRowFgPixelRange";
            this.flowRowFgPixelRange.Size = new System.Drawing.Size(200, 100);
            this.flowRowFgPixelRange.TabIndex = 0;
            this.flowRowFgPixelRange.WrapContents = false;
            // 
            // lblFgPixelRangeTitle
            // 
            this.lblFgPixelRangeTitle.AutoSize = true;
            this.lblFgPixelRangeTitle.Location = new System.Drawing.Point(0, 6);
            this.lblFgPixelRangeTitle.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.lblFgPixelRangeTitle.Name = "lblFgPixelRangeTitle";
            this.lblFgPixelRangeTitle.Size = new System.Drawing.Size(97, 12);
            this.lblFgPixelRangeTitle.TabIndex = 0;
            this.lblFgPixelRangeTitle.Text = "ROI 전경 픽셀 범위";
            // 
            // numFgPixelMin
            // 
            this.numFgPixelMin.Location = new System.Drawing.Point(103, 2);
            this.numFgPixelMin.Margin = new System.Windows.Forms.Padding(0, 2, 4, 0);
            this.numFgPixelMin.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numFgPixelMin.Name = "numFgPixelMin";
            this.numFgPixelMin.Size = new System.Drawing.Size(88, 21);
            this.numFgPixelMin.TabIndex = 1;
            this.numFgPixelMin.ValueChanged += new System.EventHandler(this.DemoOptions_Changed);
            // 
            // lblFgPixelRangeSep
            // 
            this.lblFgPixelRangeSep.AutoSize = true;
            this.lblFgPixelRangeSep.Location = new System.Drawing.Point(195, 6);
            this.lblFgPixelRangeSep.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lblFgPixelRangeSep.Name = "lblFgPixelRangeSep";
            this.lblFgPixelRangeSep.Size = new System.Drawing.Size(11, 12);
            this.lblFgPixelRangeSep.TabIndex = 2;
            this.lblFgPixelRangeSep.Text = "~";
            // 
            // numFgPixelMax
            // 
            this.numFgPixelMax.Location = new System.Drawing.Point(210, 2);
            this.numFgPixelMax.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.numFgPixelMax.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numFgPixelMax.Name = "numFgPixelMax";
            this.numFgPixelMax.Size = new System.Drawing.Size(88, 21);
            this.numFgPixelMax.TabIndex = 3;
            this.numFgPixelMax.ValueChanged += new System.EventHandler(this.DemoOptions_Changed);
            // 
            // ucInspectFlowStrip1
            // 
            this.ucInspectFlowStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucInspectFlowStrip1.Location = new System.Drawing.Point(0, 0);
            this.ucInspectFlowStrip1.Margin = new System.Windows.Forms.Padding(0);
            this.ucInspectFlowStrip1.Name = "ucInspectFlowStrip1";
            this.ucInspectFlowStrip1.Size = new System.Drawing.Size(971, 40);
            this.ucInspectFlowStrip1.TabIndex = 2;
            // 
            // UcTeachingShell
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitMain);
            this.Name = "UcTeachingShell";
            this.Size = new System.Drawing.Size(1500, 820);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.grpViewer.ResumeLayout(false);
            this.splitViewerToolbar.Panel1.ResumeLayout(false);
            this.splitViewerToolbar.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitViewerToolbar)).EndInit();
            this.splitViewerToolbar.ResumeLayout(false);
            this.flpViewerToolbar.ResumeLayout(false);
            this.tlpViewerRoot.ResumeLayout(false);
            this.pnlViewerBottom.ResumeLayout(false);
            this.pnlTeachingRightHost.ResumeLayout(false);
            this.flpTeachingRightStack.ResumeLayout(false);
            this.flpTeachingRightStack.PerformLayout();
            this.flowRowRecipeSave.ResumeLayout(false);
            this.flowRowRecipeSave.PerformLayout();
            this.flowRowNccTitle.ResumeLayout(false);
            this.flowRowNccTitle.PerformLayout();
            this.flowRowNccBtn.ResumeLayout(false);
            this.flowRowNccBtn.PerformLayout();
            this.flowRowNccMin.ResumeLayout(false);
            this.flowRowNccMin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNccMinScore)).EndInit();
            this.flowRowNccCountBtn.ResumeLayout(false);
            this.flowRowNccCountBtn.PerformLayout();
            this.flowRowNccCMin.ResumeLayout(false);
            this.flowRowNccCMin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkNccCountMin)).EndInit();
            this.flowRowNccCMax.ResumeLayout(false);
            this.flowRowNccCMax.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkNccCountMax)).EndInit();
            this.flowRowNccCountJudgeMin.ResumeLayout(false);
            this.flowRowNccCountJudgeMin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNccCountJudgeMin)).EndInit();
            this.flowRowBlobCount.ResumeLayout(false);
            this.flowRowBlobCount.PerformLayout();
            this.flowRowMinArea.ResumeLayout(false);
            this.flowRowMinArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkMinArea)).EndInit();
            this.flowRowExpected.ResumeLayout(false);
            this.flowRowExpected.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExpected)).EndInit();
            this.flowRowFgPixelRange.ResumeLayout(false);
            this.flowRowFgPixelRange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFgPixelMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFgPixelMax)).EndInit();
            this.grpTeachingLog.ResumeLayout(false);
            this.grpTeachingLog.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}

