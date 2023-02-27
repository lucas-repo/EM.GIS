using OSGeo.OGR;
using System;
using System.IO;
using System.Linq;

namespace EM.GIS.GdalExtensions
{
    /// <summary>
    /// 驱动扩展
    /// </summary>
    public static class DriverExtensions
    {
        /// <summary>
        /// 复制数据源(解决写入中文乱码)
        /// </summary>
        /// <param name="driver">驱动</param>
        /// <param name="srcDataSource">原始数据源</param>
        /// <param name="path">目录</param>
        /// <param name="options">可选项</param>
        /// <returns>新的数据源</returns>
        public static DataSource CopyDataSourceUTF8(this Driver driver, DataSource srcDataSource, string path, string[] options)
        {
            DataSource destDataSource = null;
            if (driver != null && srcDataSource != null && !string.IsNullOrEmpty(path))
            {
                string[] destOptions = GetOptionsWithUTF8(options);
                if (srcDataSource == null)
                {
                    destDataSource = driver.CreateDataSource(path, destOptions);
                }
                else
                {
                    destDataSource = driver.CopyDataSource(srcDataSource, path, destOptions);
                }
            }
            return destDataSource;
        }
        /// <summary>
        /// 获取包含UTF8编码的配置
        /// </summary>
        /// <param name="options">原有配置</param>
        /// <returns>新的配置</returns>
        public static string[] GetOptionsWithUTF8(this string[] options)
        {
            string encodingStr = "ENCODING=UTF-8";//配置增加编码，添加.cpg文件，以解决写入中文乱码
            string[] destOptions = options;
            if (destOptions == null)
            {
                destOptions = new string[] { encodingStr };
            }
            else
            {
                if (!destOptions.Contains(encodingStr))
                {
                    destOptions = new string[options.Length + 1];
                    Array.Copy(options, destOptions, options.Length);
                    destOptions[destOptions.Length - 1] = encodingStr;
                }
            }
            return destOptions;
        }
        /// <summary>
        /// 创建数据源(解决写入中文乱码)
        /// </summary>
        /// <param name="driver">驱动</param>
        /// <param name="path">路径</param>
        /// <param name="options">可选项</param>
        /// <returns>新的数据源</returns>
        public static DataSource CreateDataSourceUTF8(this Driver driver, string path, string[] options)
        {
            DataSource destDataSource = null;
            if (driver != null && !string.IsNullOrEmpty(path))
            {
                var destOptions = GetOptionsWithUTF8(options);
                destDataSource = driver.CreateDataSource(path, destOptions);
            }
            return destDataSource;
        }
        /// <summary>
        /// 删除数据源
        /// </summary>
        /// <param name="path">路径</param>
        public static void DeleteDataSource(string path)
        {
            if (File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                if (directory != null)
                {
                    var files = Directory.GetFiles(directory, $"{name}.*");
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                }
            }
        }
        /// <summary>
        /// 根据扩展名获取驱动
        /// </summary>
        /// <param name="extensions">扩展名</param>
        /// <returns>驱动</returns>
        public static OSGeo.GDAL.Driver GetGdalDriverByExtensions(this string extensions)
        {
            var driverName = extensions.ToUpper().Replace(".", "");
            switch (driverName)
            {
                case "JPG":
                    driverName = "JPEG";
                    break;
                case "TIF":
                case "TIFF":
                    driverName = "GTiff";
                    break;
            }
            var ret = OSGeo.GDAL.Gdal.GetDriverByName(driverName);
            return ret;
        }

    }
}
