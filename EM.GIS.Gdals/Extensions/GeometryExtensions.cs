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
                geometry = ToPointGeometry(coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
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
                    geometry.AddPoint(coordinate);
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
                    geometry.AddPoint(coordinate);
                }
                if (!geometry.IsRing())
                {
                    geometry.CloseRings();
                }
            }
            return geometry;
        }
        public static OSGeo.OGR.Geometry ToPolygonGeometry(this ILinearRing shell, IEnumerable<ILinearRing> holes = null)
        {
            OSGeo.OGR.Geometry geometry = null;
            if (shell == null)
            {
                return geometry;
            }
            geometry = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbPolygon);
            geometry.AddGeometryDirectly(shell.ToGeometry());
            if (holes != null)
            {
                foreach (var item in holes)
                {
                    geometry.AddGeometryDirectly(item.ToGeometry());
                }
            }
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
                    double[] buffer = new double[4];
                    geometry.SetPointZM(index, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
                }
            }
        }
        public static IPoint GetPoint(this OSGeo.OGR.Geometry geometry, int index)
        {
            IPoint point = null;
            if (geometry != null)
            {
                if (index >= 0 && index < geometry.GetPointCount())
                {
                    double[] buffer = new double[4];
                    geometry.GetPointZM(index, buffer);
                    point = new Point(buffer[0], buffer[1], buffer[2], buffer[3]);
                }
            }
            return point;
        }

        #region 将DotSpatial的几何转成自定义的几何

        public static IGeometry ToGeometry(this OSGeo.OGR.Geometry geometry)
        {
            IGeometry destGeometry = null;
            if (geometry != null && !geometry.IsEmpty())
            {
                switch (geometry.GetGeometryType())
                {
                    case OSGeo.OGR.wkbGeometryType.wkbPoint:
                        destGeometry = geometry.ToPoint();
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbLineString:
                        destGeometry = geometry.ToLineString();
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbLinearRing:
                        destGeometry = geometry.ToLinearRing();
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbPolygon:
                        destGeometry = geometry.ToPolygon();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return destGeometry;
        }
        public static IPoint ToPoint(this ICoordinate coordinate)
        {
            IPoint dsPoint = null;
            if (coordinate != null)
            {
                dsPoint = new Point(coordinate); ;
            }
            return dsPoint;
        }
        public static IPoint ToPoint(this OSGeo.OGR.Geometry geometry)
        {
            IPoint dsGeometry = null;
            if (geometry != null && !geometry.IsEmpty() && geometry.GetGeometryType() == OSGeo.OGR.wkbGeometryType.wkbPoint)
            {
                dsGeometry = new Point(geometry);
            }
            return dsGeometry;
        }
        public static ILineString ToLineString(this OSGeo.OGR.Geometry geometry)
        {
            ILineString dsGeometry = null;
            if (geometry != null && !geometry.IsEmpty() && geometry.GetGeometryType() == OSGeo.OGR.wkbGeometryType.wkbLineString)
            {
                dsGeometry = new LineString(geometry);
            }
            return dsGeometry;
        }
        public static ILinearRing ToLinearRing(this OSGeo.OGR.Geometry geometry)
        {
            ILinearRing dsGeometry = null;
            if (geometry != null && !geometry.IsEmpty() && geometry.GetGeometryType() == OSGeo.OGR.wkbGeometryType.wkbLinearRing)
            {
                dsGeometry = new LinearRing(geometry);
            }
            return dsGeometry;
        }
        public static IPolygon ToPolygon(this OSGeo.OGR.Geometry geometry)
        {
            IPolygon destGeometry = null;
            if (geometry == null && !geometry.IsEmpty() && geometry.GetGeometryType() == OSGeo.OGR.wkbGeometryType.wkbPolygon)
            {
                return destGeometry;
            }
            destGeometry = new Polygon(geometry);
            return destGeometry;
        }
        public static IMultiLineString ToMultiLineString(this OSGeo.OGR.Geometry geometry)
        {
            IMultiLineString destGeometry = null;
            if (geometry == null && !geometry.IsEmpty() && geometry.GetGeometryType() == OSGeo.OGR.wkbGeometryType.wkbMultiLineString)
            {
                return destGeometry;
            }
            destGeometry = new MultiLineString(geometry);
            return destGeometry;
        }
        #endregion


        #region 自定义的几何转成DotSpatial的几何
        public static ICoordinate ToCoordinate(this IPoint point)
        {
            ICoordinate coordinate = null;
            if (point == null)
            {
                return coordinate;
            }
            coordinate = new Coordinate(point.X, point.Y, point.Z, point.M);
            return coordinate;
        }
        #endregion
    }
}
