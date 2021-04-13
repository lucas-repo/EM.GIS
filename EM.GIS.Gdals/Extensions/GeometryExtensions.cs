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

        private static OSGeo.OGR.Geometry GetOgrGeometry(wkbGeometryType geometryType, IEnumerable<ICoordinate> coordinates)
        {
            OSGeo.OGR.Geometry ogrGeometry = null;
            var firstCoord = coordinates?.FirstOrDefault();
            if (firstCoord != null)
            {
                ogrGeometry = new OSGeo.OGR.Geometry(geometryType);
                Action<ICoordinate> addCoordAction;
                switch (firstCoord.Dimension)
                {
                    case 2:
                        addCoordAction = (coordinate) => ogrGeometry.AddPoint_2D(coordinate.X, coordinate.Y);
                        break;
                    case 3:
                        addCoordAction = (coordinate) => ogrGeometry.AddPoint(coordinate.X, coordinate.Y, coordinate.Z);
                        break;
                    case 4:
                        addCoordAction = (coordinate) => ogrGeometry.AddPointZM(coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
                        break;
                    default:
                        return ogrGeometry;
                }
                foreach (var item in coordinates)
                {
                    addCoordAction(item);
                }
            }
            return ogrGeometry;
        }

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
        public static OSGeo.OGR.Geometry ToOgrPoint(double x, double y, double z = double.NaN, double m = double.NaN)
        {
            OSGeo.OGR.Geometry ogrGeometry = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPoint);
            if (double.IsNaN(z))
            {
                ogrGeometry.AddPoint_2D(x, y);
            }
            else
            {
                if (double.IsNaN(m))
                {
                    ogrGeometry.AddPoint(x, y, z);
                }
                else
                {
                    ogrGeometry.AddPointZM(x, y, z, m);
                }
            }
            return ogrGeometry;
        }
        public static OSGeo.OGR.Geometry ToOgrPoint(this ICoordinate coordinate)
        {
            OSGeo.OGR.Geometry ogrGeometry = null;
            if (coordinate != null)
            {
                ogrGeometry = ToOgrPoint(coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
            }
            return ogrGeometry;
        }

        public static OSGeo.OGR.Geometry ToOgrLineString(this IEnumerable<ICoordinate> coordinates)
        {
            OSGeo.OGR.Geometry geometry = GetOgrGeometry(wkbGeometryType.wkbLineString, coordinates);
            return geometry;
        }
        public static OSGeo.OGR.Geometry ToOgrLinearRing(this IEnumerable<ICoordinate> coordinates)
        {
            OSGeo.OGR.Geometry geometry = GetOgrGeometry(wkbGeometryType.wkbLinearRing, coordinates);
            return geometry;
        }
        public static OSGeo.OGR.Geometry ToOgrPolygon(this IEnumerable<ICoordinate> coordinates)
        {
            OSGeo.OGR.Geometry geometry = null;
            OSGeo.OGR.Geometry ring = GetOgrGeometry(wkbGeometryType.wkbLinearRing, coordinates);
            if (ring != null)
            {
                geometry = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPolygon);
                geometry.AddGeometry(ring);
            }
            return geometry;
        }
        public static OSGeo.OGR.Geometry ToOgrPolygon(this IEnumerable<IEnumerable<ICoordinate>> ringList)
        {
            OSGeo.OGR.Geometry geometry = null;
            if (ringList?.Any() == true)
            {
                geometry = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPolygon);
                foreach (var ring in ringList)
                {
                    var ogrRing = GetOgrGeometry(wkbGeometryType.wkbLinearRing, ring);
                    if (ogrRing == null)
                    {
                        return null;
                    }
                    geometry.AddGeometry(ogrRing);
                }
            }
            return geometry;
        }
        #endregion

        #region OgrGeometry扩展方法
        public static ICoordinate GetCoordinate(this OSGeo.OGR.Geometry geometry, int index)
        {
            ICoordinate coordinate = null;
            if (geometry != null)
            {
                if (index >= 0 && index < geometry.GetPointCount())
                {
                    int dimension = geometry.GetCoordinateDimension();
                    double[] buffer = new double[dimension];
                    switch (dimension)
                    {
                        case 2:
                            geometry.GetPoint_2D(index, buffer);
                            break;
                        case 3:
                            geometry.GetPoint(index, buffer);
                            break;
                        case 4:
                            geometry.GetPointZM(index, buffer);
                            break;
                    }
                    coordinate = new Coordinate(buffer);
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
                    int dimension = geometry.GetCoordinateDimension(); 
                    switch (dimension)
                    {
                        case 2:
                            geometry.SetPoint_2D(index, coordinate.X, coordinate.Y);
                            break;
                        case 3:
                            geometry.SetPoint(index, coordinate.X, coordinate.Y, coordinate.Z);
                            break;
                        case 4:
                            geometry.SetPointZM(index, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
                            break;
                    }
                }
            }
        }
        #endregion

        #region 将DotSpatial的几何转成自定义的几何

        public static IGeometry ToGeometry(this OSGeo.OGR.Geometry geometry)
        {
            var destGeometry = new Geometry(geometry);
            return destGeometry;
        }
        public static OSGeo.OGR.Geometry ToOgrGeometry(this Geometry geometry)
        {
            var destGeometry = geometry?.OgrGeometry;
            return destGeometry;
        }
        #endregion

        public static GeometryType ToGeometryType(this wkbGeometryType wkbGeometryType)
        {
            GeometryType geometryType = GeometryType.Unknown;
            string name = wkbGeometryType.ToString().Replace("wkb", "");
            Enum.TryParse(name, out geometryType);
            return geometryType;
        }
        public static wkbGeometryType TowWkbGeometryType(this GeometryType geometryType)
        {
            wkbGeometryType wkbGeometryType = wkbGeometryType.wkbUnknown;
            string name = $"wkb{geometryType}";
            Enum.TryParse(name, out wkbGeometryType);
            return wkbGeometryType;
        }

    }
}
