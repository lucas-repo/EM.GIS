using EM.GIS.Data;
using EM.GIS.Geometries;
using System.Drawing;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 几何鼠标参数
    /// </summary>
    public interface IGeoMouseEventArgs
    {
        /// <summary>
        /// 屏幕坐标
        /// </summary>
        PointD Location { get; }
        /// <summary>
        /// 地理坐标
        /// </summary>
        ICoordinate GeographicLocation { get; }
        /// <summary>
        /// 地图
        /// </summary>
        IMap Map { get;  }
    }
}