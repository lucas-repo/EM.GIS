using EM.GIS.Data;
using EM.GIS.Symbology;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// Extend the data manager with some convenient dialog spawning options.
    /// </summary>
    public static class DataManagerExt
    {
        public static string GetVectorFilter(this IDriverFactory driverFactory)
        {
            StringBuilder sb = new StringBuilder();
            List<string> extensions = driverFactory.GetVectorReadableFileExtensions();
            if (extensions.Count > 0)
            {
                sb.Append("矢量数据|");
                for (int i = 0; i < extensions.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append($"*{extensions[i]}");
                    }
                    else
                    {
                        sb.Append($";*{extensions[i]}");
                    }
                }
            }
            return sb.ToString();
        }
        public static string GetRasterFilter(this IDriverFactory driverFactory)
        {
            StringBuilder sb = new StringBuilder();
            List<string> extensions = driverFactory.GetRasterReadableFileExtensions();
            if (extensions.Count > 0)
            {
                sb.Append("栅格数据|");
                for (int i = 0; i < extensions.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append($"*{extensions[i]}");
                    }
                    else
                    {
                        sb.Append($";*{extensions[i]}");
                    }
                }
            }
            return sb.ToString();
        }
        public static string GetFilter(this IDriverFactory driverFactory)
        {
            var vectorFilter = driverFactory.GetVectorFilter();
            var rasterFilter = driverFactory.GetRasterFilter();
            string filter= vectorFilter;
            if (string .IsNullOrEmpty(filter))
            {
                filter = rasterFilter;
            }
            else
            {
                if (!string.IsNullOrEmpty(rasterFilter))
                {
                    filter = $"{filter}|{rasterFilter}";
                }
            }
            return filter;
        }
        /// <summary>
        /// This opens a file, but populates the dialog filter with only vector formats.
        /// </summary>
        /// <param name="self">this</param>
        /// <returns>An IFeatureSet with the data from the file specified in a dialog, or null if nothing load.</returns>
        public static IFeatureSet OpenVector(this IDriverFactory self)
        {
            IFeatureSet featureSet = null;
            var ofd = new OpenFileDialog { Filter = self.GetVectorFilter() };

            var ret = ofd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                featureSet = self.OpenVector(ofd.FileName);
            }
            return featureSet;
        }

        /// <summary>
        /// This uses an open dialog filter with only vector extensions but where multi-select is
        /// enabled, hence allowing multiple vectors to be returned in this list.
        /// </summary>
        /// <param name="self">this</param>
        /// <returns>The enumerable or vectors.</returns>
        public static IEnumerable<IFeatureSet> OpenVectors(this IDriverFactory self)
        {
            var ofd = new OpenFileDialog { Filter = self.GetVectorFilter(), Multiselect = true };
            var ret = ofd.ShowDialog();
            if (!ret.HasValue || !ret.Value)
            {
                yield break;
            }
            foreach (var name in ofd.FileNames)
            {
                var ds = self.OpenVector(name);
                if (ds != null) yield return ds;
            }
        }

        /// <summary>
        /// 打开数据集
        /// </summary>
        /// <param name="driverFactory">驱动工厂</param>
        /// <returns>数据集</returns>
        public static IDataSet? OpenFile(this IDriverFactory driverFactory)
        {
            IDataSet? ret = null;
            if (driverFactory == null)
            {
                return null;
            }
            var ofd = new OpenFileDialog 
            {
                Filter = driverFactory.GetFilter()
            };
            if (ofd.ShowDialog() == true)
            {
                ret = driverFactory.Open(ofd.FileName);
            }
            return ret;
        }

        /// <summary>
        /// 打开多个数据集
        /// </summary>
        /// <param name="driverFactory">驱动工厂</param>
        /// <param name="owner">父窗体</param>
        /// <returns>多个数据集</returns>
        public static List<IDataSet> OpenFiles(this IDriverFactory driverFactory,Window? owner=null)
        {
            List<IDataSet> ret = new List<IDataSet>();
            if (driverFactory == null)
            {
                return ret;
            }
            var ofd = new OpenFileDialog
            { 
                Multiselect = true,
                Filter = driverFactory.GetFilter() 
            };
            bool? dialogResult;
            if (owner == null)
            {
                dialogResult = ofd.ShowDialog();
            }
            else
            {
                dialogResult = ofd.ShowDialog(owner);
            }
            if (dialogResult == true)
            {
                foreach (var name in ofd.FileNames)
                {
                    var ds = driverFactory.Open(name);
                    if (ds != null)
                    {
                        ret.Add(ds);
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 添加多个图层
        /// </summary>
        /// <param name="frame">地图框架</param>
        /// <param name="driverFactory">驱动工厂</param>
        /// <param name="owner">父窗体</param>
        /// <returns>已添加的图层枚举</returns>
        public static List<ILayer> AddLayers(this IFrame frame, IDriverFactory driverFactory, Window? owner = null)
        {
            List<ILayer> ret = new List<ILayer>();
            if (frame != null && driverFactory != null)
            {
                var dataSets = driverFactory.OpenFiles(owner);
                if (dataSets.Count>0)
                {
                    var layers = frame.GetAllLayers().Where(x => x.IsSelected && x is IGroup);
                    IGroup? destGroup = null;
                    if (layers.Count() == 1 && layers.First() is IGroup group)
                    {
                        destGroup = group;
                    }
                    else
                    {
                        destGroup = frame;
                    }
                    foreach (var dataSet in dataSets) 
                    {
                        if (dataSet != null)
                        {
                            var layer = destGroup.AddLayer(dataSet);
                            if (layer != null)
                            {
                                ret.Add(layer);
                            }
                        }
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// This opens a file, but populates the dialog filter with only raster formats.
        /// </summary>
        /// <param name="self">this</param>
        /// <returns>An IRaster with the data from the file specified in an open file dialog</returns>
        public static IRasterSet OpenRaster(this IDriverFactory self)
        {
            var ofd = new OpenFileDialog { Filter = self.GetRasterFilter() };
            var ret = ofd.ShowDialog();
            if (!ret.HasValue || !ret.Value) return null;
            return self.Open(ofd.FileName) as IRasterSet;
        }

        /// <summary>
        /// This uses an open dialog filter with only raster extensions but where multi-select is
        /// enabled, hence allowing multiple rasters to be returned in this list.
        /// </summary>
        /// <param name="self">this</param>
        /// <returns>An enumerable or rasters.</returns>
        public static IEnumerable<IRasterSet> OpenRasters(this IDriverFactory self)
        {
            var ofd = new OpenFileDialog { Filter = self.GetRasterFilter(), Multiselect = true };
            var ret = ofd.ShowDialog();
            if (!ret.HasValue || !ret.Value) yield break;
            foreach (var name in ofd.FileNames)
            {
                var ds = self.OpenRaster(name);
                if (ds != null) yield return ds;
            }
        }
    }
}
