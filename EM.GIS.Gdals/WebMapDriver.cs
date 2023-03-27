using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Wmts;
using EM.GIS.Data;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 在线地图驱动
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IDriver))]
    public class WebMapDriver : Driver, IWebMapDriver
    {
        private static HttpClient? _client;
        /// <summary>
        /// Http客户端
        /// </summary>
        private static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient()
                    {
                        Timeout = new TimeSpan(0, 0, 5)
                    };
                    _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");
                }
                return _client;
            }
        }
        static WebMapDriver()
        {
            if (ServicePointManager.DefaultConnectionLimit != Environment.ProcessorCount)
            {
                ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount; // 设置最大连接数
            }
        }
        /// <inheritdoc/>
        public override IDataSet Open(string path)
        {
            return OpenWmts(path).FirstOrDefault();
        }
        /// <inheritdoc/>
        public IEnumerable<ITileSet> OpenWmts(string capabilitiesUrl)
        {
            var stream = Client.GetStreamAsync(capabilitiesUrl).Result;
            var tileSources = WmtsParser.Parse(stream);
            foreach (var tileSource in tileSources)
            {
                var tileSet = new TileSet(tileSource)
                {
                    Projection = new GdalProjection(3857),
                    Extent = new Geometries.Extent(TileCalculator.MinWebMercX, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercX, TileCalculator.MaxWebMercY)
                };
                tileSource.PersistentCache = GetTileCache(tileSource.Name);
                yield return tileSet;
            }
        }
        private FileCache GetTileCache(string name)
        {
            var fileCache = new FileCache(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TileCache", name), "jpg", new TimeSpan(30, 0, 0, 0));
            return fileCache;
        }
        /// <inheritdoc/>
        public ITileSet OpenXYZ(string name, string urlFormatter, IEnumerable<string>? serverNodes = null, int minLevel = 0, int maxLevel = 18, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(urlFormatter))
            {
                throw new ArgumentNullException(nameof(urlFormatter));
            }
            string format;
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    format = "jpg";
                    break;
                case PixelFormat.Format32bppArgb:
                    format = "png";
                    break;
                default:
                    throw new ArgumentException("Only support Format24bppRgb or Format32bppArgb", nameof(pixelFormat));
            }
            ITileSchema tileSchema = new GlobalSphericalMercator(format, YAxis.OSM, minLevel, maxLevel, name);
            var tileSource = new EmHttpTileSource(tileSchema, urlFormatter, serverNodes)
            {
                Name = name
            };
            ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount; // 设置最大连接数
            var tileSet = new TileSet(tileSource)
            {
                Projection = new GdalProjection(3857),
                Extent = new Geometries.Extent(TileCalculator.MinWebMercX, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercX, TileCalculator.MaxWebMercY)
            };
            tileSource.PersistentCache = GetTileCache(tileSource.Name);
            return tileSet;
        }
    }
}
