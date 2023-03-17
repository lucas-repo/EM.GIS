using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 数据集扩展类
    /// </summary>
    public static class DataSetExtensions
    {
        /// <summary>
        /// 获取<see cref="IRasterSet"/>
        /// </summary>
        /// <param name="dataset">gdal栅格数据集</param>
        /// <param name="fileName">文件名</param>
        /// <returns>栅格数据集</returns>
        /// <exception cref="NotImplementedException">不支持的类型</exception>
        public static IRasterSet? GetRasterSet(this OSGeo.GDAL.Dataset dataset, string fileName )
        {
            IRasterSet? rasterSet = null;
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
    }
}
