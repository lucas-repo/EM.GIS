using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.GIS.Gdals
{
    public class GdalGeometryFactory : IGeometryFactory
    {
        public IGeometry GetGeometry(string wkt)
        {
            OSGeo.OGR.Geometry geometry = OSGeo.OGR.Geometry.CreateFromWkt(wkt);
            IGeometry destGeometry = geometry?.ToGeometry();
            return destGeometry;
        }

        public IGeometry GetLinearRing(IEnumerable<ICoordinate> coordinates)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = coordinates?.ToOgrLinearRing();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }

        public IGeometry GetLineString(IEnumerable<ICoordinate> coordinates)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = coordinates?.ToOgrLineString();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }

        public IGeometry GetPoint(ICoordinate coordinate)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = coordinate?.ToOgrPoint();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }

        public IGeometry GetPolygon(IEnumerable<ICoordinate> coordinates)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = coordinates?.ToOgrPolygon();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }

        public IGeometry GetPolygon(IEnumerable<IEnumerable<ICoordinate>> ringList)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = ringList?.ToOgrPolygon();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }
    }
}
