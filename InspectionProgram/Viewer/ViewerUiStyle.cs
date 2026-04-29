using System.Drawing;

namespace ImageViewerWinForms
{
    internal static class ViewerUiStyle
    {
        // View 선택 테두리 스타일
        public static Color SelectedViewBorderColor = Color.Yellow;
        public static float SelectedViewBorderThickness = 3.0f;
        public static Color NormalViewBorderColor = Color.FromArgb(100, 100, 100);
        public static float NormalViewBorderThickness = 1.0f;

        // Toolbar 버튼 스타일
        public static Size ToolbarButtonSize = new Size(28, 28);
        public static int ToolbarButtonIconPadding = 3;
        public static Color ToolbarButtonBackColor = Color.FromArgb(16, 16, 16);
        public static Color ToolbarButtonCheckedBackColor = Color.FromArgb(32, 32, 32);
        public static Color ToolbarButtonHoverBackColor = Color.FromArgb(40, 40, 40);
        public static Color ToolbarButtonPressedBackColor = Color.FromArgb(50, 50, 50);
        public static Color ToolbarButtonBorderColor = Color.FromArgb(120, 120, 120);
        public static Color ToolbarButtonCheckedBorderColor = Color.Gold;
        public static Color ToolbarButtonDisabledBackColor = Color.FromArgb(12, 12, 12);
        public static Color ToolbarButtonDisabledOverlayColor = Color.FromArgb(70, 18, 18, 18);
    }
}
