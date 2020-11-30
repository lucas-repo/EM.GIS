using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Gdals
{
    [Serializable]
    public abstract class GeometryCollection : Geometry, IGeometryCollection
    {
        public GeometryCollection(OSGeo.OGR.Geometry geometry) : base(geometry)
        {
        }
    }
}
