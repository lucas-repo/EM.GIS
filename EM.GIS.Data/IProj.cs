using EM.GIS.Geometries;
using System.Drawing;

namespace EM.GIS.Data
{
    /// <summary>
    /// 投影接口，用于计算像素坐标与世界坐标的转换
    /// </summary>
    public interface IProj
    {
        /// <summary>
        /// 世界范围
        /// </summary>
        IExtent Extent { get; set; }
        /// <summary>
        /// 像素范围
        /// </summary>
        Rectangle Bound { get; set; }
    }
}
