using EM.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public class BaseCommand : DelegateCommand, IBaseCommand
    {
        public BaseCommand(Action execute) : base(execute)
        {
        }

        public BaseCommand(Action<object> execute) : base(execute)
        {
        }

        public BaseCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
        }

        public BaseCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
        {
        }

        public object Header { get; set; }
        public string Name { get; set; }
        public object ToolTip { get; set; }
        public object Icon { get; set; }
        public object LargeIcon { get; set; }
    }
}
