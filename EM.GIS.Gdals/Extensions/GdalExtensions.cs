using EM.GIS.Data;
using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class GdalExtensions
    {

        public static RasterType ToRasterType(this DataType dataType)
        {
            RasterType rasterType = RasterType.Unknown;
            switch (dataType)
            {
                case DataType.GDT_Byte:
                    rasterType = RasterType.Byte;
                    break;
                case DataType.GDT_Int16:
                    rasterType = RasterType.Int16;
                    break;
                case DataType.GDT_Int32:
                    rasterType = RasterType.Int32;
                    break;
                case DataType.GDT_UInt16:
                    rasterType = RasterType.UInt16;
                    break;
                case DataType.GDT_UInt32:
                    rasterType = RasterType.UInt32;
                    break;
                case DataType.GDT_Float32:
                    rasterType = RasterType.Float;
                    break;
                case DataType.GDT_Float64:
                    rasterType = RasterType.Double;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return rasterType;
        }
        public static DataType ToRasterType(this RasterType rasterType)
        {
            DataType dataType = DataType.GDT_Unknown;
            switch (rasterType)
            {
                case RasterType.Byte:
                    dataType = DataType.GDT_Byte;
                    break;
                case RasterType.Int16:
                    dataType = DataType.GDT_CInt16;
                    break;
                case RasterType.Int32:
                    dataType = DataType.GDT_CInt32;
                    break;
                case RasterType.UInt16:
                    dataType = DataType.GDT_UInt16;
                    break;
                case RasterType.UInt32:
                    dataType = DataType.GDT_UInt32;
                    break;
                case RasterType.Float:
                    dataType = DataType.GDT_CFloat32;
                    break;
                case RasterType.Double:
                    dataType = DataType.GDT_CFloat64;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return dataType;
        }
        /// <summary>
        /// 根据波段值创建位图
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="rBuffer"></param>
        /// <param name="gBuffer"></param>
        /// <param name="bBuffer"></param>
        /// <param name="aBuffer"></param>
        /// <param name="noDataValue"></param>
        /// <returns></returns>
        public static unsafe Bitmap GetBitmap(int width, int height, byte[] rBuffer, byte[] gBuffer, byte[] bBuffer, byte[] aBuffer = null, double? noDataValue = null)
        {
            Bitmap result = null;
            int bufferLength = width * height;
            if (width <= 0 || height <= 0 || rBuffer == null || rBuffer.Length != bufferLength || gBuffer == null || gBuffer.Length != bufferLength || bBuffer == null || bBuffer.Length != bufferLength)
            {
                return null;
            }
            PixelFormat pixelFormat;
            int bytesPerPixel;
            if (aBuffer == null)
            {
                if (noDataValue.HasValue && noDataValue.Value >= byte.MinValue && noDataValue.Value <= byte.MaxValue)
                {
                    pixelFormat = PixelFormat.Format32bppArgb;
                    bytesPerPixel = 4;
                }
                else
                {
                    pixelFormat = PixelFormat.Format24bppRgb;
                    bytesPerPixel = 3;
                }
            }
            else
            {
                pixelFormat = PixelFormat.Format32bppArgb;
                bytesPerPixel = 4;
            }
            result = new Bitmap(width, height, pixelFormat);
            BitmapData bData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, pixelFormat);
            byte* scan0 = (byte*)bData.Scan0;
            int stride = bData.Stride;
            int dWidth = stride - width * bytesPerPixel;
            int ptrIndex = 0;
            int bufferIndex = 0;
            if (aBuffer == null)
            {
                if (noDataValue.HasValue && noDataValue.Value >= byte.MinValue && noDataValue.Value <= byte.MaxValue)
                {
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            scan0[ptrIndex] = bBuffer[bufferIndex];
                            scan0[ptrIndex + 1] = gBuffer[bufferIndex];
                            scan0[ptrIndex + 2] = rBuffer[bufferIndex];
                            if (rBuffer[bufferIndex] == noDataValue.Value || gBuffer[bufferIndex] == noDataValue.Value || bBuffer[bufferIndex] == noDataValue.Value)
                            {
                                scan0[ptrIndex + 3] = 0;
                            }
                            else
                            {
                                scan0[ptrIndex + 3] = 255;
                            }
                            ptrIndex += bytesPerPixel;
                            bufferIndex++;
                        }
                        ptrIndex += dWidth;
                    }
                }
                else
                {
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            scan0[ptrIndex] = bBuffer[bufferIndex];
                            scan0[ptrIndex + 1] = gBuffer[bufferIndex];
                            scan0[ptrIndex + 2] = rBuffer[bufferIndex];
                            ptrIndex += bytesPerPixel;
                            bufferIndex++;
                        }
                        ptrIndex += dWidth;
                    }
                }
            }
            else
            {
                if (noDataValue.HasValue && noDataValue.Value >= byte.MinValue && noDataValue.Value <= byte.MaxValue)
                {
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            scan0[ptrIndex] = bBuffer[bufferIndex];
                            scan0[ptrIndex + 1] = gBuffer[bufferIndex];
                            scan0[ptrIndex + 2] = rBuffer[bufferIndex];
                            if (rBuffer[bufferIndex] == noDataValue.Value || gBuffer[bufferIndex] == noDataValue.Value || bBuffer[bufferIndex] == noDataValue.Value)
                            {
                                scan0[ptrIndex + 3] = 0;
                            }
                            else
                            {
                                scan0[ptrIndex + 3] = aBuffer[bufferIndex];
                            }
                            ptrIndex += bytesPerPixel;
                            bufferIndex++;
                        }
                        ptrIndex += dWidth;
                    }
                }
                else
                {
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            scan0[ptrIndex] = bBuffer[bufferIndex];
                            scan0[ptrIndex + 1] = gBuffer[bufferIndex];
                            scan0[ptrIndex + 2] = rBuffer[bufferIndex];
                            scan0[ptrIndex + 3] = aBuffer[bufferIndex];
                            ptrIndex += bytesPerPixel;
                            bufferIndex++;
                        }
                        ptrIndex += dWidth;
                    }
                }
            }
            result.UnlockBits(bData);
            return result;
        }
    }
}
