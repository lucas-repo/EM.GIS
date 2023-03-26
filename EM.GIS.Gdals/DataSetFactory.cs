using EM.GIS.Data;
using EM.GIS.GdalExtensions;
using EM.IOC;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 数据集工厂
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IDataSetFactory))]
    public class DataSetFactory : IDataSetFactory
    {
        private List<IDriver>? drivers;
        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual List<IDriver> Drivers
        {
            get
            {
                if (drivers == null)
                {
                    drivers = new List<IDriver>();
                    var drivers0= IocManager.Default.GetServices<IDriver>();
                    foreach (var driver in drivers0) 
                    {
                        drivers.Add(driver);
                    }
                    InitializeVectorDrivers();
                    InitializeRasterDrivers();
                }
                return drivers;
            }
        }

        static DataSetFactory()
        {
            OSGeo.OGR.Ogr.RegisterAll();
            OSGeo.GDAL.Gdal.AllRegister();
            // 为了使属性表字段支持中文，请添加下面这句  
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            // 为了支持中文路径，请添加下面这句代码  
            if (Encoding.Default.EncodingName == Encoding.UTF8.EncodingName && Encoding.Default.CodePage == Encoding.UTF8.CodePage)
            {
                OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            }
            else
            {
                OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            }
        }
        private void InitializeVectorDrivers()
        {
            Dictionary<string, string> nameAndExtensions = new Dictionary<string, string>
            {
                ["DXF"]=".dxf",
                ["GeoJSON"] = ".geojson",
                ["GML"] = ".gml",
                ["KML"] = ".kml",
                ["ESRI Shapefile"] = ".shp",
                ["SQLite"] = ".sqlite"
            };
            foreach (var item in nameAndExtensions)
            {
                var driver = Ogr.GetDriverByName(item.Key);
                if (driver == null)
                {
                    Debug.WriteLine($"{nameof(InitializeVectorDrivers)}获取驱动 {item.Key} 失败");
                    continue;
                }
                var vectorDriver = new GdalVectorDriver(driver, item.Value);
                Drivers.Add(vectorDriver);
            }
            var vectorDriverCount = Ogr.GetDriverCount();
            for (int i = 0; i < vectorDriverCount; i++)
            {
                var driver = Ogr.GetDriver(i);
                if (Drivers.Any(x => x.Name == driver.name))
                {
                    continue;
                }
                var vectorDriver = new GdalVectorDriver(driver);
                Drivers.Add(vectorDriver);
            }
        }
        private void InitializeRasterDrivers()
        {
            Dictionary<string, string> nameAndExtensions = new Dictionary<string, string>
            {
                ["BMP"] = ".bmp",
                ["ENVI"] = ".hdr",
                ["GIF"] = ".gif",
                ["GPKG"] = ".gpkg",
                ["GTiff"] = ".tif",
                ["HFA"] = ".img",
                ["JPEG"] = ".jpg",
                ["MBTiles"] = ".mbtiles",
                ["PDF"] = ".pdf",
                ["PNG"] = ".png",
                ["SQLite"] = ".sqlite",
                ["PNG"] = ".png"
            };
            foreach (var item in nameAndExtensions)
            {
                var driver = Gdal.GetDriverByName(item.Key);
                if (driver == null)
                {
                    Debug.WriteLine($"{nameof(InitializeRasterDrivers)}获取驱动 {item.Key} 失败");
                    continue;
                }
                var rasterDriver = new GdalRasterDriver(driver, item.Value);
                Drivers.Add(rasterDriver);
            }
            var driverCount = Gdal.GetDriverCount();
            for (int i = 0; i < driverCount; i++)
            {
                var driver = Gdal.GetDriver(i);
                if (Drivers.Any(x => x.Name == driver.ShortName))
                {
                    continue;
                }
                var rasterDriver = new GdalRasterDriver(driver);
                Drivers.Add(rasterDriver);
            }
        }
        /// <inheritdoc/>
        public IFeatureSet CreateFeatureSet(string name, FeatureType featureType)
        {
            IFeatureSet ret = new GdalFeatureSet(name, featureType);
            return ret;
        }

        /// <inheritdoc/>
        public bool Delete(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }
            var srcDirectory = Path.GetDirectoryName(path);
            var srcName = Path.GetFileNameWithoutExtension(path);
            var files = Directory.GetFiles(srcDirectory, $"{srcName}.*");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            return true;
        }
        /// <inheritdoc/>
        public IDataSet? OpenDataSet(string path, ProgressDelegate? progress = null)
        {
            IDataSet? ret = OpenFeatureSet(path, progress);
            if (ret == null)
            {
                ret = OpenRasterSet(path, progress);
            }
            return ret;
        }

        /// <inheritdoc/>
        public IFeatureSet? OpenFeatureSet(string path, ProgressDelegate? progress = null)
        {
            DataSource? dataSource = null;
            try
            {
                dataSource = Ogr.Open(path, 1);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(OpenFeatureSet)} 读写方式打开 {path} 失败,{e}");
                try
                {
                    dataSource = Ogr.Open(path, 0);
                }
                catch (Exception e1)
                {
                    Debug.WriteLine($"{nameof(OpenFeatureSet)} 只读方式打开 {path} 失败,{e1}");
                }
            }
            IFeatureSet? ret = null;
            if (dataSource != null)
            {
                Layer? layer = null;
                if (dataSource.GetLayerCount() > 0)
                {
                    layer = dataSource.GetLayerByIndex(0);
                }
                ret = new GdalFeatureSet(path, dataSource, layer);
            }
            return ret;
        }

        /// <inheritdoc/>
        public IRasterSet? OpenRasterSet(string path, ProgressDelegate? progress = null)
        {
            Dataset? dataset = null;
            try
            {
                dataset = Gdal.Open(path, Access.GA_Update);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(OpenRasterSet)} 读写方式打开 {path} 失败,{e}");
                try
                {
                    dataset = Gdal.Open(path, Access.GA_ReadOnly);
                }
                catch (Exception e1)
                {
                    Debug.WriteLine($"{nameof(OpenRasterSet)} 只读方式打开 {path} 失败,{e1}");
                }
            }
            IRasterSet? ret = null;
            if (dataset != null)
            {
                ret = dataset.GetRasterSet(path );
            }
            return ret;
        }

        /// <inheritdoc/>
        public bool RenameDataSet(string path, string newName)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path) || string.IsNullOrEmpty(newName))
            {
                return false;
            }
            var srcDirectory = Path.GetDirectoryName(path);
            var srcName = Path.GetFileNameWithoutExtension(path);
            var files = Directory.GetFiles(srcDirectory, $"{srcName}.*");
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                var destFile = Path.Combine(path, $"{newName}{extension}");
                File.Move(file, destFile);
            }
            return true;
        }

        /// <inheritdoc/>
        public bool Copy(string srcFileName, string destFileName)
        {
            if (string.IsNullOrEmpty(srcFileName)||!File.Exists(srcFileName) || string.IsNullOrEmpty(destFileName))
            {
                return false;
            }
            var destDirectory=Path.GetDirectoryName(destFileName);
            var destName = Path.GetFileNameWithoutExtension(destFileName);
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            var srcDirectory= Path.GetDirectoryName(srcFileName);
            var srcName = Path.GetFileNameWithoutExtension(srcFileName);
            var files= Directory.GetFiles(srcDirectory, $"{srcName}.*");
            foreach (var file in files ) 
            {
                var extension= Path.GetExtension(file);
                var destFile = Path.Combine(destDirectory, $"{destName}{extension}");
                File.Copy(file, destFile, true);
            }
            return true;
        }
    }
}
