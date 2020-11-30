using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 点接口
    /// </summary>
    public interface IPoint:IGeometry
    {
        /// <summary>
        /// X值
        /// </summary>
        double X { get; set; }
        /// <summary>
        /// Y值
        /// </summary>
        double Y { get; set; }
        /// <summary>
        /// Z值
        /// </summary>
        double Z { get; set; }
        /// <summary>
        /// M值
        /// </summary>
        double M { get; set; }
    }
}
