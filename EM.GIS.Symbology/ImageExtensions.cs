using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using System.Diagnostics;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 位图扩展
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// 指针法拷贝位图
        /// </summary>
        /// <param name="srcBmp">源位图</param>
        /// <param name="destBmp">目标位图</param>
        /// <param name="rect">要复制的范围</param>
        public static void CopyBitmapByPointer(this Bitmap srcBmp, Bitmap destBmp,Rectangle? rect)
        {
            try
            {
                if (srcBmp.Width != destBmp.Width || srcBmp.Height != destBmp.Height||srcBmp.PixelFormat!=destBmp.PixelFormat)
                {
                    return;
                }
                int depth = Image.GetPixelFormatSize(srcBmp.PixelFormat);
                if (depth != 8 && depth != 24 && depth != 32)
                {
                    return;
                }
                int byteCount = depth / 8;
                Rectangle destRect;
                if (rect == null)
                {
                    destRect= new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
                }
                else
                {
                    int left = Math.Max(0, rect.Value.Left);
                    int top = Math.Max(0, rect.Value.Top);
                    int right = Math.Min(srcBmp.Width, rect.Value.Right);
                    int bottom = Math.Min(srcBmp.Height, rect.Value.Bottom);
                    destRect =Rectangle.FromLTRB(left, top, right, bottom);
                }
                var srcBitmapData = srcBmp.LockBits(destRect, ImageLockMode.ReadOnly,srcBmp.PixelFormat);
                var destBitmapData = destBmp.LockBits(destRect, ImageLockMode.ReadWrite,destBmp.PixelFormat);
                unsafe
                {
                    byte* srcPtr = (byte*)srcBitmapData.Scan0;
                    int srcStride = srcBitmapData.Stride;
                    int srcDWidth = srcStride - (srcBmp.Width- destRect.Width)*byteCount;
                    byte* destPtr = (byte*)destBitmapData.Scan0;

                    srcPtr += destRect.Top* srcStride + destRect.Left;
                    destPtr += destRect.Top * srcStride + destRect.Left;
                    for (int row = destRect.Top; row <= destRect.Bottom; row++)
                    {
                        for (int col = destRect.Left; col <= destRect.Right; col++)
                        {
                            for (int i = 0; i < byteCount; i++)
                            {
                                *destPtr = *srcPtr;
                                srcPtr++;
                                destPtr++;
                            }
                        }
                        srcPtr += srcDWidth;
                        destPtr += srcDWidth;
                    }
                }
                srcBmp.UnlockBits(srcBitmapData);
                destBmp.UnlockBits(destBitmapData);
            }
            catch(Exception e)
            {
                Debug.WriteLine($"{nameof(CopyBitmapByPointer)}失败，{e}");
            }
        }
        /// <summary>
        /// 内存法拷贝位图
        /// </summary>
        /// <param name="srcBmp">源位图</param>
        /// <param name="destBmp">目标位图</param>
        /// <param name="rect">拷贝的范围</param>
        public static void CopyBitmapByMemory(this Bitmap srcBmp, Bitmap destBmp, Rectangle? rect=null)
        {
            try
            {
                if (srcBmp.Width != destBmp.Width || srcBmp.Height != destBmp.Height || srcBmp.PixelFormat != destBmp.PixelFormat)
                {
                    return;
                }
                int w = srcBmp.Width, h = srcBmp.Height; PixelFormat format = srcBmp.PixelFormat;
                // Lock the bitmap's bits.  锁定位图
                Rectangle  destRect =rect?? new Rectangle(0, 0, w, h);
                BitmapData bmpDataSrc = srcBmp.LockBits(destRect, ImageLockMode.ReadOnly, format);
                // Get the address of the first line.获取首行地址
                IntPtr ptrSrc = bmpDataSrc.Scan0;
                BitmapData bmpDataDest = destBmp.LockBits(destRect, ImageLockMode.WriteOnly, format);
                IntPtr ptrDest = bmpDataDest.Scan0;// Declare an array to hold the bytes of the bitmap.定义数组保存位图
                int bytes = Math.Abs(bmpDataSrc.Stride) * h;
                byte[] rgbValues = new byte[bytes];// Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptrSrc, rgbValues, 0, bytes);//复制RGB值到数组
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptrDest, bytes);//复制到新图
                srcBmp.UnlockBits(bmpDataSrc);
                destBmp.UnlockBits(bmpDataDest);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(CopyBitmapByMemory)}失败，{e}");
            }

        }//注意使用 bmpSRC.Dispose();
    }
}
