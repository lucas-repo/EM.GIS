using EM.Bases;
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
    /// 打开地图命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class OpenMapCommand : Command
    {
        public override void Execute(object? parameter)
        {
            if (parameter is IFrame frame)
            {
                OpenFileDialog dg = new OpenFileDialog();
                if (dg.ShowDialog(Application.Current.MainWindow) == true)
                {
                    frame.Open(dg.FileName);
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return parameter is IFrame;
        }
    }
}
