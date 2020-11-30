using EM.GIS.Geometries;
using System.Drawing;

namespace EM.GIS.Data
{
    /// <summary>
    /// 获取图片接口
    /// </summary>
    public interface IGetBitmap
    {
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <returns></returns>
        Bitmap GetBitmap();

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Bitmap GetBitmap(IExtent envelope, Size size);

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        Bitmap GetBitmap(IExtent envelope, Rectangle window);
    }
}