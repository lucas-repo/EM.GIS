using EM.GIS.Data;
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
        /// <param name="mapArgs">绘制参数</param>
        /// <param name="selected">是否选择</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <param name="invalidateMapFrameAction">重绘地图委托）</param>
        void Draw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null);
    }
}
