using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ImageViewerWinForms
{
    public class ToolbarIconButton : Button
    {
        private bool _toggleMode;
        private bool _checkedState = true;
        private string _symbol = string.Empty;
        private string _iconPathOn = string.Empty;
        private string _iconPathOff = string.Empty;
        private Image _iconOn;
        private Image _iconOff;

        [DefaultValue(false)]
        public bool ToggleMode
        {
            get { return _toggleMode; }
            set
            {
                try
                {
                    _toggleMode = value;
                    Invalidate();
                }
                catch
                {
                }
            }
        }

        [DefaultValue(true)]
        public bool CheckedState
        {
            get { return _checkedState; }
            set
            {
                try
                {
                    _checkedState = value;
                    Invalidate();
                }
                catch
                {
                }
            }
        }

        [DefaultValue("")]
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                try
                {
                    _symbol = value ?? string.Empty;
                    Invalidate();
                }
                catch
                {
                }
            }
        }

        [DefaultValue("")]
        public string IconPathOn
        {
            get { return _iconPathOn; }
            set
            {
                try
                {
                    _iconPathOn = value ?? string.Empty;
                    ReloadIcons();
                    Invalidate();
                }
                catch
                {
                }
            }
        }

        [DefaultValue("")]
        public string IconPathOff
        {
            get { return _iconPathOff; }
            set
            {
                try
                {
                    _iconPathOff = value ?? string.Empty;
                    ReloadIcons();
                    Invalidate();
                }
                catch
                {
                }
            }
        }

        public ToolbarIconButton()
        {
            try
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw, true);

                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                FlatAppearance.MouseDownBackColor = ViewerUiStyle.ToolbarButtonPressedBackColor;
                FlatAppearance.MouseOverBackColor = ViewerUiStyle.ToolbarButtonHoverBackColor;
                BackColor = ViewerUiStyle.ToolbarButtonBackColor;
                ForeColor = Color.White;
                Font = new Font("Segoe UI Symbol", 9.0f, FontStyle.Bold);
                Margin = new Padding(1);
                Padding = new Padding(0);
                Size = ViewerUiStyle.ToolbarButtonSize;
                Text = string.Empty;
                TabStop = false;
                Cursor = Cursors.Hand;
            }
            catch
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_iconOn != null)
                    {
                        _iconOn.Dispose();
                        _iconOn = null;
                    }

                    if (_iconOff != null)
                    {
                        _iconOff.Dispose();
                        _iconOff = null;
                    }
                }
            }
            catch
            {
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            try
            {
                Graphics g = pevent.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.Clear(BackColor);

                Rectangle rect = ClientRectangle;
                bool isCheckedButton = ToggleMode && CheckedState;
                Color fillColor = Enabled
                    ? (isCheckedButton ? ViewerUiStyle.ToolbarButtonCheckedBackColor : BackColor)
                    : ViewerUiStyle.ToolbarButtonDisabledBackColor;
                Color borderColor = isCheckedButton
                    ? ViewerUiStyle.ToolbarButtonCheckedBorderColor
                    : ViewerUiStyle.ToolbarButtonBorderColor;

                using (SolidBrush brush = new SolidBrush(fillColor))
                {
                    g.FillRectangle(brush, rect);
                }

                using (Pen borderPen = new Pen(borderColor))
                {
                    g.DrawRectangle(borderPen, 0, 0, rect.Width - 1, rect.Height - 1);
                }

                Rectangle iconRect = Rectangle.Inflate(rect, -ViewerUiStyle.ToolbarButtonIconPadding, -ViewerUiStyle.ToolbarButtonIconPadding);
                Image icon = ResolveIcon();

                if (icon != null)
                {
                    g.DrawImage(icon, iconRect);
                }
                else
                {
                    DrawSymbol(g, iconRect);
                }

                if (ToggleMode && CheckedState == false && string.IsNullOrWhiteSpace(IconPathOff))
                {
                    using (Pen slashPen = new Pen(Color.Red, 2.2f))
                    {
                        g.DrawLine(slashPen, 4, Height - 5, Width - 5, 4);
                    }
                }

                if (Enabled == false)
                {
                    using (SolidBrush overlayBrush = new SolidBrush(ViewerUiStyle.ToolbarButtonDisabledOverlayColor))
                    {
                        g.FillRectangle(overlayBrush, rect);
                    }
                }
            }
            catch
            {
                base.OnPaint(pevent);
            }
        }

        private void DrawSymbol(Graphics g, Rectangle iconRect)
        {
            try
            {
                TextRenderer.DrawText(
                    g,
                    string.IsNullOrWhiteSpace(Symbol) ? "?" : Symbol,
                    Font,
                    iconRect,
                    Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }
            catch
            {
            }
        }

        private Image ResolveIcon()
        {
            try
            {
                if (ToggleMode)
                {
                    return CheckedState ? _iconOn : _iconOff ?? _iconOn;
                }

                return _iconOn;
            }
            catch
            {
                return null;
            }
        }

        private void ReloadIcons()
        {
            try
            {
                if (_iconOn != null)
                {
                    _iconOn.Dispose();
                    _iconOn = null;
                }

                if (_iconOff != null)
                {
                    _iconOff.Dispose();
                    _iconOff = null;
                }

                _iconOn = LoadIcon(IconPathOn);
                _iconOff = LoadIcon(IconPathOff);
            }
            catch
            {
            }
        }

        private Image LoadIcon(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
                    return null;

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (Image src = Image.FromStream(fs))
                {
                    return new Bitmap(src);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
