using System.Windows.Controls;

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
