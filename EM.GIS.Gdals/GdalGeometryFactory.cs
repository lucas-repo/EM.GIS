using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.IOC;
using System.Collections.Generic;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// gdal几何工厂
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IGeometryFactory))]
    public class GdalGeometryFactory : IGeometryFactory
    {
        /// <inheritdoc/>
        public IGeometry GetGeometry(string wkt)
        {
            OSGeo.OGR.Geometry geometry = OSGeo.OGR.Geometry.CreateFromWkt(wkt);
            IGeometry destGeometry = geometry?.ToGeometry();
            return destGeometry;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
