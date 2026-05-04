namespace InspectionProgram.GUI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.Panel pnlHeader;
        /// <summary>Header: column 0 = mode tabs, 1 = device/count/log/time, 2 = network / language (see Visual Studio designer).</summary>
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.FlowLayoutPanel flpHeaderModes;
        private System.Windows.Forms.FlowLayoutPanel flpHeaderCenter;
        private System.Windows.Forms.FlowLayoutPanel flpHeaderRight;
        private System.Windows.Forms.Button btnModeAutoRun;
        private System.Windows.Forms.Button btnModeTeaching;
        private System.Windows.Forms.Button btnModeOption;
        private System.Windows.Forms.Label lblTimeValue;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Panel pnlToolbar;
        private System.Windows.Forms.TableLayoutPanel tlpToolbar;
        private System.Windows.Forms.TableLayoutPanel tlpToolbarButtons;
        private System.Windows.Forms.Button btnMenu1;
        private System.Windows.Forms.Button btnMenu2;
        private System.Windows.Forms.Button btnMenu3;
        private System.Windows.Forms.Button btnMenu4;
        private System.Windows.Forms.Button btnMenu5;
        private System.Windows.Forms.Panel pnlToolbarRight;
        private System.Windows.Forms.Panel pnlContentHost;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.flpHeaderModes = new System.Windows.Forms.FlowLayoutPanel();
            this.btnModeAutoRun = new System.Windows.Forms.Button();
            this.btnModeTeaching = new System.Windows.Forms.Button();
            this.btnModeOption = new System.Windows.Forms.Button();
            this.flpHeaderCenter = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTimeValue = new System.Windows.Forms.Label();
            this.flpHeaderRight = new System.Windows.Forms.FlowLayoutPanel();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.tlpToolbar = new System.Windows.Forms.TableLayoutPanel();
            this.tlpToolbarButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnMenu1 = new System.Windows.Forms.Button();
            this.btnMenu2 = new System.Windows.Forms.Button();
            this.btnMenu3 = new System.Windows.Forms.Button();
            this.btnMenu4 = new System.Windows.Forms.Button();
            this.btnMenu5 = new System.Windows.Forms.Button();
            this.pnlToolbarRight = new System.Windows.Forms.Panel();
            this.pnlContentHost = new System.Windows.Forms.Panel();
            this.tlpRoot.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.flpHeaderModes.SuspendLayout();
            this.flpHeaderCenter.SuspendLayout();
            this.flpHeaderRight.SuspendLayout();
            this.pnlToolbar.SuspendLayout();
            this.tlpToolbar.SuspendLayout();
            this.tlpToolbarButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpRoot.Controls.Add(this.pnlToolbar, 0, 1);
            this.tlpRoot.Controls.Add(this.pnlContentHost, 0, 2);
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Location = new System.Drawing.Point(0, 0);
            this.tlpRoot.Margin = new System.Windows.Forms.Padding(0);
            this.tlpRoot.Name = "tlpRoot";
            this.tlpRoot.RowCount = 3;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Size = new System.Drawing.Size(1924, 1000);
            this.tlpRoot.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.tlpHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1924, 52);
            this.pnlHeader.TabIndex = 0;
            // 
            // tlpHeader
            // 
            this.tlpHeader.ColumnCount = 3;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 600F));
            this.tlpHeader.Controls.Add(this.flpHeaderModes, 0, 0);
            this.tlpHeader.Controls.Add(this.flpHeaderCenter, 1, 0);
            this.tlpHeader.Controls.Add(this.flpHeaderRight, 2, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 1;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.Size = new System.Drawing.Size(1924, 52);
            this.tlpHeader.TabIndex = 16;
            // 
            // flpHeaderModes
            // 
            this.flpHeaderModes.Controls.Add(this.btnModeAutoRun);
            this.flpHeaderModes.Controls.Add(this.btnModeTeaching);
            this.flpHeaderModes.Controls.Add(this.btnModeOption);
            this.flpHeaderModes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpHeaderModes.Location = new System.Drawing.Point(0, 0);
            this.flpHeaderModes.Margin = new System.Windows.Forms.Padding(0);
            this.flpHeaderModes.Name = "flpHeaderModes";
            this.flpHeaderModes.Padding = new System.Windows.Forms.Padding(8, 10, 4, 8);
            this.flpHeaderModes.Size = new System.Drawing.Size(360, 52);
            this.flpHeaderModes.TabIndex = 0;
            this.flpHeaderModes.WrapContents = false;
            // 
            // btnModeAutoRun
            // 
            this.btnModeAutoRun.Location = new System.Drawing.Point(8, 10);
            this.btnModeAutoRun.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnModeAutoRun.Name = "btnModeAutoRun";
            this.btnModeAutoRun.Size = new System.Drawing.Size(100, 30);
            this.btnModeAutoRun.TabIndex = 0;
            this.btnModeAutoRun.Text = "Auto Run";
            this.btnModeAutoRun.UseVisualStyleBackColor = true;
            this.btnModeAutoRun.Click += new System.EventHandler(this.btnModeAutoRun_Click);
            // 
            // btnModeTeaching
            // 
            this.btnModeTeaching.Location = new System.Drawing.Point(124, 10);
            this.btnModeTeaching.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnModeTeaching.Name = "btnModeTeaching";
            this.btnModeTeaching.Size = new System.Drawing.Size(100, 30);
            this.btnModeTeaching.TabIndex = 1;
            this.btnModeTeaching.Text = "Teaching";
            this.btnModeTeaching.UseVisualStyleBackColor = true;
            this.btnModeTeaching.Click += new System.EventHandler(this.btnModeTeaching_Click);
            // 
            // btnModeOption
            // 
            this.btnModeOption.Location = new System.Drawing.Point(232, 10);
            this.btnModeOption.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnModeOption.Name = "btnModeOption";
            this.btnModeOption.Size = new System.Drawing.Size(100, 30);
            this.btnModeOption.TabIndex = 2;
            this.btnModeOption.Text = "Option";
            this.btnModeOption.UseVisualStyleBackColor = true;
            this.btnModeOption.Visible = false;
            this.btnModeOption.Click += new System.EventHandler(this.btnModeOption_Click);
            // 
            // flpHeaderCenter
            // 
            this.flpHeaderCenter.Controls.Add(this.lblTimeValue);
            this.flpHeaderCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpHeaderCenter.Location = new System.Drawing.Point(360, 0);
            this.flpHeaderCenter.Margin = new System.Windows.Forms.Padding(0);
            this.flpHeaderCenter.Name = "flpHeaderCenter";
            this.flpHeaderCenter.Padding = new System.Windows.Forms.Padding(6, 10, 6, 8);
            this.flpHeaderCenter.Size = new System.Drawing.Size(964, 52);
            this.flpHeaderCenter.TabIndex = 1;
            this.flpHeaderCenter.WrapContents = false;
            // 
            // lblTimeValue
            // 
            this.lblTimeValue.Location = new System.Drawing.Point(16, 10);
            this.lblTimeValue.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblTimeValue.Name = "lblTimeValue";
            this.lblTimeValue.Size = new System.Drawing.Size(160, 22);
            this.lblTimeValue.TabIndex = 9;
            this.lblTimeValue.Text = "2026-04-03 10:00:00";
            this.lblTimeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flpHeaderRight
            // 
            this.flpHeaderRight.Controls.Add(this.cboLanguage);
            this.flpHeaderRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpHeaderRight.Location = new System.Drawing.Point(1324, 0);
            this.flpHeaderRight.Margin = new System.Windows.Forms.Padding(0);
            this.flpHeaderRight.Name = "flpHeaderRight";
            this.flpHeaderRight.Padding = new System.Windows.Forms.Padding(4, 10, 12, 8);
            this.flpHeaderRight.Size = new System.Drawing.Size(600, 52);
            this.flpHeaderRight.TabIndex = 2;
            this.flpHeaderRight.WrapContents = false;
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Items.AddRange(new object[] {
            "Kr",
            "En",
            "Cn",
            "Jp",
            "Vn",
            "In"});
            this.cboLanguage.Location = new System.Drawing.Point(8, 12);
            this.cboLanguage.Margin = new System.Windows.Forms.Padding(4, 2, 0, 0);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(58, 20);
            this.cboLanguage.TabIndex = 0;
            this.cboLanguage.SelectedIndexChanged += new System.EventHandler(this.cboLanguage_SelectedIndexChanged);
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.Controls.Add(this.tlpToolbar);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlToolbar.Location = new System.Drawing.Point(0, 52);
            this.pnlToolbar.Margin = new System.Windows.Forms.Padding(0);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(1924, 96);
            this.pnlToolbar.TabIndex = 1;
            // 
            // tlpToolbar
            // 
            this.tlpToolbar.ColumnCount = 2;
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.5842F));
            this.tlpToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.4158F));
            this.tlpToolbar.Controls.Add(this.tlpToolbarButtons, 0, 0);
            this.tlpToolbar.Controls.Add(this.pnlToolbarRight, 1, 0);
            this.tlpToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpToolbar.Location = new System.Drawing.Point(0, 0);
            this.tlpToolbar.Margin = new System.Windows.Forms.Padding(0);
            this.tlpToolbar.Name = "tlpToolbar";
            this.tlpToolbar.RowCount = 1;
            this.tlpToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpToolbar.Size = new System.Drawing.Size(1924, 96);
            this.tlpToolbar.TabIndex = 0;
            // 
            // tlpToolbarButtons
            // 
            this.tlpToolbarButtons.ColumnCount = 5;
            this.tlpToolbarButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpToolbarButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpToolbarButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpToolbarButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpToolbarButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpToolbarButtons.Controls.Add(this.btnMenu1, 0, 0);
            this.tlpToolbarButtons.Controls.Add(this.btnMenu2, 1, 0);
            this.tlpToolbarButtons.Controls.Add(this.btnMenu3, 2, 0);
            this.tlpToolbarButtons.Controls.Add(this.btnMenu4, 3, 0);
            this.tlpToolbarButtons.Controls.Add(this.btnMenu5, 4, 0);
            this.tlpToolbarButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpToolbarButtons.Location = new System.Drawing.Point(3, 3);
            this.tlpToolbarButtons.Name = "tlpToolbarButtons";
            this.tlpToolbarButtons.RowCount = 1;
            this.tlpToolbarButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpToolbarButtons.Size = new System.Drawing.Size(948, 90);
            this.tlpToolbarButtons.TabIndex = 0;
            // 
            // btnMenu1
            // 
            this.btnMenu1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMenu1.Location = new System.Drawing.Point(3, 3);
            this.btnMenu1.Name = "btnMenu1";
            this.btnMenu1.Size = new System.Drawing.Size(183, 84);
            this.btnMenu1.TabIndex = 0;
            this.btnMenu1.Text = "Menu1";
            this.btnMenu1.UseVisualStyleBackColor = false;
            this.btnMenu1.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnMenu2
            // 
            this.btnMenu2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMenu2.Location = new System.Drawing.Point(192, 3);
            this.btnMenu2.Name = "btnMenu2";
            this.btnMenu2.Size = new System.Drawing.Size(183, 84);
            this.btnMenu2.TabIndex = 1;
            this.btnMenu2.Text = "Menu2";
            this.btnMenu2.UseVisualStyleBackColor = false;
            this.btnMenu2.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnMenu3
            // 
            this.btnMenu3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMenu3.Location = new System.Drawing.Point(381, 3);
            this.btnMenu3.Name = "btnMenu3";
            this.btnMenu3.Size = new System.Drawing.Size(183, 84);
            this.btnMenu3.TabIndex = 2;
            this.btnMenu3.Text = "Menu3";
            this.btnMenu3.UseVisualStyleBackColor = false;
            this.btnMenu3.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnMenu4
            // 
            this.btnMenu4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMenu4.Location = new System.Drawing.Point(570, 3);
            this.btnMenu4.Name = "btnMenu4";
            this.btnMenu4.Size = new System.Drawing.Size(183, 84);
            this.btnMenu4.TabIndex = 3;
            this.btnMenu4.Text = "Menu4";
            this.btnMenu4.UseVisualStyleBackColor = false;
            this.btnMenu4.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // btnMenu5
            // 
            this.btnMenu5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMenu5.Location = new System.Drawing.Point(759, 3);
            this.btnMenu5.Name = "btnMenu5";
            this.btnMenu5.Size = new System.Drawing.Size(186, 84);
            this.btnMenu5.TabIndex = 4;
            this.btnMenu5.Text = "Menu5";
            this.btnMenu5.UseVisualStyleBackColor = false;
            this.btnMenu5.Click += new System.EventHandler(this.btnToolbar_Click);
            // 
            // pnlToolbarRight
            // 
            this.pnlToolbarRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlToolbarRight.Location = new System.Drawing.Point(954, 0);
            this.pnlToolbarRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlToolbarRight.Name = "pnlToolbarRight";
            this.pnlToolbarRight.Size = new System.Drawing.Size(970, 96);
            this.pnlToolbarRight.TabIndex = 1;
            // 
            // pnlContentHost
            // 
            this.pnlContentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContentHost.Location = new System.Drawing.Point(2, 150);
            this.pnlContentHost.Margin = new System.Windows.Forms.Padding(2);
            this.pnlContentHost.Name = "pnlContentHost";
            this.pnlContentHost.Size = new System.Drawing.Size(1920, 848);
            this.pnlContentHost.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1924, 1000);
            this.Controls.Add(this.tlpRoot);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vision Inspect";
            this.Load += new System.EventHandler(this.FrmVisionMain_Load);
            this.tlpRoot.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.flpHeaderModes.ResumeLayout(false);
            this.flpHeaderCenter.ResumeLayout(false);
            this.flpHeaderRight.ResumeLayout(false);
            this.pnlToolbar.ResumeLayout(false);
            this.tlpToolbar.ResumeLayout(false);
            this.tlpToolbarButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
