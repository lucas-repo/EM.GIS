using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Drawing;

namespace EM.GIS.Data
{
    /// <summary>
    /// 地图参数
    /// </summary>
    public class MapArgs :BaseCopy, IProj
    {
        /// <summary>
        /// 画布
        /// </summary>
        public Graphics Graphics { get; set; }
        /// <summary>
        /// 地图范围
        /// </summary>
        public IExtent Extent { get; set; }
        /// <summary>
        /// 画布大小
        /// </summary>
        public Rectangle Bound { get; set; }
        /// <summary>
        /// 投影
        /// </summary>
        public ProjectionInfo Projection { get; set; }
        /// <summary>
        /// 要绘制的范围
        /// </summary>
        public IExtent DestExtent { get; set; }
        public MapArgs(Rectangle rectangle, IExtent extent, Graphics graphics, ProjectionInfo projection, IExtent destExtent):this(rectangle,  extent,  projection,  destExtent)
        {
            Graphics = graphics;
        }
        public MapArgs(Rectangle rectangle, IExtent extent,ProjectionInfo projection, IExtent destExtent)
        {
            Projection = projection;
            DestExtent = destExtent;
            Extent = extent;
            Bound = rectangle;
        }
    }
}
