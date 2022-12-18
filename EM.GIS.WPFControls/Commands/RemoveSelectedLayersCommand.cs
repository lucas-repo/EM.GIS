using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Symbology;
using EM.IOC;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 添加图层命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class RemoveSelectedLayersCommand : Command
    {
        protected override void OnExecute(object? parameter)
        {
            if (parameter is IFrame frame )
            {
                if (frame.Children.IsAnyItemSelected())
                {
                    if (MessageBox.Show(Application.Current.MainWindow, "是否删除已选择的图层？", "删除确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        frame.Children.RemoveSelectedItems();
                    }
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return parameter is IFrame;
        }
    }
}
