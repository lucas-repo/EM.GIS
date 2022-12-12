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
        public ICommand NewCmd { get; }
        /// <summary>
        /// 打开新地图
        /// </summary>
        public ICommand OpenCmd { get; }
        /// <summary>
        /// 保存地图
        /// </summary>
        public ICommand SaveCmd { get; }
        /// <summary>
        /// 地图另存为
        /// </summary>
        public ICommand SaveAsCmd { get; }

        /// <summary>
        /// 撤销
        /// </summary>
        public ICommand UndoCmd { get; }
        /// <summary>
        /// 重做
        /// </summary>
        public ICommand RedoCmd { get; }

        /// <summary>
        /// 添加图层
        /// </summary>
        public ICommand AddLayersCmd { get; }
        /// <summary>
        /// 移除选择的图层
        /// </summary>
        public ICommand RemoveSelectedLayersCmd { get; }

        /// <summary>
        /// 添加图层
        /// </summary>
        public ICommand PanCmd { get; }
        /// <summary>
        /// 缩放至全图
        /// </summary>
        public ICommand ZoomToMaxExtentCmd { get; }
        /// <summary>
        /// 识别
        /// </summary>
        public ICommand IdentifyCmd { get; }

        public MainWindowViewModel(MainWindow t, IWpfAppManager appManager, IIocManager iocManager) : base(t)
        {
            AppManager = appManager ?? throw new NullReferenceException(nameof(appManager));
            IocManager = iocManager ?? throw new NullReferenceException(nameof(iocManager));
            NewCmd = iocManager.GetService<NewMapCommand>();
            OpenCmd = iocManager.GetService<OpenMapCommand>();
            SaveCmd = iocManager.GetService<SaveMapCommand>();
            SaveAsCmd = iocManager.GetService<SaveMapAsCommand>();
            AddLayersCmd = iocManager.GetService<AddLayersCommand>();
            RemoveSelectedLayersCmd = iocManager.GetService<RemoveSelectedLayersCommand>();
            PanCmd = iocManager.GetService<PanCommand>();
            UndoCmd = iocManager.GetService<UndoCommand>();
            RedoCmd = iocManager.GetService<RedoCommand>();
            ZoomToMaxExtentCmd = iocManager.GetService<ZoomToMaxExtentCommand>();
            IdentifyCmd = iocManager.GetService<IdentifyCommand>();
            if (appManager?.Map != null)
            {
                appManager.Map.GeoMouseMove += Map_GeoMouseMove;
            }
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