using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 命令工厂
    /// </summary>
    public class CommandFactory :NotifyClass, ICommandFactory
    {
        public ObservableCollection<IBaseCommand> Commands { get; } = new ObservableCollection<IBaseCommand>();
    }
}
