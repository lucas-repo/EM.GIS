using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Symbology;
using EM.IOC;
using EM.WpfBases;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace EM.GIS.WPFControls.ViewModels
{
    /// <summary>
    /// 主窗体视图模型
    /// </summary>
    public class MainWindowViewModel : ViewModel<MainWindow>
    {
        /// <summary>
        /// 地图
        /// </summary>
        public IMap? Map => AppManager?.Map;
        /// <summary>
        /// 地图框架
        /// </summary>
        public IFrame? Frame => Map?.Frame;
        private IDataSetFactory? DataSetFactory=>IocManager.Default.GetService<IDataSetFactory>();
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
        public IAppManager AppManager { get; }
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
        /// 平移
        /// </summary>
        public Command PanCmd { get; }
        /// <summary>
        /// 选择
        /// </summary>
        public Command SelectCmd { get; }
        /// <summary>
        /// 缩放至全图
        /// </summary>
        public Command ZoomToMaxExtentCmd { get; }
        /// <summary>
        /// 识别
        /// </summary>
        public Command IdentifyCmd { get; }
        /// <summary>
        /// 在线地图  
        /// </summary>
        public ObservableCollection<TileMap> TileMaps { get; } = new ObservableCollection<TileMap>();
        private TileMap _tileMap;
        /// <summary>
        /// 选择的瓦片地图
        /// </summary>
        public TileMap TileMap
        {
            get { return _tileMap; }
            set { SetProperty(ref _tileMap, value); }
        }
        private string _progressStr;
        /// <summary>
        /// 进度显示文字
        /// </summary>
        public string ProgressStr
        {
            get { return _progressStr; }
            set { SetProperty(ref _progressStr, value); }
        }
        private int _progressValue;
        /// <summary>
        /// 进度值
        /// </summary>
        public int ProgressValue
        {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
        }

        public MainWindowViewModel(MainWindow t) : base(t)
        {
            var iocManager = IocManager.Default;
            var AppManager = iocManager.GetService<IAppManager>();
            if (AppManager == null)
            {
                throw new Exception($"未注册{nameof(IAppManager)}");
            }
            AppManager.Progress = ReportProgress;
            NewCmd = iocManager.GetService<ICommand,NewMapCommand>();
            OpenCmd = iocManager.GetService<ICommand, OpenMapCommand>();
            SaveCmd = iocManager.GetService<ICommand, SaveMapCommand>();
            SaveAsCmd = iocManager.GetService<ICommand, SaveMapAsCommand>();
            AddLayersCmd = iocManager.GetService<ICommand, AddLayersCommand>();
            RemoveSelectedLayersCmd = iocManager.GetService<ICommand, RemoveSelectedLayersCommand>();
            PanCmd = iocManager.GetService<ICommand, PanCommand>();
            SelectCmd = iocManager.GetService<ICommand, SelectCommand>();
            UndoCmd = iocManager.GetService<ICommand, UndoCommand>();
            RedoCmd = iocManager.GetService<ICommand, RedoCommand>();
            ZoomToMaxExtentCmd = iocManager.GetService<ICommand, ZoomToMaxExtentCommand>();
            IdentifyCmd = iocManager.GetService<ICommand, IdentifyCommand>();
            if (AppManager.Map != null)
            {
                AppManager.Map.GeoMouseMove += Map_GeoMouseMove;
                AppManager.Map.Frame.View.Progress = ReportProgress;
            }

            LoadTileMaps();
            PropertyChanged += MainWindowViewModel_PropertyChanged;
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="percent">百分比</param>
        private void ReportProgress(string text,int percent)
        {
            View.Dispatcher.BeginInvoke(() =>
            {
                ProgressStr= text;
                ProgressValue = percent;
            });
        }
        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TileMap):
                    if (Map!=null&& DataSetFactory!=null&&!Map.Frame.Children.Any(x => x.Text == TileMap.Name))
                    {
                        var driver = DataSetFactory.Drivers.FirstOrDefault(x => x is IWebMapDriver) as IWebMapDriver;
                        if (driver != null)
                        {
                            var tileSet= driver.OpenXYZ(TileMap.Name, TileMap.Url, TileMap.Servers, TileMap.MinLevel, TileMap.MaxLevel);
                            Map.Frame.Children.AddLayer(tileSet);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 读取在线地图配置
        /// </summary>
        private void LoadTileMaps()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("TileMaps.json");
            var configuration = builder.Build();
            foreach (var tileMapItem in configuration.GetChildren())
            {
                if (tileMapItem == null)
                {
                    continue;
                }
                TileMap tileMap = new TileMap()
                {
                    Name= tileMapItem.Key
                };
                TileMaps.Add(tileMap);
                foreach (var item in tileMapItem.GetChildren())
                {
                    if (string.IsNullOrEmpty(item.Value))
                    {
                        continue;
                    }
                    switch (item.Key)
                    {
                        case "Url":
                            tileMap.Url = item.Value;
                            break;
                        case "Servers":
                            tileMap.Servers = item.Value.Split(',');
                            break;
                        case "MinLevel":
                            if (int.TryParse(item.Value, out int minLevel))
                            {
                                tileMap.MinLevel = minLevel;
                            }
                            break;
                        case "MaxLevel":
                            if (int.TryParse(item.Value, out int maxLevel))
                            {
                                tileMap.MaxLevel = maxLevel;
                            }
                            break;
                        case "EPSG":
                            if (int.TryParse(item.Value, out int epsg))
                            {
                                tileMap.EPSG = epsg;
                            }
                            break;
                    }
                }
            }
        }
        private void Map_GeoMouseMove(object? sender, IGeoMouseEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                CoordStr = $"X:{e.GeographicLocation.X.ToString("F3")},Y:{e.GeographicLocation.Y.ToString("F3")}";
            });
        }
    }
}