using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Geometries
{
    public interface IPolygon : IGeometry
    {
        /// <summary>
        /// 外圈多边形
        /// </summary>
        ILinearRing Shell { get; }

        int HoleCount { get; }
        ILinearRing GetHole(int index);
    }
}
