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
        private IFrame? Frame { get; }
        public SaveMapCommand(IFrame? frame)
        {
            Frame = frame;
        }

        protected override void OnExecute(object? parameter)
        {
            if (Frame != null)
            {
                if (string.IsNullOrEmpty(Frame.FileName))
                {
                    SaveFileDialog dg = new SaveFileDialog();
                    if (dg.ShowDialog(Application.Current.MainWindow) == true)
                    {
                        Frame.SaveAs(dg.FileName);
                    }
                }
                else
                {
                    Frame.Save();
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return Frame != null;
        }
    }
}
