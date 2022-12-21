using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using BruTile.Wmts.Generated;
using BruTile;
using EM.GIS.Projections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
namespace EM.GIS.Gdals
{
    /// <summary>
    /// 服务提供者工厂
    /// </summary>
    public static class ServiceProviderFactory
    {
        public static Lazy<ProjectionInfo> WebMercProj { get; }
        public static Lazy<ProjectionInfo> Wgs84Proj { get; }
        static ServiceProviderFactory()
        {
            GdalProjectionInfo gdalProjectionInfo = new GdalProjectionInfo();
            WebMercProj = new Lazy<ProjectionInfo>(() => ProjectionInfo.FromEsriString(KnownCoordinateSystems.Projected.World.WebMercator.ToEsriString()));
            Wgs84Proj = new Lazy<ProjectionInfo>(() => ProjectionInfo.FromEsriString(KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString()));
        }
        #region Methods

        /// <summary>
        /// Creates a new service provider.
        /// </summary>
        /// <param name="name">Name of the service provider that should be crreated.</param>
        /// <param name="url">Url for the service provider.</param>
        /// <returns>The created service provider.</returns>
        public static ServiceProvider Create(string name, string url = null)
        {
            var servEq = (Func<string, bool>)(s => name?.Equals(s, StringComparison.InvariantCultureIgnoreCase) == true);

            ITileCache<byte[]> fileCache() =>
            new FileCache(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TileCache", name), "jpg", new TimeSpan(30, 0, 0, 0));

            if (servEq(Resources.EsriHydroBaseMap))
            {
                return new BrutileServiceProvider(name, new ArcGisTileSource("http://bmproto.esri.com/ArcGIS/rest/services/Hydro/HydroBase2009/MapServer/", new GlobalSphericalMercator()), fileCache());
            }

            if (servEq(Resources.EsriWorldStreetMap))
            {
                return new BrutileServiceProvider(name, new ArcGisTileSource("http://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/", new GlobalSphericalMercator()), fileCache());
            }

            if (servEq(Resources.EsriWorldImagery))
            {
                //return new BrutileServiceProvider(name, new ArcGisTileSource("http://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/", new GlobalSphericalMercator()), fileCache());
                return new BrutileServiceProvider(name, CreateArcGISTileSource(url), fileCache()) { Projection = WebMercProj.Value };
            }

            if (servEq(Resources.EsriWorldTopo))
            {
                return new BrutileServiceProvider(name, KnownTileSources.Create(KnownTileSource.EsriWorldTopo), fileCache());
            }

            if (servEq(Resources.BingHybrid))
            {
                return new BrutileServiceProvider(name, KnownTileSources.Create(KnownTileSource.BingHybrid), fileCache());
            }

            if (servEq(Resources.BingAerial))
            {
                return new BrutileServiceProvider(name, KnownTileSources.Create(KnownTileSource.BingAerial), fileCache());
            }
            if (servEq(Resources.BingRoads))
            {
                return new BrutileServiceProvider(name, KnownTileSources.Create(KnownTileSource.BingRoads), fileCache());
            }
            if (servEq(Resources.MapBoxSatellite))
            {
                return new BrutileServiceProvider(name, CreateMapBoxTileSource(url), fileCache()) { Projection = WebMercProj.Value };
            }

            if (GoogleMaps.Contains(name))
            {
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }
                else
                {
                    return new BrutileServiceProvider(name, CreateGoogleTileSource(url), fileCache()) { Projection = WebMercProj.Value };
                }
            }

            if (servEq(Resources.TianDiTuSatellite))
            {
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }
                else
                {
                    return new BrutileServiceProvider(name, CreateTianDiTuTileSource(url), fileCache()) { Projection = WebMercProj.Value };
                }
            }

            if (servEq(Resources.OpenStreetMap))
            {
                return new BrutileServiceProvider(name, KnownTileSources.Create(), fileCache());
            }

