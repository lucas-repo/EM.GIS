using AvalonDock;
using EM.GIS.Controls;
using EM.GIS.Data;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// wpf应用管理类
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IWpfAppManager))]
    public class WpfAppManager : AppManager, IWpfAppManager
    {
        private Ribbon? _ribbon;
        /// <inheritdoc/>
        public Ribbon? Ribbon
        {
            get { return _ribbon; }
            set { SetProperty(ref _ribbon, value); }
        }
        private Window? _window;
        /// <inheritdoc/>
        public Window? Window
        {
            get { return _window; }
            set { SetProperty(ref _window, value); }
        }
        private StatusBar? _statusBar;
        /// <inheritdoc/>
        public StatusBar? StatusBar
        {
            get { return _statusBar; }
            set { SetProperty(ref _statusBar, value); }
        }
        private DockingManager? _dockingManager;
        /// <inheritdoc/>
        public DockingManager? DockingManager
        {
            get { return _dockingManager; }
            set { SetProperty(ref _dockingManager, value); }
        }
        public WpfAppManager()
        { }
    }
}
