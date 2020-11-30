using EM.GIS.Geometries;
using System;

namespace EM.GIS.Gdals
{
    [Serializable]
    public class MultiLineString:GeometryCollection, IMultiLineString
    {
        public MultiLineString(OSGeo.OGR.Geometry geometry) : base(geometry)
        { }
        public override object Clone()
        {
            return new MultiLineString(OgrGeometry.Clone());
        }
    }
}
