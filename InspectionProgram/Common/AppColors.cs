using System.Drawing;

namespace InspectionProgram.Common
{
    public static class AppColors
    {
        /// <summary>Main shell background (black).</summary>
        public static readonly Color Background = Color.Black;
        /// <summary>Group boxes and raised panels (slightly above black).</summary>
        public static readonly Color Surface = Color.FromArgb(20, 20, 20);
        public static readonly Color SurfaceDark = Color.FromArgb(12, 12, 12);
        public static readonly Color SurfaceLight = Color.FromArgb(40, 40, 40);
        public static readonly Color HeaderBar = Color.FromArgb(8, 8, 8);
        /// <summary>Flat button and grid lines (gray outline on black UI).</summary>
        public static readonly Color Border = Color.FromArgb(120, 120, 120);
        public static readonly Color Foreground = Color.FromArgb(246, 246, 252);
        /// <summary>Primary highlight / active mode (#f39c12).</summary>
        public static readonly Color Accent = Color.FromArgb(243, 156, 18);
        public static readonly Color AccentForeground = Color.White;
        /// <summary>Selection / emphasis (#3498db).</summary>
        public static readonly Color SelectionHighlight = Color.FromArgb(52, 152, 219);
        public static readonly Color Good = Color.FromArgb(72, 199, 116);
        public static readonly Color Reject = Color.FromArgb(232, 72, 85);
        public static readonly Color Warning = Color.FromArgb(255, 214, 102);
        public static readonly Color ViewerBackground = Color.Black;
        /// <summary>Teaching / Option tool buttons: caption on tinted backgrounds.</summary>
        public static readonly Color ButtonCaption = Color.White;
        /// <summary>Auto / Teaching mode header toggles when latched on.</summary>
        public static readonly Color ModeToggleHighlight = Color.FromArgb(52, 120, 186);
    }
}
