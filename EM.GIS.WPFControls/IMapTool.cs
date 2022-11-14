using EM.GIS.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 地图工具接口
    /// </summary>
    public interface IMapTool:ITool
    {
        /// <summary>
        /// 按键按下事件
        /// </summary>
        event EventHandler<KeyEventArgs> KeyDown;
        /// <summary>
        /// 按键弹起事件
        /// </summary>
        event EventHandler<KeyEventArgs> KeyUp;
        /// <summary>
        /// 鼠标双击事件
        /// </summary>
        event EventHandler<GeoMouseButtonEventArgs> MouseDoubleClick;
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        event EventHandler<GeoMouseButtonEventArgs> MouseDown;
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        event EventHandler<GeoMouseEventArgs> MouseMove;
        /// <summary>
        /// 鼠标弹起事件
        /// </summary>
        event EventHandler<GeoMouseButtonEventArgs> MouseUp;
        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        event EventHandler<GeoMouseWheelEventArgs> MouseWheel;
        /// <summary>
        /// 处理键盘按下方法
        /// </summary>
        /// <param name="e"></param>
        void DoKeyDown(KeyEventArgs e);
        /// <summary>
        /// 处理键盘弹起方法
        /// </summary>
        /// <param name="e"></param>
        void DoKeyUp(KeyEventArgs e);
        /// <summary>
        /// 处理鼠标双击方法
        /// </summary>
        /// <param name="e"></param>
        void DoMouseDoubleClick(GeoMouseButtonEventArgs e);
        /// <summary>
        /// 处理鼠标按下方法
        /// </summary>
        /// <param name="e"></param>
        void DoMouseDown(GeoMouseButtonEventArgs e);
        /// <summary>
        /// 处理鼠标移动方法
        /// </summary>
        /// <param name="e"></param>
        void DoMouseMove(GeoMouseEventArgs e);
        /// <summary>
        /// 处理鼠标弹起方法
        /// </summary>
        /// <param name="e"></param>
        void DoMouseUp(GeoMouseButtonEventArgs e);
        /// <summary>
        /// 处理鼠标滚轮
        /// </summary>
        /// <param name="e"></param>
        void DoMouseWheel(GeoMouseWheelEventArgs e);
    }
}
