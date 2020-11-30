using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public class GdalGeometryFactory : IGeometryFactory
    {
        public IGeometry GetGeometryFromWkt(string wkt)
        {
            OSGeo.OGR.Geometry geometry = OSGeo.OGR.Geometry.CreateFromWkt(wkt);
            IGeometry destGeometry = geometry?.ToGeometry();
            return destGeometry;
        }

        public ILinearRing GetLinearRing(IEnumerable<ICoordinate> coordinates)
        {
            var  geometry = new LinearRing(coordinates);
            return geometry;
        }

        public ILineString GetLineString(IEnumerable<ICoordinate> coordinates)
        {
            var geometry = new LineString(coordinates);
            return geometry;
        }

        public IPoint GetPoint(ICoordinate coordinate)
        {
            var geometry = new Point(coordinate);
            return geometry;
        }

        public IPolygon GetPolygon(ILinearRing shell, IEnumerable<ILinearRing> holes = null)
        {
            var geometry = new Polygon(shell, holes);
            return geometry;
        }
    }
}
