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
    }
}