using EM.GIS.Data;
using EM.GIS.Geometries;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using System.Linq;
using EM.GIS.Projections;
using EM.GIS.GdalExtensions;

namespace EM.GIS.Gdals
{
    internal static class TileCalculator
    {
        #region Fields

        /// <summary>
        /// The earth radius in kms.
        /// </summary>
        public const double EarthRadiusKms = 6378137;

        /// <summary>
        /// The maximal latitude.
        /// </summary>
        public const double MaxLatitude = 85.05112878;

        /// <summary>
        /// The maximal longitude.
        /// </summary>
        public const double MaxLongitude = 180;

        /// <summary>
        /// The maximal webmercator X.
        /// </summary>
        public const double MaxWebMercX = 20037497.2108402;

        /// <summary>
        /// The maximal webmercator Y.
        /// </summary>
        public const double MaxWebMercY = 20037508.3430388;

        /* Adapted from methods at http://msdn.microsoft.com/en-us/library/bb259689.aspx */

        /// <summary>
        /// The minimal latitude.
        /// </summary>
        public const double MinLatitude = -85.05112878;

        /// <summary>
        /// The minimal longitude.
        /// </summary>
        public const double MinLongitude = -180;

        /// <summary>
        /// The minimal webmercator X.
        /// </summary>
        public const double MinWebMercX = -20037497.2108402;

        /// <summary>
        /// The minimal webmercator Y.
        /// </summary>
        public const double MinWebMercY = -20037508.3430388;

        #endregion

        /// <summary>
        /// WEB墨卡托坐标系
        /// </summary>
        public static Lazy<Projection> WebMercProj { get; }
        /// <summary>
        /// WGS84坐标系
        /// </summary>
        public static Lazy<Projection> Wgs84Proj { get; }
        static TileCalculator()
        {
            GdalConfiguration.ConfigureGdal();
            WebMercProj = new Lazy<Projection>(() => new GdalProjection(3857));
            Wgs84Proj = new Lazy<Projection>(() => new GdalProjection(4326));
        }
        #region Methods

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        public static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }
        /// <summary>
        /// 判断缩放级别
        /// </summary>
        /// <param name="geoExtent">地理范围</param>
        /// <param name="rectangle">窗口大小</param>
        /// <param name="minLevel">最小级别</param>
        /// <param name="maxLevel">最大级别</param>
        /// <returns>The zoom level.</returns>
        public static int DetermineZoomLevel(IExtent geoExtent, RectangleF rectangle, int minLevel = 0, int maxLevel = 19)
        {
            int zoom = -1;
            double metersAcross = EarthRadiusKms * geoExtent.Width * Math.PI / 180; // find the arc length represented by the displayed map
            metersAcross *= Math.Cos(geoExtent.Center.Y * Math.PI / 180); // correct for the center latitude

            double metersAcrossPerPixel = metersAcross / rectangle.Width; // find the resolution in meters per pixel
            if (GroundResolution(geoExtent.Center.Y, minLevel) < metersAcrossPerPixel)
            {
                zoom = minLevel;
            }
            else if (GroundResolution(geoExtent.Center.Y, maxLevel) > metersAcrossPerPixel)
            {
                zoom = maxLevel;
            }
            else
            {
                // find zoomlevel such that metersAcrossPerPix is close
                for (int i = minLevel; i <= maxLevel; i++)
                {
                    double groundRes = GroundResolution(geoExtent.Center.Y, i);
                    if (metersAcrossPerPixel > groundRes)
                    {
                        zoom = i - 1;
                        break;
                    }
                }
            }

            return zoom;
        }

