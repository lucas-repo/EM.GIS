using System;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 地图工具模式
    /// </summary>
    [Flags]
    public enum MapToolMode
    {
        /// <summary>
        /// 无
        /// </summary>
        None=0,
        /// <summary>
        /// 左键
        /// </summary>
        LeftButton=1,
        /// <summary>
        /// 右键
        /// </summary>
        RightButton =2,
        /// <summary>
        /// 中键
        /// </summary>
        Middle = 4,
        /// <summary>
        /// 键盘
        /// </summary>
        Keyboard = 8,
        /// <summary>
        /// 始终启动
        /// </summary>
        AlwaysOn = 16
    }
}