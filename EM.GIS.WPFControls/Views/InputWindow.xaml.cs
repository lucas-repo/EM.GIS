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

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        /// <summary>
        /// 输入的值
        /// </summary>
        public string Value
        {
            get=>textBox.Text; 
            set=>textBox.Text = value;
        }
        public InputWindow(string title="请输入值",string value="")
        {
            InitializeComponent();
            confirmBtn.Click += ConfirmBtn_Click;
            Title = title;
            Value = value;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Value))
            {
                MessageBox.Show(this, "输入不能为空！","错误",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            DialogResult = true;
        }
    }
}
