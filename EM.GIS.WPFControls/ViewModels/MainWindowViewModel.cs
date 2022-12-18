using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Symbology;
using EM.IOC;
using EM.WpfBases;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EM.GIS.WPFControls.ViewModels
{
    /// <summary>
    /// 主窗体视图模型
    /// </summary>
    public class MainWindowViewModel : ViewModel<MainWindow>
    {
        private IMap? Map => AppManager?.Map;
        private IFrame? Frame => Map?.Frame;
        private IDriverFactory? DriverFactory
        {
            get
            {
                var driverFactory = IocManager.GetService<IDriverFactory>();
                return driverFactory;
            }
        }
        private string _coordStr=string.Empty;
        /// <summary>
        /// 坐标字符串
        /// </summary>
        public string CoordStr
        {
            get { return _coordStr; }
            set { SetProperty(ref _coordStr, value); }
        }

        /// <summary>
        /// app管理器
        /// </summary>
        public IWpfAppManager AppManager { get; }
        /// <summary>
        /// 容器管理器
        /// </summary>
        public IIocManager IocManager { get; }
        /// <summary>
        /// 清空地图
        /// </summary>
        public Command NewCmd { get; }
        /// <summary>
        /// 打开新地图
        /// </summary>
        public Command OpenCmd { get; }
        /// <summary>
        /// 保存地图
        /// </summary>
        public Command SaveCmd { get; }
        /// <summary>
        /// 地图另存为
        /// </summary>
        public Command SaveAsCmd { get; }

        /// <summary>
        /// 撤销
        /// </summary>
        public Command UndoCmd { get; }
        /// <summary>
        /// 重做
        /// </summary>
        public Command RedoCmd { get; }

        /// <summary>
        /// 添加图层
        /// </summary>
        public Command AddLayersCmd { get; }
        /// <summary>
        /// 移除选择的图层
        /// </summary>
        public Command RemoveSelectedLayersCmd { get; }

        /// <summary>
        /// 添加图层
        /// </summary>
        public Command PanCmd { get; }
        /// <summary>
        /// 缩放至全图
        /// </summary>
        public Command ZoomToMaxExtentCmd { get; }
        /// <summary>
        /// 识别
        /// </summary>
        public Command IdentifyCmd { get; }

        public MainWindowViewModel(MainWindow t, IWpfAppManager appManager, IIocManager iocManager) : base(t)
        {
            AppManager = appManager ?? throw new NullReferenceException(nameof(appManager));
            IocManager = iocManager ?? throw new NullReferenceException(nameof(iocManager));
            NewCmd = iocManager.GetService<ICommand,NewMapCommand>();
            OpenCmd = iocManager.GetService<ICommand, OpenMapCommand>();
            SaveCmd = iocManager.GetService<ICommand, SaveMapCommand>();
            SaveAsCmd = iocManager.GetService<ICommand, SaveMapAsCommand>();
            AddLayersCmd = iocManager.GetService<ICommand, AddLayersCommand>();
            RemoveSelectedLayersCmd = iocManager.GetService<ICommand, RemoveSelectedLayersCommand>();
            PanCmd = iocManager.GetService<ICommand, PanCommand>();
            UndoCmd = iocManager.GetService<ICommand, UndoCommand>();
            RedoCmd = iocManager.GetService<ICommand, RedoCommand>();
            ZoomToMaxExtentCmd = iocManager.GetService<ICommand, ZoomToMaxExtentCommand>();
            IdentifyCmd = iocManager.GetService<ICommand, IdentifyCommand>();
            if (appManager?.Map != null)
            {
                appManager.Map.GeoMouseMove += Map_GeoMouseMove;
            }
            Command[] commands0 = {NewCmd,OpenCmd,SaveCmd,SaveAsCmd,AddLayersCmd,RemoveSelectedLayersCmd,PanCmd,UndoCmd, RedoCmd, ZoomToMaxExtentCmd,IdentifyCmd};
            //foreach (var item in commands0)
            //{
            //    //item.CanExecuteFunc = (obj) => Map?.Frame != null;
            //    item.RaiseCanExecuteChanged();
            //}
        }

        private void Map_GeoMouseMove(object? sender, IGeoMouseEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(() =>
            {
                CoordStr = $"X:{Math.Round(e.GeographicLocation.X, 3)},Y:{Math.Round(e.GeographicLocation.Y, 3)}";
            });
        }
    }
}