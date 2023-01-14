using EM.Bases;
using EM.GIS.Data;
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
    /// 添加图层命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class RemoveSelectedLayersCommand : ContextCommand
    {
        private IFrame? Frame { get; }
        public RemoveSelectedLayersCommand(IFrame? frame)
        {
            Frame = frame;
            Name = nameof(RemoveSelectedLayersCommand);
            Header = "移除元素";
            ToolTip = "移除选择的元素";
            Image = ResourcesHelper.GetBitmapImage("Remove16.png");
            LargeImage = ResourcesHelper.GetBitmapImage("Remove32.png");
        }
        protected override void OnExecute(object? parameter)
        {
            if (Frame!=null&&Frame.Children.IsAnyItemSelected())
            {
                if (MessageBox.Show(Application.Current.MainWindow, "是否删除已选择的图层？", "删除确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Frame.RemoveSelectedItems();
                }
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return Frame != null && Frame.Children.IsAnyItemSelected();
        }
    }
}
