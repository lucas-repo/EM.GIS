using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
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
    public interface ILegendItem :  ITreeItem, IDisposable
    {
        /// <summary>
        /// 右键菜单命令集合
        /// </summary>
        ObservableCollection<IContextCommand> ContextCommands { get; }
        /// <summary>
        /// 绘制图例
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="rectangle">矩形</param>
        void DrawLegend(Graphics graphics, Rectangle rectangle);
        /// <summary>
        /// 在地图上是否可见
        /// </summary>
        /// <returns>是否可见</returns>
        bool GetVisible();
        /// <summary>
        /// 地图框架
        /// </summary>
        IFrame? Frame { get; set; }
    }
}