        /// <summary>
        /// Determines the ground resolution (in meters per pixel) at a specified latitude and level of detail.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the ground resolution.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail) to 23 (highest detail).</param>
        /// <param name="useDegrees">Whether use degrees</param>
        /// <returns>The ground resolution, in meters or degrees per pixel.</returns>
        public static double GroundResolution(double latitude, int levelOfDetail, bool useDegrees = false)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            var ret = Math.Cos(latitude * Math.PI / 180) / MapSize(levelOfDetail);
            if (useDegrees)
            {
                ret *= 360;
            }
            else
            {
                ret *= 2 * Math.PI * EarthRadiusKms;
            }
            return ret;
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="latitude">Latitude of the point, in degrees.</param>
        /// <param name="longitude">Longitude of the point, in degrees.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail) to 23 (highest detail).</param>
        /// <returns>Pixel XY coordinate</returns>
        public static Point LatLongToPixelXy(double latitude, double longitude, int levelOfDetail)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            var x = (longitude + 180) / 360;
            var sinLatitude = Math.Sin(latitude * Math.PI / 180);
            var y = 0.5 - (Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI));

            var mapSize = MapSize(levelOfDetail);

            return new Point((int)Clip((x * mapSize) + 0.5, 0, mapSize - 1), (int)Clip((y * mapSize) + 0.5, 0, mapSize - 1));
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="coord">Latitude, Longitude Coordinate of the point, in degrees.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail) to 23 (highest detail). </param>
        /// <returns>Pixel XY coordinate.</returns>
        public static Point LatLongToPixelXy(Coordinate coord, int levelOfDetail)
        {
            return LatLongToPixelXy(coord.Y, coord.X, levelOfDetail);
        }

        /// <summary>
        /// Converts a WGS-84 Lat/Long coordinate to the tile XY of the tile containing that point at the given levelOfDetail.
        /// </summary>
        /// <param name="coord">WGS-84 Lat/Long</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail) to 23 (highest detail).</param>
        /// <returns>Tile XY Point</returns>
        public static Point LatLongToTileXy(Coordinate coord, int levelOfDetail)
        {
            Point pixelXy = LatLongToPixelXy(coord.Y, coord.X, levelOfDetail);
            Point tileXy = PixelXyToTileXy(pixelXy);

            return tileXy;
        }

        /// <summary>
        /// Determines the map scale at a specified latitude, level of detail, and screen resolution.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the map scale.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail) to 23 (highest detail).</param>
        /// <param name="screenDpi">Resolution of the screen, in dots per inch.</param>
        /// <returns>The map scale, expressed as the denominator N of the ratio 1 : N.</returns>
        public static double MapScale(double latitude, int levelOfDetail, int screenDpi)
        {
            return GroundResolution(latitude, levelOfDetail) * screenDpi / 0.0254;
        }

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level of detail.
        /// </summary>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail) to 23 (highest detail).</param>
        /// <returns>The map width and height in pixels.</returns>
        public static uint MapSize(int levelOfDetail)
        {
            return 256U << levelOfDetail;
        }

        /// <summary>
        /// Converts a pixel from pixel XY coordinates at a specified level of detail into latitude/longitude WGS-84 coordinates (in degrees).
        /// </summary>
        /// <param name="pixelX">X coordinate of the point, in pixels.</param>
        /// <param name="pixelY">Y coordinates of the point, in pixels.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail) to 23 (highest detail).</param>
        /// <returns>The resulting WGS84 coordinate.</returns>
        public static Coordinate PixelXyToLatLong(int pixelX, int pixelY, int levelOfDetail)
        {
            double mapSize = MapSize(levelOfDetail);
            double x = (Clip(pixelX, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (Clip(pixelY, 0, mapSize - 1) / mapSize);

            double latitude = 90 - (360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI);
            double longitude = 360 * x;

            return new Coordinate(longitude, latitude);
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates of the tile containing the specified pixel.
        /// </summary>
        /// <param name="pixelX">Pixel X coordinate.</param>
        /// <param name="pixelY">Pixel Y coordinate.</param>
        /// <returns>Tile XY coordinate.</returns>
        public static Point PixelXyToTileXy(int pixelX, int pixelY)
        {
            return new Point(pixelX / 256, pixelY / 256);
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates of the tile containing the specified pixel.
        /// </summary>
        /// <param name="point">Pixel X,Y point.</param>
        /// <returns>Tile XY coordinate</returns>
        public static Point PixelXyToTileXy(Point point)
        {
            return PixelXyToTileXy(point.X, point.Y);
        }

        #endregion
    }
}
