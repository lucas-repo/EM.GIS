using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 动态可见性接口
    /// </summary>
    public interface IDynamicVisibility
    {
        /// <summary>
        /// 使用动态可见性
        /// </summary>
        bool UseDynamicVisibility { get; set; }
        /// <summary>
        /// 最大比例尺倒数
        /// </summary>
        double MaxInverseScale { get; set; }
        /// <summary>
        /// 最小比例尺倒数
        /// </summary>
        double MinInverseScale { get; set; }
    }
}
