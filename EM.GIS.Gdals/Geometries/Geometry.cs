using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Gdals
{
    [Serializable]
    public abstract class Geometry : IGeometry
    {
        public OSGeo.OGR.Geometry OgrGeometry { get; }
        public int PointCount => OgrGeometry.GetPointCount();

        public int GeometryCount => OgrGeometry.GetGeometryCount();

        public IExtent Extent
        {
            get
            {
                OSGeo.OGR.Envelope envelope = new  OSGeo.OGR.Envelope();
                OgrGeometry.GetEnvelope(envelope);
               return envelope.ToExtent();
            }
    }

        public GeometryType GeometryType
        {
            get
            {
                GeometryType geometryType = GeometryType.Point; 
                switch (OgrGeometry.GetGeometryType())
                {
                    case OSGeo.OGR.wkbGeometryType.wkbPoint:
                        geometryType = GeometryType.Point;
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbLineString:
                        geometryType = GeometryType.LineString;
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbLinearRing:
                        geometryType = GeometryType.LinearRing;
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbPolygon:
                        geometryType = GeometryType.Polygon;
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbMultiPoint:
                        geometryType = GeometryType.MultiPoint;
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbMultiLineString:
                        geometryType = GeometryType.MultiLineString;
                        break;
                    case OSGeo.OGR.wkbGeometryType.wkbMultiPolygon:
                        geometryType = GeometryType.MultiPolygon;
                        break;
                    default:
                        throw new NotImplementedException("暂未实现的类型");
                }
                return geometryType;
            }
        }

        public double Area => OgrGeometry.Area();

        public double Length => OgrGeometry.Length();

        public ICoordinate Coord => OgrGeometry.GetCoordinate(0);

        public bool IsEmpty() => OgrGeometry.IsEmpty();

        public IGeometry GetGeometry(int index)
        {
            var numGeometry = OgrGeometry.GetGeometryRef(index);
            var dsGeometry = numGeometry.ToGeometry();
            return dsGeometry;
        }

        public ICoordinate GetCoord(int index)
        {
            ICoordinate point = OgrGeometry.GetCoordinate(index);
            return point;
        }


        public void SetCoord(int index, ICoordinate coordinate)
        {
            OgrGeometry.SetCoordinate(index, coordinate);
        }
        public string ToWkt()
        {
            var ret= OgrGeometry.ExportToWkt(out string wkt);
            return wkt;
        }
        public override string ToString()
        {
            return ToWkt();
        }

        public bool Contains(IGeometry g)
        {
            bool ret = false;
            if (g != null)
            {
                ret = OgrGeometry.Contains(g.ToGeometry());
            }
            return ret;
        }

        public bool Intersects(IGeometry g)
        {
            bool ret = false;
            if (g is Geometry geometry)
            {
                ret = OgrGeometry.Intersects(geometry.OgrGeometry);
            }
            return ret;
        }

        public IGeometry Intersection(IGeometry other)
        {
            IGeometry destGeometry = null;
            if (other != null)
            {
                destGeometry = OgrGeometry.Intersection(other.ToGeometry()).ToGeometry();
            }
            return destGeometry;
        }

        public double Distance(IGeometry g)
        {
            double ret = 0;
            if (g != null)
            {
                ret = OgrGeometry.Distance(g.ToGeometry());
            }
            return ret;
        }

        public IGeometry Union(IGeometry other)
        {
            IGeometry destGeometry = null;
            if (other != null)
            {
                var geometry = OgrGeometry.Union(other.ToGeometry());
                destGeometry = geometry.ToGeometry();
            }
            return destGeometry;
        }

        public double Distance(ICoordinate coord)
        {
            double ret = 0;
            if (coord != null)
            {
                ret = OgrGeometry.Distance(coord.ToPointGeometry());
            }
            return ret;
        }

        public List<ICoordinate> GetAllCoords()
        {
            List<ICoordinate> coords = new List<ICoordinate>();
            for (int i = 0; i < PointCount; i++)
            {
                ICoordinate coord = GetCoord(i);
                coords.Add(coord);
            }
            return coords;
        }

        public abstract object Clone();

        public Geometry(OSGeo.OGR.Geometry geometry)
        {
            if (geometry == null || geometry.IsEmpty())
            {
                throw new Exception("非法的参数");
            }
            OgrGeometry = geometry;
        }
        public override bool Equals(object obj)
        {
            bool ret = false;
            if (obj is Geometry geometry)
            {
                ret = OgrGeometry == geometry.OgrGeometry;
            }
            return ret;
        }

        public override int GetHashCode()
        {
            int hashCode = GeometryType.GetHashCode() ^ OgrGeometry.GetHashCode();
            return hashCode;
        }
        
    }
}
