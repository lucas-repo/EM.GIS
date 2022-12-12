using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    /// <summary>
    /// 重采样方法
    /// </summary>
    public enum Resampling
    {
        /// <summary>
        /// 平均值
        /// </summary>
        AVERAGE,
        /// <summary>
        /// 平均mag/相空间中的复杂数据
        /// </summary>
        AVERAGE_MAGPHASE,
        /// <summary>
        /// 二次平均值
        /// </summary>
        RMS,
        /// <summary>
        /// 双线性卷积核
        /// </summary>
        BILINEAR,
        /// <summary>
        /// 三次卷积核
        /// </summary>
        CUBIC,
        /// <summary>
        /// B样条卷积核
        /// </summary>
        CUBICSPLINE,
        /// <summary>
        /// 高斯核(在计算总览之前应用高斯核)
        /// </summary>
        GAUSS,
        /// <summary>
        /// 窗口sinc卷积核
        /// </summary>
        LANCZOS,
        /// <summary>
        /// 在所有采样点中最常出现的值
        /// </summary>
        MODE,
        /// <summary>
        /// 近邻（简单采样）重采样器
        /// </summary>
        NEAREST,
        /// <summary>
        /// 无
        /// </summary>
        NONE
    }
}
