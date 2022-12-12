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
    public class AddLayersCommand : Command
    {
        private IDriverFactory? DriverFactory { get; }
        public AddLayersCommand(IDriverFactory? driverFactory)
        {
            DriverFactory = driverFactory;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is IFrame frame && DriverFactory != null)
            {
                frame.AddLayers(DriverFactory, Application.Current.MainWindow);
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return parameter is IFrame && DriverFactory != null;
        }
    }
}
