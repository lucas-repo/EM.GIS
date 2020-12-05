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
        private static object _lockObj=new object();
        /// <summary>
        /// 尝试以读写打开数据，若失败则以只读打开
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Dataset Open(string fileName)
        {
            Dataset dataset = null;
            if (File.Exists(fileName))
            {
                try
                {
                    dataset = Gdal.Open(fileName, Access.GA_Update);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"以读写打开“{fileName}”失败，将尝试以只读打开，错误信息：{e}");
                    try
                    {
                        dataset = Gdal.Open(fileName, Access.GA_ReadOnly);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"以只读打开“{fileName}”失败，错误信息：{ex}");
                    }
                }
            }
            return dataset;
        }

        public static int CreateOverview(this Dataset dataset, string resampling = "NEAREST", int[] overviewlist = null)
        {
            int value = -1;
            if (dataset == null || dataset.RasterCount <= 0)
            {
                return value;
            }

            if (overviewlist == null)
            {
                List<int> intList = new List<int>();
                int width = dataset.RasterXSize;
                int height = dataset.RasterYSize;
                int k = 1;
                while (width > 256 && height > 256)
                {
                    k *= 2;
                    intList.Add(k);
                    width /= 2;
                    height /= 2;
                }

                overviewlist = intList.ToArray();
            }

            value = dataset.BuildOverviews(resampling, overviewlist);
            return value;
        }

        public static void NormalizeSizeToBand(int xOffset, int yOffset, int xSize, int ySize, Band band, out int width, out int height)
        {
            width = xSize;
            height = ySize;

            if (xOffset + width > band.XSize)
            {
                width = band.XSize - xOffset;
            }

            if (yOffset + height > band.YSize)
            {
                height = band.YSize - yOffset;
            }
        }

        /// <summary>
        /// 读取栅格块为字节数组（自动拉伸）
        /// </summary>
        /// <param name="band"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static byte[] ReadBand(this Band band, int xOffset, int yOffset, int xSize, int ySize, int width, int height)
        {
            byte[] buffer = null;
            if (band == null || width == 0 || height == 0)
            {
                return buffer;
            }
            int length = width * height;
            DataType dataType = band.DataType;
            IntPtr bufferPtr;
            // Percentage truncation
            double minPercent = 0.5;
            double maxPercent = 0.5;
            band.GetMaximum(out double maxValue, out int hasvalue);
            band.GetMinimum(out double minValue, out hasvalue);
            double dValue = maxValue - minValue;
            double highValue = maxValue - dValue * maxPercent / 100;
            double lowValue = minValue + dValue * minPercent / 100;
            double factor = 255 / (highValue - lowValue); // 系数
            CPLErr err = CPLErr.CE_None;
            lock (_lockObj)
            {
                switch (dataType)
                {
                    case DataType.GDT_Unknown:
                        throw new Exception("Unknown datatype");
                    case DataType.GDT_Byte:
                        {
                            buffer = new byte[length];
                            bufferPtr = GCHandleHelper.GetIntPtr(buffer);
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            //for (int i = 0; i < length; i++)
                            //{
                            //    buffer[i] = buffer[i].StretchToByteValue(highValue, lowValue, factor);//做拉伸时才需要
                            //}
                        }
                        break;
                    case DataType.GDT_UInt16:
                        {
                            ushort[] tmpBuffer = new ushort[length];
                            bufferPtr = GCHandleHelper.GetIntPtr(tmpBuffer);
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            buffer = new byte[length];
                            for (int i = 0; i < length; i++)
                            {
                                buffer[i] = tmpBuffer[i].StretchToByteValue(highValue, lowValue, factor);
                            }
                        }
                        break;
                    case DataType.GDT_Int16:
                        {
                            short[] tmpBuffer = new short[length];
                            bufferPtr = GCHandleHelper.GetIntPtr(tmpBuffer);
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            buffer = new byte[length];
                            for (int i = 0; i < length; i++)
                            {
                                buffer[i] = tmpBuffer[i].StretchToByteValue(highValue, lowValue, factor);
                            }
                        }
                        break;
                    case DataType.GDT_UInt32:
                        {
                            uint[] tmpBuffer = new uint[length];
                            bufferPtr = GCHandleHelper.GetIntPtr(tmpBuffer);
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            buffer = new byte[length];
                            for (int i = 0; i < length; i++)
                            {
                                buffer[i] = tmpBuffer[i].StretchToByteValue(highValue, lowValue, factor);
                            }
                        }
                        break;
                    case DataType.GDT_Int32:
                        {
                            int[] tmpBuffer = new int[length];
                            bufferPtr = GCHandleHelper.GetIntPtr(tmpBuffer);
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            buffer = new byte[length];
                            for (int i = 0; i < length; i++)
                            {
                                buffer[i] = tmpBuffer[i].StretchToByteValue(highValue, lowValue, factor);
                            }
                        }
                        break;
                    case DataType.GDT_Float32:
                        {
                            float[] tmpBuffer = new float[length];
                            bufferPtr = GCHandleHelper.GetIntPtr(tmpBuffer);
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            buffer = new byte[length];
                            for (int i = 0; i < length; i++)
                            {
                                buffer[i] = tmpBuffer[i].StretchToByteValue(highValue, lowValue, factor);
                            }
                        }
                        break;
                    case DataType.GDT_Float64:
                        {
                            double[] tmpBuffer = new double[length];
                            bufferPtr = GCHandleHelper.GetIntPtr(tmpBuffer);
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            buffer = new byte[length];
                            for (int i = 0; i < length; i++)
                            {
                                buffer[i] = tmpBuffer[i].StretchToByteValue(highValue, lowValue, factor);
                            }
                        }
                        break;
                    case DataType.GDT_CInt16:
                    case DataType.GDT_CInt32:
                    case DataType.GDT_CFloat32:
                    case DataType.GDT_CFloat64:
                    case DataType.GDT_TypeCount:
                        throw new NotImplementedException();
                }
            }
            return buffer;
        }
        public static RasterType ToRasterType(this DataType dataType)
        {
            RasterType rasterType = RasterType.Unknown;
            switch (dataType)
            {
                case DataType.GDT_Byte:
                    rasterType = RasterType.Byte;
                    break;
                case DataType.GDT_CInt16:
                    rasterType = RasterType.Int16;
                    break;
                case DataType.GDT_CInt32:
                    rasterType = RasterType.Int32;
                    break;
                case DataType.GDT_UInt16:
                    rasterType = RasterType.UInt16;
                    break;
                case DataType.GDT_UInt32:
                    rasterType = RasterType.UInt32;
                    break;
                case DataType.GDT_CFloat32:
                    rasterType = RasterType.Float;
                    break;
                case DataType.GDT_CFloat64:
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
        /// 计算读取块大小
        /// </summary>
        /// <param name="band"></param>
        /// <param name="blockXsize"></param>
        /// <param name="blockYsize"></param>
        public static void ComputeBlockSize(this Band band, out int blockXsize, out int blockYsize)
        {
            int minSize = 1024;
            int maxSize = 4096;
            band.GetBlockSize(out blockXsize, out blockYsize);
            if (blockXsize > maxSize)
            {
                blockXsize = Math.Min(maxSize, blockXsize);
            }
            else if (blockXsize < minSize)
            {
                blockXsize = Math.Min(minSize, band.XSize);
            }
            if (blockYsize > maxSize)
            {
                blockYsize = Math.Min(maxSize, blockYsize);
            }
            else if (blockYsize < minSize)
            {
                blockYsize = Math.Min(minSize, band.YSize);
            }
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
