using EM.GIS.Data;
using EM.GIS.Geometries;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 绘制参数
    /// </summary>
    public class DrawArgs : IProj
    {
        public DrawArgs(Rectangle bound, IExtent extent, IExtent destExtent)
        {
            Extent = extent;
            Bound = bound;
            DestExtent = destExtent;
        }

        /// <inheritdoc/>
        public IExtent Extent { get; }

        /// <inheritdoc/>
        public Rectangle Bound  { get; }
        /// <summary>
        /// 要绘制的范围
        /// </summary>
        public IExtent DestExtent { get; }
    }
}
