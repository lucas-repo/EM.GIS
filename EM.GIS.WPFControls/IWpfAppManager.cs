using AvalonDock;
using EM.GIS.Controls;
using Fluent;
using System.Windows;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// wpf应用管理接口
    /// </summary>
    public interface IWpfAppManager:IAppManager
    {
        /// <summary>
        /// 功能区控件
        /// </summary>
        Ribbon Ribbon { get; set; }
        /// <summary>
        /// 状态栏
        /// </summary>
        StatusBar StatusBar { get; set; }
        /// <summary>
        /// 停靠管理器
        /// </summary>
        DockingManager DockingManager { get; set; }
        /// <summary>
        /// 窗体
        /// </summary>
        Window Window { get; set; }
    }
}