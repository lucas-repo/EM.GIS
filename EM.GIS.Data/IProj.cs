using EM.GIS.Geometries;
using System.Drawing;

namespace EM.GIS.Data
{
    /// <summary>
    /// 带范围和边界的接口
    /// </summary>
    public interface IProj
    {
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; set; }
        /// <summary>
        /// 边界
        /// </summary>
        Rectangle Bound { get; set; }
    }
}
