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
    /// 添加图层命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class PanCommand : Command
    {
        private IMap? Map { get; }
        public PanCommand(IMap? map)
        {
            Map = map;
        }

        protected override void OnExecute(object? parameter)
        {
            if (Map != null)
            {
                var panTool = Map.MapTools.FirstOrDefault(x => x is MapToolPan);
                if (panTool != null)
                {
                    Map.ActivateMapToolWithZoom(panTool);
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return Map != null;
        }
    }
}
