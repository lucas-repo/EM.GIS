using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据集工厂扩展类
    /// </summary>
    public static class DataSetFactoryExtensions
    {
        /// <summary>
        /// 获取矢量驱动集合
        /// </summary>
        /// <param name="dataSetFactory">数据集工厂</param>
        /// <returns>矢量驱动集合</returns>
        public static IEnumerable<IVectorDriver> GetVectorDrivers(this IDataSetFactory dataSetFactory)
        {
            foreach (var driver in dataSetFactory.Drivers)
            {
                if (driver is IVectorDriver vectorDriver)
                {
                    yield return vectorDriver;
                }
            }
        }

        /// <summary>
        /// 获取栅格驱动集合
        /// </summary>
        /// <param name="dataSetFactory">数据集工厂</param>
        /// <returns>栅格驱动集合</returns>
        public static IEnumerable<IRasterDriver> GetRasterDrivers(this IDataSetFactory dataSetFactory)
        {
            foreach (var driver in dataSetFactory.Drivers)
            {
                if (driver is IRasterDriver rasterDriver)
                {
                    yield return rasterDriver;
                }
            }
        }
        /// <summary>
        /// 获取打开文件窗口的过滤字符串
        /// </summary>
        /// <param name="dataSetFactory">数据集工厂</param>
        /// <returns>过滤字符串</returns>
        public static string GetFilter(this IDataSetFactory dataSetFactory)
        {
            StringBuilder sb=new StringBuilder();
            var vectorDrivers = dataSetFactory.GetVectorDrivers();
            var vectorFilter = GetFilterString(vectorDrivers);
            if (!string.IsNullOrEmpty(vectorFilter))
            {
                sb.Append($"矢量数据|{vectorFilter}");
            }
            var rasterDrivers = dataSetFactory.GetRasterDrivers();
            var rasterFilter = GetFilterString(rasterDrivers);
            if (!string.IsNullOrEmpty(rasterFilter))
            {
                if (sb.Length == 0)
                {
                    sb.Append($"栅格数据|{rasterFilter}");
                }
                else
                {
                    sb.Append($"|栅格数据|{rasterFilter}");
                }
            }
            return sb.ToString();
        }
        private static string GetFilterString(IEnumerable<IDriver> drivers)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < drivers.Count(); i++)
            {
                var extensions = drivers.ElementAt(i).Extensions;
                if (string.IsNullOrEmpty(extensions))
                {
                    continue;
                }
                if (i == 0)
                {
                    sb.Append($"*{extensions}");
                }
                else
                {
                    sb.Append($";*{extensions}");
                }
            }
            return sb.ToString();
        }
    }
}
