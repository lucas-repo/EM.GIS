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
        public Graphics Device { get; }
        public IExtent Extent { get; }
        public Rectangle Bound { get; }
        public double Dx { get; }
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
