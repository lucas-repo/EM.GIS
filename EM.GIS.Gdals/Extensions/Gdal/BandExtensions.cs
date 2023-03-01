using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class BandExtensions
    {
        private static object _lockObj = new object();

        /// <summary>
        /// 根据指定对象创建内存指针
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>指针</returns>
        public static IntPtr GetIntPtr(this object obj)
        {
            GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            IntPtr bufferPtr = handle.AddrOfPinnedObject();
            if (handle.IsAllocated) { handle.Free(); }
            return bufferPtr;
        }

        public static void NormalizeSizeToBand(this Band band, int xOffset, int yOffset, int xSize, int ySize, out int width, out int height)
        {
            width = xSize;
            height = ySize;

            if (xOffset + width > band.XSize)
            {
                width = band.XSize - xOffset;
            }
            if (width < 0)
            {
                width = 0;
            }

            if (yOffset + height > band.YSize)
            {
                height = band.YSize - yOffset;
            }
            if (height < 0)
            {
                height = 0;
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
            if (band == null || width == 0 || height == 0)
            {
                return new byte[0];
            }
            byte[] buffer;
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
                    case DataType.GDT_Byte:
                        {
                            buffer = new byte[length];
                            bufferPtr = buffer.GetIntPtr();
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
                            bufferPtr = tmpBuffer.GetIntPtr();
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
                            bufferPtr = tmpBuffer.GetIntPtr();
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
                            bufferPtr = tmpBuffer.GetIntPtr();
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
                            bufferPtr = tmpBuffer.GetIntPtr();
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
                            bufferPtr = tmpBuffer.GetIntPtr();
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
                            bufferPtr = tmpBuffer.GetIntPtr();
                            err = band.ReadRaster(xOffset, yOffset, xSize, ySize, bufferPtr, width, height, dataType, 0, 0);
                            buffer = new byte[length];
                            for (int i = 0; i < length; i++)
                            {
                                buffer[i] = tmpBuffer[i].StretchToByteValue(highValue, lowValue, factor);
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return buffer;
        }

        public static void WriteBand(this Band band, int xOffset, int yOffset, int xSize, int ySize, int width, int height)
        {
            if (band == null ||)
            {
                return;
            }
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
    }
}
