using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 绘制参数
    /// </summary>
    public class DrawingArgs:BaseCopy,IProj
    {
        /// <inheritdoc/>
        public IExtent Extent { get; }
        /// <inheritdoc/>
        public Rectangle Bound { get; }
        /// <summary>
        /// 绘制的范围
        /// </summary>
        public IExtent DrawingExtent { get; }
        public DrawingArgs(Rectangle bound, IExtent extent,IExtent drawingExtent)
        {
            Bound = bound;
            Extent = extent;
            DrawingExtent = drawingExtent;
        }
    }
}
