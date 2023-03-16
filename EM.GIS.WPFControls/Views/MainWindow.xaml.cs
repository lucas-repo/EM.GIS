using AvalonDock;
using EM.GIS.Controls;
using EM.IOC;
using System.Windows;
using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using EM.GIS.WPFControls.ViewModels;
using EM.WpfBases;
using System.Windows.Media;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private MainWindowViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var iocManager = IocManager.Default;
            var appManager = iocManager.GetService<IAppManager, IWpfAppManager>();
            if (appManager == null)
            {
                throw new Exception($"未注册{nameof(IWpfAppManager)}");
            }
            var map = iocManager.GetService<IMap, Map>();
            mapDocument.Content = map;
            appManager.Map = map;
            var legend = iocManager.GetService<ILegend, Legend>();
            legendAnchorable.Content = legend;
            appManager.Map.Legend = legend;
            appManager.Legend = legend;
            appManager.Ribbon = ribbon;
            appManager.StatusBar = statusBar;
            appManager.DockingManager = dockingManager;
            ViewModel = new MainWindowViewModel(this);

            DataContext = ViewModel;
            var plugins = iocManager.LoadPlugins();
        }
     
    }
}
