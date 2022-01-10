using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    public static class DatasetExtensions
    {
        /// <summary>
        /// 尝试以读写打开数据，若失败则以只读打开
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Dataset Open(string fileName)
        {
            Dataset dataset = null;
            if (File.Exists(fileName))
            {
                try
                {
                    dataset = Gdal.Open(fileName, Access.GA_Update);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"以读写打开“{fileName}”失败，将尝试以只读打开，错误信息：{e}");
                    try
                    {
                        dataset = Gdal.Open(fileName, Access.GA_ReadOnly);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"以只读打开“{fileName}”失败，错误信息：{ex}");
                    }
                }
            }
            return dataset;
        }

        public static int CreateOverview(this Dataset dataset, string resampling = "NEAREST", int[] overviewlist = null)
        {
            int value = -1;
            if (dataset == null || dataset.RasterCount <= 0)
            {
                return value;
            }

            if (overviewlist == null)
            {
                List<int> intList = new List<int>();
                int width = dataset.RasterXSize;
                int height = dataset.RasterYSize;
                int k = 1;
                while (width > 256 && height > 256)
                {
                    k *= 2;
                    intList.Add(k);
                    width /= 2;
                    height /= 2;
                }

                overviewlist = intList.ToArray();
            }

            value = dataset.BuildOverviews(resampling, overviewlist);
            return value;
        }
    }
}
