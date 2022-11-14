using AvalonDock;
using EM.GIS.Controls;
using Fluent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// wpf应用管理类
    /// </summary>
    public class WpfAppManager : AppManager, IWpfAppManager
    {
        public Ribbon Ribbon { get; set; }
        public Window Window { get; set; }
        public StatusBar StatusBar { get; set; }
        public DockingManager DockingManager { get; set; }
    }
}
