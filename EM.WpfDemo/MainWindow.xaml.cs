using EM.GIS.WPFControls;
using Fluent;
using System;
using System.Windows;

namespace EM.WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private IWpfAppManager App { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App = new WpfAppManager()
            {
                Window = this,
                BaseDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            App.LoadPlugins();
        }
    }
}
