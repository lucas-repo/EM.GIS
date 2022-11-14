using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 图片辅助类
    /// </summary>
    public static class BitmapHelper
    {
        private static byte[] BitmapImageToBuffer(BitmapImage bmpImage, BitmapEncoder bmpEncoder)
        {
            byte[] result = null;
            bmpEncoder.Frames.Add(BitmapFrame.Create(bmpImage));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmpEncoder.Save(memoryStream);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    result = binaryReader.ReadBytes((int)memoryStream.Length);
                    binaryReader.Close();
                }

                memoryStream.Close();
            }

            return result;
        }

        public static byte[] BitmapToBuffer(Bitmap bmp, ImageFormat imageFormat)
        {
            byte[] array = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, imageFormat);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                array = new byte[memoryStream.Length];
                memoryStream.Read(array, 0, array.Length);
            }

            return array;
        }

        public static byte[] PngBitmapImageToBuffer(BitmapImage bmpImage)
        {
            PngBitmapEncoder bmpEncoder = new PngBitmapEncoder();
            return BitmapImageToBuffer(bmpImage, bmpEncoder);
        }

        public static byte[] JpgBitmapImageToBuffer(BitmapImage bmpImage)
        {
            JpegBitmapEncoder bmpEncoder = new JpegBitmapEncoder();
            return BitmapImageToBuffer(bmpImage, bmpEncoder);
        }

        public static BitmapImage BufferToBitmapImage(byte[] buffer)
        {
            if (buffer == null)
            {
                return null;
            }

            BitmapImage bitmapImage = new BitmapImage();
            MemoryStream streamSource = new MemoryStream(buffer);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = streamSource;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static BitmapImage GetBitmapImage(string fileName)
        {
            BitmapImage result = null;
            if (!File.Exists(fileName))
            {
                return result;
            }

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    byte[] array = new byte[fileStream.Length];
                    binaryReader.Read(array, 0, array.Length);
                    result = BufferToBitmapImage(array);
                    binaryReader.Close();
                }

                fileStream.Close();
            }

            return result;
        }

        public static BitmapImage BitmapToBitmapImage(Image image, ImageFormat imageFormat = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (imageFormat != null)
            {
                image.Save(memoryStream, imageFormat);
            }
            else
            {
                image.Save(memoryStream, ImageFormat.Png);
            }

            memoryStream.Seek(0L, SeekOrigin.Begin);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
