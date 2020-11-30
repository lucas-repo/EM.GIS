using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 线接口
    /// </summary>
    public interface ILineString : IGeometry
    {
        IPoint StartPoint { get; }

        IPoint EndPoint { get; }

        double Angle { get; }
    }
}
