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
        public object Header { get; set; }
        public string Name { get; set; }
        public object ToolTip { get; set; }
        public object Icon { get; set; }
        public object LargeIcon { get; set; }
    }
}
