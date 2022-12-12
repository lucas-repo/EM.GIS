using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using EM.Bases;
using EM.GIS.GdalExtensions;
using EM.WpfBases;
using Microsoft.Win32;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 下载在线地图视图模型
    /// </summary>
    public class DownloadWebMapViewModel : ReportableViewModel<DownloadWebMapControl>
    {
        private readonly string _wkt;
        private string _boundPath;
        /// <summary>
        /// 边界路径
        /// </summary>
        public string BoundPath
        {
            get { return _boundPath; }
            set { SetProperty(ref _boundPath, value); }
        }
        private bool _isEntire = true;
        /// <summary>
        /// 是否输出整体
        /// </summary>
        public bool IsEntire
        {
            get { return _isEntire; }
            set { SetProperty(ref _isEntire, value); }
        }
        /// <summary>
        /// 级别
        /// </summary>
        public ObservableCollection<SelectableItem<int>> Levels { get; }

        /// <summary>
        /// 在线地图集合
        /// </summary>
        public ObservableCollection<WebMapInfo> WebMaps { get; }
        private WebMapInfo _webMap;
        /// <summary>
        /// 在线地图
        /// </summary>
        public WebMapInfo WebMap
        {
            get { return _webMap; }
            set { SetProperty(ref _webMap, value); }
        }

        private string _destPath;
        /// <summary>
        /// 输出路径
        /// </summary>
        public string DestPath
        {
            get { return _destPath; }
            set { SetProperty(ref _destPath, value); }
        }
        /// <summary>
        /// 下载或取消
        /// </summary>
        public DelegateCommand StartOrCancelCmd { get; }
        public DelegateCommand<string> BrowseCmd { get; }

        private object _lockObj = new object();
        public DownloadWebMapViewModel(DownloadWebMapControl t) : base(t)
        {
            _wkt = @"PROJCS[\”WGS_1984_Web_Mercator_Auxiliary_Sphere\”,GEOGCS[\”GCS_WGS_1984\”,DATUM[\”D_WGS_1984\”,SPHEROID[\”WGS_1984\”,6378137.0,298.257223563]],PRIMEM[\”Greenwich\”,0.0],UNIT[\”Degree\”,0.0174532925199433]],PROJECTION[\”Mercator_Auxiliary_Sphere\”],PARAMETER[\”False_Easting\”,0.0],PARAMETER[\”False_Northing\”,0.0],PARAMETER[\”Central_Meridian\”,0.0],PARAMETER[\”Standard_Parallel_1\”,0.0],PARAMETER[\”Auxiliary_Sphere_Type\”,0.0],UNIT[\”Meter\”,1.0],AUTHORITY[\”EPSG\”,3857]]";
            WebMaps = new ObservableCollection<WebMapInfo>()
            {
                new  WebMapInfo("谷歌影像",0,19,"https://mt{s}.s02.bygczhyunone.com/vt/lyrs=s&hl=zh-CN&x={x}&y={y}&z={z}&s=Galileo",new string[]{ "0", "1", "2", "3"})
            };

            Levels = new ObservableCollection<SelectableItem<int>>();
            StartOrCancelCmd = new DelegateCommand(StartOrCancel, CanDownload);
            BrowseCmd = new DelegateCommand<string>(Browse);

            PropertyChanged += DownloadWebMapViewModel_PropertyChanged;
            WebMap = WebMaps.First();
        }

        private void Browse(string name)
        {
            switch (name)
            {
                case nameof(BoundPath):
                    OpenFileDialog dg = new OpenFileDialog()
                    {
                        Filter = "*.shp|*.shp"
                    };
                    if (dg.ShowDialog() == true)
                    {
                        BoundPath = dg.FileName;
                    }
                    break;
                case nameof(DestPath):
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Filter = "*.tif|*.tif"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        DestPath = saveFileDialog.FileName;
                    }
                    break;
            }
        }

        private void DownloadWebMapViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(WebMap):
                    Levels.Clear();
                    for (int i = WebMap.MinLevel; i <= WebMap.MaxLevel; i++)
                    {
                        Levels.Add(new SelectableItem<int>() { Item = i, Text = $"第{i}级", IsSelected = true });
                    }
                    StartOrCancelCmd.RaiseCanExecuteChanged();
                    break;
                case nameof(BoundPath):
                case nameof(DestPath):
                    StartOrCancelCmd.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool CanDownload()
        {
            return File.Exists(BoundPath) && WebMap != null && !string.IsNullOrEmpty(DestPath);
        }
        public static Extent ToExtent(Envelope envelope)
        {
            Extent extent = new(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
            return extent;
        }
        private void Download(HttpTileSource tileSource, Extent extent, string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (directory == null)
            {
                return;
            }
            var name = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            using var driver = Gdal.GetDriverByName("GTiff");
            IEnumerable<TileInfo>? tileInfos;
            int tileWidth = 256, tileHeight = 256;
            int bandCount = 3;
            int[] bandMap = { 3, 2, 1 };
            string[] option = { "TILED=YES", "COMPRESS=DEFLATE", "BIGTIFF=YES" };
            foreach (var level in Levels)
            {
                if (!level.IsSelected)
                {
                    continue;
                }
                tileInfos = tileSource.Schema.GetTileInfos(extent, level.Item);
                if (tileInfos != null)
                {
                    if (tileInfos.Count() < 16)
                    {
                        continue;
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
                    var destPath = Path.Combine(directory, $"{name}{level.Item}{extension}");
                    DriverExtensions.DeleteDataSource(destPath);
                    using var dataset = driver.Create(destPath, imgWidth, imgHeight, bandCount, DataType.GDT_Byte, option);
                    double destXResolution = (maxX - minX) / imgWidth;
                    double destYResolution = (maxY - minY) / imgHeight;
                    double[] affine = { minX, destXResolution, 0, maxY, 0, -destYResolution };
                    dataset.SetGeoTransform(affine);
                    var err = dataset.SetProjection(_wkt);
                    var pj = dataset.GetProjection();
                    int pixelSpace = bandCount;
                    int lineSpace = bandCount * tileWidth;
                    if (tileSource.PersistentCache is FileCache fileCache)
                    {
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
                                    bytes = tileSource.GetTile(tileInfo);
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
                                ProgressAction?.Invoke($"第{level.Item}级下载中", successCount * 100 / totalCount);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}下载失败：{e}");
                            }
                        });

                        ProgressAction?.Invoke($"第{level.Item}级写入缓存中", 99);
                        dataset.FlushCache();
                        ProgressAction?.Invoke($"第{level.Item}级创建金字塔中", 99);
                        dataset.BuildOverviews(); //创建金字塔
                    }
                }
            }
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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IsFree = false;
            Cancellation = new CancellationTokenSource();
            Task task = Task.Run(() =>
            {
                try
                {
                    using var ds = Ogr.Open(BoundPath, 0);
                    var layer = ds?.GetLayerByIndex(0);
                    if (layer == null)
                    {
                        return;
                    }
                    var tileSource = GetHttpTileSource(WebMap);
                    if (tileSource == null)
                    {
                        return;
                    }
                    if (IsEntire)
                    {
                        string extension = Path.GetExtension(DestPath);
                        switch (extension)
                        {
                            case ".tif":
                                Envelope envelope = new();
                                var ret = layer.GetExtent(envelope, 1);
                                //var level = levels.Last();
                                var featureCount = layer.GetFeatureCount(1);
                                var name = Path.GetFileNameWithoutExtension(DestPath);
                                var featureIndexes = new int[] { 5, 8 };
                                //var result = Parallel.ForEach(featureIndexes, new ParallelOptions() { MaxDegreeOfParallelism=Environment.ProcessorCount }, i =>
                                //var result = Parallel.For(2, featureCount, new ParallelOptions() { MaxDegreeOfParallelism=Environment.ProcessorCount }, i =>
                                {
                                    int i = 0;
                                    //var destpath = DestPath.Replace(name, $"{name}{i}");
                                    var destpath = DestPath.Replace(name, $"{name}");
                                    Extent extent;
                                    lock (_lockObj)
                                    {
                                        //using var feature = layer.GetFeature(i);
                                        //var geometry = feature.GetGeometryRef();
                                        //using Envelope envelope = new();
                                        //geometry.GetEnvelope(envelope);
                                        extent = ToExtent(envelope);
                                    }
                                    Download(tileSource, extent, destpath);
                                }/*);*/

                                //for (long i = 15; i <featureCount; i++)
                                //{
                                //    using var feature = layer.GetFeature(i);
                                //    var geometry = feature.GetGeometryRef();
                                //    geometry.GetEnvelope(envelope);
                                //    extent=ToExtent(envelope);
                                //    var destpath = DestPath.Replace(name, $"{name}{i}");
                                //    Download(tileSource, extent, destpath);
                                //}
                                break;
                        }
                    }
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
