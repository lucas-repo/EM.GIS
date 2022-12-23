using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可绘制的接口
    /// </summary>
    public interface IDrawableLayer
    {
        /// <summary>
        /// 绘制图层组到画布
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="projection">投影</param>
        /// <param name="rectangle">屏幕范围</param>
        /// <param name="extent">世界范围</param>
        /// <param name="selected">是否选择</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <param name="invalidateMapFrameAction">重绘地图委托）</param>
        void Draw(Graphics graphics,ProjectionInfo projection, Rectangle rectangle, IExtent extent, bool selected = false, Action<string, int> progressAction = null, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null);
    }
}
