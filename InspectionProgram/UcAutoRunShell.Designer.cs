namespace InspectionProgram.GUI
{
    partial class UcAutoRunShell
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpViewer;
        private System.Windows.Forms.SplitContainer splitViewerToolbar;
        private System.Windows.Forms.TableLayoutPanel tlpViewerRoot;
        private System.Windows.Forms.FlowLayoutPanel flpViewerToolbar;
        private UcInspectFlowStrip ucInspectFlowStrip1;
        private System.Windows.Forms.Button btnToolLoad;
        private System.Windows.Forms.Button btnToolClear;
        private System.Windows.Forms.Button btnToolSave;
        private System.Windows.Forms.Button btnToolZm;
        private System.Windows.Forms.Button btnToolZIn;
        private System.Windows.Forms.Button btnToolZOut;
        private System.Windows.Forms.Panel pnlViewerHost;
        private System.Windows.Forms.Panel pnlViewerBottom;
        private System.Windows.Forms.TabControl tabCamera;
        private System.Windows.Forms.TableLayoutPanel tlpRight;
        private System.Windows.Forms.GroupBox grpCount;
        private System.Windows.Forms.TableLayoutPanel tlpCount;
        private System.Windows.Forms.Label lblCurrentDeviceTitle;
        private System.Windows.Forms.Label lblCurrentDeviceValue;
        private System.Windows.Forms.DataGridView dgvCount;
        private System.Windows.Forms.GroupBox grpInspectionLog;
        private System.Windows.Forms.TextBox txtInspectionLog;
        private System.Windows.Forms.GroupBox grpSystemLog;
        private System.Windows.Forms.TextBox txtSystemLog;

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
            this.btnToolClear = new System.Windows.Forms.Button();
            this.btnToolSave = new System.Windows.Forms.Button();
            this.btnToolZm = new System.Windows.Forms.Button();
            this.btnToolZIn = new System.Windows.Forms.Button();
            this.btnToolZOut = new System.Windows.Forms.Button();
            this.tlpViewerRoot = new System.Windows.Forms.TableLayoutPanel();
            this.ucInspectFlowStrip1 = new InspectionProgram.GUI.UcInspectFlowStrip();
            this.pnlViewerHost = new System.Windows.Forms.Panel();
            this.pnlViewerBottom = new System.Windows.Forms.Panel();
            this.tabCamera = new System.Windows.Forms.TabControl();
            this.tlpRight = new System.Windows.Forms.TableLayoutPanel();
            this.grpCount = new System.Windows.Forms.GroupBox();
            this.tlpCount = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurrentDeviceTitle = new System.Windows.Forms.Label();
            this.lblCurrentDeviceValue = new System.Windows.Forms.Label();
            this.dgvCount = new System.Windows.Forms.DataGridView();
            this.grpInspectionLog = new System.Windows.Forms.GroupBox();
            this.txtInspectionLog = new System.Windows.Forms.TextBox();
            this.grpSystemLog = new System.Windows.Forms.GroupBox();
            this.txtSystemLog = new System.Windows.Forms.TextBox();
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
            this.tlpRight.SuspendLayout();
            this.grpCount.SuspendLayout();
            this.tlpCount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCount)).BeginInit();
            this.grpInspectionLog.SuspendLayout();
            this.grpSystemLog.SuspendLayout();
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
            this.splitMain.Panel2.Controls.Add(this.tlpRight);
            this.splitMain.Size = new System.Drawing.Size(1500, 820);
            this.splitMain.SplitterDistance = 930;
            this.splitMain.TabIndex = 0;
            // 
            // grpViewer
            // 
            this.grpViewer.Controls.Add(this.splitViewerToolbar);
            this.grpViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpViewer.Location = new System.Drawing.Point(0, 0);
            this.grpViewer.Name = "grpViewer";
            this.grpViewer.Size = new System.Drawing.Size(930, 820);
            this.grpViewer.TabIndex = 0;
            this.grpViewer.TabStop = false;
            this.grpViewer.Text = "Auto Run";
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
            this.splitViewerToolbar.Size = new System.Drawing.Size(924, 800);
            this.splitViewerToolbar.SplitterDistance = 32;
            this.splitViewerToolbar.SplitterWidth = 6;
            this.splitViewerToolbar.TabIndex = 1;
            // 
            // flpViewerToolbar
            // 
            this.flpViewerToolbar.Controls.Add(this.btnToolLoad);
            this.flpViewerToolbar.Controls.Add(this.btnToolClear);
            this.flpViewerToolbar.Controls.Add(this.btnToolSave);
            this.flpViewerToolbar.Controls.Add(this.btnToolZm);
            this.flpViewerToolbar.Controls.Add(this.btnToolZIn);
            this.flpViewerToolbar.Controls.Add(this.btnToolZOut);
            this.flpViewerToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpViewerToolbar.Location = new System.Drawing.Point(0, 0);
            this.flpViewerToolbar.Margin = new System.Windows.Forms.Padding(0);
            this.flpViewerToolbar.Name = "flpViewerToolbar";
            this.flpViewerToolbar.Padding = new System.Windows.Forms.Padding(2, 3, 2, 1);
            this.flpViewerToolbar.Size = new System.Drawing.Size(924, 32);
            this.flpViewerToolbar.TabIndex = 1;
            this.flpViewerToolbar.WrapContents = false;
            // 
            // btnToolLoad
            // 
            this.btnToolLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolLoad.Location = new System.Drawing.Point(4, 5);
            this.btnToolLoad.Margin = new System.Windows.Forms.Padding(2);
            this.btnToolLoad.Name = "btnToolLoad";
            this.btnToolLoad.Size = new System.Drawing.Size(80, 24);
            this.btnToolLoad.TabIndex = 100;
            this.btnToolLoad.TabStop = false;
            this.btnToolLoad.Tag = "LOAD";
            this.btnToolLoad.Text = "Load";
            this.btnToolLoad.UseVisualStyleBackColor = true;
            this.btnToolLoad.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolClear
            // 
            this.btnToolClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolClear.Location = new System.Drawing.Point(88, 5);
            this.btnToolClear.Margin = new System.Windows.Forms.Padding(2);
            this.btnToolClear.Name = "btnToolClear";
            this.btnToolClear.Size = new System.Drawing.Size(77, 24);
            this.btnToolClear.TabIndex = 101;
            this.btnToolClear.TabStop = false;
            this.btnToolClear.Tag = "CLEAR";
            this.btnToolClear.Text = "Clear";
            this.btnToolClear.UseVisualStyleBackColor = true;
            this.btnToolClear.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolSave
            // 
            this.btnToolSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolSave.Location = new System.Drawing.Point(169, 5);
            this.btnToolSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnToolSave.Name = "btnToolSave";
            this.btnToolSave.Size = new System.Drawing.Size(69, 24);
            this.btnToolSave.TabIndex = 102;
            this.btnToolSave.TabStop = false;
            this.btnToolSave.Tag = "SAVE";
            this.btnToolSave.Text = "Save";
            this.btnToolSave.UseVisualStyleBackColor = true;
            this.btnToolSave.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolZm
            // 
            this.btnToolZm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolZm.Location = new System.Drawing.Point(242, 5);
            this.btnToolZm.Margin = new System.Windows.Forms.Padding(2);
            this.btnToolZm.Name = "btnToolZm";
            this.btnToolZm.Size = new System.Drawing.Size(40, 24);
            this.btnToolZm.TabIndex = 3;
            this.btnToolZm.TabStop = false;
            this.btnToolZm.Tag = "ZM";
            this.btnToolZm.Text = "ZM";
            this.btnToolZm.UseVisualStyleBackColor = true;
            this.btnToolZm.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolZIn
            // 
            this.btnToolZIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolZIn.Location = new System.Drawing.Point(286, 5);
            this.btnToolZIn.Margin = new System.Windows.Forms.Padding(2);
            this.btnToolZIn.Name = "btnToolZIn";
            this.btnToolZIn.Size = new System.Drawing.Size(40, 24);
            this.btnToolZIn.TabIndex = 4;
            this.btnToolZIn.TabStop = false;
            this.btnToolZIn.Tag = "Z+";
            this.btnToolZIn.Text = "+";
            this.btnToolZIn.UseVisualStyleBackColor = true;
            this.btnToolZIn.Click += new System.EventHandler(this.btnViewerTool_Click);
            // 
            // btnToolZOut
            // 
            this.btnToolZOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToolZOut.Location = new System.Drawing.Point(330, 5);
            this.btnToolZOut.Margin = new System.Windows.Forms.Padding(2);
            this.btnToolZOut.Name = "btnToolZOut";
            this.btnToolZOut.Size = new System.Drawing.Size(40, 24);
            this.btnToolZOut.TabIndex = 5;
            this.btnToolZOut.TabStop = false;
            this.btnToolZOut.Tag = "Z-";
            this.btnToolZOut.Text = "-";
            this.btnToolZOut.UseVisualStyleBackColor = true;
            this.btnToolZOut.Click += new System.EventHandler(this.btnViewerTool_Click);
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
            this.tlpViewerRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tlpViewerRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpViewerRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tlpViewerRoot.Size = new System.Drawing.Size(924, 762);
            this.tlpViewerRoot.TabIndex = 0;
            // 
            // ucInspectFlowStrip1
            // 
            this.ucInspectFlowStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucInspectFlowStrip1.Location = new System.Drawing.Point(0, 0);
            this.ucInspectFlowStrip1.Margin = new System.Windows.Forms.Padding(0);
            this.ucInspectFlowStrip1.Name = "ucInspectFlowStrip1";
            this.ucInspectFlowStrip1.Size = new System.Drawing.Size(924, 42);
            this.ucInspectFlowStrip1.TabIndex = 4;
            // 
            // pnlViewerHost
            // 
            this.pnlViewerHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewerHost.Location = new System.Drawing.Point(3, 45);
            this.pnlViewerHost.Name = "pnlViewerHost";
            this.pnlViewerHost.Size = new System.Drawing.Size(918, 676);
            this.pnlViewerHost.TabIndex = 2;
            // 
            // pnlViewerBottom
            // 
            this.pnlViewerBottom.Controls.Add(this.tabCamera);
            this.pnlViewerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewerBottom.Location = new System.Drawing.Point(0, 724);
            this.pnlViewerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.pnlViewerBottom.Name = "pnlViewerBottom";
            this.pnlViewerBottom.Size = new System.Drawing.Size(924, 38);
            this.pnlViewerBottom.TabIndex = 3;
            // 
            // tabCamera
            // 
            this.tabCamera.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabCamera.Location = new System.Drawing.Point(0, 0);
            this.tabCamera.Name = "tabCamera";
            this.tabCamera.SelectedIndex = 0;
            this.tabCamera.Size = new System.Drawing.Size(792, 36);
            this.tabCamera.TabIndex = 0;
            this.tabCamera.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabCamera_DrawItem);
            this.tabCamera.SelectedIndexChanged += new System.EventHandler(this.tabCamera_SelectedIndexChanged);
            // 
            // tlpRight
            // 
            this.tlpRight.ColumnCount = 1;
            this.tlpRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.Controls.Add(this.grpCount, 0, 0);
            this.tlpRight.Controls.Add(this.grpInspectionLog, 0, 1);
            this.tlpRight.Controls.Add(this.grpSystemLog, 0, 2);
            this.tlpRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRight.Location = new System.Drawing.Point(0, 0);
            this.tlpRight.Name = "tlpRight";
            this.tlpRight.RowCount = 3;
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpRight.Size = new System.Drawing.Size(566, 820);
            this.tlpRight.TabIndex = 0;
            // 
            // grpCount
            // 
            this.grpCount.Controls.Add(this.tlpCount);
            this.grpCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCount.Location = new System.Drawing.Point(3, 3);
            this.grpCount.Name = "grpCount";
            this.grpCount.Size = new System.Drawing.Size(560, 224);
            this.grpCount.TabIndex = 0;
            this.grpCount.TabStop = false;
            this.grpCount.Text = "Count";
            // 
            // tlpCount
            // 
            this.tlpCount.ColumnCount = 2;
            this.tlpCount.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpCount.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCount.Controls.Add(this.lblCurrentDeviceTitle, 0, 0);
            this.tlpCount.Controls.Add(this.lblCurrentDeviceValue, 1, 0);
            this.tlpCount.Controls.Add(this.dgvCount, 0, 1);
            this.tlpCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCount.Location = new System.Drawing.Point(3, 17);
            this.tlpCount.Name = "tlpCount";
            this.tlpCount.RowCount = 2;
            this.tlpCount.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpCount.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCount.Size = new System.Drawing.Size(554, 204);
            this.tlpCount.TabIndex = 0;
            // 
            // lblCurrentDeviceTitle
            // 
            this.lblCurrentDeviceTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentDeviceTitle.Location = new System.Drawing.Point(3, 0);
            this.lblCurrentDeviceTitle.Name = "lblCurrentDeviceTitle";
            this.lblCurrentDeviceTitle.Size = new System.Drawing.Size(114, 28);
            this.lblCurrentDeviceTitle.TabIndex = 0;
            this.lblCurrentDeviceTitle.Text = "Current Device";
            this.lblCurrentDeviceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentDeviceValue
            // 
            this.lblCurrentDeviceValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentDeviceValue.Location = new System.Drawing.Point(123, 0);
            this.lblCurrentDeviceValue.Name = "lblCurrentDeviceValue";
            this.lblCurrentDeviceValue.Size = new System.Drawing.Size(428, 28);
            this.lblCurrentDeviceValue.TabIndex = 1;
            this.lblCurrentDeviceValue.Text = "CAM 01";
            this.lblCurrentDeviceValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvCount
            // 
            this.tlpCount.SetColumnSpan(this.dgvCount, 2);
            this.dgvCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCount.Location = new System.Drawing.Point(3, 31);
            this.dgvCount.Name = "dgvCount";
            this.dgvCount.Size = new System.Drawing.Size(548, 170);
            this.dgvCount.TabIndex = 2;
            // 
            // grpInspectionLog
            // 
            this.grpInspectionLog.Controls.Add(this.txtInspectionLog);
            this.grpInspectionLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInspectionLog.Location = new System.Drawing.Point(3, 233);
            this.grpInspectionLog.Name = "grpInspectionLog";
            this.grpInspectionLog.Size = new System.Drawing.Size(560, 464);
            this.grpInspectionLog.TabIndex = 1;
            this.grpInspectionLog.TabStop = false;
            this.grpInspectionLog.Text = "Inspection Log";
            // 
            // txtInspectionLog
            // 
            this.txtInspectionLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInspectionLog.Location = new System.Drawing.Point(3, 17);
            this.txtInspectionLog.Multiline = true;
            this.txtInspectionLog.Name = "txtInspectionLog";
            this.txtInspectionLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInspectionLog.Size = new System.Drawing.Size(554, 444);
            this.txtInspectionLog.TabIndex = 0;
            // 
            // grpSystemLog
            // 
            this.grpSystemLog.Controls.Add(this.txtSystemLog);
            this.grpSystemLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSystemLog.Location = new System.Drawing.Point(3, 703);
            this.grpSystemLog.Name = "grpSystemLog";
            this.grpSystemLog.Size = new System.Drawing.Size(560, 114);
            this.grpSystemLog.TabIndex = 2;
            this.grpSystemLog.TabStop = false;
            this.grpSystemLog.Text = "System Log";
            // 
            // txtSystemLog
            // 
            this.txtSystemLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSystemLog.Location = new System.Drawing.Point(3, 17);
            this.txtSystemLog.Multiline = true;
            this.txtSystemLog.Name = "txtSystemLog";
            this.txtSystemLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSystemLog.Size = new System.Drawing.Size(554, 94);
            this.txtSystemLog.TabIndex = 0;
            // 
            // UcAutoRunShell
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitMain);
            this.Name = "UcAutoRunShell";
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
            this.tlpRight.ResumeLayout(false);
            this.grpCount.ResumeLayout(false);
            this.tlpCount.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCount)).EndInit();
            this.grpInspectionLog.ResumeLayout(false);
            this.grpInspectionLog.PerformLayout();
            this.grpSystemLog.ResumeLayout(false);
            this.grpSystemLog.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}

