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
                    if (item.IsSelected!=value)
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

        public DownloadWebMapViewModel(DownloadWebMapControl t, IGeometry? dounloadExtent = null) : base(t)
        {
            var frame = IocManager.Default.GetService<IFrame>();
            Frame = frame ?? throw new Exception($"未注册 {nameof(IFrame)}");
            TileSets = new ObservableCollection<ITileSet>();
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

            Provinces = new ObservableCollection<CityInfo>(BoundaryHelper.GetCityInfos());
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

        private void Browse()
        {
            switch (OutType.Value)
            {
                case "":
                    CommonSaveFileDialog commonSaveFileDialog = new CommonSaveFileDialog
                    {
                        RestoreDirectory = true
                    };
                    if (commonSaveFileDialog.ShowDialog(Window.GetWindow(View)) == CommonFileDialogResult.Ok)
                    {
                        OutPath = commonSaveFileDialog.FileName;
                    }
                    break;
                default:
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Filter=$"{OutType.Key}|*{OutType.Value}"
                    };
                    if (saveFileDialog.ShowDialog(Window.GetWindow(View))==true)
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
                                OutPath =Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"downloads");
                                break;
                            case ".tif":
                                OutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads/测试.tif");
                                break;
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

        private void DownloadTiles(Dictionary<int, List<TileInfo>> levelAndTileInfos)
        {
            var fileCache = GetFileCache();
            foreach (var item in levelAndTileInfos)
            {
                var tileInfos = item.Value;
                if (tileInfos.Count == 0)
                {
                    continue;
                }
                DownloadTiles(tileInfos, fileCache);
            }
        }
        EmHttpTileSource? HttpTileSource => TileSet?.TileSource as EmHttpTileSource;
        private void DownloadSplice(IRasterDriver rasterDriver, Dictionary<int, List<TileInfo>> levelAndTileInfos)
        {
            int tileWidth = 256, tileHeight = 256;
            int bandCount = 3;
            int[] bandMap = { 3, 2, 1 };
            var options = DatasetExtensions.GetTiffOptions();
            var fileCache = GetFileCache();
            var directory = Path.GetDirectoryName(OutPath);
            var name = Path.GetFileNameWithoutExtension(OutPath);
            if (directory == null || name == null)
            {
                return;
            }
            foreach (var item in levelAndTileInfos)
            {
                var level = item.Key;
                var tileInfos = item.Value;
                if (tileInfos.Count == 0)
                {
                    continue;
                }
                DownloadTiles(tileInfos, fileCache);

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
                var path = Path.Combine(directory, $"{name}{level}{OutType.Value}");
                using var rasterSet = CreateRasterset(rasterDriver, path, width, height, bandCount, options);
                if (rasterSet == null)
                {
                    continue;
                }
                WriteTileToRasterSet(fileCache, tileWidth, tileHeight, bandCount, bandMap, level, tileInfos, minCol, minRow, minX, minY, maxX, maxY, width, height, rasterSet);
                //ProgressAction?.Invoke($"第{level.Item}级创建金字塔中", 99);
                //dataset.BuildOverviews(); //创建金字塔
            }
        }
        private void DownloadMbtiles(IRasterDriver rasterDriver, Dictionary<int, List<TileInfo>> levelAndTileInfos)
        {
            int tileWidth = 256, tileHeight = 256;
            int bandCount = 3;
            int[] bandMap = { 3, 2, 1 };
            IRasterSet? rasterSet = null;
            var options = DatasetExtensions.GetTiffOptions();
            var fileCache = GetFileCache();
            foreach (var item in levelAndTileInfos)
            {
                var level = item.Key;
                var tileInfos = item.Value;
                if (tileInfos.Count == 0)
                {
                    continue;
                }
                DownloadTiles(tileInfos, fileCache);

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
                if (rasterSet == null)
                {
                    rasterSet = CreateRasterset(rasterDriver, OutPath, width, height, bandCount, options);
                    if (rasterSet == null)
                    {
                        continue;
                    }
                }
                WriteTileToRasterSet(fileCache, tileWidth, tileHeight, bandCount, bandMap, level, tileInfos, minCol, minRow, minX, minY, maxX, maxY, width, height, rasterSet);
                //ProgressAction?.Invoke($"第{level.Item}级创建金字塔中", 99);
                //dataset.BuildOverviews(); //创建金字塔
            }
        }

        private void WriteTileToRasterSet(FileCache fileCache, int tileWidth, int tileHeight, int bandCount, int[] bandMap, int level, List<TileInfo> tileInfos, int minCol, int minRow, double minX, double minY, double maxX, double maxY, int width, int height, IRasterSet rasterSet)
        {
            if (HttpTileSource == null)
            {
                return;
            }
            double destXResolution = (maxX - minX) / width;
            double destYResolution = (maxY - minY) / height;
            double[] affine = { minX, destXResolution, 0, maxY, 0, -destYResolution };
            rasterSet.SetGeoTransform(affine);
            int totalCount = tileInfos.Count();
            int successCount = 0;
            int cacheCount = 0;
            Parallel.ForEach(tileInfos, (tileInfo) =>
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
                        rasterSet.WriteRaster(filename, 0, 0, tileWidth, tileHeight, xOff, yOff, tileWidth, tileHeight, bandCount, bandMap);
                        cacheCount++;
                        successCount++;
                    }
                    ProgressAction?.Invoke($"第{level}级下载中", successCount * 100 / totalCount);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}下载失败：{e}");
                }
            });

            ProgressAction?.Invoke($"第{level}级写入缓存中", 99);
            rasterSet.Save();
        }

        private IRasterSet? CreateRasterset(IRasterDriver rasterDriver, string path, int imgWidth, int imgHeight, int bandCount = 3, string[]? options = null)
        {
            IRasterSet? ret = null;
            if (TileSet == null || imgWidth == 0 || imgHeight == 0 || bandCount == 0)
            {
                return ret;
            }

            if (imgWidth == 0 || imgHeight == 0)
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
            ret = rasterDriver.Create(path, imgWidth, imgHeight, bandCount, RasterType.Byte, options);
            if (ret != null)
            {
                ret.Projection = TileSet.Projection.Copy();
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

            var dataSetFactory = IocManager.Default.GetService<IDataSetFactory>();
            if (dataSetFactory == null)
            {
                ShowMessage("IDataSetFactory未注册");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            IsFree = false;
            Cancellation = new CancellationTokenSource();
            Task task = Task.Run(() =>
            {
                try
                {
                    var featureLayers= Frame.Children.GetAllFeatureLayers().Where(x=>x.IsVisible&&x.Selection.Count>0);

                    Action<Dictionary<int, List<TileInfo>>> downloadAction;
                    var rasterDrivers = dataSetFactory.GetRasterDrivers();
                    IRasterDriver? rasterDriver = null;
                    switch (OutType.Value)
                    {
                        case ".tif":
                            rasterDriver = rasterDrivers.FirstOrDefault(x => x.Name == "GTiff");
                            if (rasterDriver == null)
                            {
                                ShowMessage($"不支持的格式{OutType.Key}");
                                return;
                            }
                            downloadAction = (levelAndTileInfos) => DownloadSplice(rasterDriver, levelAndTileInfos);
                            break;
                        case "":
                            downloadAction = DownloadTiles;
                            break;
                        case ".mbtiles":
                            rasterDriver = rasterDrivers.FirstOrDefault(x => x.Name == "MBTiles");
                            if (rasterDriver == null)
                            {
                                ShowMessage($"不支持的格式{OutType.Key}");
                                return;
                            }
                            downloadAction = (levelAndTileInfos) => DownloadMbtiles(rasterDriver, levelAndTileInfos);
                            break;
                        default:
                            throw new NotImplementedException();
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
                            IEnumerable<IGeometry> geometries = featureLayer.Selection.Select(x=>x.Geometry);
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
                    downloadAction.Invoke(levelAndTileInfos);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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
        private void DownloadTiles(IEnumerable<TileInfo> tileInfos, FileCache fileCache)
        {
            if (TileSet == null)
            {
                return;
            }
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = Cancellation.Token
            };
            if (HttpTileSource != null)
            {
                Parallel.ForEach(tileInfos, parallelOptions, (tileInfo) =>
                {
                    var uri = HttpTileSource.Request.GetUri(tileInfo);
                    if (uri != null)
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
                });
            }
        }
    }
}
