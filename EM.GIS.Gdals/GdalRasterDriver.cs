using EM.GIS.Data;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// gdal栅格驱动
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IDriver))]
    public class GdalRasterDriver : Driver, IRasterDriver
    {
        OSGeo.GDAL.Driver driver;
        /// <summary>
        /// 初始化栅格驱动
        /// </summary>
        /// <param name="driver">gdal栅格驱动</param>
        /// <param name="extension">扩展</param>
        public GdalRasterDriver(OSGeo.GDAL.Driver driver, string extension = "")
        {
            this.driver = driver;
            Name = driver.ShortName;
            if (!string.IsNullOrEmpty(extension))
            {
                Extensions=extension;
                Filter = $"*{extension}|*{extension}";
            }
        }

        /// <inheritdoc/>
        public IRasterSet? Create(string filename, int width, int height, int bandCount, RasterType rasterType, string[]? options = null)
        {
            IRasterSet? rasterSet = null;
            var driverCount = OSGeo.GDAL.Gdal.GetDriverCount();
            for (int i = 0; i < driverCount; i++)
            {
                using (var driver = OSGeo.GDAL.Gdal.GetDriver(i))
                {
                    var dataset = driver.Create(filename,  width,  height,  bandCount,  rasterType.ToRasterType(),options);
                    if (dataset != null)
                    {
                        rasterSet = dataset.GetRasterSet(filename );
                    }
                }
            }
            return rasterSet;
        }

        public override IDataSet? Open(string path)
        {
            return (this as IRasterDriver).Open(path);
        }

        IRasterSet? IRasterDriver.Open(string path)
        {
            OSGeo.GDAL.Dataset? dataset = null; 
            try
            {
                dataset = OSGeo.GDAL.Gdal.Open(path,  OSGeo.GDAL.Access.GA_Update);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(Open)} 读写方式打开 {path} 失败,{e}");
                try
                {
                    dataset = OSGeo.GDAL.Gdal.Open(path, OSGeo.GDAL.Access.GA_ReadOnly);
                }
                catch (Exception e1)
                {
                    Debug.WriteLine($"{nameof(Open)} 只读方式打开 {path} 失败,{e1}");
                }
            }
            IRasterSet? ret = null;
            if (dataset != null)
            {
                ret = dataset.GetRasterSet(path);
            }
            return ret;
        }
    }
}
