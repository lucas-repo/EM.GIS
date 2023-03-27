using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 写入或读取的栅格块参数
    /// </summary>
    public struct RasterArgs
    {
        public RasterArgs(int xOff, int yOff, int xSize, int ySize, int bufferXSize, int bufferYSize, int bandCount, int[] bandMap, int pixelSpace=0, int lineSpace=0, int bandSpace=0)
        {
            XOff = xOff;
            YOff = yOff;
            XSize = xSize;
            YSize = ySize;
            BufferXSize = bufferXSize;
            BufferYSize = bufferYSize;
            BandCount = bandCount;
            BandMap = bandMap;
            PixelSpace = pixelSpace;
            LineSpace = lineSpace;
            BandSpace = bandSpace;
        }

        /// <summary>
        /// X偏移
        /// </summary>
        public int XOff { get; set; }
        /// <summary>
        /// Y偏移
        /// </summary>
        public int YOff { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int XSize { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int YSize { get; set; }
        /// <summary>
        /// 缓冲区宽度
        /// </summary>
        public int BufferXSize { get; set; }
        /// <summary>
        /// 缓冲区高度
        /// </summary>
        public int BufferYSize { get; set; }
        /// <summary>
        /// 波段数
        /// </summary>
        public int BandCount { get; set; }
        /// <summary>
        /// 波段匹配
        /// </summary>
        public int[] BandMap { get; set; }
        /// <summary>
        /// 像素间隔
        /// </summary>
        public int PixelSpace { get; set; }
        /// <summary>
        /// 扫描行间隔
        /// </summary>
        public int LineSpace { get; set; }
        /// <summary>
        /// 波段间隔
        /// </summary>
        public int BandSpace { get; set; }
    }
}
