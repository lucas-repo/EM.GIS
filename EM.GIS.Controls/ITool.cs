using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 工具接口
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// 工具已激活事件
        /// </summary>
        event EventHandler Activated;
        /// <summary>
        /// 工具已冻结事件
        /// </summary>
        event EventHandler Deactivated;
        /// <summary>
        /// 绘制事件
        /// </summary>
        event EventHandler<MapEventArgs> Drawn;
        /// <summary>
        /// 图标
        /// </summary>
        Image Image { get; set; }
        /// <summary>
        /// 光标
        /// </summary>
        Stream Cursor { get; set; }
        /// <summary>
        /// 是否已激活
        /// </summary>
        bool IsActivated { get; }
        /// <summary>
        /// 地图
        /// </summary>
        IMap Map { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 地图工具模式
        /// </summary>
        MapToolMode MapToolMode { get; set; }
        /// <summary>
        /// 是否正与地图交互
        /// </summary>
        bool BusySet { get; set; }
        /// <summary>
        /// 激活
        /// </summary>
        void Activate();
        /// <summary>
        /// 冻结
        /// </summary>
        void Deactivate();
        /// <summary>
        /// 处理绘制
        /// </summary>
        /// <param name="e">参数</param>
        void DoDraw(MapEventArgs e);

    }
}
