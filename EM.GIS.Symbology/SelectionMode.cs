using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 选择模式
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// 要素的顶点都在区域内(且不触及区域边界)
        /// </summary>
        /// <remarks>
        /// 使用ContainsExtent进行更快的选择
        /// </remarks>
        Contains,

        /// <summary>
        /// 更快的选择方式，要选择的要素必须完全包含在范围中
        /// </summary>
        ContainsExtent,

        /// <summary>
        /// 整个区域都在范围内，不触及边界
        /// </summary>
        CoveredBy,

        /// <summary>
        /// 整个区域都在范围内，可触及边界
        /// </summary>
        Covers,

        /// <summary>
        /// 穿过指定范围
        /// </summary>
        Crosses,

        /// <summary>
        /// 无相交
        /// </summary>
        Disjoint,

        /// <summary>
        /// *更快的方法，如果该项的任何部分在范围中可见，则该项将被选中
        /// </summary>
        IntersectsExtent,

        /// <summary>
        /// 相交.
        /// </summary>
        Intersects,

        /// <summary>
        /// 交叉.
        /// </summary>
        Overlaps,

        /// <summary>
        /// 有相同边
        /// </summary>
        Touches,

        /// <summary>
        /// 范围在指定要素内
        /// </summary>
        Within
    }
}
