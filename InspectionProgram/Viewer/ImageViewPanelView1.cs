using System;
using System.Diagnostics;

namespace ImageViewerWinForms
{
    public class ImageViewPanelView1 : ImageViewPanelBase
    {
        public ImageViewPanelView1()
        {
            try
            {
                Name = "viewPanel1";
                ViewTitle = "Display View 1";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ImageViewPanelView1
            // 
            this.Name = "ImageViewPanelView1";
            this.Size = new System.Drawing.Size(1532, 482);
            this.ResumeLayout(false);

        }
    }
}
