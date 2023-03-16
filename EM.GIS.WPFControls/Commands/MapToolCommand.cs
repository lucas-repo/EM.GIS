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
    /// 地图工具命令
    /// </summary>
    public abstract class MapToolCommand<T> : Command where T :  ITool
    {
        public IMap? Map { get; }
        public MapToolCommand(IMap? map)
        {
            Map = map;
        }

        protected override void OnExecute(object? parameter)
        {
            if (Map != null)
            {
                var tool = Map.MapTools.FirstOrDefault(x => x is T);
                if (tool != null)
                {
                    Map.ActivateMapToolWithZoom(tool);
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return Map != null;
        }
    }
}
