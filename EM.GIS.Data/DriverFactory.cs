using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace EM.GIS.Data
{
    /// <summary>
    /// 驱动工厂
    /// </summary>
    public class DriverFactory : IDriverFactory
    {
        [Browsable(false)]
        [ImportMany(AllowRecomposition = true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IEnumerable<IDriver> Drivers { get; set; }

        public IEnumerable<IVectorDriver> VectorDrivers => Drivers.Where(x => x is IVectorDriver).Select(x => x as IVectorDriver);

        public IEnumerable<IRasterDriver> RasterDrivers => Drivers.Where(x => x is IRasterDriver).Select(x => x as IRasterDriver);
        private IProgressHandler _progressHandler;

        [Category("Handlers")]
        [Description("Gets or sets the object that implements the IProgressHandler interface for recieving status messages.")]
        public IProgressHandler ProgressHandler
        {
            get { return _progressHandler; }
            set
            {
                _progressHandler = value;
                if (Drivers != null)
                {
                    foreach (var item in Drivers)
                    {
                        item.ProgressHandler = value;
                    }
                }
            }
        }
        public DriverFactory()
        {
            Drivers = new List<IDriver>();
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
            if (Drivers != null)
            {
                foreach (var driver in Drivers)
                {
                    try
                    {
                        dataSet = driver.Open(path, true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        try
                        {
                            dataSet = driver.Open(path, false);
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