using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素分类
    /// </summary>
    public interface IFeatureCategory : ICategory
    {
        /// <summary>
        /// 过滤表达式
        /// </summary>
        string FilterExpression { get; set; }
        /// <summary>
        /// 选择的符号
        /// </summary>
        IFeatureSymbolizer SelectionSymbolizer { get; set; }
        /// <summary>
        /// 符号
        /// </summary>
        IFeatureSymbolizer Symbolizer { get; set; }
    }
}
