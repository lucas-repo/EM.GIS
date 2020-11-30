using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Gdals
{
    [Serializable]
    public class LinearRing : LineString, ILinearRing
    {
        public bool IsRing => OgrGeometry.IsRing();
        public LinearRing(OSGeo.OGR.Geometry geometry) : base(geometry)
        {
            if (geometry.GetGeometryType() != OSGeo.OGR.wkbGeometryType.wkbLineString)
            {
                throw new Exception("非法的参数");
            }
        }
        public LinearRing(IEnumerable<ICoordinate> coordinates) : base(coordinates.ToLinearRingGeometry())
        {
        }
        public override object Clone()
        {
            return new LinearRing(OgrGeometry.Clone());
        }
        public void CloseRings() => OgrGeometry.CloseRings();
    }
}
