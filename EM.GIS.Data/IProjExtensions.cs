
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EM.GIS.Data
{
    public static class IProjExtensions
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this map frame should define its buffer
        /// region to be the same size as the client, or three times larger.
        /// </summary>
        public static bool ExtendBuffer { get; set; }

        /// <summary>
        /// Gets or sets the coefficient used for ExtendBuffer. This coefficient should not be modified.
        /// </summary>
        public static int ExtendBufferCoeff { get; set; } = 3;

        #endregion

        #region Methods
        /// <summary>
        /// 像素坐标转世界坐标
        /// </summary>
        /// <param name="minWorldX"></param>
        /// <param name="maxWorldY"></param>
        /// <param name="worldWidth"></param>
        /// <param name="worldHeight"></param>
        /// <param name="pixelX"></param>
        /// <param name="pixelY"></param>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static (double X, double Y) PointFToXY(double minWorldX, double maxWorldY, double worldWidth, double worldHeight, float pixelX, float pixelY, float pixelWidth, float pixelHeight, float x, float y)
        {
            double destX = x;
            double destY = y;
            if (pixelWidth != 0 && pixelHeight != 0)
            {
                destX = (destX - pixelX) * worldWidth / pixelWidth + minWorldX;
                destY = maxWorldY - (destY - pixelY) * worldHeight / pixelHeight;
            }
            return (destX, destY);
        }
        /// <summary>
        /// 像素坐标转世界坐标
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="rectangle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static (double X, double Y) PointFToXY(IExtent extent, Rectangle rectangle, float x, float y)
        {
            (double X, double Y) ret;
            if (extent == null)
            {
                ret = (x, y);
            }
            else
            {
                ret = PointFToXY(extent.MinX, extent.MaxY, extent.Width, extent.Height, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, x, y);
            }
            return ret;
        }
        /// <summary>
        /// 像素坐标转世界坐标
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="rectangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static (double X, double Y) PointFToXY(IExtent extent, Rectangle rectangle, PointF point)
        {
            (double X, double Y) ret;
            if (extent == null)
            {
                ret = (point.X, point.Y);
            }
            else
            {
                ret = PointFToXY(extent, rectangle, point.X, point.Y);
            }
            return ret;
        }
        /// <summary>
        /// 像素坐标转世界坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static (double X, double Y) PointFToXY(this IProj self, float x, float y)
        {
            (double X, double Y) ret;
            if (self == null)
            {
                ret = (x, y);
            }
            else
            {
                ret = PointFToXY(self.Extent, self.Bound, x, y);
            }
            return ret;
        }
        /// <summary>
        /// 像素坐标转世界坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static (double X, double Y) PointFToXY(this IProj self, PointF point)
        {
            (double X, double Y) ret;
            if (self == null)
            {
                ret = (point.X, point.Y);
            }
            else
            {
                ret = PointFToXY(self.Extent, self.Bound, point);
            }
            return ret;
        }
        /// <summary>
        /// 像素坐标转世界坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Coordinate PointFToCoordinate(this IProj self, PointF point)
        {
            Coordinate coordinate = null;
            if (self != null)
            {
                var ret = PointFToXY(self.Extent, self.Bound, point);
                coordinate = new Coordinate(ret.X, ret.Y);
            }
            return coordinate;
        }
        /// <summary>
        /// 像素范围转为世界范围
        /// </summary>
        /// <param name="self"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static IExtent RectangleFToExtent(this IProj self, RectangleF rect)
        {
            var topLeft = self.PointFToXY(rect.X, rect.Y);
            var bottomRight = self.PointFToXY(rect.Right, rect.Bottom);
            return new Extent()
            {
                MinX = topLeft.X,
                MinY = bottomRight.Y,
                MaxX = bottomRight.X,
                MaxY = topLeft.Y
            };
        }

        /// <summary>
        /// Projects all of the rectangles int the specified list of rectangles into geographic regions.
        /// </summary>
        /// <param name="self">This IProj</param>
        /// <param name="clipRects">The clip rectangles</param>
        /// <returns>A List of IEnvelope geographic bounds that correspond to the specified clip rectangles.</returns>
        public static List<IExtent> PixelToProj(this IProj self, List<Rectangle> clipRects)
        {
            List<IExtent> result = new List<IExtent>();
            foreach (Rectangle r in clipRects)
            {
                result.Add(RectangleFToExtent(self, r));
            }
            return result;
        }


        /// <summary>
        /// 世界坐标转像素坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static PointF CoordinateToPointF(this IProj self, ICoordinate coordinate)
        {
            PointF point = PointF.Empty;
            if (self != null && coordinate != null)
            {
                point = self.CoordinateToPointF(coordinate.X, coordinate.Y);
            }
            return point;
        }
        /// <summary>
        /// 世界坐标转像素坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static PointF CoordinateToPointF(this IProj self, double x, double y)
        {
            PointF point = PointF.Empty;
            if (self != null)
            {
                point = CoordinateToPointF(self.Extent, self.Bound, x, y);
            }
            return point;
        }
        /// <summary>
        /// 世界坐标转像素坐标
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="rectangle"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static PointF CoordinateToPointF(IExtent extent, RectangleF rectangle, ICoordinate coordinate)
        {
            PointF point = PointF.Empty;
            if (extent != null && coordinate != null)
            {
                point = CoordinateToPointF(extent, rectangle, coordinate.X, coordinate.Y);
            }
            return point;
        }
        /// <summary>
        /// 世界坐标转像素坐标
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="rectangle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static PointF CoordinateToPointF(IExtent extent, RectangleF rectangle, double x, double y)
        {
            PointF point = PointF.Empty;
            if (extent != null)
            {
                point = CoordinateToPointF(extent.MinX, extent.MaxY, extent.Width, extent.Height, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, x, y);
            }
            return point;
        }
        /// <summary>
        /// 世界坐标转像素坐标
        /// </summary>
        /// <param name="minWorldX">世界范围最小X</param>
        /// <param name="maxWorldY">世界范围最大Y</param>
        /// <param name="worldWidth">世界范围宽度</param>
        /// <param name="worldHeight">世界范围高度</param>
        /// <param name="pixelX">像素范围X</param>
        /// <param name="pixelY">像素范围Y</param>
        /// <param name="pixelWidth">像素范围宽度</param>
        /// <param name="pixelHeight">像素范围高度</param>
        /// <param name="x">世界坐标X</param>
        /// <param name="y">世界坐标Y</param>
        /// <returns>像素坐标</returns>
        public static PointF CoordinateToPointF(double minWorldX, double maxWorldY, double worldWidth, double worldHeight, float pixelX, float pixelY, float pixelWidth, float pixelHeight, double x, double y)
        {
            if (worldWidth == 0 || worldHeight == 0) return Point.Empty;
            try
            {
                float dextX = Convert.ToSingle(pixelX + (x - minWorldX) * (pixelWidth / worldWidth));
                float dextY = Convert.ToSingle(pixelY + (maxWorldY - y) * (pixelHeight / worldHeight));
                return new PointF(dextX, dextY);
            }
            catch (OverflowException)
            {
                return PointF.Empty;
            }
        }
        /// <summary>
        /// 将世界范围转为像素范围
        /// </summary>
        /// <param name="self"></param>
        /// <param name="extent"></param>
        /// <returns></returns>
        public static RectangleF ExtentToRectangleF(this IProj self, IExtent extent)
        {
            PointF topLeft = CoordinateToPointF(self, extent.MinX, extent.MaxY);
            PointF bottomRight = CoordinateToPointF(self, extent.MaxX, extent.MinY);
            return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }
        /// <summary>
        /// 将世界范围转为像素范围
        /// </summary>
        /// <param name="self"></param>
        /// <param name="extent"></param>
        /// <returns></returns>
        public static Rectangle ExtentToRectangle(this IProj self, IExtent extent)
        {
            PointF topLeft = CoordinateToPointF(self, extent.MinX, extent.MaxY);
            PointF bottomRight = CoordinateToPointF(self, extent.MaxX, extent.MinY);
            int left = (int)topLeft.X;
            int top = (int)topLeft.Y;
            int right = (int)bottomRight.X;
            int bottom = (int)bottomRight.Y;
            return new Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Translates all of the geographic regions, forming an equivalent list of rectangles.
        /// </summary>
        /// <param name="self">This IProj</param>
        /// <param name="regions">The list of geographic regions to project</param>
        /// <returns>A list of pixel rectangles that describe the specified region</returns>
        public static List<Rectangle> ProjToPixel(this IProj self, List<IExtent> regions)
        {
            List<Rectangle> result = new List<Rectangle>();
            foreach (var region in regions)
            {
                if (region == null) continue;
                result.Add(ExtentToRectangle(self, region));
            }

            return result;
        }

        /// <summary>
        /// Calculates an integer length distance in pixels that corresponds to the double length specified in the image.
        /// </summary>
        /// <param name="self">The IProj that this describes</param>
        /// <param name="distance">The double distance to obtain in pixels</param>
        /// <returns>The integer distance in pixels</returns>
        public static double ProjToPixel(this IProj self, double distance)
        {
            return distance * self.Bound.Width / self.Extent.Width;
        }

        #endregion
    }
}