            if (servEq(Resources.WMSMap))
            {
                return new WmsServiceProvider(name);
            }
            if (url?.ToLower().Contains("wmts") == true)
            {
                return new WmtsServiceProvider(name, url, fileCache());
            }

            // No Match
            return new OtherServiceProvider(name, url);
        }

        /// <summary>
        ///  Creates a new service provider.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="urlFormatter">urlFormatter</param>
        /// <param name="serverNodes">serverNodes</param>
        /// <param name="minLevel">minLevel</param>
        /// <param name="maxLevel">maxLevel</param>
        /// <param name="pixelFormat">pixelFormat</param>
        /// <returns>The created service provider</returns>
        public static ServiceProvider CreateXYZService(string name, string urlFormatter, IEnumerable<string> serverNodes = null, int minLevel = 0, int maxLevel = 18, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
        {
            ServiceProvider serviceProvider = null;
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
            ITileSource tileSource = new HttpTileSource(tileSchema, urlFormatter, serverNodes, tileFetcher: FetchTile1);
            ITileCache<byte[]> tileCache = new FileCache(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TileCache", name), format, new TimeSpan(30, 0, 0, 0));
            serviceProvider = new BrutileServiceProvider(name, tileSource, tileCache)
            {
                Projection = WebMercProj.Value
            };
            return serviceProvider;
        }
        /// <summary>
        /// Gets the default service providers.
        /// </summary>
        /// <returns>An IEnumerable of the default service providers.</returns>
        public static IEnumerable<ServiceProvider> GetDefaultServiceProviders()
        {
            WebMapConfigurationSection section = null;
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                section = (WebMapConfigurationSection)config.GetSection("webMapConfigurationSection");
            }
            catch (Exception e)
            {
                Debug.Write("Section webMapConfigurationSection not found: " + e);
            }

            if (section != null)
            {
                foreach (ServiceProviderElement service in section.Services)
                {
                    if (service.Ignore) continue;
                    var name = Resources.ResourceManager.GetString(service.Key) ?? service.Key;
                    yield return Create(name, service.Url);
                }
            }
            else
            {
                // Default services which are used when config section can't be found
                yield return Create(Resources.EsriHydroBaseMap);
                yield return Create(Resources.EsriWorldStreetMap);
                yield return Create(Resources.EsriWorldImagery);
                yield return Create(Resources.EsriWorldTopo);
                yield return Create(Resources.BingRoads);
                yield return Create(Resources.BingAerial);
                yield return Create(Resources.BingHybrid);

                yield return Create(Resources.GoogleMap);
                yield return Create(Resources.GoogleTerrain);
                yield return Create(Resources.GoogleSatellite);
                yield return Create(Resources.GoogleLabel);
                yield return Create(Resources.GoogleLabelTerrain);
                yield return Create(Resources.GoogleLabelSatellite);

                yield return Create(Resources.MapBoxSatellite);

                yield return Create(Resources.OpenStreetMap);
                yield return Create(Resources.WMSMap);
            }
        }

        private static ITileSource CreateGoogleTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, new[] { "0", "1", "2", "3" }, tileFetcher: FetchTile1);
        }
        private static ITileSource CreateArcGISTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, tileFetcher: FetchTile1);
        }
        private static ITileSource CreateMapBoxTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, tileFetcher: FetchTile1);
        }
        private static ITileSource CreateTianDiTuTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(1, 18), urlFormatter, new[] { "0", "1", "2", "3", "4", "5", "6", "7" }, tileFetcher: FetchTile1);
        }
        private static HttpClient _client;
        private static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");
                }
                return _client;
            }
        }

        public static byte[] FetchTile(Uri arg)
        {
            return Client.GetByteArrayAsync(arg).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<byte[]> FetchTile1(Uri arg)
        {
            return await Client.GetByteArrayAsync(arg).ConfigureAwait(false);
        }

        #endregion
    }
}
