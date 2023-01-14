using EM.Bases;
using EM.GIS.Resources;
using EM.GIS.Symbology;
using EM.IOC;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 打开地图命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class ZoomToItemCommand : ContextCommand
    {
        private IFrame? Frame { get; }
        public ZoomToItemCommand(IFrame? frame)
        {
            Frame = frame;
            Name = nameof(ZoomToItemCommand);
            Header = "居中";
            ToolTip = "居中至选择的元素";
            Image = ResourcesHelper.GetBitmapImage("Global16.png");
            LargeImage = ResourcesHelper.GetBitmapImage("Global32.png");
        }

        /// <inheritdoc/>
        protected override void OnExecute(object? parameter)
        {
            if (Frame != null)
            {
                Frame.ZoomToSelectedItems();
            }
        }
        /// <inheritdoc/>
        public override bool CanExecute(object? parameter)
        {
            return Frame != null;
        }
    }
}
