using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ImageViewerWinForms
{
    public enum ROIType
    {
        Rectangle,
        Circle,
        Polygon,
        Line,
        Point
    }

    public abstract class ROI_Manager
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public int Width { get; set; }
        public bool Visible { get; set; }
        public bool Selected { get; set; }

        protected ROI_Manager()
        {
            Name = string.Empty;
            Color = Color.Lime;
            Width = 2;
            Visible = true;
            Selected = false;
        }

        public abstract void Draw(ImageCanvasControl canvas);
        public abstract Rectangle GetBounds();
        public abstract Point[] GetHandlePoints();
        public abstract bool HitTest(Point imagePoint, int tolerance);
        public abstract int HitTestHandle(Point imagePoint, int tolerance);
        public abstract void Move(int dx, int dy);
        public abstract void ResizeByHandle(int handleIndex, Point imagePoint);
    }

    public sealed class ROIRectangle : ROI_Manager
    {
        public Rectangle Rect { get; set; }

        public ROIRectangle()
        {
            Rect = Rectangle.Empty;
        }

        public ROIRectangle(Rectangle rect, Color color, int width)
        {
            Rect = NormalizeRect(rect);
            Color = color;
            Width = width;
            Visible = true;
        }

        public override void Draw(ImageCanvasControl canvas)
        {
            if (canvas == null || Visible == false)
                return;

            canvas.DrawRect(Rect, Color, Width);
        }

        public override Rectangle GetBounds()
        {
            return NormalizeRect(Rect);
        }

        public override Point[] GetHandlePoints()
        {
            Rectangle rect = NormalizeRect(Rect);
            return new[]
            {
                new Point(rect.Left, rect.Top),
                new Point(rect.Right, rect.Top),
                new Point(rect.Right, rect.Bottom),
                new Point(rect.Left, rect.Bottom),
                new Point(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2))
            };
        }

        public override bool HitTest(Point imagePoint, int tolerance)
        {
            Rectangle rect = NormalizeRect(Rect);
            Rectangle expanded = Rectangle.Inflate(rect, tolerance, tolerance);
            return expanded.Contains(imagePoint);
        }

        public override int HitTestHandle(Point imagePoint, int tolerance)
        {
            Point[] handles = GetHandlePoints();
            for (int i = 0; i < handles.Length; i++)
            {
                Rectangle handleRect = new Rectangle(handles[i].X - tolerance, handles[i].Y - tolerance, tolerance * 2, tolerance * 2);
                if (handleRect.Contains(imagePoint))
                    return i;
            }

            return -1;
        }

        public override void Move(int dx, int dy)
        {
            Rect = new Rectangle(Rect.X + dx, Rect.Y + dy, Rect.Width, Rect.Height);
        }

        public override void ResizeByHandle(int handleIndex, Point imagePoint)
        {
            Rectangle rect = NormalizeRect(Rect);

            switch (handleIndex)
            {
                case 0:
                    rect = Rectangle.FromLTRB(imagePoint.X, imagePoint.Y, rect.Right, rect.Bottom);
                    break;
                case 1:
                    rect = Rectangle.FromLTRB(rect.Left, imagePoint.Y, imagePoint.X, rect.Bottom);
                    break;
                case 2:
                    rect = Rectangle.FromLTRB(rect.Left, rect.Top, imagePoint.X, imagePoint.Y);
                    break;
                case 3:
                    rect = Rectangle.FromLTRB(imagePoint.X, rect.Top, rect.Right, imagePoint.Y);
                    break;
                case 4:
                    int centerX = rect.Left + (rect.Width / 2);
                    int centerY = rect.Top + (rect.Height / 2);
                    Move(imagePoint.X - centerX, imagePoint.Y - centerY);
                    return;
                default:
                    return;
            }

            rect = NormalizeRect(rect);
            rect.Width = Math.Max(4, rect.Width);
            rect.Height = Math.Max(4, rect.Height);
            Rect = rect;
        }

        private static Rectangle NormalizeRect(Rectangle rect)
        {
            int left = Math.Min(rect.Left, rect.Right);
            int top = Math.Min(rect.Top, rect.Bottom);
            int right = Math.Max(rect.Left, rect.Right);
            int bottom = Math.Max(rect.Top, rect.Bottom);
            return Rectangle.FromLTRB(left, top, right, bottom);
        }
    }

    public sealed class ROILine : ROI_Manager
    {
        public Point FirstPoint { get; set; }
        public Point SecondPoint { get; set; }

        public override void Draw(ImageCanvasControl canvas)
        {
            if (canvas == null || Visible == false)
                return;

            canvas.DrawLine(FirstPoint, SecondPoint, Color, Width);
        }

        public override Rectangle GetBounds()
        {
            int left = Math.Min(FirstPoint.X, SecondPoint.X);
            int top = Math.Min(FirstPoint.Y, SecondPoint.Y);
            int right = Math.Max(FirstPoint.X, SecondPoint.X);
            int bottom = Math.Max(FirstPoint.Y, SecondPoint.Y);
            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        public override Point[] GetHandlePoints()
        {
            return new[]
            {
                FirstPoint,
                SecondPoint,
                new Point((FirstPoint.X + SecondPoint.X) / 2, (FirstPoint.Y + SecondPoint.Y) / 2)
            };
        }

        public override bool HitTest(Point imagePoint, int tolerance)
        {
            using (GraphicsPath path = new GraphicsPath())
            using (Pen pen = new Pen(Color.Black, Math.Max(Width, tolerance * 2)))
            {
                path.AddLine(FirstPoint, SecondPoint);
                return path.IsOutlineVisible(imagePoint, pen);
            }
        }

        public override int HitTestHandle(Point imagePoint, int tolerance)
        {
            Point[] handles = GetHandlePoints();
            for (int i = 0; i < handles.Length; i++)
            {
                Rectangle handleRect = new Rectangle(handles[i].X - tolerance, handles[i].Y - tolerance, tolerance * 2, tolerance * 2);
                if (handleRect.Contains(imagePoint))
                    return i;
            }

            return -1;
        }

        public override void Move(int dx, int dy)
        {
            FirstPoint = new Point(FirstPoint.X + dx, FirstPoint.Y + dy);
            SecondPoint = new Point(SecondPoint.X + dx, SecondPoint.Y + dy);
        }

        public override void ResizeByHandle(int handleIndex, Point imagePoint)
        {
            if (handleIndex == 0)
                FirstPoint = imagePoint;
            else if (handleIndex == 1)
                SecondPoint = imagePoint;
            else if (handleIndex == 2)
            {
                Point mid = new Point((FirstPoint.X + SecondPoint.X) / 2, (FirstPoint.Y + SecondPoint.Y) / 2);
                Move(imagePoint.X - mid.X, imagePoint.Y - mid.Y);
            }
        }
    }

    public sealed class ROIPoint : ROI_Manager
    {
        public Point Point { get; set; }
        public string PointType { get; set; }

        public ROIPoint()
        {
            PointType = "cross";
        }

        public override void Draw(ImageCanvasControl canvas)
        {
            if (canvas == null || Visible == false)
                return;

            canvas.DrawPoint(Point, PointType, Color, Width);
        }

        public override Rectangle GetBounds()
        {
            return new Rectangle(Point.X - 3, Point.Y - 3, 6, 6);
        }

        public override Point[] GetHandlePoints()
        {
            return new[] { Point };
        }

        public override bool HitTest(Point imagePoint, int tolerance)
        {
            Rectangle rect = new Rectangle(Point.X - tolerance, Point.Y - tolerance, tolerance * 2, tolerance * 2);
            return rect.Contains(imagePoint);
        }

        public override int HitTestHandle(Point imagePoint, int tolerance)
        {
            return HitTest(imagePoint, tolerance) ? 0 : -1;
        }

        public override void Move(int dx, int dy)
        {
            Point = new Point(Point.X + dx, Point.Y + dy);
        }

        public override void ResizeByHandle(int handleIndex, Point imagePoint)
        {
            Point = imagePoint;
        }
    }

    public sealed class ROICircle : ROI_Manager
    {
        public Circle Circle { get; set; }

        public ROICircle()
        {
            Circle = new Circle();
        }

        public override void Draw(ImageCanvasControl canvas)
        {
            if (canvas == null || Visible == false)
                return;

            canvas.DrawCircle(Circle, Color, Width);
        }

        public override Rectangle GetBounds()
        {
            if (Circle == null)
                return Rectangle.Empty;

            return new Rectangle(
                Circle.Center.X - Circle.Radius,
                Circle.Center.Y - Circle.Radius,
                Circle.Radius * 2,
                Circle.Radius * 2);
        }

        public override Point[] GetHandlePoints()
        {
            if (Circle == null)
                return new Point[0];

            return new[]
            {
                Circle.Center,
                new Point(Circle.Center.X + Circle.Radius, Circle.Center.Y)
            };
        }

        public override bool HitTest(Point imagePoint, int tolerance)
        {
            if (Circle == null)
                return false;

            int dx = imagePoint.X - Circle.Center.X;
            int dy = imagePoint.Y - Circle.Center.Y;
            double distance = Math.Sqrt((dx * dx) + (dy * dy));
            return distance <= Circle.Radius + tolerance;
        }

        public override int HitTestHandle(Point imagePoint, int tolerance)
        {
            Point[] handles = GetHandlePoints();
            for (int i = 0; i < handles.Length; i++)
            {
                Rectangle handleRect = new Rectangle(handles[i].X - tolerance, handles[i].Y - tolerance, tolerance * 2, tolerance * 2);
                if (handleRect.Contains(imagePoint))
                    return i;
            }

            return -1;
        }

        public override void Move(int dx, int dy)
        {
            if (Circle == null)
                return;

            Circle = new Circle(new Point(Circle.Center.X + dx, Circle.Center.Y + dy), Circle.Radius);
        }

        public override void ResizeByHandle(int handleIndex, Point imagePoint)
        {
            if (Circle == null)
                return;

            if (handleIndex == 0)
            {
                Circle = new Circle(imagePoint, Circle.Radius);
                return;
            }

            int dx = imagePoint.X - Circle.Center.X;
            int dy = imagePoint.Y - Circle.Center.Y;
            int radius = (int)Math.Round(Math.Sqrt((dx * dx) + (dy * dy)));
            Circle = new Circle(Circle.Center, Math.Max(4, radius));
        }
    }

    public sealed class ROIPolygon : ROI_Manager
    {
        public Point[] Points { get; set; }

        public ROIPolygon()
        {
            Points = new Point[0];
        }

        public override void Draw(ImageCanvasControl canvas)
        {
            if (canvas == null || Visible == false)
                return;

            canvas.DrawPolygon(Points, Color, Width);
        }

        public override Rectangle GetBounds()
        {
            if (Points == null || Points.Length == 0)
                return Rectangle.Empty;

            int left = Points.Min(x => x.X);
            int top = Points.Min(x => x.Y);
            int right = Points.Max(x => x.X);
            int bottom = Points.Max(x => x.Y);
            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        public override Point[] GetHandlePoints()
        {
            return Points ?? new Point[0];
        }

        public override bool HitTest(Point imagePoint, int tolerance)
        {
            if (Points == null || Points.Length < 3)
                return false;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(Points);
                using (Pen pen = new Pen(Color.Black, Math.Max(Width, tolerance * 2)))
                {
                    return path.IsVisible(imagePoint) || path.IsOutlineVisible(imagePoint, pen);
                }
            }
        }

        public override int HitTestHandle(Point imagePoint, int tolerance)
        {
            if (Points == null)
                return -1;

            for (int i = 0; i < Points.Length; i++)
            {
                Rectangle handleRect = new Rectangle(Points[i].X - tolerance, Points[i].Y - tolerance, tolerance * 2, tolerance * 2);
                if (handleRect.Contains(imagePoint))
                    return i;
            }

            return -1;
        }

        public override void Move(int dx, int dy)
        {
            if (Points == null)
                return;

            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = new Point(Points[i].X + dx, Points[i].Y + dy);
            }
        }

        public override void ResizeByHandle(int handleIndex, Point imagePoint)
        {
            if (Points == null || handleIndex < 0 || handleIndex >= Points.Length)
                return;

            Points[handleIndex] = imagePoint;
        }
    }

    public sealed class ROICollection
    {
        private readonly List<ROI_Manager> _items = new List<ROI_Manager>();

        public IList<ROI_Manager> Items
        {
            get { return _items; }
        }

        public void Add(ROI_Manager roi)
        {
            if (roi == null)
                return;

            if (string.IsNullOrWhiteSpace(roi.Name))
                roi.Name = "ROI_" + (_items.Count + 1).ToString();

            _items.Add(roi);
        }

        public ROIRectangle Add(ROIType roiType, Point leftTop, int width, int height, Color? drawColor)
        {
            Color color = drawColor ?? Color.Lime;
            if (roiType != ROIType.Rectangle)
                return null;

            ROIRectangle roi = new ROIRectangle(new Rectangle(leftTop.X, leftTop.Y, width, height), color, 2);
            roi.Name = "Rectangle_" + (_items.Count + 1).ToString();
            Add(roi);
            return roi;
        }

        public ROICircle Add(ROIType roiType, Point center, int radius, Color? drawColor)
        {
            Color color = drawColor ?? Color.Lime;
            if (roiType != ROIType.Circle)
                return null;

            ROICircle roi = new ROICircle
            {
                Circle = new Circle(center, radius),
                Color = color,
                Width = 2,
                Name = "Circle_" + (_items.Count + 1).ToString()
            };
            Add(roi);
            return roi;
        }

        public ROIPolygon Add(ROIType roiType, Point[] pois, Color? drawColor)
        {
            Color color = drawColor ?? Color.Lime;
            if (roiType != ROIType.Polygon || pois == null || pois.Length < 2)
                return null;

            ROIPolygon roi = new ROIPolygon
            {
                Points = pois.ToArray(),
                Color = color,
                Width = 2,
                Name = "Polygon_" + (_items.Count + 1).ToString()
            };
            Add(roi);
            return roi;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void DrawAll(ImageCanvasControl canvas)
        {
            if (canvas == null)
                return;

            foreach (ROI_Manager roi in _items)
            {
                if (roi != null)
                    canvas.AddROI(roi);
            }
        }
    }
}
