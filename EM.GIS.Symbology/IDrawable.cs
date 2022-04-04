using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可绘制的接口
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// 绘制图层组到画布
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="rectangle">屏幕范围</param>
        /// <param name="extent">世界范围</param>
        /// <param name="selected">是否选择</param>
        /// <param name="cancelFunc">取消绘制委托</param>
        /// <param name="invalidateMapFrameAction">使地图框无效委托（重绘）</param>
        void Draw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null);
    }
}
