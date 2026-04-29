namespace InspectionProgram.GUI
{
    partial class UcInspectFlowStrip
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flpRoot;
        private System.Windows.Forms.Button btnPrevImage;
        private System.Windows.Forms.Button btnNextImage;
        private System.Windows.Forms.Label lblThresholdTitle;
        private System.Windows.Forms.TrackBar trkThreshold;
        private System.Windows.Forms.Label lblThresholdValue;
        private System.Windows.Forms.CheckBox chkAutoCycle;
        private System.Windows.Forms.Button btnRunInspection;
        private System.Windows.Forms.Button btnStopBatch;
        private System.Windows.Forms.Button btnSaveCsv;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flpRoot = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrevImage = new System.Windows.Forms.Button();
            this.btnNextImage = new System.Windows.Forms.Button();
            this.lblThresholdTitle = new System.Windows.Forms.Label();
            this.trkThreshold = new System.Windows.Forms.TrackBar();
            this.lblThresholdValue = new System.Windows.Forms.Label();
            this.chkAutoCycle = new System.Windows.Forms.CheckBox();
            this.btnRunInspection = new System.Windows.Forms.Button();
            this.btnStopBatch = new System.Windows.Forms.Button();
            this.btnSaveCsv = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trkThreshold)).BeginInit();
            this.flpRoot.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpRoot
            // 
            this.flpRoot.Controls.Add(this.btnPrevImage);
            this.flpRoot.Controls.Add(this.btnNextImage);
            this.flpRoot.Controls.Add(this.lblThresholdTitle);
            this.flpRoot.Controls.Add(this.trkThreshold);
            this.flpRoot.Controls.Add(this.lblThresholdValue);
            this.flpRoot.Controls.Add(this.chkAutoCycle);
            this.flpRoot.Controls.Add(this.btnRunInspection);
            this.flpRoot.Controls.Add(this.btnStopBatch);
            this.flpRoot.Controls.Add(this.btnSaveCsv);
            this.flpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpRoot.Location = new System.Drawing.Point(0, 0);
            this.flpRoot.Margin = new System.Windows.Forms.Padding(0);
            this.flpRoot.Name = "flpRoot";
            this.flpRoot.Padding = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.flpRoot.Size = new System.Drawing.Size(800, 56);
            this.flpRoot.TabIndex = 0;
            this.flpRoot.WrapContents = true;
            // 
            // btnPrevImage
            // 
            this.btnPrevImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevImage.Margin = new System.Windows.Forms.Padding(2, 1, 2, 0);
            this.btnPrevImage.Name = "btnPrevImage";
            this.btnPrevImage.Size = new System.Drawing.Size(30, 26);
            this.btnPrevImage.TabIndex = 10;
            this.btnPrevImage.TabStop = false;
            this.btnPrevImage.Tag = "PREV";
            this.btnPrevImage.Text = "<";
            this.btnPrevImage.UseVisualStyleBackColor = true;
            // 
            // btnNextImage
            // 
            this.btnNextImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextImage.Margin = new System.Windows.Forms.Padding(2, 1, 10, 0);
            this.btnNextImage.Name = "btnNextImage";
            this.btnNextImage.Size = new System.Drawing.Size(30, 26);
            this.btnNextImage.TabIndex = 11;
            this.btnNextImage.TabStop = false;
            this.btnNextImage.Tag = "NEXT";
            this.btnNextImage.Text = ">";
            this.btnNextImage.UseVisualStyleBackColor = true;
            // 
            // lblThresholdTitle
            // 
            this.lblThresholdTitle.AutoSize = true;
            this.lblThresholdTitle.Location = new System.Drawing.Point(3, 9);
            this.lblThresholdTitle.Margin = new System.Windows.Forms.Padding(4, 9, 4, 0);
            this.lblThresholdTitle.Name = "lblThresholdTitle";
            this.lblThresholdTitle.Size = new System.Drawing.Size(60, 12);
            this.lblThresholdTitle.TabIndex = 0;
            this.lblThresholdTitle.Text = "Threshold";
            this.lblThresholdTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trkThreshold
            // 
            this.trkThreshold.AutoSize = false;
            this.trkThreshold.Location = new System.Drawing.Point(4, 4);
            this.trkThreshold.Margin = new System.Windows.Forms.Padding(2);
            this.trkThreshold.Maximum = 255;
            this.trkThreshold.Name = "trkThreshold";
            this.trkThreshold.Size = new System.Drawing.Size(200, 28);
            this.trkThreshold.TabIndex = 1;
            this.trkThreshold.TickFrequency = 16;
            this.trkThreshold.Value = 128;
            // 
            // lblThresholdValue
            // 
            this.lblThresholdValue.AutoSize = true;
            this.lblThresholdValue.Location = new System.Drawing.Point(4, 9);
            this.lblThresholdValue.Margin = new System.Windows.Forms.Padding(4, 9, 12, 0);
            this.lblThresholdValue.MinimumSize = new System.Drawing.Size(36, 0);
            this.lblThresholdValue.Name = "lblThresholdValue";
            this.lblThresholdValue.Size = new System.Drawing.Size(36, 12);
            this.lblThresholdValue.TabIndex = 2;
            this.lblThresholdValue.Text = "128";
            this.lblThresholdValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkAutoCycle
            // 
            this.chkAutoCycle.AutoSize = true;
            this.chkAutoCycle.Margin = new System.Windows.Forms.Padding(8, 4, 6, 0);
            this.chkAutoCycle.Name = "chkAutoCycle";
            this.chkAutoCycle.Size = new System.Drawing.Size(124, 16);
            this.chkAutoCycle.TabIndex = 3;
            this.chkAutoCycle.Text = "자동 사이클 (Run)";
            this.chkAutoCycle.UseVisualStyleBackColor = true;
            // 
            // btnRunInspection
            // 
            this.btnRunInspection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRunInspection.Margin = new System.Windows.Forms.Padding(2);
            this.btnRunInspection.Name = "btnRunInspection";
            this.btnRunInspection.Size = new System.Drawing.Size(96, 26);
            this.btnRunInspection.TabIndex = 4;
            this.btnRunInspection.TabStop = false;
            this.btnRunInspection.Tag = "RUN";
            this.btnRunInspection.Text = "3) Run inspect";
            this.btnRunInspection.UseVisualStyleBackColor = true;
            // 
            // btnStopBatch
            // 
            this.btnStopBatch.Enabled = false;
            this.btnStopBatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopBatch.Margin = new System.Windows.Forms.Padding(2, 1, 6, 0);
            this.btnStopBatch.Name = "btnStopBatch";
            this.btnStopBatch.Size = new System.Drawing.Size(52, 26);
            this.btnStopBatch.TabIndex = 5;
            this.btnStopBatch.TabStop = false;
            this.btnStopBatch.Tag = "STOP";
            this.btnStopBatch.Text = "Stop";
            this.btnStopBatch.UseVisualStyleBackColor = true;
            // 
            // btnSaveCsv
            // 
            this.btnSaveCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCsv.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveCsv.Name = "btnSaveCsv";
            this.btnSaveCsv.Size = new System.Drawing.Size(120, 26);
            this.btnSaveCsv.TabIndex = 6;
            this.btnSaveCsv.TabStop = false;
            this.btnSaveCsv.Tag = "CSV";
            this.btnSaveCsv.Text = "4) CSV 저장";
            this.btnSaveCsv.UseVisualStyleBackColor = true;
            // 
            // UcInspectFlowStrip
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flpRoot);
            this.Name = "UcInspectFlowStrip";
            this.Size = new System.Drawing.Size(800, 56);
            ((System.ComponentModel.ISupportInitialize)(this.trkThreshold)).EndInit();
            this.flpRoot.ResumeLayout(false);
            this.flpRoot.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
