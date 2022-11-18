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
        /// 地图范围
        /// </summary>
        IExtent Extent { get; }
        /// <summary>
        /// 窗口范围
        /// </summary>
        Rectangle Bound { get; }
    }
}
