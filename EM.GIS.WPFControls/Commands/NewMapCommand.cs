using EM.Bases;
using EM.GIS.Data;
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
        private IFrame? Frame { get; }
        public NewMapCommand(IFrame? frame)
        {
            Frame = frame;
        }

        /// <inheritdoc/>
        protected override void OnExecute(object? parameter)
        {
            if (Frame!=null)
            {
                Frame.New();
            }
        }
        /// <inheritdoc/>
        public override bool CanExecute(object? parameter)
        {
            return Frame != null;
        }
    }
}
