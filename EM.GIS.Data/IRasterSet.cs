using System;
using System.Collections;
using System.Collections.Generic;

namespace EM.GIS.Data
{
    /// <summary>
    /// 栅格数据集
    /// </summary>
    public interface IRasterSet:IDataSet, IDrawable
    {
        /// <summary>
        /// 栅格类型
        /// </summary>
        RasterType RasterType { get; }
        /// <summary>
        /// 子栅格集
        /// </summary>
        IEnumerable<IRasterSet> Rasters { get; }
        /// <summary>
        /// 波段数
        /// </summary>
        int BandCount { get; }

        /// <summary>
        /// 高度
        /// </summary>
        int Height { get; }
        /// <summary>
        /// 长度
        /// </summary>
        int Width { get; }
        /// <summary>
        /// 无数据值
        /// </summary>
        double? NoDataValue { get; set; }
        /// <summary>
        /// 获取统计值
        /// </summary>
        /// <returns>统计值</returns>
        Statistics GetStatistics();
        /// <summary>
        /// 设置几何变换矩阵
        /// </summary>
        /// <param name="affine">仿射六参数</param>
        void SetGeoTransform(double[] affine);

        /// <summary>
        /// 写入指定的栅格文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="srcXOff">源X偏移</param>
        /// <param name="srcYOff">源Y偏移</param>
        /// <param name="srcWidth">源宽度</param>
        /// <param name="srcHeight">源高度</param>
        /// <param name="destXOff">目标X偏移</param>
        /// <param name="destYOff">目标Y偏移</param>
        /// <param name="destWidth">目标宽度</param>
        /// <param name="destHeight">目标高度</param>
        /// <param name="bandCount">波段数</param>
        /// <param name="bandMap">波段顺序</param>
        void WriteRaster(string filename, int srcXOff, int srcYOff, int srcWidth, int srcHeight, int destXOff, int destYOff, int destWidth, int destHeight,int bandCount,int[] bandMap);
    }
}