
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace EM.GIS.Data
{
    /// <summary>
    /// 投影扩展类
    /// </summary>
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
        /// 将像素坐标转为世界坐标
        /// </summary>
        /// <param name="point">像素坐标</param>
        /// <param name="rectangle">像素范围</param>
        /// <param name="extent">世界范围</param>
        /// <returns>世界坐标</returns>
        public static ICoordinate PixelToProj(this Point point, Rectangle rectangle, IExtent extent)
        {
            if (rectangle.Width == 0 || rectangle.Height == 0 || extent == null || extent.IsEmpty())
            {
                return null;
            }
            return PixelToProj(point.X, point.Y, rectangle, extent);
        }
        /// <summary>
        /// 将像素坐标转为世界坐标
        /// </summary>
        /// <param name="point">像素坐标</param>
        /// <param name="rectangle">像素范围</param>
        /// <param name="extent">世界范围</param>
        /// <returns>世界坐标</returns>
        public static ICoordinate PixelToProj(this ICoordinate point, Rectangle rectangle, IExtent extent)
        {
            if (rectangle.Width == 0 || rectangle.Height == 0 || extent == null || extent.IsEmpty())
            {
                return null;
            }
            return PixelToProj(point.X, point.Y, rectangle, extent);
        }
        /// <summary>
        /// 将像素坐标转为世界坐标
        /// </summary>
        /// <param name="x">像素X坐标</param>
        /// <param name="y">像素Y坐标</param>
        /// <param name="rectangle">像素范围</param>
        /// <param name="extent">世界范围</param>
        /// <returns>世界坐标</returns>
        public static ICoordinate PixelToProj(double x, double y, Rectangle rectangle, IExtent extent)
        {
            if (rectangle.Width == 0 || rectangle.Height == 0 || extent == null || extent.IsEmpty())
            {
                return null;
            }
            var destX = (x - rectangle.X) * extent.Width / rectangle.Width + extent.MinX;
            var destY = extent.MaxY - (y - rectangle.Y) * extent.Height / rectangle.Height;
            return new Coordinate(destX, destY);
        }

        /// <summary>
        /// 将像素坐标转为世界坐标
        /// </summary>
        /// <param name="point">像素坐标</param>
        /// <param name="rectangle">像素范围</param>
        /// <param name="extent">世界范围</param>
        /// <returns>世界坐标</returns>
        public static ICoordinate PixelToProj(this PointF point, Rectangle rectangle, IExtent extent)
        {
            if (rectangle.Width == 0 || rectangle.Height == 0 || extent == null || extent.IsEmpty())
            {
                return null;
            }
            return PixelToProj(point.X, point.Y, rectangle, extent);
        }

        /// <summary>
        /// 将像素坐标转为世界坐标
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="position">像素坐标</param>
        /// <returns>世界坐标</returns>
        public static ICoordinate PixelToProj(this IProj self, Point position)
        {
            if (self == null)
            {
                return null;
            }
            return position.PixelToProj(self.Bound, self.Extent);
        }

        /// <summary>
        /// 将像素坐标转为世界坐标
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="x">像素坐标x</param>
        /// <param name="y">像素坐标y</param>
        /// <returns>世界坐标</returns>
        public static ICoordinate PixelToProj(this IProj self, double x, double y)
        {
            if (self == null)
            {
                return null;
            }
            return PixelToProj(x, y, self.Bound, self.Extent);
        }
        /// <summary>
        /// 将像素坐标转为世界坐标
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="position">像素坐标</param>
        /// <returns>世界坐标</returns>
        public static ICoordinate PixelToProj(this IProj self, PointF position)
        {
            if (self == null)
            {
                return null;
            }
            return position.PixelToProj(self.Bound, self.Extent);
        }

        /// <summary>
        /// 将像素范围转为世界范围
        /// </summary>
        /// <param name="destRect">目标像素范围</param>
        /// <param name="srcRectangle">原像素范围</param>
        /// <param name="srcExtent">原世界范围</param>
        /// <returns>世界范围</returns>
        public static Extent PixelToProj(this Rectangle destRect, Rectangle srcRectangle, IExtent srcExtent)
        {
            if (srcRectangle.Width == 0 || srcRectangle.Height == 0 || srcExtent == null || srcExtent.IsEmpty())
            {
                return null;
            }
            var tl = new Point(destRect.X, destRect.Y);
            var br = new Point(destRect.Right, destRect.Bottom);
            var topLeft = PixelToProj(tl, srcRectangle, srcExtent);
            var bottomRight = PixelToProj(br, srcRectangle, srcExtent);
            return new Extent(topLeft.X, bottomRight.Y, bottomRight.X, topLeft.Y);
        }

        /// <summary>
        /// 将像素范围转为世界范围
        /// </summary>
        /// <param name="destRect">目标像素范围</param>
        /// <param name="srcRectangle">原像素范围</param>
        /// <param name="srcExtent">原世界范围</param>
        /// <returns>世界范围</returns>
        public static IExtent PixelToProj(this RectangleF destRect, Rectangle srcRectangle, IExtent srcExtent)
        {
            if (srcRectangle.Width == 0 || srcRectangle.Height == 0 || srcExtent == null || srcExtent.IsEmpty())
            {
                return null;
            }
            var tl = new PointF(destRect.X, destRect.Y);
            var br = new PointF(destRect.Right, destRect.Bottom);
            var topLeft = PixelToProj(tl, srcRectangle, srcExtent);
            var bottomRight = PixelToProj(br, srcRectangle, srcExtent);
            return new Extent(topLeft.X, bottomRight.Y, bottomRight.X, topLeft.Y);
        }

        /// <summary>
        /// 将像素范围转为世界范围
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="rect">原像素范围</param>
        /// <returns>世界范围</returns>
        public static IExtent PixelToProj(this IProj self, Rectangle rect)
        {
            if (self == null)
            {
                return null;
            }
            return rect.PixelToProj(self.Bound, self.Extent);
        }
        /// <summary>
        /// 将像素范围转为世界范围
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="rect">原像素范围</param>
        /// <returns>世界范围</returns>
        public static IExtent PixelToProj(this IProj self, RectangleF rect)
        {
            if (self == null)
            {
                return null;
            }
            return rect.PixelToProj(self.Bound, self.Extent);
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
                result.Add(PixelToProj(self, r));
            }

            return result;
        }

        /// <summary>
        /// 将世界坐标转为像素坐标
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="location">世界坐标</param>
        /// <returns>像素坐标</returns>
        public static Point ProjToPixel(this IProj self, ICoordinate location)
        {
            Point point = Point.Empty;
            if (self != null && location != null)
            {
                point = location.ProjToPixel(self.Bound, self.Extent);
            }
            return point;
        }
        /// <summary>
        /// 将世界坐标转为像素坐标
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="location">世界坐标</param>
        /// <returns>像素坐标</returns>
        public static PointF ProjToPixelF(this IProj self, ICoordinate location)
        {
            PointF point = PointF.Empty;
            if (self != null && location != null)
            {
                point = location.ProjToPixelF(self.Bound, self.Extent);
            }
            return point;
        }
        /// <summary>
        /// 将世界坐标转为像素坐标
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="location">世界坐标</param>
        /// <returns>像素坐标</returns>
        public static (double X, double Y) ProjToPixelD(this IProj self, ICoordinate location)
        {
            (double X, double Y) ret = default;
            if (self != null && location != null)
            {
                ret = location.ProjToPixelD(self.Bound, self.Extent);
            }
            return ret;
        }

        /// <summary>
        /// 将世界坐标转为像素坐标
        /// </summary>
        /// <param name="location">世界坐标</param>
        /// <param name="rectangle">像素矩阵</param>
        /// <param name="extent">范围</param>
        /// <returns>像素坐标</returns>
        public static Point ProjToPixel(this ICoordinate location, Rectangle rectangle, IExtent extent)
        {
            if (location == null || location.IsEmpty() || rectangle.IsEmpty || extent == null || extent.IsEmpty() || extent.Width == 0 || extent.Height == 0)
            {
                return Point.Empty;
            }
            try
            {
                double dx = rectangle.Width / extent.Width;
                double dy = rectangle.Height / extent.Height;
                var x = (int)(rectangle.X + (location.X - extent.MinX) * dx);
                var y = (int)(rectangle.Y + (extent.MaxY - location.Y) * dy);
                return new Point(x, y);
            }
            catch
            {
                return Point.Empty;
            }
        }

        /// <summary>
        /// 将世界坐标转为像素坐标
        /// </summary>
        /// <param name="location">世界坐标</param>
        /// <param name="rectangle">像素矩阵</param>
        /// <param name="extent">范围</param>
        /// <returns>像素坐标</returns>
        public static PointF ProjToPixelF(this ICoordinate location, RectangleF rectangle, IExtent extent)
        {
            var pointD = ProjToPixelD(location, rectangle, extent);
            var ret = new PointF((float)pointD.X, (float)pointD.Y);
            return ret;
        }
        /// <summary>
        /// 将世界坐标转为像素坐标
        /// </summary>
        /// <param name="location">世界坐标</param>
        /// <param name="rectangle">像素矩阵</param>
        /// <param name="extent">范围</param>
        /// <returns>像素坐标</returns>
        public static (double X, double Y) ProjToPixelD(this ICoordinate location, RectangleF rectangle, IExtent extent)
        {
            (double X, double Y) ret = default;
            if (location == null || location.IsEmpty() || rectangle.IsEmpty || extent == null || extent.IsEmpty() || extent.Width == 0 || extent.Height == 0)
            {
                throw new ArgumentNullException($"参数错误_{location}_{rectangle}_{extent}");
            }
            try
            {
                double dx = rectangle.Width / extent.Width;
                double dy = rectangle.Height / extent.Height;
                var x = rectangle.X + (location.X - extent.MinX) * dx;
                var y = rectangle.Y + (extent.MaxY - location.Y) * dy;
                ret = (x, y);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(ProjToPixelD)}失败_{location}_{rectangle}_{extent},{e}");
            }
            return ret;
        }

        /// <summary>
        /// 将世界范围转为像素范围
        /// </summary>
        /// <param name="destExtent">指定世界范围</param>
        /// <param name="srcRectangle">原有像素范围</param>
        /// <param name="srcExtent">原有世界范围</param>
        /// <returns>像素范围</returns>
        public static Rectangle ProjToPixel(this IExtent destExtent, Rectangle srcRectangle, IExtent srcExtent)
        {
            if (destExtent == null || destExtent.IsEmpty() || srcRectangle.IsEmpty || srcExtent == null || srcExtent.IsEmpty() || srcExtent.Width == 0 || srcExtent.Height == 0)
            {
                return Rectangle.Empty;
            }
            Coordinate tl = new Coordinate(destExtent.MinX, destExtent.MaxY);
            Coordinate br = new Coordinate(destExtent.MaxX, destExtent.MinY);
            var topLeft = ProjToPixel(tl, srcRectangle, srcExtent);
            var bottomRight = ProjToPixel(br, srcRectangle, srcExtent);
            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }

        /// <summary>
        /// 将世界范围转为像素范围
        /// </summary>
        /// <param name="destExtent">指定世界范围</param>
        /// <param name="srcRectangle">原有像素范围</param>
        /// <param name="srcExtent">原有世界范围</param>
        /// <returns>像素范围</returns>
        public static RectangleF ProjToPixelF(this IExtent destExtent, RectangleF srcRectangle, IExtent srcExtent)
        {
            if (destExtent == null || destExtent.IsEmpty() || srcRectangle.IsEmpty || srcExtent == null || srcExtent.IsEmpty() || srcExtent.Width == 0 || srcExtent.Height == 0)
            {
                return RectangleF.Empty;
            }
            Coordinate tl = new Coordinate(destExtent.MinX, destExtent.MaxY);
            Coordinate br = new Coordinate(destExtent.MaxX, destExtent.MinY);
            var topLeft = ProjToPixelF(tl, srcRectangle, srcExtent);
            var bottomRight = ProjToPixelF(br, srcRectangle, srcExtent);
            return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }

        /// <summary>
        /// 将世界范围转为像素范围
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="env">指定世界范围</param>
        /// <returns>像素范围</returns>
        public static Rectangle ProjToPixel(this IProj self, IExtent env)
        {
            var rectangle = Rectangle.Empty;
            if (self != null && env != null)
            {
                rectangle = env.ProjToPixel(self.Bound, self.Extent);
            }
            return rectangle;
        }

        /// <summary>
        /// 将世界范围转为像素范围
        /// </summary>
        /// <param name="self">投影类</param>
        /// <param name="env">指定世界范围</param>
        /// <returns>像素范围</returns>
        public static RectangleF ProjToPixelF(this IProj self, IExtent env)
        {
            var rectangle = RectangleF.Empty;
            if (self != null && env != null)
            {
                rectangle = env.ProjToPixelF(self.Bound, self.Extent);
            }
            return rectangle;
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
                result.Add(ProjToPixel(self, region));
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
