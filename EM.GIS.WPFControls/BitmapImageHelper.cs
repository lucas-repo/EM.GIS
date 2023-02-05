using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EM.GIS.WPFControls
{
    public static class BitmapImageHelper
    {
        public static byte[] GetPictureData(string imagepath)
        {
            /**/
            ////根据图片文件的路径使用文件流打开，并保存为byte[] 
            FileStream fs = new FileStream(imagepath, FileMode.Open);//可以是其他重载方法 
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close();
            return byData;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }

            return bmp;
        }

        public static byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] byteArray = null;

            try
            {
                Stream sMarket = bmp.StreamSource;

                if (sMarket != null && sMarket.Length > 0)
                {
                    //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。 
                    sMarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(sMarket))
                    {
                        byteArray = br.ReadBytes((int)sMarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling 
            }

            return byteArray;
        }
        public static MemoryStream ToMemoryStream(this Image bitmap, ImageFormat imageFormat)
        {
            MemoryStream ms = null;
            if (bitmap != null)
            {
                ms = new MemoryStream();
                bitmap.Save(ms, imageFormat);
                ms.Seek(0, SeekOrigin.Begin);
            }
            return ms;
        }
        public static BitmapImage ToBitmapImage(this Image bitmap, ImageFormat imageFormat)
        {
            BitmapImage bitmapImage = null;
            if (bitmap != null)
            {
                using (MemoryStream ms = bitmap.ToMemoryStream(imageFormat))
                {
                    if (ms != null)
                    {
                        byte[] buffer = ms.ToArray();
                        bitmapImage = ByteArrayToBitmapImage(buffer);
                    }
                }
            }
            return bitmapImage;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="hObject">对象句柄</param>
        /// <returns>成功与否</returns>
        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
        public static BitmapImage BitmapToBitmapImage(this Image image, ImageFormat? imageFormat = null)
        {
            BitmapImage result = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                ImageFormat destImageFormat = imageFormat ?? ImageFormat.Png;
                image.Save(stream, destImageFormat); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                //result.Freeze();
            }
            return result;
        }
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            BitmapSource source;
            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
            return source;
        }
        /// <summary>
        /// 将<see cref="RectangleF"/>转为<see cref="Rect"/>
        /// </summary>
        /// <param name="rectangle"><see cref="RectangleF"/></param>
        /// <returns><see cref="Rect"/></returns>
        public static Rect ToRect(this RectangleF rectangle)
        {
            return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
        /// <summary>
        /// 将<see cref="Rectangle"/>转为<see cref="Rect"/>
        /// </summary>
        /// <param name="rectangle"><see cref="Rectangle"/></param>
        /// <returns><see cref="Rect"/></returns>
        public static Rect ToRect(this Rectangle rectangle)
        {
            return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// 将Bitmap转成WriteableBitmap 
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <returns><see cref="WriteableBitmap"/> </returns>
        /// <exception cref="NotImplementedException">不支持的格式</exception>
        public static WriteableBitmap ToWriteableBitmap(Bitmap bitmap)
        {
            System.Windows.Media.PixelFormat format;
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format16bppRgb555:
                    format = System.Windows.Media.PixelFormats.Bgr555;
                    break;
                case PixelFormat.Format16bppRgb565:
                    format = System.Windows.Media.PixelFormats.Bgr565;
                    break;
                case PixelFormat.Format24bppRgb:
                    format = System.Windows.Media.PixelFormats.Bgr24;
                    break;
                case PixelFormat.Format32bppRgb:
                    format = System.Windows.Media.PixelFormats.Bgr32;
                    break;
                case PixelFormat.Format32bppPArgb:
                    format = System.Windows.Media.PixelFormats.Pbgra32;
                    break;
                case PixelFormat.Format32bppArgb:
                    format = System.Windows.Media.PixelFormats.Bgra32;
                    break;
                default:
                    throw new NotImplementedException($"不支持的格式 {bitmap.PixelFormat}");
            }
            var wb = new WriteableBitmap(bitmap.Width, bitmap.Height, 0, 0, format, null);
            bitmap.CopyToWriteableBitmap(wb, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0);
            return wb;
        }
        /// <summary>
        /// 将Bitmap数据写入WriteableBitmap中
        /// </summary>
        /// <param name="src">源位图</param>
        /// <param name="dst">目标位图</param>
        /// <param name="srcRect">原始范围</param>
        /// <param name="destinationX">目标x坐标</param>
        /// <param name="destinationY">目标y坐标</param>
        public static void CopyToWriteableBitmap(this Bitmap src, WriteableBitmap dst, Rectangle srcRect, int destinationX, int destinationY)
        {
            var data = src.LockBits(new Rectangle(new System.Drawing.Point(0, 0), src.Size), ImageLockMode.ReadOnly, src.PixelFormat);
            dst.WritePixels(new Int32Rect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), data.Scan0, data.Height * data.Stride, data.Stride, destinationX, destinationY);
            src.UnlockBits(data);
        }
    }
}
