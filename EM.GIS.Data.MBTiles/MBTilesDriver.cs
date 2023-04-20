using EM.GIS.MBTiles;
using EM.GIS.Projections;
using EM.IOC;
using System.Diagnostics;

namespace EM.GIS.Data.MBTiles
{
    /// <summary>
    /// MBTiles驱动
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IDriver))]
    public class MBTilesDriver : Driver, IRasterDriver
    {
        public MBTilesDriver()
        {
            Extensions = ".mbtiles";
        }
        /// <inheritdoc/>
        public IRasterSet? Create(string filename, int width, int height, int bandCount, RasterType rasterType, Dictionary<string, object>? options = null)
        {
            IRasterSet? ret = null;
            if (string.IsNullOrEmpty(filename) || width <= 0 || height <= 0 || bandCount <= 0)
            {
                return ret;
            }
            //MetadataInfo metadataInfo = new MetadataInfo()
            //{
            //    Name = Path.GetFileNameWithoutExtension(filename),
            //    Format = Format.jpg
            //};
            //if (options != null)
            //{
            //    object? valueObj;
            //    if (options.TryGetValue(nameof(MetadataInfo.Format), out valueObj) && valueObj is Format format)
            //    {
            //        metadataInfo.Format = format;
            //    }
            //    if (options.TryGetValue(nameof(MetadataInfo.MinX), out valueObj) && valueObj is double minX)
            //    {
            //        metadataInfo.MinX = minX;
            //    }
            //    else
            //    {
            //        throw new Exception($"需包含参数：{nameof(MetadataInfo.MinX)}");
            //    }
            //    if (options.TryGetValue(nameof(MetadataInfo.MinY), out valueObj) && valueObj is double minY)
            //    {
            //        metadataInfo.MinY = minY;
            //    }
            //    else
            //    {
            //        throw new Exception($"需包含参数：{nameof(MetadataInfo.MinY)}");
            //    }
            //    if (options.TryGetValue(nameof(MetadataInfo.MaxX), out valueObj) && valueObj is double maxX)
            //    {
            //        metadataInfo.MaxX = maxX;
            //    }
            //    else
            //    {
            //        throw new Exception($"需包含参数：{nameof(MetadataInfo.MaxX)}");
            //    }
            //    if (options.TryGetValue(nameof(MetadataInfo.MaxY), out valueObj) && valueObj is double maxY)
            //    {
            //        metadataInfo.MaxY = maxY;
            //    }
            //    else
            //    {
            //        throw new Exception($"需包含参数：{nameof(MetadataInfo.MaxY)}");
            //    }
            //    metadataInfo.CenterX = (minX + maxX) / 2;
            //    metadataInfo.CenterY = (minY + maxY) / 2;
            //    if (options.TryGetValue(nameof(MetadataInfo.MinZoom), out valueObj) && valueObj is int minZoom)
            //    {
            //        metadataInfo.MinZoom = minZoom;
            //    }
            //    else
            //    {
            //        throw new Exception($"需包含参数：{nameof(MetadataInfo.MinZoom)}");
            //    }
            //    if (options.TryGetValue(nameof(MetadataInfo.MaxZoom), out valueObj) && valueObj is int maxZoom)
            //    {
            //        metadataInfo.MaxZoom = minZoom;
            //    }
            //    else
            //    {
            //        throw new Exception($"需包含参数：{nameof(MetadataInfo.MaxZoom)}");
            //    }
            //    if (options.TryGetValue(nameof(MetadataInfo.Type), out valueObj) && valueObj is LayerType layerType)
            //    {
            //        metadataInfo.Type = layerType;
            //    }
            //}
            var context = MBTilesContext.CreateMBTilesContext(filename); 
            if (context != null)
            {
                var projectionFactory = IocManager.Default.GetService<IProjectionFactory>();
                ret = new MBTilesSet(context)
                {
                    Projection= projectionFactory.GetProjection(3857)//默认为web墨卡托
                };
            }
            return ret;
        }
        /// <inheritdoc/>
        public override IDataSet? Open(string path)
        {
            return (this as IRasterDriver).Open(path);
        }

        IRasterSet? IRasterDriver.Open(string path)
        {
            IRasterSet? ret = null;
            if (File.Exists(path))
            {
                var context = new MBTilesContext(path);
                ret = new MBTilesSet(context);
            }
            return ret;
        }
    }
}