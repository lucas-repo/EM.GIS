using EM.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EM.GIS.Data
{
    /// <summary>
    /// 驱动工厂
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IDriverFactory))]
    public class DriverFactory : IDriverFactory
    {
        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IEnumerable<IDriver> Drivers { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IVectorDriver> VectorDrivers => Drivers.Where(x => x is IVectorDriver).Select(x => x as IVectorDriver);

        /// <inheritdoc/>
        public IEnumerable<IRasterDriver> RasterDrivers => Drivers.Where(x => x is IRasterDriver).Select(x => x as IRasterDriver);
        private ProgressDelegate _progress;

        /// <inheritdoc/>
        [Category("Handlers")]
        [Description("Gets or sets the object that implements the IProgressHandler interface for recieving status messages.")]
        public ProgressDelegate Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                if (Drivers != null)
                {
                    foreach (var item in Drivers)
                    {
                        item.Progress = value;
                    }
                }
            }
        }
        public DriverFactory(IEnumerable<IDriver> drivers)
        {
            Drivers = drivers;
        }
        public IRasterSet OpenRaster(string fileName)
        {
            IRasterSet dataSet = null;
            if (Drivers != null)
            {
                foreach (var driver in RasterDrivers)
                {
                    try
                    {
                        dataSet = driver.Open(fileName, true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        try
                        {
                            dataSet = driver.Open(fileName, false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    if (dataSet != null)
                    {
                        break;
                    }
                }
            }
            return dataSet;
        }

        public IFeatureSet OpenVector(string fileName)
        {
            IFeatureSet dataSet = null;
            if (Drivers != null)
            {
                foreach (var driver in VectorDrivers)
                {
                    try
                    {
                        dataSet = driver.Open(fileName, true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        try
                        {
                            dataSet = driver.Open(fileName, false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    if (dataSet != null)
                    {
                        break;
                    }
                }
            }
            return dataSet;
        }


        public IDataSet Open(string path)
        {
            IDataSet dataSet = null;
            if (string.IsNullOrEmpty(path))
            {
                return dataSet;
            }
            if (Drivers != null)
            {
                foreach (var driver in Drivers)
                {
                    try
                    {
                        dataSet = driver.Open(path);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    if (dataSet != null)
                    {
                        break;
                    }
                }
            }
            return dataSet;
        }

        public List<string> GetVectorReadableFileExtensions()
        {
            List<string> extensions = new List<string>();
            foreach (var item in VectorDrivers)
            {
                extensions.AddRange(item.GetReadableFileExtensions());
            }
            return extensions;
        }

        public List<string> GetRasterWritableFileExtensions()
        {
            List<string> extensions = new List<string>();
            foreach (var item in RasterDrivers)
            {
                extensions.AddRange(item.GetWritableFileExtensions());
            }
            return extensions;
        }

        public List<string> GetRasterReadableFileExtensions()
        {
            List<string> extensions = new List<string>();
            foreach (var item in RasterDrivers) 
            {
                extensions.AddRange(item.GetReadableFileExtensions());
            }
            return extensions;
        }

        public List<string> GetVectorWritableFileExtensions()
        {
            List<string> extensions = new List<string>();
            foreach (var item in VectorDrivers)
            {
                extensions.AddRange(item.GetWritableFileExtensions());
            }
            return extensions;
        }
    }
}