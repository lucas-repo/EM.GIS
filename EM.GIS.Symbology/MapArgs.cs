using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 地图参数
    /// </summary>
    public class MapArgs : IProj
    {
        /// <summary>
        /// 画布
        /// </summary>
        public Graphics Device { get; }
        /// <summary>
        /// 范围
        /// </summary>
        public IExtent Extent { get; }
        /// <summary>
        /// 窗口范围
        /// </summary>
        public Rectangle Bound { get; }
        /// <summary>
        /// x分辨率
        /// </summary>
        public double Dx { get; }
        /// <summary>
        /// y分辨率
        /// </summary>
        public double Dy { get; }
        public MapArgs(Rectangle rectangle, IExtent extent)
        {
            Extent = extent;
            Bound = rectangle;
            double worldWidth = extent.Width;
            double worldHeight = extent.Height;
            Dx = rectangle.Width != 0 ? worldWidth / rectangle.Width : 0;
            Dy = rectangle.Height != 0 ? worldHeight / rectangle.Height : 0;
        }
        public MapArgs(Rectangle rectangle, IExtent extent, Graphics g) : this(rectangle, extent)
        {
            Device = g;
        }
    }
}
