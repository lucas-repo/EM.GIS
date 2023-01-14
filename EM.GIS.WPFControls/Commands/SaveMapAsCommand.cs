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
    /// 地图另存为命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class SaveMapAsCommand : Command
    {
        private IFrame? Frame { get; }
        public SaveMapAsCommand(IFrame? frame)
        {
            Frame = frame;
        }
        protected override void OnExecute(object? parameter)
        {
            if (Frame!=null)
            {
                SaveFileDialog dg = new SaveFileDialog();
                if (dg.ShowDialog(Application.Current.MainWindow) == true)
                {
                    Frame.SaveAs(dg.FileName);
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return Frame != null;
        }
    }
}
