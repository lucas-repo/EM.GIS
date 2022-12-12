using EM.Bases;
using EM.GIS.Symbology;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 新建空白地图命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class NewMapCommand : Command
    {
        public override void Execute(object? parameter)
        {
            if (parameter is IFrame frame)
            {
                frame.New();
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return parameter is IFrame;
        }
    }
}
