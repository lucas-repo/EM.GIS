using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 可绘制的接口
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// 根据指定的范围，将当前内容绘制到指定的画布
        /// </summary>
        /// <param name="mapArgs">参数</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <returns>图片</returns>
        void Draw(MapArgs mapArgs, Action< int> progressAction = null, Func<bool> cancelFunc = null);
    }
}
