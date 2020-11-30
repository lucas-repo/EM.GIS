using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public class PolygonGradientSymbol : PolygonSymbol, IPolygonGradientSymbol
    {
        public PolygonGradientSymbol() : base(PolygonSymbolType.Gradient)
        { }

        public double Angle { get; set; }
        public Color[] Colors { get; set; }
        public GradientType GradientType { get; set; }
        public float[] Positions { get; set; }
        public override Brush GetBrush()
        {
            Brush brush = base.GetBrush();
            RectangleF bounds = Bounds;
            if (bounds.IsEmpty || bounds.Width == 0 || bounds.Height == 0)
            {
                return brush;
            }
            ColorBlend cb = new ColorBlend
            {
                Positions = Positions,
                Colors = Colors
            };
            switch (GradientType)
            {
                case GradientType.Linear:
                    LinearGradientBrush linearGradientBrush = new LinearGradientBrush(bounds, Colors[0], Colors[Colors.Length - 1], (float)-Angle);
                    linearGradientBrush.InterpolationColors = cb;
                    brush = linearGradientBrush;
                    break;
                case GradientType.Circular:
                    {
                        PointF center = new PointF(bounds.X + (bounds.Width / 2), bounds.Y + (bounds.Height / 2));
                        float x = (float)(center.X - (Math.Sqrt(2) * bounds.Width / 2));
                        float y = (float)(center.Y - (Math.Sqrt(2) * bounds.Height / 2));
                        float w = (float)(bounds.Width * Math.Sqrt(2));
                        float h = (float)(bounds.Height * Math.Sqrt(2));
                        RectangleF circum = new RectangleF(x, y, w, h);
                        GraphicsPath round = new GraphicsPath();
                        round.AddEllipse(circum);
                        PathGradientBrush pgb = new PathGradientBrush(round);
                        pgb.InterpolationColors = cb;
                        brush = pgb;
                    }
                    break;
                //case GradientType.Contour:
                //    break;
                case GradientType.Rectangular:
                    {
                        double a = bounds.Width / 2;
                        double b = bounds.Height / 2;
                        double angle = Angle;
                        if (angle < 0) angle = 360 + angle;
                        angle = angle % 90;
                        angle = 2 * (Math.PI * angle / 180);
                        double x = a * Math.Cos(angle);
                        double y = -b - (a * Math.Sin(angle));

                        PointF center = new PointF(bounds.X + (bounds.Width / 2), bounds.Y + (bounds.Height / 2));
                        PointF[] points = new PointF[5];
                        points[0] = new PointF((float)x + center.X, (float)y + center.Y);
                        x = a + (b * Math.Sin(angle));
                        y = b * Math.Cos(angle);
                        points[1] = new PointF((float)x + center.X, (float)y + center.Y);
                        x = -a * Math.Cos(angle);
                        y = b + (a * Math.Sin(angle));
                        points[2] = new PointF((float)x + center.X, (float)y + center.Y);
                        x = -a - (b * Math.Sin(angle));
                        y = -b * Math.Cos(angle);
                        points[3] = new PointF((float)x + center.X, (float)y + center.Y);
                        points[4] = points[0];

                        GraphicsPath rect = new GraphicsPath();
                        rect.AddPolygon(points);

                        PathGradientBrush pgb = new PathGradientBrush(rect);
                        pgb.InterpolationColors = cb;
                        brush = pgb;
                    }
                    break;
            }
            return brush;
        }
    }
}
