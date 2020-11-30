using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素接口
    /// </summary>
    public interface ILegendItem : IChangeItem, IParentItem<ILegendItem>,ICloneable
    {
        /// <summary>
        /// 是否展开
        /// </summary>
        bool IsExpanded { get; set; }
        /// <summary>
        /// 是否选择
        /// </summary>
        bool IsSelected { get; set; }
        /// <summary>
        /// 图例类型
        /// </summary>
        LegendType LegendType { get; set; }
        /// <summary>
        /// 是否可见
        /// </summary>
        bool IsVisible { get; set; }
        /// <summary>
        /// 文字
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// 子元素集合
        /// </summary>
        ILegendItemCollection Items { get; }
        /// <summary>
        /// Gets or sets a list of menu items that should appear for this layer.
        /// These are in addition to any that are supported by default.
        /// Handlers should be added to this list before it is retrieved.
        /// </summary>
        List<SymbologyMenuItem> ContextMenuItems { get; set; }

        LegendMode LegendSymbolMode { get; set; }

        void DrawLegend(Graphics context, Rectangle rectangle);
    }
}