using EM.GIS.GdalExtensions;
using EM.GIS.Symbology;
using System.Windows;
using System.Windows.Controls;

namespace EM.GIS.Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IFrame frame)
        {
            InitializeComponent();
            GdalConfiguration.ConfigureOgr();
            foreach (var item in tabControl.Items)
            {
                if (item is TabItem tebItem)
                {
                    if (tebItem.Content is FrameworkElement element && element.DataContext is IReportable reportable)
                    {
                        reportable.ProgressAction = ReportProgress;
                    }
                }
            }
            downloadWebMapControl.Initialize(frame);
        }
        private void ReportProgress(string message, int percent)
        {
            Dispatcher.Invoke(() =>
            {
                progressTextBlock.Text = message;
                progressBar.Value = percent;
            });
        }
    }
}
