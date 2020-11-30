using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Gdals
{
    [Serializable]
    public class Polygon : Geometry, IPolygon
    {
        public ILinearRing Shell
        {
            get
            {
                ILinearRing geometry = null;
                if (OgrGeometry != null && OgrGeometry.GetGeometryCount() > 0)
                {
                    geometry = OgrGeometry.GetGeometryRef(0).ToLinearRing();
                }
                return geometry;
            }
        }

        public int HoleCount
        {
            get
            {
                int holeCount = 0;
                if (OgrGeometry != null)
                {
                    var count = OgrGeometry.GetGeometryCount();
                    if (count > 0)
                    {
                        holeCount = count - 1;
                    }
                }
                return holeCount;
            }
        }

        public Polygon(OSGeo.OGR.Geometry geometry) : base(geometry)
        { }
        public Polygon(ILinearRing shell, IEnumerable<ILinearRing> holes = null) : base(shell.ToPolygonGeometry(holes))
        { }
        public ILinearRing GetHole(int index)
        {
            ILinearRing linearRing = null;
            if (index >= 0 && index < HoleCount)
            {
                linearRing = OgrGeometry.GetGeometryRef(index).ToLinearRing();
            }
            return linearRing;
        }
        public override object Clone()
        {
            return new Polygon(OgrGeometry.Clone());
        }

    }
}
