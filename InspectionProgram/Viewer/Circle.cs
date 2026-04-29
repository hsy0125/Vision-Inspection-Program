using System.Drawing;

namespace ImageViewerWinForms
{
    public class Circle
    {
        public Point Center { get; set; }
        public int Radius { get; set; }

        public Circle()
        {
            Center = Point.Empty;
            Radius = 0;
        }

        public Circle(Point center, int radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
