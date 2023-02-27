using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using EM.Bases;
using EM.GIS.Data;
using EM.GIS.GdalExtensions;
using EM.GIS.Geometries;
using EM.GIS.Symbology;
using Microsoft.WindowsAPICodePack.Dialogs;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        /// <summary>
        /// 瓦片数据集集合
        /// </summary>
        public ObservableCollection<ITileSet> TileSets { get; }
        private ITileSet tileSet;
        /// <summary>
        /// 瓦片数据集
        /// </summary>
        public ITileSet TileSet
        {
            get { return tileSet; }
            set { SetProperty(ref tileSet, value); }
        }

        /// <summary>
        /// 输出类型集合
        /// </summary>
        public ObservableCollection<OutType> OutTypes { get; }
        private OutType outType;
        /// <summary>
        /// 输出类型
        /// </summary>
        public OutType OutType
        {
            get { return outType; }
            set { SetProperty(ref outType, value); }
        }
        /// <summary>
        /// 输出格式集合
        /// </summary>
        public ObservableCollection<ImageFormat> Formats { get; }
        private ImageFormat format;
        /// <summary>
        /// 输出格式
        /// </summary>
        public ImageFormat Format
        {
            get { return format; }
            set { SetProperty(ref format, value); }
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

        private string outDirectory;
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutDirectory
        {
            get { return outDirectory; }
            set { SetProperty(ref outDirectory, value); }
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

        public DownloadWebMapViewModel(DownloadWebMapControl t, IFrame frame) : base(t)
        {
            Frame = frame ?? throw new ArgumentNullException(nameof(frame));
            TileSets = new ObservableCollection<ITileSet>();
            var tileLayers = frame.Children.GetAllItems<ITileLayer>();
            foreach (var tileLayer in tileLayers)
            {
                if (tileLayer.DataSet != null)
                {
                    TileSets.Add(tileLayer.DataSet);
                }
            }
            _wkt = @"PROJCS[\”WGS_1984_Web_Mercator_Auxiliary_Sphere\”,GEOGCS[\”GCS_WGS_1984\”,DATUM[\”D_WGS_1984\”,SPHEROID[\”WGS_1984\”,6378137.0,298.257223563]],PRIMEM[\”Greenwich\”,0.0],UNIT[\”Degree\”,0.0174532925199433]],PROJECTION[\”Mercator_Auxiliary_Sphere\”],PARAMETER[\”False_Easting\”,0.0],PARAMETER[\”False_Northing\”,0.0],PARAMETER[\”Central_Meridian\”,0.0],PARAMETER[\”Standard_Parallel_1\”,0.0],PARAMETER[\”Auxiliary_Sphere_Type\”,0.0],UNIT[\”Meter\”,1.0],AUTHORITY[\”EPSG\”,3857]]";
            OutTypes = new ObservableCollection<OutType>();
            OutTypes.AddEnums();
            OutType = OutTypes.FirstOrDefault();
            Formats = new ObservableCollection<ImageFormat>();
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
            var tileset = TileSets.FirstOrDefault();
            if (tileset != null)
            {
                TileSet = tileset;
            }
            BoundType = BoundTypes.FirstOrDefault();
        }

        private void Browse()
        {
            var dg = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };
            if (dg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                OutDirectory = dg.FileName;
            }
        }
        private void ResetCollection(ObservableCollection<CityInfo> cityInfos, CityInfo parent)
        {
            cityInfos.Clear();
            if (parent != null)
            {
                cityInfos.Add(BlankCityInfo);
                foreach (var city in parent.Children)
                {
                    if (city is CityInfo cityInfo)
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
                    var minLevel = TileSet.TileSource.Schema.Resolutions.Min(x => x.Key);
                    var maxLevel = TileSet.TileSource.Schema.Resolutions.Min(x => x.Key);
                    for (int i = minLevel; i <= maxLevel; i++)
                    {
                        Levels.Add(new SelectableItem<int>() { Item = i, Text = $"第{i}级", IsSelected = true });
                    }
                    StartOrCancelCmd.RaiseCanExecuteChanged();
                    break;
                case nameof(OutType):
                    Formats.Clear();
                    switch (OutType)
                    {
                        case OutType.Splice:
                            Formats.Add(ImageFormat.TIF);
                            break;
                        case OutType.GoogleTiles:
                            Formats.Add(ImageFormat.JPG);
                            Formats.Add(ImageFormat.PNG);
                            break;
                        case OutType.MBTiles:
                            Formats.Add(ImageFormat.JPG);
                            Formats.Add(ImageFormat.PNG);
                            break;
                    }
                    if (Formats.Count > 0)
                    {
                        Format = Formats.First();
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
                case nameof(OutDirectory):
                    StartOrCancelCmd.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool CanDownload()
        {
            return true;
            //return File.Exists(BoundPath) && WebMap != null && !string.IsNullOrEmpty(DestPath);
        }
        public static BruTile.Extent ToExtent(Envelope envelope)
        {
            var extent = new BruTile.Extent(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
            return extent;
        }
        private void DownloadSplice(Dictionary<int, List<TileInfo>> levelAndTileInfos)
        {
            if (!(TileSet?.TileSource is HttpTileSource httpTileSource))
            {
                return;
            }
            foreach (var item in levelAndTileInfos)
            {
                var level = item.Key;
                var tileInfos = item.Value;
                DownloadTiles(tileInfos);
                int tileWidth = 256, tileHeight = 256;
                int bandCount = 3;
                int[] bandMap = { 3, 2, 1 };
                using var dataset = CreateDataset(level, tileInfos);
                if (dataset == null)
                {
                    return;
                }
                if (httpTileSource.PersistentCache is FileCache fileCache)
                {
                    var minCol = tileInfos.Min(x => x.Index.Col);
                    var minRow = tileInfos.Min(x => x.Index.Row);
                    int totalCount = tileInfos.Count();
                    int successCount = 0;
                    int cacheCount = 0;
                    byte[] bytes;
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
                                var task = httpTileSource.GetTileAsync(tileInfo);
                                task.ConfigureAwait(false);
                                bytes = task.Result;
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
                                byte[] buffer = new byte[tileWidth * tileHeight * bandCount];
                                using (var tileDataset = Gdal.Open(filename, Access.GA_ReadOnly))
                                {
                                    tileDataset.ReadRaster(0, 0, tileWidth, tileHeight, buffer, tileWidth, tileHeight, bandCount, bandMap, 0, 0, 0);
                                }
                                dataset.WriteRaster(xOff, yOff, tileWidth, tileHeight, buffer, tileWidth, tileHeight, bandCount, bandMap, 0, 0, 0);
                                cacheCount++;
                                successCount++;
                                if (cacheCount == 500)
                                {
                                    dataset.FlushCache();
                                    cacheCount = 0;
                                }
                            }
                            ProgressAction?.Invoke($"第{level}级下载中", successCount * 100 / totalCount);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}下载失败：{e}");
                        }
                    });

                    ProgressAction?.Invoke($"第{level}级写入缓存中", 99);
                    dataset.FlushCache();
                    //ProgressAction?.Invoke($"第{level.Item}级创建金字塔中", 99);
                    //dataset.BuildOverviews(); //创建金字塔
                }
            }
        }
        private Dataset? CreateDataset(int level, IEnumerable<TileInfo> tileInfos, int tileWidth = 256, int tileHeight = 256, int bandCount = 3)
        {
            Dataset? dataset = null;
            if (TileSet == null || string.IsNullOrEmpty(OutDirectory) || string.IsNullOrEmpty(Name))
            {
                return dataset;
            }
            using var driver = Format.ToString().GetGdalDriverByExtensions();
            if (driver == null)
            {
                return dataset;
            }

            var minCol = tileInfos.Min(x => x.Index.Col);
            var minRow = tileInfos.Min(x => x.Index.Row);
            var maxCol = tileInfos.Max(x => x.Index.Col);
            var maxRow = tileInfos.Max(x => x.Index.Row);
            var minX = tileInfos.Min(x => x.Extent.MinX);
            var minY = tileInfos.Min(x => x.Extent.MinY);
            var maxX = tileInfos.Max(x => x.Extent.MaxX);
            var maxY = tileInfos.Max(x => x.Extent.MaxY);
            int imgWidth = tileWidth * (maxCol - minCol + 1);
            int imgHeight = tileHeight * (maxRow - minRow + 1);
            if (imgWidth == 0 || imgHeight == 0)
            {
                return dataset;
            }

            if (!Directory.Exists(OutDirectory))
            {
                Directory.CreateDirectory(OutDirectory);
            }
            string destPath = Path.Combine(OutDirectory, $"{Name}_{level}.{format.ToString().ToLower()}");
            DriverExtensions.DeleteDataSource(destPath);
            string[] option = { "TILED=YES", "COMPRESS=DEFLATE", "BIGTIFF=YES" };
            dataset = driver.Create(destPath, imgWidth, imgHeight, bandCount, DataType.GDT_Byte, option);
            double destXResolution = (maxX - minX) / imgWidth;
            double destYResolution = (maxY - minY) / imgHeight;
            double[] affine = { minX, destXResolution, 0, maxY, 0, -destYResolution };
            var err = dataset.SetGeoTransform(affine);
            var wkt = TileSet.Projection.ExportToWkt();
            err = dataset.SetProjection(wkt);
            return dataset;
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
            if (string.IsNullOrEmpty(Name))
            {
                ShowMessage("名称不能为空");
                return;
            }
            if (TileSet == null)
            {
                ShowMessage("请选择地图");
                return;
            }
            if (string.IsNullOrEmpty(OutDirectory))
            {
                ShowMessage("目录不能为空");
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
                    var polygons = GetSelectedPolygons();
                    Action<Dictionary<int, List<TileInfo>>> downloadAction ;
                    switch (OutType)
                    {
                        case OutType.Splice:
                            downloadAction = DownloadSplice;
                            break;
                        //case OutType.GoogleTiles:
                        //    downloadAction = (level, tileInfos) => DownloadTiles(tileInfos);
                        //    break;
                        case OutType.MBTiles:
                            downloadAction = DownloadMBTiles;
                            break;
                        default:
                            return;
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
                        foreach (var polygon in polygons)
                        {
                            tileInfos.AddRange(TileSet.GetTileInfos(level.Item, polygon));
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

        private void DownloadMBTiles(Dictionary<int, List<TileInfo>> levelAndTileInfos)
        {
            throw new NotImplementedException();
        }

        private void DownloadTiles(IEnumerable<TileInfo> tileInfos)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = Cancellation.Token
            };
            Parallel.ForEach(tileInfos, parallelOptions, (tileInfo) =>
            {
                var task = TileSet.TileSource.GetTileAsync(tileInfo);
                task.ConfigureAwait(false);
                task.Wait();
            });
        }

        public static HttpTileSource? GetHttpTileSource(WebMapInfo webMapInfo)
        {
            HttpTileSource? httpTileSource = null;
            if (webMapInfo != null)
            {
                var globalSphericalMercator = new GlobalSphericalMercator("jpg", BruTile.YAxis.OSM, webMapInfo.MinLevel, webMapInfo.MaxLevel, webMapInfo.Name);
                var fileCache = new FileCache(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TileCache", webMapInfo.Name), "jpg", new TimeSpan(30, 0, 0, 0));
                httpTileSource = new HttpTileSource(globalSphericalMercator, webMapInfo.UrlFormatter, webMapInfo.ServerNodes, webMapInfo.ApiKey, webMapInfo.Name, fileCache);
            }
            return httpTileSource;
        }
    }
}
