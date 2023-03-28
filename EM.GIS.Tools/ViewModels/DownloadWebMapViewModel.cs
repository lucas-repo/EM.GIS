using BruTile;
using BruTile.Cache;
using EM.Bases;
using EM.GIS.Data;
using EM.GIS.GdalExtensions;
using EM.GIS.Gdals;
using EM.GIS.Geometries;
using EM.GIS.Symbology;
using EM.IOC;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 下载在线地图视图模型
    /// </summary>
    public class DownloadWebMapViewModel : ReportableViewModel<DownloadWebMapControl>
    {
        private readonly string _wkt;

        /// <summary>
        /// 瓦片数据集集合
        /// </summary>
        public ObservableCollection<ITileSet> TileSets { get; }
        private ITileSet? tileSet;
        /// <summary>
        /// 瓦片数据集
        /// </summary>
        public ITileSet? TileSet
        {
            get { return tileSet; }
            set { SetProperty(ref tileSet, value); }
        }
        private IGeometry? geometry;
        /// <summary>
        /// 下载在线底图的几何体
        /// </summary>
        public IGeometry? Geometry
        {
            get { return geometry; }
            set { SetProperty(ref geometry, value); }
        }

        /// <summary>
        /// 输出类型集合
        /// </summary>
        public Dictionary<string, string> OutTypes { get; }
        private KeyValuePair<string, string> outType;
        /// <summary>
        /// 输出类型
        /// </summary>
        public KeyValuePair<string, string> OutType
        {
            get { return outType; }
            set { SetProperty(ref outType, value); }
        }
        /// <summary>
        /// 瓦片格式集合
        /// </summary>
        public Dictionary<string, string> TileFormats { get; }
        private KeyValuePair<string, string> tileFormat;
        /// <summary>
        /// 瓦片格式
        /// </summary>
        public KeyValuePair<string, string> TileFormat
        {
            get { return tileFormat; }
            set { SetProperty(ref tileFormat, value); }
        }
        /// <summary>
        /// 范围类型集合
        /// </summary>
        public ObservableCollection<BoundType> BoundTypes { get; }
        private BoundType boundType;
        /// <summary>
        /// 范围类型
        /// </summary>
        public BoundType BoundType
        {
            get { return boundType; }
            set { SetProperty(ref boundType, value); }
        }
        private string selectedFeaturesStr;
        /// <summary>
        /// 已选择的要素字符串
        /// </summary>
        public string SelectedFeaturesStr
        {
            get { return selectedFeaturesStr; }
            set { SetProperty(ref selectedFeaturesStr, value); }
        }
        /// <summary>
        /// 省集合
        /// </summary>
        public ObservableCollection<CityInfo> Provinces { get; }
        private CityInfo province;
        /// <summary>
        /// 省
        /// </summary>
        public CityInfo Province
        {
            get { return province; }
            set { SetProperty(ref province, value); }
        }
        /// <summary>
        /// 市集合
        /// </summary>
        public ObservableCollection<CityInfo> Cities { get; }
        private CityInfo city;
        /// <summary>
        /// 市
        /// </summary>
        public CityInfo City
        {
            get { return city; }
            set { SetProperty(ref city, value); }
        }
        /// <summary>
        /// 县集合
        /// </summary>
        public ObservableCollection<CityInfo> Counties { get; }
        private CityInfo county;
        /// <summary>
        /// 县
        /// </summary>
        public CityInfo County
        {
            get { return county; }
            set { SetProperty(ref county, value); }
        }

        /// <summary>
        /// 级别
        /// </summary>
        public ObservableCollection<SelectableItem<int>> Levels { get; }

        private string outPath;
        /// <summary>
        /// 输出路径
        /// 下载瓦片时为文件夹，否则为保存文件名称
        /// </summary>
        public string OutPath
        {
            get { return outPath; }
            set { SetProperty(ref outPath, value); }
        }
        /// <summary>
        /// 全选
        /// </summary>
        public bool IsAllSelected
        {
            get
            {
                bool ret = true;
                foreach (var item in Levels)
                {
                    if (!item.IsSelected)
                    {
                        ret = false;
                        break;
                    }
                }
                return ret;
            }
            set
            {
                foreach (var item in Levels)
                {
                    if (item.IsSelected != value)
                    {
                        item.IsSelected = value;
                    }
                }
            }
        }

        /// <summary>
        /// 下载或取消
        /// </summary>
        public DelegateCommand StartOrCancelCmd { get; }
        /// <summary>
        /// 浏览命令
        /// </summary>
        public DelegateCommand BrowseCmd { get; }

        private object _lockObj = new object();
        /// <summary>
        /// 空白地区信息
        /// </summary>
        private readonly CityInfo BlankCityInfo = new CityInfo(0, "");
        private int _progressValue;
        /// <summary>
        /// 进度值
        /// </summary>
        public int ProgressValue
        {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
        }
        private string _progressText;
        /// <summary>
        /// 进度文字
        /// </summary>
        public string ProgressText
        {
            get { return _progressText; }
            set { SetProperty(ref _progressText, value); }
        }

        public DownloadWebMapViewModel(DownloadWebMapControl t, IGeometry? dounloadExtent = null) : base(t)
        {
            var frame = IocManager.Default.GetService<IFrame>();
            Frame = frame ?? throw new Exception($"未注册 {nameof(IFrame)}");
            TileSets = new ObservableCollection<ITileSet>();
            ProgressAction = ReportProgress;
            if (dounloadExtent == null)
            {
                var featureLayers = frame.Children.GetAllFeatureLayers();
                foreach (var item in featureLayers)
                {
                    if (item.GetVisible())
                    {
                        geometry = item.Selection.FirstOrDefault()?.Geometry;
                        if (geometry != null)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                geometry = dounloadExtent;
            }
            var tileLayers = frame.Children.GetAllItems<ITileLayer>();
            foreach (var tileLayer in tileLayers)
            {
                if (tileLayer.DataSet != null)
                {
                    TileSets.Add(tileLayer.DataSet);
                }
            }
            _wkt = @"PROJCS[\”WGS_1984_Web_Mercator_Auxiliary_Sphere\”,GEOGCS[\”GCS_WGS_1984\”,DATUM[\”D_WGS_1984\”,SPHEROID[\”WGS_1984\”,6378137.0,298.257223563]],PRIMEM[\”Greenwich\”,0.0],UNIT[\”Degree\”,0.0174532925199433]],PROJECTION[\”Mercator_Auxiliary_Sphere\”],PARAMETER[\”False_Easting\”,0.0],PARAMETER[\”False_Northing\”,0.0],PARAMETER[\”Central_Meridian\”,0.0],PARAMETER[\”Standard_Parallel_1\”,0.0],PARAMETER[\”Auxiliary_Sphere_Type\”,0.0],UNIT[\”Meter\”,1.0],AUTHORITY[\”EPSG\”,3857]]";
            OutTypes = new Dictionary<string, string>()
            {
                ["拼接（*.tif）"] = ".tif",
                ["瓦片（谷歌）"] = string.Empty,
                ["瓦片库（*.mbtiles）"] = ".mbtiles"
            };
            TileFormats = new Dictionary<string, string>()
            {
                ["JPEG（*.jpg）"] = ".jpg",
                ["PNG（*.png）"] = ".png"
            };
            TileFormat = TileFormats.FirstOrDefault();
            BoundTypes = new ObservableCollection<BoundType>();
            BoundTypes.AddEnums();

            Provinces = new ObservableCollection<CityInfo>(/*BoundaryHelper.GetCityInfos()*/);
            Provinces.Insert(0, BlankCityInfo);
            Cities = new ObservableCollection<CityInfo>();
            Counties = new ObservableCollection<CityInfo>();
            Levels = new ObservableCollection<SelectableItem<int>>();
            StartOrCancelCmd = new DelegateCommand(StartOrCancel, CanDownload);
            BrowseCmd = new DelegateCommand(Browse);

            PropertyChanged += DownloadWebMapViewModel_PropertyChanged;
            if (OutTypes.Count > 0)
            {
                OutType = OutTypes.FirstOrDefault();
            }
            if (TileSets.Count > 0)
            {
                TileSet = TileSets.First();
            }
            boundType = BoundTypes.FirstOrDefault();
            OnPropertyChanged(nameof(BoundType));
            if (Provinces.Count > 0)
            {
                Province = Provinces.First();
            }
        }

        private void ReportProgress(string arg1, int arg2)
        {
            View.Dispatcher.Invoke(() =>
            {
                ProgressText = arg1;
                ProgressValue = arg2;
            });
        }

        private void Browse()
        {
            switch (OutType.Value)
            {
                case "":
                    var commonOpenFileDialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = true
                    };
                    if (commonOpenFileDialog.ShowDialog(Window.GetWindow(View)) == CommonFileDialogResult.Ok)
                    {
                        OutPath = commonOpenFileDialog.FileName;
                    }
                    break;
                default:
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        InitialDirectory = Path.GetDirectoryName(OutPath),
                        Filter = $"{OutType.Key}|*{OutType.Value}"
                    };
                    if (saveFileDialog.ShowDialog(Window.GetWindow(View)) == true)
                    {
                        OutPath = saveFileDialog.FileName;
                    }
                    break;
            }
        }
        private void ResetCollection(ObservableCollection<CityInfo> cityInfos, CityInfo parent)
        {
            cityInfos.Clear();
            if (parent != null)
            {
                cityInfos.Add(BlankCityInfo);
                foreach (var item in parent.Children)
                {
                    if (item is CityInfo cityInfo)
                    {
                        cityInfos.Add(cityInfo);
                    }
                }
            }
        }
        private IFrame Frame { get; }
        private IEnumerable<IGeometry> GetSelectedPolygons()
        {
            if (Frame != null)
            {
                foreach (var item in Frame.Children)
                {
                    if (item is IFeatureLayer layer && layer.GetVisible() && layer.DataSet?.FeatureType == FeatureType.Polygon && layer.Selection.Count > 0)
                    {
                        foreach (var feature in layer.Selection)
                        {
                            yield return feature.Geometry;
                        }
                    }
                }
            }
        }
        private void DownloadWebMapViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TileSet):
                    Levels.Clear();
                    if (TileSet != null)
                    {
                        var minLevel = TileSet.TileSource.Schema.Resolutions.Min(x => x.Key);
                        var maxLevel = TileSet.TileSource.Schema.Resolutions.Max(x => x.Key);
                        for (int i = minLevel; i <= maxLevel; i++)
                        {
                            Levels.Add(new SelectableItem<int>() { Item = i, Text = $"第{i}级", IsSelected = true });
                        }
                        StartOrCancelCmd.RaiseCanExecuteChanged();
                    }
                    break;
                case nameof(BoundType):
                    switch (BoundType)
                    {
                        case BoundType.SelectedFeatures:
                            SelectedFeaturesStr = $"已选择 {GetSelectedPolygons().Count()} 个面";
                            break;
                        case BoundType.China:
                            if (Province == null)
                            {
                                var firstProvince = Provinces.FirstOrDefault();
                                if (firstProvince != null)
                                {
                                    Province = firstProvince;
                                }
                            }
                            break;
                    }
                    break;
                case nameof(Province):
                    ResetCollection(Cities, Province);
                    var firstCity = Cities.FirstOrDefault();
                    if (firstCity != null)
                    {
                        City = firstCity;
                    }
                    break;
                case nameof(City):
                    ResetCollection(Counties, City);
                    var firstCounty = Counties.FirstOrDefault();
                    if (firstCounty != null)
                    {
                        County = firstCounty;
                    }
                    break;
                case nameof(OutPath):
                    StartOrCancelCmd.RaiseCanExecuteChanged();
                    break;
                case nameof(OutType):
                    if (string.IsNullOrEmpty(OutPath))
                    {
                        switch (OutType.Value)
                        {
                            case "":
                                OutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads");
                                break;
                            case ".tif":
                                if (TileSet != null)
                                {
                                    OutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"downloads\{TileSet.Name}.tif");
                                }
                                else
                                {
                                    OutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"downloads\下载.tif");
                                }
                                break;
                        }
                    }
                    else
                    {
                        string? directory = null;
                        string name;
                        if (Directory.Exists(OutPath))
                        {
                            directory = OutPath;
                            if (TileSet != null)
                            {
                                name = TileSet.Name;
                            }
                            else
                            {
                                name = "下载";
                            }
                        }
                        else
                        {
                            directory = Path.GetDirectoryName(OutPath);
                            name = Path.GetFileNameWithoutExtension(OutPath);
                        }
                        if (directory != null)
                        {
                            switch (OutType.Value)
                            {
                                case "":
                                    OutPath = directory;
                                    break;
                                default:
                                    OutPath = Path.Combine(directory, $"{name}{OutType.Value}");
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        private bool CanDownload()
        {
            if (TileSet == null)
            {
                return false;
            }
            if (Geometry == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(OutPath))
            {
                return false;
            }
            return true;
        }

        EmHttpTileSource? HttpTileSource => TileSet?.TileSource as EmHttpTileSource;
        private void DownloadMbtiles(IRasterDriver rasterDriver, Dictionary<int, List<TileInfo>> levelAndTileInfos)
        {
            int tileWidth = 256, tileHeight = 256;
            int bandCount = 3;
            int[] bandMap = { 3, 2, 1 };

            TileFormat tileFormat;
            switch (TileFormat.Value)
            {
                case ".jpg":
                    tileFormat = GdalExtensions.TileFormat.JPEG;
                    break;
                case ".png":
                    tileFormat = GdalExtensions.TileFormat.PNG;
                    break;
                default:
                    throw new NotImplementedException();
            }
            var options = DatasetExtensions.GetMBTilesOptions(tileFormat: tileFormat);
            var fileCache = GetFileCache();
            Action<string, int> downloadProgressAction = (txt, progress) => ProgressAction?.Invoke(txt, progress / 2);
            Action<string, int> writeProgressAction = (txt, progress) => ProgressAction?.Invoke(txt, 50 + progress / 2);
            var maxLevelAndTileInfos = levelAndTileInfos.Last();
            var sortedLevelAndTileInfos = levelAndTileInfos.OrderByDescending(x => x.Key);
            var level = maxLevelAndTileInfos.Key;
            var tileInfos = maxLevelAndTileInfos.Value;
            if (tileInfos.Count == 0)
            {
                return;
            }
            DownloadTiles(maxLevelAndTileInfos, fileCache, downloadProgressAction);

            var minCol = tileInfos.Min(x => x.Index.Col);
            var minRow = tileInfos.Min(x => x.Index.Row);
            var maxCol = tileInfos.Max(x => x.Index.Col);
            var maxRow = tileInfos.Max(x => x.Index.Row);
            var minX = tileInfos.Min(x => x.Extent.MinX);
            var minY = tileInfos.Min(x => x.Extent.MinY);
            var maxX = tileInfos.Max(x => x.Extent.MaxX);
            var maxY = tileInfos.Max(x => x.Extent.MaxY);
            int width = tileWidth * (maxCol - minCol + 1);
            int height = tileHeight * (maxRow - minRow + 1);
            var rasterSet = CreateRasterset(rasterDriver, OutPath, minX, minY, maxX, maxY, width, height, bandCount, options);
            if (rasterSet != null)
            {
                WriteTileToRasterSet(fileCache, tileWidth, tileHeight, bandCount, bandMap, level, tileInfos, minCol, minRow, rasterSet, writeProgressAction);
                ProgressAction?.Invoke($"创建金字塔中", 99);
                rasterSet.BuildOverviews(256, 256);
            }
        }

        private void WriteTileToRasterSet(FileCache fileCache, int tileWidth, int tileHeight, int bandCount, int[] bandMap, int level, List<TileInfo> tileInfos, int minCol, int minRow, IRasterSet rasterSet, Action<string, int>? progressAction)
        {
            if (HttpTileSource == null)
            {
                return;
            }
            int totalCount = tileInfos.Count();
            int successCount = 0;
            ParallelOptions options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1 /*Environment.ProcessorCount*/
            };
            Parallel.ForEach(tileInfos, options, (tileInfo) =>
            {
                if (Cancellation.IsCancellationRequested)
                {
                    return;
                }
                var filename = fileCache.GetFileName(tileInfo.Index);
                if (!File.Exists(filename))
                {
                    try
                    {
                        var task = HttpTileSource.GetTileAsync(tileInfo);
                        task.ConfigureAwait(false);
                        task.Wait();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    if (!File.Exists(filename))
                    {
                        return;
                    }
                }
                var xOff = (tileInfo.Index.Col - minCol) * tileWidth;
                var yOff = (tileInfo.Index.Row - minRow) * tileHeight;
                try
                {
                    lock (_lockObj)
                    {
                        RasterArgs readArgs = new RasterArgs(0, 0, tileWidth, tileHeight, tileWidth, tileHeight, bandCount, bandMap);
                        RasterArgs writeArgs = new RasterArgs(xOff, yOff, tileWidth, tileHeight, tileWidth, tileHeight, bandCount, bandMap);
                        rasterSet.WriteRaster(filename, readArgs, writeArgs);
                        successCount++;
                    }
                    progressAction?.Invoke($"第{level}级写入缓存中", successCount * 100 / totalCount);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}下载失败：{e}");
                }
            });

            progressAction?.Invoke($"第{level}级写入缓存中", 99);
            rasterSet.Save();
        }

        private IRasterSet? CreateRasterset(IRasterDriver rasterDriver, string path, double minX, double minY, double maxX, double maxY, int width, int height, int bandCount = 3, string[]? options = null)
        {
            IRasterSet? ret = null;
            if (TileSet == null || width == 0 || height == 0 || bandCount == 0)
            {
                return ret;
            }

            if (width == 0 || height == 0)
            {
                return ret;
            }
            var directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
            {
                return ret;
            }
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            DriverExtensions.DeleteDataSource(path);
            ret = rasterDriver.Create(path, width, height, bandCount, RasterType.Byte, options);
            if (ret != null)
            {
                ret.Projection = TileSet.Projection.Copy();
                double destXResolution = (maxX - minX) / width;
                double destYResolution = (maxY - minY) / height;
                double[] affine = { minX, destXResolution, 0, maxY, 0, -destYResolution };
                ret.SetGeoTransform(affine);
            }
            return ret;
        }

        private CancellationTokenSource _cancellation;
        private CancellationTokenSource Cancellation
        {
            get { return _cancellation; }
            set
            {
                if (_cancellation != value)
                {
                    if (_cancellation != null)
                    {
                        _cancellation.Dispose();
                    }
                    _cancellation = value;
                }
            }
        }
        private HttpClient? _client;
        /// <summary>
        /// Http客户端
        /// </summary>
        public HttpClient HttpClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient()
                    {
                        Timeout = new TimeSpan(0, 3, 0)
                    };
                    _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");
                }
                return _client;
            }
        }
        public override void Cancel()
        {
            if (IsFree || Cancellation.IsCancellationRequested)
            {
                return;
            }
            Cancellation.Cancel();
        }
        private void ShowMessage(string text)
        {
            MessageBox.Show(Window.GetWindow(View), text);
        }

        private void StartOrCancel()
        {
            if (!IsFree)
            {
                if (MessageBox.Show(Window.GetWindow(View), $"是否取消", "取消确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Cancel();
                }
                return;
            }

            if (TileSet == null)
            {
                ShowMessage("请选择地图");
                return;
            }
            if (Geometry == null)
            {
                ShowMessage("请选择下载范围");
                return;
            }
            if (string.IsNullOrEmpty(OutPath))
            {
                ShowMessage("路径不能为空");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            IsFree = false;
            Cancellation = new CancellationTokenSource();
            Task task = Task.Run(() =>
            {
#if !DEBUG
                try
                {
#endif
                var featureLayers = Frame.Children.GetAllFeatureLayers().Where(x => x.IsVisible && x.Selection.Count > 0);

                string tileDirectory;
                Downloader? downloader=null;
                switch (OutType.Value)
                {
                    case ".tif":
                        var directory = Path.GetDirectoryName(OutPath);
                        if (directory != null)
                        {
                            tileDirectory= Path.Combine(directory, "Tiles");
                            downloader = new SpliceDownloader(TileSet, tileDirectory, TileFormat.Value, OutPath);
                        }
                        break;
                    case "":
                        tileDirectory = Path.Combine(OutPath, "Tiles");
                        downloader = new GoogleTileDownloader(TileSet, tileDirectory, TileFormat.Value);
                        break;
                    //case ".mbtiles":
                    //    downloadAction = (levelAndTileInfos) => DownloadMbtiles(rasterDriver, levelAndTileInfos);
                    //    break;
                    default:
                        throw new NotImplementedException();
                }
                if (downloader == null)
                {
                    throw new Exception();
                }
                var levelAndTileInfos = new Dictionary<int, List<TileInfo>>();
                foreach (var level in Levels)
                {
                    if (Cancellation.IsCancellationRequested)
                    {
                        break;
                    }
                    if (!level.IsSelected)
                    {
                        continue;
                    }
                    List<TileInfo> tileInfos = new List<TileInfo>();
                    foreach (var featureLayer in featureLayers)
                    {
                        if (featureLayer.DataSet == null)
                        {
                            continue;
                        }
                        IEnumerable<IGeometry> geometries = featureLayer.Selection.Select(x => x.Geometry);
                        if (!Equals(featureLayer.DataSet.Projection, TileSet.Projection))
                        {
                            geometries = featureLayer.Selection.Select(x => x.Geometry.Copy()).ToList();
                            foreach (var geometry in geometries)
                            {
                                featureLayer.DataSet.Projection.ReProject(TileSet.Projection, geometry);
                            }
                        }
                        foreach (var geometry in geometries)
                        {
                            tileInfos.AddRange(TileSet.GetTileInfos(level.Item, geometry));
                        }
                    }
                    levelAndTileInfos[level.Item] = tileInfos;
                }
                Cancellation = new CancellationTokenSource();
                downloader.Download(levelAndTileInfos, Cancellation.Token);
#if !DEBUG
                 }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
#endif
            });
            task.ContinueWith(t =>
            {
                IsFree = true;
                sw.Stop();
                TimeSpan timeSpan = new TimeSpan(sw.ElapsedMilliseconds * 10000);
                View.Dispatcher.Invoke(() =>
                {
                    var window = Window.GetWindow(View);
                    MessageBox.Show(window, $"耗时{timeSpan}", "下载完成");
                });
                ProgressAction?.Invoke(string.Empty, 0);
            });
        }
        private FileCache GetFileCache()
        {
            string destDirectory;
            if (string.IsNullOrEmpty(OutType.Value))
            {
                destDirectory = Path.Combine(OutPath, "Tiles");
            }
            else
            {
                var dir = Path.GetDirectoryName(OutPath);
                if (string.IsNullOrEmpty(dir))
                {
                    throw new Exception($"输出目录有误：{OutPath}");
                }
                destDirectory = Path.Combine(dir, "Tiles");
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            var fileCache = new FileCache(destDirectory, TileFormat.Value.Replace(".", ""));
            return fileCache;
        }
        private void DownloadTiles(KeyValuePair<int, List<TileInfo>> tileInfos, FileCache fileCache, Action<string, int>? progressAction)
        {
            if (TileSet == null)
            {
                return;
            }
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = Cancellation.Token
            };
            var totalCount = tileInfos.Value.Count;
            var level = tileInfos.Key;
            var successCount = 0;
            if (HttpTileSource != null)
            {
                Parallel.ForEach(tileInfos.Value, parallelOptions, (tileInfo) =>
                {
                    var uri = HttpTileSource.Request.GetUri(tileInfo);
                    if (uri != null)
                    {
                        try
                        {
                            var bytes = fileCache.Find(tileInfo.Index);
                            if (bytes == null)
                            {
                                var task = HttpClient.GetByteArrayAsync(uri);
                                task.ConfigureAwait(false);
                                bytes = task.Result;
                                if (bytes != null)
                                {
                                    fileCache.Add(tileInfo.Index, bytes);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"下载瓦片失败{uri}，{e}");
                        }
                    }
                    lock (_lockObj)
                    {
                        successCount++;
                        progressAction?.Invoke($"第{level}级下载中", successCount * 100 / totalCount);
                    }
                });
            }
        }
    }
}
