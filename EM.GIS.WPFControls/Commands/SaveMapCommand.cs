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
    /// 保存地图命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class SaveMapCommand : Command
    {
        public override void Execute(object? parameter)
        {
            if (parameter is IFrame frame)
            {
                if (string.IsNullOrEmpty(frame.FileName))
                {
                    SaveFileDialog dg = new SaveFileDialog();
                    if (dg.ShowDialog(Application.Current.MainWindow) == true)
                    {
                        frame.SaveAs(dg.FileName);
                    }
                }
                else
                {
                    frame.Save();
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return parameter is IFrame;
        }
    }
}
