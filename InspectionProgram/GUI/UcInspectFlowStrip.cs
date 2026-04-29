using System.Drawing;
using System.Windows.Forms;
using InspectionProgram.Common;

namespace InspectionProgram.GUI
{
    /// <summary>Threshold, 자동 사이클, Run/Stop, CSV — Auto Run / Teaching 공용 툴 스트립.</summary>
    public partial class UcInspectFlowStrip : UserControl
    {
        public Button BtnPrevImage => btnPrevImage;
        public Button BtnNextImage => btnNextImage;
        public Label LblThresholdTitle => lblThresholdTitle;
        public TrackBar TrkThreshold => trkThreshold;
        public Label LblThresholdValue => lblThresholdValue;
        public CheckBox ChkAutoCycle => chkAutoCycle;
        public Button BtnRunInspection => btnRunInspection;
        public Button BtnStopBatch => btnStopBatch;
        public Button BtnSaveCsv => btnSaveCsv;

        public UcInspectFlowStrip()
        {
            InitializeComponent();
            TryApplyArrowIcons();
            ApplyLanguage(LanguageType.Kr);
        }

        public void ApplyLanguage(LanguageType language)
        {
            try
            {
                if (lblThresholdTitle != null)
                    lblThresholdTitle.Text = LocalizationService.GetText("Threshold", language);
                if (chkAutoCycle != null)
                    chkAutoCycle.Text = LocalizationService.GetText("AutoCycleRun", language);
                if (btnRunInspection != null)
                    btnRunInspection.Text = LocalizationService.GetText("RunInspectStep", language);
                if (btnStopBatch != null)
                    btnStopBatch.Text = LocalizationService.GetText("StopShort", language);
                if (btnSaveCsv != null)
                    btnSaveCsv.Text = LocalizationService.GetText("SaveCsvStep", language);
            }
            catch
            {
            }
        }

        private void TryApplyArrowIcons()
        {
            try
            {
                if (btnPrevImage != null)
                {
                    btnPrevImage.Text = string.Empty;
                    btnPrevImage.Image = CreateArrowBitmap(left: true);
                    btnPrevImage.ImageAlign = ContentAlignment.MiddleCenter;
                }

                if (btnNextImage != null)
                {
                    btnNextImage.Text = string.Empty;
                    btnNextImage.Image = CreateArrowBitmap(left: false);
                    btnNextImage.ImageAlign = ContentAlignment.MiddleCenter;
                }
            }
            catch
            {
            }
        }

        private static Bitmap CreateArrowBitmap(bool left)
        {
            var bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var p = new Pen(Color.White, 2f))
                {
                    if (left)
                    {
                        g.DrawLines(p, new[]
                        {
                            new Point(10, 3),
                            new Point(6, 8),
                            new Point(10, 13),
                        });
                    }
                    else
                    {
                        g.DrawLines(p, new[]
                        {
                            new Point(6, 3),
                            new Point(10, 8),
                            new Point(6, 13),
                        });
                    }
                }
            }
            return bmp;
        }

        public void ApplyAppTheme()
        {
            try
            {
                BackColor = AppColors.SurfaceDark;
                flpRoot.BackColor = AppColors.SurfaceDark;
                if (chkAutoCycle != null)
                {
                    chkAutoCycle.BackColor = AppColors.SurfaceDark;
                    chkAutoCycle.ForeColor = AppColors.Foreground;
                }

                foreach (Control c in flpRoot.Controls)
                {
                    var b = c as Button;
                    if (b == null)
                        continue;
                    b.UseVisualStyleBackColor = false;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = AppColors.Border;
                    b.BackColor = AppColors.SurfaceLight;
                    b.ForeColor = AppColors.Foreground;
                    b.Font = AppFontHelper.Create(8.25F);
                }

                if (lblThresholdTitle != null)
                    lblThresholdTitle.ForeColor = AppColors.Foreground;
                if (lblThresholdValue != null)
                    lblThresholdValue.ForeColor = AppColors.Foreground;
            }
            catch
            {
            }
        }
    }
}
