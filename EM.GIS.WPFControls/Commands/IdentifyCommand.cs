using EM.Bases;
using EM.GIS.Controls;
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
    /// 识别命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class IdentifyCommand : Command
    {
        public override void Execute(object? parameter)
        {
            if (parameter is IMap map)
            {
                //var panTool = map.MapTools.FirstOrDefault(x => x is MapToolPan);
                //if (panTool != null)
                //{
                //    map.ActivateMapToolWithZoom(panTool);
                //}
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return parameter is IMap;
        }
    }
}
