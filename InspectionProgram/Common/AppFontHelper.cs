using System.Drawing;

namespace InspectionProgram.Common
{
    public static class AppFontHelper
    {
        public static Font Create(float size)
        {
            try
            {
                return new Font("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Point);
            }
            catch
            {
                return SystemFonts.DefaultFont;
            }
        }

        public static Font CreateBold(float size)
        {
            try
            {
                return new Font("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point);
            }
            catch
            {
                return SystemFonts.DefaultFont;
            }
        }
    }
}
