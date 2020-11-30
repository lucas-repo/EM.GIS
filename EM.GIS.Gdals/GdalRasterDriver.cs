using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EM.GIS.Gdals
{
    public class GdalRasterDriver : Driver, IRasterDriver
    {
        static GdalRasterDriver()
        {
            GdalConfiguration.ConfigureGdal();
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
        public IRasterSet Create(string fileName, int xsize, int ysize, int bands, RasterType eType)
        {
            IRasterSet rasterSet = null;
            var driverCount = OSGeo.GDAL.Gdal.GetDriverCount();
            for (int i = 0; i < driverCount; i++)
            {
                using (var driver = OSGeo.GDAL.Gdal.GetDriver(i))
                {
                    var dataset = driver.Create(fileName, xsize, ysize, bands, eType.ToRasterType(), null);
                    if (dataset != null)
                    {
                        rasterSet = GetRasterSet(fileName, dataset);
                    }
                }
            }
            return rasterSet;
        }

        IRasterSet IRasterDriver.Open(string fileName, bool update)
        {
            OSGeo.GDAL.Dataset dataset = null;
            var access = update ? OSGeo.GDAL.Access.GA_Update : OSGeo.GDAL.Access.GA_ReadOnly;
            try
            {
                dataset = OSGeo.GDAL.Gdal.Open(fileName, access);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"打开“{fileName}”失败，错误信息：{e}");
            }
            IRasterSet rasterSet = GetRasterSet(fileName, dataset);
            return rasterSet;
        }
        private IRasterSet GetRasterSet(string fileName, OSGeo.GDAL.Dataset dataset)
        {
            IRasterSet rasterSet = null;
            if (dataset != null && dataset.RasterCount > 0)
            {
                using var band = dataset.GetRasterBand(1);
                if (band == null)
                {
                    return null;
                }
                switch (band.DataType)
                {
                    case OSGeo.GDAL.DataType.GDT_Byte:
                        rasterSet = new GdalRasterSet<byte>(fileName, dataset);
                        break;
                    case OSGeo.GDAL.DataType.GDT_Int16:
                        rasterSet = new GdalRasterSet<short>(fileName, dataset);
                        break;
                    case OSGeo.GDAL.DataType.GDT_UInt16:
                        rasterSet = new GdalRasterSet<ushort>(fileName, dataset);
                        break;
                    case OSGeo.GDAL.DataType.GDT_Int32:
                        rasterSet = new GdalRasterSet<int>(fileName, dataset);
                        break;
                    case OSGeo.GDAL.DataType.GDT_UInt32:
                        rasterSet = new GdalRasterSet<uint>(fileName, dataset);
                        break;
                    case OSGeo.GDAL.DataType.GDT_CFloat32:
                        rasterSet = new GdalRasterSet<float>(fileName, dataset);
                        break;
                    case OSGeo.GDAL.DataType.GDT_CFloat64:
                        rasterSet = new GdalRasterSet<double>(fileName, dataset);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return rasterSet;
        }
        public override bool CopyFiles(string srcFileName, string destFileName)
        {
            bool ret = false;
            using var ds = OSGeo.GDAL.Gdal.Open(srcFileName, 0);
            if (ds != null)
            {
                using var driver = ds.GetDriver();
                var error = driver.CopyFiles(destFileName, srcFileName);
                ret = error == OSGeo.GDAL.CPLErr.CE_None;
            }
            return ret;
        }

        public override IDataSet Open(string fileName, bool update)
        {
            return (this as IRasterDriver).Open(fileName, update);
        }
        public override bool Delete(string fileName)
        {
            bool ret = false;
            using var ds = OSGeo.GDAL.Gdal.Open(fileName, 0);
            if (ds != null)
            {
                using var driver = ds.GetDriver();
                var error = driver.Delete(fileName);
                ret = error == OSGeo.GDAL.CPLErr.CE_None;
            }
            return ret;
        }
        public override bool Rename(string srcFileName, string destFileName)
        {
            bool ret = false;
            using var ds = OSGeo.GDAL.Gdal.Open(srcFileName, 0);
            if (ds != null)
            {
                using var driver = ds.GetDriver();
                var error = driver.CopyFiles(destFileName, srcFileName);
                if (error == OSGeo.GDAL.CPLErr.CE_None)
                {
                    error = driver.Delete(srcFileName);
                    ret = error == OSGeo.GDAL.CPLErr.CE_None;
                }
            }
            return ret;
        }
        public override List<string> GetReadableFileExtensions()
        {
            List<string> extensions = new List<string>() 
            {
                ".asc",".adf",".bil",".gen",".thf",".blx",".xlb",".bt",".dt0",".dt1",".dt2",".tif",".dem",".ter",".mem",".img",".nc"
            };
            return extensions;
        }
        public override List<string> GetWritableFileExtensions()
        {
            List<string> extensions = new List<string>()
            {
                ".asc",".adf",".dt0",".dt1",".dt2",".tif",".dem",".ter",".bil",".mem",".img",".nc"
            };
            return extensions;
        }
    }
}
