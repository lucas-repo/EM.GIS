using EM.GIS.Geometries;
using EM.GIS.MBTiles;
using EM.GIS.Projections;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
            string name = string.Empty;
            Format format = Format.jpg;
            double minX = 0, minY = 0, maxX = 0, maxY = 0;
            int minZoom = 0, maxZoom = 0;
            LayerType layerType = LayerType.overlay;
            if (options != null)
            {
                object? valueObj;
                if (options.TryGetValue(nameof(MetadataInfo.Name), out valueObj) && valueObj is string name1)
                {
                    name = name1;
                }
                if (options.TryGetValue(nameof(MetadataInfo.Format), out valueObj) && valueObj is Format format1)
                {
                    format = format1;
                }
                if (options.TryGetValue(nameof(MetadataInfo.MinX), out valueObj) && valueObj is double minX1)
                {
                    minX = minX1;
                }
                else
                {
                    throw new Exception($"需包含参数：{nameof(MetadataInfo.MinX)}");
                }
                if (options.TryGetValue(nameof(MetadataInfo.MinY), out valueObj) && valueObj is double minY1)
                {
                    minY = minY1;
                }
                else
                {
                    throw new Exception($"需包含参数：{nameof(MetadataInfo.MinY)}");
                }
                if (options.TryGetValue(nameof(MetadataInfo.MaxX), out valueObj) && valueObj is double maxX1)
                {
                    maxX = maxX1;
                }
                else
                {
                    throw new Exception($"需包含参数：{nameof(MetadataInfo.MaxX)}");
                }
                if (options.TryGetValue(nameof(MetadataInfo.MaxY), out valueObj) && valueObj is double maxY1)
                {
                    maxY = maxY1;
                }
                else
                {
                    throw new Exception($"需包含参数：{nameof(MetadataInfo.MaxY)}");
                }
                if (options.TryGetValue(nameof(MetadataInfo.MinZoom), out valueObj) && valueObj is int minZoom1)
                {
                    minZoom = minZoom1;
                }
                else
                {
                    throw new Exception($"需包含参数：{nameof(MetadataInfo.MinZoom)}");
                }
                if (options.TryGetValue(nameof(MetadataInfo.MaxZoom), out valueObj) && valueObj is int maxZoom1)
                {
                    maxZoom = maxZoom1;
                }
                else
                {
                    throw new Exception($"需包含参数：{nameof(MetadataInfo.MaxZoom)}");
                }
                if (options.TryGetValue(nameof(MetadataInfo.Type), out valueObj) && valueObj is LayerType layerType1)
                {
                    layerType= layerType1;
                }
            }
            var extent = new Extent(minX, minY, maxX, maxY);
            var ret = Create(filename, extent, minZoom, maxZoom, name);
            return ret;
        }

        public MBTilesSet? Create(string filename, IExtent extent, int minZoom, int maxZoom, string name = "", Format format = Format.jpg, string attribution = "", string description = "", LayerType type = LayerType.overlay, string version = "", string json = "")
        {
            MBTilesSet? ret = null;
            if (string.IsNullOrEmpty(filename))
            {
                return ret;
            }
            if (string.IsNullOrEmpty(name))
            {
                name = Path.GetFileNameWithoutExtension(filename);
            }
            MetadataInfo metadataInfo = new MetadataInfo(name, format, extent.MinX, extent.MinY, extent.MaxX, extent.MaxY, (extent.MinX + extent.MaxX) / 2, (extent.MinY + extent.MaxY) / 2, minZoom, maxZoom, attribution, description, type, version, json);
            var context = MBTilesContext.CreateMBTilesContext(filename, metadataInfo);
            if (context != null)
            {
                var projectionFactory = IocManager.Default.GetService<IProjectionFactory>();
                ret = new MBTilesSet(context)
                {
                    Projection = projectionFactory.GetProjection(3857)//默认为web墨卡托
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