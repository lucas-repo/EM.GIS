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

        public IGeometry GetLinearRing(IEnumerable<ICoordinate> coordinates)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = coordinates.ToLinearRingGeometry();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }

        public IGeometry GetLineString(IEnumerable<ICoordinate> coordinates)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = coordinates.ToLineStringGeometry();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }

        public IGeometry GetPoint(ICoordinate coordinate)
        {
            Geometry geometry = null;
            OSGeo.OGR.Geometry ogrGeometry = coordinate.ToPointGeometry();
            if (ogrGeometry != null)
            {
                geometry = new Geometry(ogrGeometry);
            }
            return geometry;
        }

        public IGeometry GetPolygon(IGeometry shell, IEnumerable<IGeometry> holes = null)
        {
            Geometry geometry = null;
            if (shell?.GeometryType != GeometryType.LinearRing)
            {
                return geometry;
            }
            OSGeo.OGR.Geometry ogrGeometry = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbPolygon);
            ogrGeometry.AddGeometry(shell.ToGeometry());
            if (holes != null)
            {
                foreach (var hole in holes)
                {
                    if (hole?.GeometryType == GeometryType.LinearRing)
                    {
                        ogrGeometry.AddGeometry(hole.ToGeometry());
                    }
                }
            }
            geometry = new Geometry(ogrGeometry);
            return geometry;
        }
    }
}
