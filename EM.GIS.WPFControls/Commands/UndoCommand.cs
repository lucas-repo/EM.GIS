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
    /// 撤销命令
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ICommand))]
    public class UndoCommand : Command
    {
        public override void Execute(object? parameter)
        {
           
        }
        public override bool CanExecute(object? parameter)
        {
            return true;
        }
    }
}
