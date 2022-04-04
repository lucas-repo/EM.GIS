using EM.Bases;
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
    public interface ILegendItem :  ITreeItem, ICloneable, INotifyPropertyChanged, IProgressHandler
    {
        /// <summary>
        /// 父元素
        /// </summary>
        new ILegendItem Parent { get; set; }
        /// <summary>
        /// 右键菜单命令集合
        /// </summary>
        ObservableCollection<IBaseCommand> ContextCommands { get; }
        /// <summary>
        /// 绘制图例
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="rectangle">矩形</param>
        void DrawLegend(Graphics graphics, Rectangle rectangle);
    }
}