
using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public static class PathHelper
    {
        public static GraphicsPath ToPath(this RectangleF rectangle)
        {
            float xMin = rectangle.X;
            float yMin = rectangle.Y;
            float xMax = xMin + rectangle.Width;
            float yMax = yMin + rectangle.Height;
            GraphicsPath path = ExtentToPath(xMin, yMin, xMax, yMax);
            return path;
        }
        public static PointF[] ToPoints(this RectangleF rectangle)
        {
            float xMin = rectangle.X;
            float yMin = rectangle.Y;
            float xMax = xMin + rectangle.Width;
            float yMax = yMin + rectangle.Height;
            PointF[] points = ExtentToPoints(xMin, yMin, xMax, yMax);
            return points;
        }
        public static GraphicsPath ToPath(this Size size)
        {
            float xMin = -size.Width / 2;
            float yMin = -size.Height / 2;
            float xMax = xMin + size.Width;
            float yMax = yMin + size.Height;
            GraphicsPath path = ExtentToPath(xMin, yMin, xMax, yMax);
            return path;
        }
        public static PointF[] ToPoints(this SizeF size)
        {
            float xMin = -size.Width / 2;
            float yMin = -size.Height / 2;
            float xMax = xMin + size.Width;
            float yMax = yMin + size.Height;
            PointF[] points = ExtentToPoints(xMin, yMin, xMax, yMax);
            return points;
        }
        public static GraphicsPath ToPath(this SizeF size)
        {
            float xMin = -size.Width / 2;
            float yMin = -size.Height / 2;
            float xMax = xMin + size.Width;
            float yMax = yMin + size.Height;
            GraphicsPath path = ExtentToPath(xMin, yMin, xMax, yMax); 
            return path;
        }
        public static PointF[] ExtentToPoints(float xMin, float yMin, float xMax, float yMax)
        {
            PointF[] polyPoints = new PointF[]
            {
                new PointF(xMin,yMin),
                new PointF(xMin,yMax),
                new PointF(xMax,yMax),
                new PointF(xMax,yMin),
                new PointF(xMin,yMin)
            };
            return polyPoints;
        }
        public static GraphicsPath ExtentToPath(float xMin, float yMin, float xMax, float yMax)
        {
            PointF[] polyPoints = ExtentToPoints(xMin, yMin, xMax, yMax);
            GraphicsPath path = new GraphicsPath();
            path.AddLines(polyPoints);
            return path;
        }
        public static GraphicsPath ToPath(this PointF[] points)
        {
            GraphicsPath path = null;
            if (points == null || points.Length < 2)
            {
                return path;
            }
            path = new GraphicsPath();
            path.AddLines(points);
            return path;
        }
        public static GraphicsPath ToPath(this Point[] points)
        {
            PointF[] pointFs = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                pointFs[i] = new PointF(points[i].X, points[i].Y);
            }
            GraphicsPath path = pointFs.ToPath();
            return path;
        }
        public static void AddEllipse(GraphicsPath gp, SizeF scaledSize)
        {
            PointF upperLeft = new PointF(-scaledSize.Width / 2, -scaledSize.Height / 2);
            RectangleF destRect = new RectangleF(upperLeft, scaledSize);
            gp.AddEllipse(destRect);
        }
        public static PointF[] ToRegularPoints(this SizeF scaledSize, int numSides)
        {
            PointF[] polyPoints = new PointF[numSides + 1];
            for (int i = 0; i <= numSides; i++)
            {
                double ang = i * (2 * Math.PI) / numSides;
                float x = Convert.ToSingle(Math.Cos(ang)) * scaledSize.Width / 2f;//todo 待测试
                float y = Convert.ToSingle(Math.Sin(ang)) * scaledSize.Height / 2f;
                polyPoints[i] = new PointF(x, y);
            }
            return polyPoints;
        }
        public static void AddRegularPoly(GraphicsPath gp, SizeF scaledSize, int numSides)
        {
            PointF[] polyPoints = new PointF[numSides + 1];

            // Instead of figuring out the points in cartesian, figure them out in angles and re-convert them.
            for (int i = 0; i <= numSides; i++)
            {
                double ang = i * (2 * Math.PI) / numSides;
                float x = Convert.ToSingle(Math.Cos(ang)) * scaledSize.Width / 2f;
                float y = Convert.ToSingle(Math.Sin(ang)) * scaledSize.Height / 2f;
                polyPoints[i] = new PointF(x, y);
            }

            gp.AddPolygon(polyPoints);
        }
        public static GraphicsPath ToRegularPolyPath(this SizeF scaledSize, int numSides)
        {
            PointF[] polyPoints = ToRegularPoints(scaledSize, numSides);
            GraphicsPath path = polyPoints.ToPath();
            return path;
        }
        public static PointF[] ToStarsPoints(this SizeF scaledSize)
        {
            PointF[] polyPoints = new PointF[11];
            for (int i = 0; i <= 10; i++)
            {
                double ang = i * Math.PI / 5;
                float x = Convert.ToSingle(Math.Cos(ang)) * scaledSize.Width / 2f;
                float y = Convert.ToSingle(Math.Sin(ang)) * scaledSize.Height / 2f;
                if (i % 2 == 0)
                {
                    x /= 2; // the shorter radius points of the star
                    y /= 2;
                }
                polyPoints[i] = new PointF(x, y);
            }
            return polyPoints;
        }
        public static void AddStar(GraphicsPath gp, SizeF scaledSize)
        {
            PointF[] polyPoints = new PointF[11];
            GetStars(scaledSize, polyPoints);
            gp.AddPolygon(polyPoints);
        }
        private static void GetStars(SizeF scaledSize, PointF[] polyPoints)
        {
            for (int i = 0; i <= 10; i++)
            {
                double ang = i * Math.PI / 5;
                float x = Convert.ToSingle(Math.Cos(ang)) * scaledSize.Width / 2f;
                float y = Convert.ToSingle(Math.Sin(ang)) * scaledSize.Height / 2f;
                if (i % 2 == 0)
                {
                    x /= 2; // the shorter radius points of the star
                    y /= 2;
                }

                polyPoints[i] = new PointF(x, y);
            }
        }
        public static GraphicsPath ToStarsPath(this SizeF scaledSize)
        {
            PointF[] polyPoints = new PointF[11];
            for (int i = 0; i <= 10; i++)
            {
                double ang = i * Math.PI / 5;
                float x = Convert.ToSingle(Math.Cos(ang)) * scaledSize.Width / 2f;
                float y = Convert.ToSingle(Math.Sin(ang)) * scaledSize.Height / 2f;
                if (i % 2 == 0)
                {
                    x /= 2; // the shorter radius points of the star
                    y /= 2;
                }
                polyPoints[i] = new PointF(x, y);
            }
            GraphicsPath path = polyPoints.ToPath();
            return path;
        }
    }
}
