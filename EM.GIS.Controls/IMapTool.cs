using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 地图工具接口
    /// </summary>
    public interface IMapTool
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
        event EventHandler<GeoMouseArgs> MouseDoubleClick;
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        event EventHandler<GeoMouseArgs> MouseDown;
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        event EventHandler<GeoMouseArgs> MouseMove;
        /// <summary>
        /// 鼠标弹起事件
        /// </summary>
        event EventHandler<GeoMouseArgs> MouseUp;
        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        event EventHandler<GeoMouseArgs> MouseWheel;
        /// <summary>
        /// 绘制事件
        /// </summary>
        event EventHandler<MapDrawArgs> Drawn;
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
        /// 激活
        /// </summary>
        void Activate();
        /// <summary>
        /// 冻结
        /// </summary>
        void Deactivate();
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
        void DoMouseDoubleClick(GeoMouseArgs e);
        /// <summary>
        /// 处理鼠标按下方法
        /// </summary>
        /// <param name="e"></param>
        void DoMouseDown(GeoMouseArgs e);
        /// <summary>
        /// 处理鼠标移动方法
        /// </summary>
        /// <param name="e"></param>
        void DoMouseMove(GeoMouseArgs e);
        /// <summary>
        /// 处理鼠标弹起方法
        /// </summary>
        /// <param name="e"></param>
        void DoMouseUp(GeoMouseArgs e);
        /// <summary>
        /// 处理鼠标滚轮
        /// </summary>
        /// <param name="e"></param>
        void DoMouseWheel(GeoMouseArgs e);
        /// <summary>
        /// 处理绘制
        /// </summary>
        /// <param name="e"></param>
        void DoDraw(MapDrawArgs e);

    }
}
