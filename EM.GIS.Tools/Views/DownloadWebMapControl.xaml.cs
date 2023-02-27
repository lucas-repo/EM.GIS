using EM.GIS.Symbology;
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
    /// DownloadImageryControl.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadWebMapControl : UserControl
    {
        public DownloadWebMapControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="frame">地图框架</param>
        public void Initialize(IFrame frame)
        {
            if (DataContext == null)
            {
                DataContext = new DownloadWebMapViewModel(this, frame);
            }
        }
    }
}
