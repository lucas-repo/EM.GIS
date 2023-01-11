using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 位图扩展
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        /// 读取位图rgb数组
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <returns>rgb数组</returns>
        /// <exception cref="NotSupportedException">不支持的位图类型</exception>
        public static unsafe byte[]? GetRgbBytes(this Bitmap bitmap)
        {
            byte[]? ret = null;
            if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
            {
                return ret;
            }
            int width = bitmap.Width;
            int height = bitmap.Height;
            int bytesPerPixel;
            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    bytesPerPixel = 3;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    bytesPerPixel = 4;
                    break;
                default:
                    throw new NotSupportedException();
            }
            ret = new byte[width * height * bytesPerPixel];

            BitmapData bData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0;
            int stride = bData.Stride;
            int dWidth = stride - width * bytesPerPixel;
            int readByteCount = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    ret[readByteCount + 2] = scan0[0];
                    ret[readByteCount + 1] = scan0[1];
                    ret[readByteCount] = scan0[2];
                    scan0 += bytesPerPixel;
                    readByteCount += bytesPerPixel;
                }
                scan0 += dWidth;
            }
            bitmap.UnlockBits(bData);
            return ret;
        }
        /// <summary>
        /// 读取位图rgb数组
        /// </summary>
        /// <param name="bmpPath">位图路径</param>
        /// <returns>rgb数组</returns>
        public static byte[]? GetRgbBytes(string bmpPath)
        {
            byte[]? ret = null;
            if (!File.Exists(bmpPath))
            {
                return ret;
            }
            using Bitmap bitmap = new Bitmap(bmpPath);
            if (bitmap == null)
            {
                return ret;
            }
            ret = bitmap.GetRgbBytes();
            return ret;
        }
    }
}
