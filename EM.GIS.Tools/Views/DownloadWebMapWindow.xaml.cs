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
using System.Windows.Shapes;

namespace EM.GIS.Tools
{
    /// <summary>
    /// DownloadWebMapWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadWebMapWindow : Window
    {
        public DownloadWebMapWindow()
        {
            InitializeComponent();
            if (downloadWebMapControl.DataContext is IReportable reportable)
            {
                reportable.ProgressAction = ReportProgress;
            }
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
