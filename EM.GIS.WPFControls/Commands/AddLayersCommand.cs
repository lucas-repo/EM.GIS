using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Resources;
using EM.GIS.Symbology;
using EM.IOC;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 添加图层命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class AddLayersCommand : ContextCommand
    {
        private IDriverFactory? DriverFactory { get; }
        private IFrame? Frame { get; }
        public AddLayersCommand(IDriverFactory? driverFactory,IFrame? frame)
        {
            DriverFactory = driverFactory;
            Frame = frame;
            Name = nameof(AddLayersCommand);
            Header = "添加图层";
            ToolTip = "添加图层至当前分组下";
            Image = ResourcesHelper.GetBitmapImage("Add16.png");
            LargeImage = ResourcesHelper.GetBitmapImage("Add32.png");
        }

        protected override void OnExecute(object? parameter)
        {
            if (Frame != null && DriverFactory != null)
            {
                Frame.AddLayers(DriverFactory, Application.Current.MainWindow);
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return Frame != null && DriverFactory != null;
        }
    }
}
