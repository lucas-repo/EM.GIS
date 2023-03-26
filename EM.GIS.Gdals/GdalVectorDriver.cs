using EM.GIS.Data;
using EM.GIS.GdalExtensions;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 矢量数据驱动类
    /// </summary>
    public class GdalVectorDriver : Driver, IVectorDriver
    {
        readonly OSGeo.OGR.Driver driver;
        /// <summary>
        /// 初始化矢量驱动
        /// </summary>
        /// <param name="driver">ogr矢量驱动</param>
        /// <param name="extension">扩展</param>
        public GdalVectorDriver(OSGeo.OGR.Driver driver, string extension="")
        {
            this.driver = driver;
            Name = driver.name;
            if (!string.IsNullOrEmpty(extension))
            {
                Extensions = extension;
                Filter = $"*{extension}|*{extension}";
            }
        }
        /// <inheritdoc/>
        public override IDataSet? Open(string path)
        {
            return (this as IVectorDriver).Open(path);
        }

        /// <inheritdoc/>
        IFeatureSet? IVectorDriver.Open(string path)
        {
            OSGeo.OGR.DataSource? dataSource = null;
            try
            {
                dataSource = driver.Open(path, 1);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(Open)} 读写方式打开 {path} 失败,{e}");
                try
                {
                    dataSource = driver.Open(path, 0);
                }
                catch (Exception e1)
                {
                    Debug.WriteLine($"{nameof(Open)} 只读方式打开 {path} 失败,{e1}");
                }
            }
            IFeatureSet? ret = null;
            if (dataSource != null)
            {
                OSGeo.OGR.Layer? layer = null;
                if (dataSource.GetLayerCount() > 0)
                {
                    layer = dataSource.GetLayerByIndex(0);
                }
                ret = new GdalFeatureSet(path, dataSource, layer);
            }
            return ret;
        }
    }
}
