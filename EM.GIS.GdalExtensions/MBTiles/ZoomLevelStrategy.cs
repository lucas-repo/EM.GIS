using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    /// <summary>
    /// 缩放级别策略
    /// </summary>
    public enum ZoomLevelStrategy
    {
        /// <summary>
        /// 自动选择最接近的缩放级别
        /// </summary>
        AUTO,
        /// <summary>
        /// 紧靠下方的缩放级别
        /// </summary>
        LOWER,
        /// <summary>
        /// 紧靠上方的缩放级别
        /// </summary>
        UPPER
    }
}
