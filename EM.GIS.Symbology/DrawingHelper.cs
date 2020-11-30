
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Symbology
{
    public static class DrawingHelper
    {
        public static float GetAngle(PointF startPoint, PointF endPoint, bool useDegree)
        {
            float angle = (float)GetAngle(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, useDegree);
            return angle;
        }
        public static double GetAngle(double x0, double y0, double x1, double y1, bool useDegree)
        {
            double value = 0;
            double dx = x1 - x0;
            double dy = y1 - y0;
            if (dx == 0)
            {
                if (dy > 0)
                {
                    value = Math.PI / 2;
                }
                else if (dy < 0)
                {
                    value = Math.PI * 1.5;
                }
            }
            else
            {
                value = Math.Atan(dy / dx);
            }
            if (useDegree)
            {
                value = value * 180 / Math.PI;
                if (value < 0)
                {

                }
            }
            float angle = Convert.ToSingle(value);
            return angle;
        }
        public static PointF GetPoint(PointF startPoint, PointF endPoint, float length)
        {
            var array = GetPoint(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, length);
            return new PointF((float)array[0], (float)array[1]);
        }
        public static double[] GetPoint(double x0, double y0, double x1, double y1, double length)
        {
            double distance = Distance(x0, y0, x1, y1);
            double x = x0;
            double y = y0;
            if (distance != 0)
            {
                double ratio = length / distance;
                x = (x1 - x0) * ratio + x0;
                y = (y1 - y0) * ratio + y0;
            }
            return new double[] { x, y };
        }
        public static double Distance(double x0, double y0, double x1, double y1)
        {
            double distance = Math.Sqrt(Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2));
            return distance;
        }
    }
}
