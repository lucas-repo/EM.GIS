using EM.GIS.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

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
                foreach (var item in extensions)
                {
                    sb.Append($"*{item}");
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
                foreach (var item in extensions)
                {
                    sb.Append($"*{item}");
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
        /// This launches an open file dialog and attempts to load the specified file.
        /// </summary>
        /// <param name="self">this</param>
        /// <returns>An IDataSet with the data from the file specified in an open file dialog</returns>
        public static IDataSet OpenFile(this IDriverFactory self)
        {
            var ofd = new OpenFileDialog { Filter = self.GetFilter() };
            var ret = ofd.ShowDialog();
            if (!ret.HasValue || !ret.Value)
            { return null; }
            return self.Open(ofd.FileName);
        }

        /// <summary>
        /// This launches an open file dialog that allows loading of several files at once
        /// and returns the datasets in a list.
        /// </summary>
        /// <param name="self">this</param>
        /// <returns>An enumerable of all the files that were opened.</returns>
        public static IEnumerable<IDataSet> OpenFiles(this IDriverFactory self)
        {
            var ofd = new OpenFileDialog { Multiselect = true, Filter = self.GetFilter() };
            var ret = ofd.ShowDialog();
            if (!ret.HasValue || !ret.Value) yield break;

            var filterparts = ofd.Filter.Split('|');
            var pos = (ofd.FilterIndex - 1) * 2;
            int index = filterparts[pos].IndexOf(" - ", StringComparison.Ordinal);
            foreach (var name in ofd.FileNames)
            {
                var ds = self.Open(name);
                if (ds != null) yield return ds;
            }

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
