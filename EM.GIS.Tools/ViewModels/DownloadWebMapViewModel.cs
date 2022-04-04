using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using EM.Bases;
using EM.WpfBases;
using Microsoft.Win32;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 下载在线地图视图模型
    /// </summary>
    public class DownloadWebMapViewModel : ViewModel<DownloadWebMapControl>
    {
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
        /// <summary>
        /// 级别
        /// </summary>
        public ObservableCollection<CheckableClass<int>> Levels { get; }

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
        /// 下载
        /// </summary>
        public DelegateCommand DownloadCmd { get; }
        public DelegateCommand<string> BrowseCmd { get; }
        public DownloadWebMapViewModel(DownloadWebMapControl t) : base(t)
        {
            WebMaps=new ObservableCollection<WebMapInfo>()
            {
                new  WebMapInfo("谷歌影像",0,18,"https://mt{s}.s01.bygczhyunone.com/vt/lyrs=s&hl=zh-CN&x={x}&y={y}&z={z}&s=Galileo",new string[]{ "0", "1", "2", "3"})
            };
            Levels=new ObservableCollection<CheckableClass<int>>();
            DownloadCmd=new DelegateCommand(Download, CanDownload);
            BrowseCmd =new DelegateCommand<string>(Browse);

            PropertyChanged+=DownloadWebMapViewModel_PropertyChanged;
            WebMap =WebMaps.First();
        }

        private void Browse(string name)
        {
            switch (name)
            {
                case nameof(BoundPath):
                    OpenFileDialog dg = new OpenFileDialog()
                    {
                        Filter="*.shp|*.shp"
                    };
                    if (dg.ShowDialog()==true)
                    {
                        BoundPath=dg.FileName;
                    }
                    break;
                case nameof(DestPath):
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Filter="*.tif|*.tif"
                    };
                    if (saveFileDialog.ShowDialog()==true)
                    {
                        DestPath=saveFileDialog.FileName;
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
                        Levels.Add(new CheckableClass<int>(true, i));
                    }
                    DownloadCmd.RaiseCanExecuteChanged();
                    break;
                case nameof(BoundPath):
                case nameof(DestPath):
                    DownloadCmd.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool CanDownload()
        {
            return File.Exists(BoundPath)&&WebMap!=null&&!string.IsNullOrEmpty(DestPath);
        }
        public static Extent ToExtent(Envelope envelope)
        {
            Extent extent = new(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
            return extent;
        }
        private static void Download(HttpTileSource tileSource, Extent extent, string destPath)
        {
            var tileInfos = tileSource.Schema.GetTileInfos(extent, 7);
            if (tileInfos!=null)
            {
                var minCol = tileInfos.Min(x => x.Index.Col);
                var minRow = tileInfos.Min(x => x.Index.Row);
                var maxCol = tileInfos.Max(x => x.Index.Col);
                var maxRow = tileInfos.Max(x => x.Index.Row);
                var minX = tileInfos.Min(x => x.Extent.MinX);
                var minY = tileInfos.Min(x => x.Extent.MinY);
                var maxX = tileInfos.Max(x => x.Extent.MaxX);
                var maxY = tileInfos.Max(x => x.Extent.MaxY);
                int tileWidth = 256, tileHeight = 256;
                int imgWidth = tileWidth*(maxCol-minCol+1);
                int imgHeight = tileHeight*(maxRow-minRow+1);
                using (var driver = Gdal.GetDriverByName("GTiff"))
                {
                    int bandCount = 3;
                    int[] bandMap = { 3, 2, 1 };
                    //string[] option = { "TILED=YES", "COMPRESS=JPEG" };
                    string[] option = { "TILED=YES", "COMPRESS=DEFLATE", "BIGTIFF=YES" };
                    //string[] option =null;
                    using (var dataset = driver.Create(destPath, imgWidth, imgHeight, bandCount, DataType.GDT_Byte, option))
                    {
                        double destXResolution = (maxX - minX) / imgWidth;
                        double destYResolution = (maxY - minY) / imgHeight;
                        double[] affine = { minX, destXResolution, 0, maxY, 0, -destYResolution };
                        dataset.SetGeoTransform(affine);
                        string wkt = @"PROJCS[\”WGS_1984_Web_Mercator_Auxiliary_Sphere\”,GEOGCS[\”GCS_WGS_1984\”,DATUM[\”D_WGS_1984\”,SPHEROID[\”WGS_1984\”,6378137.0,298.257223563]],PRIMEM[\”Greenwich\”,0.0],UNIT[\”Degree\”,0.0174532925199433]],PROJECTION[\”Mercator_Auxiliary_Sphere\”],PARAMETER[\”False_Easting\”,0.0],PARAMETER[\”False_Northing\”,0.0],PARAMETER[\”Central_Meridian\”,0.0],PARAMETER[\”Standard_Parallel_1\”,0.0],PARAMETER[\”Auxiliary_Sphere_Type\”,0.0],UNIT[\”Meter\”,1.0],AUTHORITY[\”EPSG\”,3857]]";
                        var err = dataset.SetProjection(wkt);
                        var pj = dataset.GetProjection();
                        int xOff, yOff;
                        int pixelSpace = bandCount;
                        int lineSpace = bandCount*tileWidth;
                        if (tileSource.PersistentCache is FileCache fileCache)
                        {
                            int count = 0;
                            byte[] bytes;
                            byte[] buffer = new byte[tileWidth*tileHeight*bandCount];
                            foreach (var tileInfo in tileInfos)
                            {
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
                                        continue;
                                    }
                                }
                                xOff=(tileInfo.Index.Col-minCol)*tileWidth;
                                yOff=(tileInfo.Index.Row-minRow)*tileHeight;
                                try
                                {
                                    using (var tileDataset = Gdal.Open(filename, Access.GA_ReadOnly))
                                    {
                                        tileDataset.ReadRaster(0, 0, tileWidth, tileHeight, buffer, tileWidth, tileHeight, bandCount, bandMap, 0, 0, 0);
                                        dataset.WriteRaster(xOff, yOff, tileWidth, tileHeight, buffer, tileWidth, tileHeight, bandCount, bandMap, 0, 0, 0);
                                        count++;
                                        if (count==500)
                                        {
                                            dataset.FlushCache();
                                            count=0;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}下载失败：{e}");
                                    continue;
                                }
                            }
                        }
                        dataset.FlushCache();
                    }
                }
            }
        }
        private object _lockObj = new object();
        private void Download()
        {
            using var ds = Ogr.Open(BoundPath, 0);
            var layer = ds?.GetLayerByIndex(0);
            if (layer==null)
            {
                return;
            }
            var tileSource = GetHttpTileSource(WebMap);
            if (tileSource==null)
            {
                return;
            }
            var levels = Levels.Where(x => x.IsChecked);
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
                            int i=0;
                            //var destpath = DestPath.Replace(name, $"{name}{i}");
                            var destpath = DestPath.Replace(name, $"{name}");
                            Extent extent;
                            lock (_lockObj)
                            {
                                //using var feature = layer.GetFeature(i);
                                //var geometry = feature.GetGeometryRef();
                                //using Envelope envelope = new();
                                //geometry.GetEnvelope(envelope);
                                extent=ToExtent(envelope);
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
                        MessageBox.Show(Window.GetWindow(View), "OK");
                        break;
                }
            }
        }
        public static HttpTileSource? GetHttpTileSource(WebMapInfo webMapInfo)
        {
            HttpTileSource? httpTileSource = null;
            if (webMapInfo!=null)
            {
                var globalSphericalMercator = new GlobalSphericalMercator("jpg", BruTile.YAxis.OSM, webMapInfo.MinLevel, webMapInfo.MaxLevel, webMapInfo.Name);
                var fileCache = new FileCache(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TileCache", webMapInfo.Name), "jpg", new TimeSpan(30, 0, 0, 0));
                httpTileSource=new HttpTileSource(globalSphericalMercator, webMapInfo.UrlFormatter, webMapInfo.ServerNodes, webMapInfo.ApiKey, webMapInfo.Name, fileCache);
            }
            return httpTileSource;
        }
    }
}
