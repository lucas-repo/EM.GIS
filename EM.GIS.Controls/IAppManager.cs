using EM.GIS.Data;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EM.GIS.Controls
{
    /// <summary>
    /// app管理接口
    /// </summary>
    public interface IAppManager:INotifyPropertyChanged
    {
        /// <summary>
        /// 进度处理
        /// </summary>
        Action<string, int> Progress { get; set; }
        /// <summary>
        /// 地图
        /// </summary>
        IMap Map { get; set; }
        /// <summary>
        /// 图例
        /// </summary>
        ILegend Legend { get; set; }
        /// <summary>
        /// 命令工厂
        /// </summary>
        ICommandFactory CommandFactory { get; }
    }
}
