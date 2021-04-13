using EM.GIS.Geometries;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EM.GIS.Gdals
{
    public static class GeometryExtensions
    {
        #region 几何体
        public static int GetPointCount(this Feature feature)
        {
            int count = 0;
            if (feature != null)
            {
                using (var geometry = feature.GetGeometryRef())
                {
                    geometry.GetPointCount(ref count);
                }
            }
            return count;
        }
        public static void GetPointCount(this OSGeo.OGR.Geometry geometry, ref int count)
        {
            if (geometry != null)
            {
                int geoCount = geometry.GetGeometryCount();
                if (geoCount > 0)
                {
                    for (int i = 0; i < geoCount; i++)
                    {
                        using (var childGeo = geometry.GetGeometryRef(i))
                        {
                            childGeo.GetPointCount(ref count);
                        }
                    }
                }
                else
                {
                    count += geometry.GetPointCount();
                }
            }
        }
        public static ICoordinate ToCoordinate(this IEnumerable<double> array)
        {
            ICoordinate coordinate = null;
            if (array == null)
            {
                return coordinate;
            }
            var count = array.Count();
            if (count >= 0)
            {
                coordinate = new Coordinate();
                for (int i = 0; i < coordinate.MaxPossibleOrdinates && i < count; i++)
                {
                    coordinate[i] = array.ElementAt(i);
                }
            }
            return coordinate;
        }
        #endregion
        private static void AddPoint(this OSGeo.OGR.Geometry geometry, double x, double y, double z = double.NaN, double m = double.NaN)
        {
            if (geometry != null)
            {
                if (double.IsNaN(z))
                {
                    if (double.IsNaN(m))
                    {
                        geometry.AddPoint_2D(x, y);
                    }
                    else
                    {
                        geometry.AddPointM(x, y, m);
                    }
                }
                else
                {
                    if (double.IsNaN(m))
                    {
                        geometry.AddPoint(x, y, z);
                    }
                    else
                    {
                        geometry.AddPointZM(x, y, z, m);
                    }
                }
            }
        }
        private static void AddPoint(this OSGeo.OGR.Geometry geometry, ICoordinate coordinate)
        {
            if (geometry != null && coordinate != null)
            {
                geometry.AddPoint(coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
            }
        }
        #region 转成OGR的几何体
        public static OSGeo.OGR.Geometry ToGeometry(this IGeometry geometry)
        {
            OSGeo.OGR.Geometry destGeometry = (geometry as Geometry)?.OgrGeometry;
            return destGeometry;
        }
        public static OSGeo.OGR.Geometry ToPointGeometry(double x, double y, double z = double.NaN, double m = double.NaN)
        {
            OSGeo.OGR.Geometry geometry = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbPoint);
            geometry.AddPoint(x, y, z, m);
            return geometry;
        }
        public static OSGeo.OGR.Geometry ToPointGeometry(this ICoordinate coordinate)
        {
            OSGeo.OGR.Geometry geometry = null;
            if (coordinate != null)
            {
                geometry = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbLineString);
                switch (coordinate.Dimension)
                {
                    case 2:
                        geometry.AddPoint_2D(coordinate.X, coordinate.Y);
                        break;
                    case 3:
                        geometry.AddPoint(coordinate.X, coordinate.Y, coordinate.Z);
                        break;
                    case 4:
                        geometry.AddPointZM(coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
                        break;
                }
            }
            return geometry;
        }
        public static OSGeo.OGR.Geometry ToLineStringGeometry(this IEnumerable<ICoordinate> coordinates)
        {
            OSGeo.OGR.Geometry geometry = null;
            if (coordinates != null && coordinates.Count() > 1)
            {
                geometry = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbLineString);
                foreach (var coordinate in coordinates)
                {
                    switch (coordinate.Dimension)
                    {
                        case 2:
                            geometry.AddPoint_2D(coordinate.X, coordinate.Y);
                            break;
                        case 3:
                            geometry.AddPoint(coordinate.X, coordinate.Y, coordinate.Z);
                            break;
                        case 4:
                            geometry.AddPointZM(coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
                            break;
                    }
                }
            }
            return geometry;
        }
        public static OSGeo.OGR.Geometry ToLinearRingGeometry(this IEnumerable<ICoordinate> coordinates)
        {
            OSGeo.OGR.Geometry geometry = null;
            if (coordinates != null && coordinates.Count() > 1)
            {
                geometry = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbLinearRing);
                foreach (var coordinate in coordinates)
                {
                    switch (coordinate.Dimension)
                    {
                        case 2:
                            geometry.AddPoint_2D(coordinate.X, coordinate.Y);
                            break;
                        case 3:
                            geometry.AddPoint(coordinate.X, coordinate.Y, coordinate.Z);
                            break;
                        case 4:
                            geometry.AddPointZM(coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
                            break;
                    }
                }
            }
            var geo= new OSGeo.OGR.Geometry(wkbGeometryType.wkbCurvePolygon);
            var geo = OSGeo.OGR.Geometry.CreateFromWkt("LINEARRING (104.57277242529 30.4319636253167,104.57277242529 30.4322491606373,104.581980939382 30.4322491606373,104.581980939382 30.4319636253167,104.57277242529 30.4319636253167)");
            return geometry;
        }
        #endregion

        public static ICoordinate GetCoordinate(this OSGeo.OGR.Geometry geometry, int index)
        {
            ICoordinate coordinate = null;
            if (geometry != null)
            {
                if (index >= 0 && index < geometry.GetPointCount())
                {
                    double[] buffer = new double[4];
                    geometry.GetPointZM(index, buffer);
                    coordinate = new Coordinate(buffer[0], buffer[1], buffer[2], buffer[3]);
                }
            }
            return coordinate;
        }
        public static void SetCoordinate(this OSGeo.OGR.Geometry geometry, int index, ICoordinate coordinate)
        {
            if (geometry != null && coordinate != null)
            {
                if (index >= 0 && index < geometry.GetPointCount())
                {
                    geometry.SetPointZM(index, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M); 
                }
            }
        }
        #region 将DotSpatial的几何转成自定义的几何

        public static IGeometry ToGeometry(this OSGeo.OGR.Geometry geometry)
        {
            var destGeometry = new Geometry(geometry);
            return destGeometry;
        }
        public static OSGeo.OGR.Geometry ToGeometry(this Geometry geometry)
        {
            var destGeometry = geometry?.OgrGeometry;
            return destGeometry;
        }
        #endregion

        public static GeometryType ToGeometryType(this wkbGeometryType wkbGeometryType)
        {
            GeometryType geometryType = GeometryType.Unknown;
            string name= wkbGeometryType.ToString().Replace("wkb", "");
            Enum.TryParse(name, out geometryType);
            return geometryType;
        }
        public static  wkbGeometryType TowWkbGeometryType(this GeometryType geometryType)
        {
            wkbGeometryType wkbGeometryType = wkbGeometryType.wkbUnknown;
            string name = $"wkb{geometryType}";
            Enum.TryParse(name, out wkbGeometryType);
            return wkbGeometryType;
        }

    }
}
