using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EM.GIS.Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
        }

        private void ReportProgress(string message, int percent)
        {
            Dispatcher.Invoke(() =>
            {
                progressTextBlock.Text=message;
                progressBar.Value=percent;
            });
        }
    }
}
