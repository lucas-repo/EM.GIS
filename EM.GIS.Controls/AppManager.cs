using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Gdals;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Transactions;

namespace EM.GIS.Controls
{
    /// <summary>
    /// app管理类
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IAppManager))]
    public class AppManager : NotifyClass, IAppManager
    {
        private IMap _map;
        public IMap Map
        {
            get { return _map; }
            set { SetProperty(ref _map, value); }
        }
        private ILegend _legend;
        public ILegend Legend
        {
            get { return _legend; }
            set { SetProperty(ref _legend, value); }
        }
        private Action<string, int> _progress;
        public Action<string, int> Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        public ICommandFactory CommandFactory { get; }

        public AppManager(IMap map, ILegend legend, Action<string, int> progress) : this()
        {
            Map = map;
            Legend = legend;
            Progress = progress;
        }
        public AppManager()
        {
            CommandFactory = new CommandFactory();
        }
    }
}
