using EM.GIS.Data;
using System;
using System.Collections.Generic;
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
        static GdalVectorDriver()
        {
            GdalConfiguration.ConfigureOgr();
            // 为了使属性表字段支持中文，请添加下面这句  
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
        }
        public override bool CopyFiles(string srcFileName, string destFileName)
        {
            bool ret = false;
            using var ds = OSGeo.OGR.Ogr.Open(srcFileName, 0);
            if (ds != null)
            {
                using var driver = ds.GetDriver();
                using var destDs = driver.CopyDataSource(ds, destFileName, null);
                ret = destDs != null;
            }
            return ret;
        }

        IFeatureSet IVectorDriver.Open(string fileName, bool update)
        {
            IFeatureSet featureSet = null;
            int updateValue = update ? 1 : 0;
            var ds = OSGeo.OGR.Ogr.Open(fileName, updateValue);
            if (ds != null)
            {
                using var driver = ds.GetDriver();
                switch (driver.name)
                {
                    case "ESRI Shapefile":
                        featureSet = new GdalFeatureSet(fileName, ds);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return featureSet;
        }
        public override IDataSet Open(string fileName, bool update)
        {
            return (this as IVectorDriver).Open(fileName, update);
        }
        public override bool Delete(string fileName)
        {
            bool ret = false;
            using var ds = OSGeo.OGR.Ogr.Open(fileName, 0);
            if (ds != null)
            {
                using var driver = ds.GetDriver();
                ret = driver.DeleteDataSource(fileName) == 1;
            }
            return ret;
        }
        public override bool Rename(string srcFileName, string destFileName)
        {
            bool ret = false;
            using var ds = OSGeo.OGR.Ogr.Open(srcFileName, 0);
            if (ds != null)
            {
                using var driver = ds.GetDriver();
                using var destDs = driver.CopyDataSource(ds, destFileName, null);
                if (destDs != null)
                {
                    ret = driver.DeleteDataSource(srcFileName) == 1;
                }
            }
            return ret;
        }
        public override List<string> GetReadableFileExtensions()
        {
            List<string> extensions = new List<string>()
            {
                ".shp",".kml",".dxf"
            };
            return extensions;
        }
        public override List<string> GetWritableFileExtensions()
        {
            List<string> extensions = new List<string>()
            {
                ".shp",".kml",".dxf"
            };
            return extensions;
        }
    }
}
