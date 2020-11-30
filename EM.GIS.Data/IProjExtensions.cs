
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
        /// Converts a single point location into an equivalent geographic coordinate
        /// </summary>
        /// <param name="self">This IProj</param>
        /// <param name="position">The client coordinate relative to the map control</param>
        /// <returns>The geographic ICoordinate interface</returns>
        public static double[] PixelToProj(this IProj self, Point position)
        {
            double x = Convert.ToDouble(position.X);
            double y = Convert.ToDouble(position.Y);
            if (self != null && self.Extent != null)
            {
                x = (x - self.Bounds.X) * self.Extent.Width / self.Bounds.Width + self.Extent.MinX;
                y = self.Extent.MaxY - (y - self.Bounds.Y) * self.Extent.Height / self.Bounds.Height;
            }

            return new double[] { x, y };
        }
        /// <summary>
        /// Converts a rectangle in pixel coordinates relative to the map control into
        /// a geographic envelope.
        /// </summary>
        /// <param name="self">This IProj</param>
        /// <param name="rect">The rectangle to convert</param>
        /// <returns>An IEnvelope interface</returns>
        public static IExtent PixelToProj(this IProj self, Rectangle rect)
        {
            Point tl = new Point(rect.X, rect.Y);
            Point br = new Point(rect.Right, rect.Bottom);
            var topLeft = PixelToProj(self, tl);
            var bottomRight = PixelToProj(self, br);
            return new Extent()
            {
                MinX = topLeft[0],
                MinY = bottomRight[1],
                MaxX = bottomRight[0],
                MaxY = topLeft[1]
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
                result.Add(PixelToProj(self, r));
            }
            return result;
        }

        /// <summary>
        /// Converts a single geographic location into the equivalent point on the screen relative to the top left corner of the map.
        /// </summary>
        /// <param name="self">This IProj</param>
        /// <param name="location">The geographic position to transform</param>
        /// <returns>A Point with the new location.</returns>
        public static Point ProjToPixelPoint(this IProj self, double[] location)
        {
            if (self.Extent.Width == 0 || self.Extent.Height == 0) return Point.Empty;
            try
            {
                int x = Convert.ToInt32(self.Bounds.X + (location[0] - self.Extent.MinX) *
                                    (self.Bounds.Width / self.Extent.Width));
                int y = Convert.ToInt32(self.Bounds.Y + (self.Extent.MaxY - location[1]) *
                                        (self.Bounds.Height / self.Extent.Height));

                return new Point(x, y);
            }
            catch (OverflowException)
            {
                return Point.Empty;
            }
        }
        /// <summary>
        /// Converts a single geographic location into the equivalent point on the screen relative to the top left corner of the map.
        /// </summary>
        /// <param name="self">This IProj</param>
        /// <param name="location">The geographic position to transform</param>
        /// <returns>A Point with the new location.</returns>
        public static PointF ProjToPixelPointF(this IProj self, double[] location)
        {
            if (self.Extent.Width == 0 || self.Extent.Height == 0) return Point.Empty;
            //try
            //{
            //    float x = (float)(self.Rectangle.X + (location[0] - self.Envelope.MinX) *
            //                        (self.Rectangle.Width / self.Envelope.Width()));
            //    float y = (float)(self.Rectangle.Y + (self.Envelope.MaxY - location[1]) *
            //                            (self.Rectangle.Height / self.Envelope.Height()));

            //    return new PointF(x, y);
            //}
            //catch (OverflowException)
            //{
            //    return Point.Empty;
            //}

            if (self.Extent.Width == 0 || self.Extent.Height == 0) return Point.Empty;
            try
            {
                int x = Convert.ToInt32(self.Bounds.X + (location[0] - self.Extent.MinX) *
                                    (self.Bounds.Width / self.Extent.Width));
                int y = Convert.ToInt32(self.Bounds.Y + (self.Extent.MaxY - location[1]) *
                                        (self.Bounds.Height / self.Extent.Height));

                return new Point(x, y);
            }
            catch (OverflowException)
            {
                return Point.Empty;
            }
        }
        public static PointF ProjToPixelPointF(this IProj self, ICoordinate location)
        {
            if (self.Extent.Width == 0 || self.Extent.Height == 0) return Point.Empty;
            if (self.Extent.Width == 0 || self.Extent.Height == 0) return Point.Empty;
            try
            {
                int x = Convert.ToInt32(self.Bounds.X + (location.X - self.Extent.MinX) *
                                    (self.Bounds.Width / self.Extent.Width));
                int y = Convert.ToInt32(self.Bounds.Y + (self.Extent.MaxY - location.Y) *
                                        (self.Bounds.Height / self.Extent.Height));

                return new Point(x, y);
            }
            catch (OverflowException)
            {
                return Point.Empty;
            }
        }
        /// <summary>
        /// Converts a single geographic envelope into an equivalent Rectangle as it would be drawn on the screen.
        /// </summary>
        /// <param name="self">This IProj</param>
        /// <param name="env">The geographic IEnvelope</param>
        /// <returns>A Rectangle</returns>
        public static Rectangle ProjToPixel(this IProj self, IExtent env)
        {
            var tl = new double[] { env.MinX, env.MaxY };
            var br = new double[] { env.MaxX, env.MinY };
            Point topLeft = ProjToPixelPoint(self, tl);
            Point bottomRight = ProjToPixelPoint(self, br);
            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
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
            return distance * self.Bounds.Width / self.Extent.Width;
        }

        #endregion
    }
}
