using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素接口
    /// </summary>
    public interface ILegendItem : IChangeItem, IParentItem<ILegendItem>, ICloneable, INotifyPropertyChanged
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
        ILegendItemCollection LegendItems { get; }
        /// <summary>
        /// 右键菜单命令集合
        /// </summary>
        ObservableCollection<IBaseCommand> ContextCommands { get; }
        /// <summary>
        /// 图例类型
        /// </summary>
        LegendMode LegendSymbolMode { get; set; }
        /// <summary>
        /// 进度处理
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
        /// <summary>
        /// 绘制图例
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="rectangle">矩形</param>
        void DrawLegend(Graphics graphics, Rectangle rectangle);
    }
}