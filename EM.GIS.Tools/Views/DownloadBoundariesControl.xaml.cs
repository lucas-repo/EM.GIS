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
    /// 行政界线下载控件
    /// </summary>
    public partial class DownloadBoundariesControl : UserControl
    {
        public DownloadBoundariesControl()
        {
            InitializeComponent();
            DataContext=new DownloadBoundariesViewModel(this);
        }
    }
}
