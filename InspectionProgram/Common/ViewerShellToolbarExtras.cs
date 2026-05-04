using System;
using System.Drawing;
using System.Windows.Forms;
using ImageViewerWinForms;

namespace InspectionProgram.Common
{
    /// <summary>
    /// AutoRun / Teaching 셸의 <see cref="FlowLayoutPanel"/>에 뷰어 기능 버튼을 붙이고 토글 표시를 맞춥니다.
    /// </summary>
    internal static class ViewerShellToolbarExtras
    {
        public static Button[] Append(FlowLayoutPanel flp, EventHandler onToolClick)
        {
            Button[] buttons = CreateButtons(onToolClick);
            foreach (Button b in buttons)
                flp.Controls.Add(b);
            return buttons;
        }

        public static Button[] InsertBefore(FlowLayoutPanel flp, Control anchor, EventHandler onToolClick)
        {
            Button[] buttons = CreateButtons(onToolClick);
            int idx = anchor != null ? flp.Controls.IndexOf(anchor) : -1;
            if (idx < 0)
            {
                foreach (Button b in buttons)
                    flp.Controls.Add(b);
                return buttons;
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                flp.Controls.Add(buttons[i]);
                flp.Controls.SetChildIndex(buttons[i], idx + i);
            }

            return buttons;
        }

        public static void ApplyToolbarLanguage(FlowLayoutPanel flp, LanguageType language)
        {
            if (flp?.Controls == null)
                return;
            foreach (Control c in flp.Controls)
            {
                Button b = c as Button;
                if (b == null)
                    continue;
                string tag = b.Tag as string;
                if (string.IsNullOrEmpty(tag))
                    continue;
                switch (tag)
                {
                    case "FIT":
                        b.Text = LocalizationService.GetText("FitImage", language);
                        break;
                    case "CLR_OVR":
                        b.Text = LocalizationService.GetText("OvClrShort", language);
                        break;
                    case "CROSS":
                        b.Text = LocalizationService.GetText("Cross", language);
                        break;
                    case "GRAY":
                        b.Text = LocalizationService.GetText("Gray", language);
                        break;
                    case "AVG":
                        b.Text = LocalizationService.GetText("Average", language);
                        break;
                    case "SYNC":
                        b.Text = LocalizationService.GetText("SyncShort", language);
                        break;
                    case "MAP":
                        b.Text = LocalizationService.GetText("MapShort", language);
                        break;
                }
            }
        }

        public static void RefreshToggleButtonColors(ImageViewPanelView1 viewer, FlowLayoutPanel flp)
        {
            if (viewer?.CanvasControl == null || flp == null)
                return;

            Color onColor = AppColors.SelectionHighlight;
            Color offColor = AppColors.SurfaceLight;

            foreach (Control c in flp.Controls)
            {
                Button btn = c as Button;
                if (btn == null)
                    continue;

                string tag = btn.Tag as string;
                if (string.IsNullOrEmpty(tag))
                    continue;

                bool? on = null;
                switch (tag)
                {
                    case "ZM":
                        on = viewer.CanvasControl.ZoomModeEnabled;
                        break;
                    case "CROSS":
                        on = viewer.CanvasControl.ShowCenterCross;
                        break;
                    case "GRAY":
                        on = viewer.CanvasControl.ShowPixelGrayValue;
                        break;
                    case "AVG":
                        on = viewer.CanvasControl.AvgModeEnabled;
                        break;
                    case "MAP":
                        on = viewer.CanvasControl.ShowMiniMap;
                        break;
                    case "SYNC":
                        on = ViewSyncManager.IsSyncEnabled;
                        break;
                }

                if (on.HasValue)
                    btn.BackColor = on.Value ? onColor : offColor;
            }
        }

        private static Button[] CreateButtons(EventHandler onToolClick)
        {
            Button Mk(string tag, string text, int w)
            {
                Button b = new Button
                {
                    Tag = tag,
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(2, 0, 6, 0),
                    Size = new Size(w, 24),
                    TabStop = false,
                    UseVisualStyleBackColor = true,
                };
                b.Click += onToolClick;
                return b;
            }

            return new[]
            {
                Mk("FIT", "Fit", 40),
                Mk("CLR_OVR", "OvClr", 48),
                Mk("CROSS", "Cross", 48),
                Mk("GRAY", "Gray", 44),
                Mk("AVG", "Avg", 40),
                Mk("SYNC", "Sync", 44),
                Mk("MAP", "Map", 40),
            };
        }
    }
}
