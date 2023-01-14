using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Symbology;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 缩放至全图命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class ZoomToMaxExtentCommand : Command
    {
        private IMap? Map { get; }
        public ZoomToMaxExtentCommand(IMap? map)
        {
            Map = map;
        }
        protected override void OnExecute(object? parameter)
        {
            if (Map?.Frame.View!=null)
            {
                Map.Frame.View.ZoomToMaxExtent();
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return Map?.Frame.View != null;
        }
    }
}
