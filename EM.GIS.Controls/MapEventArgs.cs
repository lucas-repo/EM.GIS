using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 地图事件参数
    /// </summary>
    public class MapEventArgs : EventArgs, IProj
    {
        #region  Constructors

        /// <summary>
        /// 初始化实例 <see cref="MapEventArgs"/> .
        /// </summary>
        /// <param name="rectangle">窗口范围</param>
        /// <param name="extent">世界范围.</param>
        public MapEventArgs(Rectangle rectangle, IExtent extent)
        {
            Bound = rectangle;
            Extent = extent;
        }

        /// <summary>
        /// 初始化实例 <see cref="MapEventArgs"/> .
        /// </summary>
        /// <param name="rectangle">窗口范围</param>
        /// <param name="extent">世界范围.</param>
        /// <param name="g">画布.</param>
        public MapEventArgs(Rectangle bufferRectangle, IExtent bufferEnvelope, Graphics g)
        {
            Bound = bufferRectangle;
            Extent = bufferEnvelope;
            Device = g;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 画布
        /// </summary>
        public Graphics Device { get; }

        /// <summary>
        /// 1个地图单位对应的X轴像素大小
        /// </summary>
        public double Dx => Extent.Width != 0.0 ? Bound.Width / Extent.Width : 0.0;

        /// <summary>
        /// 1个地图单位对应的Y轴像素大小
        /// </summary>
        public double Dy => Extent.Height != 0.0 ? Bound.Height / Extent.Height : 0.0;

        /// <inheritdoc/>
        public IExtent Extent { get;  }

        /// <inheritdoc/>
        public Rectangle Bound { get;  }

        /// <summary>
        /// 最大Y值
        /// </summary>
        public double MaxY => Extent.MaxY;

        /// <summary>
        /// 最小X值
        /// </summary>
        public double MinX => Extent.MinX;

        #endregion
    }
}
