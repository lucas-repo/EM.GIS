using AvalonDock;
using EM.GIS.Controls;
using EM.IOC;
using System.Windows;
using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using EM.GIS.WPFControls.ViewModels;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private MainWindowViewModel ViewModel { get; }
        public MainWindow(IWpfAppManager appManager, IIocManager iocManager)
        {
            InitializeComponent();
            if (appManager == null)
            {
                throw new ArgumentNullException(nameof(appManager));
            }
            Loaded += MainWindow_Loaded;
            appManager.Map = map;
            appManager.Legend = legend;
            appManager.Progress = ReportProgress;
            appManager.Ribbon = ribbon;
            appManager.StatusBar = statusBar;
            appManager.DockingManager = dockingManager;
            DataContext = new MainWindowViewModel(this, appManager, iocManager);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IocManager != null)
            {
                ViewModel.IocManager.LoadPlugins();
            }
        }
        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="percent">百分比</param>
        /// <param name="text">文本</param>
        private void ReportProgress(int percent, string text)
        {
            Dispatcher.BeginInvoke(() =>
            {
                statusBarItem.Content = text;
                progressBar.Value=percent;
            });
        }
    }
}
