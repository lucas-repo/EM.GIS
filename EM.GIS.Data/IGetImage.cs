using EM.GIS.Geometries;
using System;
using System.Drawing;

namespace EM.GIS.Data
{
    /// <summary>
    /// 获取图片接口
    /// </summary>
    public interface IGetImage1
    {
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <returns>图片</returns>
        Image GetImage();

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="envelope">世界范围</param>
        /// <param name="size">图片大小</param>
        /// <returns>图片</returns>
        Image GetImage(IExtent envelope, Size size);

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="extent">世界范围</param>
        /// <param name="rectangle">屏幕范围</param>
        /// <param name="progressAction">进度委托</param>
        /// <returns>图片</returns>
        Image GetImage(IExtent extent, Rectangle rectangle, Action<int> progressAction = null, Func<bool> cancelFunc = null);
    }
}