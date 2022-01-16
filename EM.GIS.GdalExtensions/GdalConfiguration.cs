using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Gdal = OSGeo.GDAL.Gdal;
using Ogr = OSGeo.OGR.Ogr;

namespace EM.GIS.GdalExtensions
{
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
    /// <summary>
    /// gdal配置
    /// </summary>
    public static partial class GdalConfiguration
    {
        private static volatile bool _configuredOgr;
        private static volatile bool _configuredGdal;
        private static volatile bool _usable;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDefaultDllDirectories(uint directoryFlags);
        //               LOAD_LIBRARY_SEARCH_USER_DIRS | LOAD_LIBRARY_SEARCH_SYSTEM32
        private const uint DllSearchFlags = 0x00000400 | 0x00000800;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddDllDirectory(string lpPathName);

        //static GdalConfiguration()
        //{
        //    string executingDirectory = null, gdalPath = null, nativePath = null;
        //    try
        //    {
        //        if (!IsWindows)
        //        {
        //            const string notSet = "_Not_set_";
        //            string tmp = Gdal.GetConfigOption("GDAL_DATA", notSet);
        //            _usable = tmp != notSet;
        //            return;
        //        }

        //        string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
        //        executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

        //        if (string.IsNullOrEmpty(executingDirectory))
        //            throw new InvalidOperationException("cannot get executing directory");


        //        // modify search place and order
        //        SetDefaultDllDirectories(DllSearchFlags);

        //        gdalPath = Path.Combine(executingDirectory, "gdal");
        //        nativePath = Path.Combine(gdalPath, GetPlatform());
        //        if (!Directory.Exists(nativePath))
        //            throw new DirectoryNotFoundException($"GDAL native directory not found at '{nativePath}'");
        //        if (!File.Exists(Path.Combine(nativePath, "gdal_wrap.dll")))
        //            throw new FileNotFoundException(
        //                $"GDAL native wrapper file not found at '{Path.Combine(nativePath, "gdal_wrap.dll")}'");

        //        // Add directories
        //        AddDllDirectory(nativePath);
        //        AddDllDirectory(Path.Combine(nativePath, "plugins"));

        //        // Set the additional GDAL environment variables.
        //        string gdalData = Path.Combine(gdalPath, "data");
        //        Environment.SetEnvironmentVariable("GDAL_DATA", gdalData);
        //        Gdal.SetConfigOption("GDAL_DATA", gdalData);

        //        string driverPath = Path.Combine(nativePath, "plugins");
        //        Environment.SetEnvironmentVariable("GDAL_DRIVER_PATH", driverPath);
        //        Gdal.SetConfigOption("GDAL_DRIVER_PATH", driverPath);

        //        Environment.SetEnvironmentVariable("GEOTIFF_CSV", gdalData);
        //        Gdal.SetConfigOption("GEOTIFF_CSV", gdalData);

        //        string projSharePath = Path.Combine(gdalPath, "share");
        //        Environment.SetEnvironmentVariable("PROJ_LIB", projSharePath);
        //        Gdal.SetConfigOption("PROJ_LIB", projSharePath);

        //        _usable = true;
        //    }
        //    catch (Exception e)
        //    {
        //        _usable = false;
        //        Trace.WriteLine(e, "error");
        //        Trace.WriteLine($"Executing directory: {executingDirectory}", "error");
        //        Trace.WriteLine($"gdal directory: {gdalPath}", "error");
        //        Trace.WriteLine($"native directory: {nativePath}", "error");

        //        //throw;
        //    }
        //}
        static GdalConfiguration()
        {
            try
            {
                if (!IsWindows)
                {
                    const string notSet = "_Not_set_";
                    string tmp = Gdal.GetConfigOption("GDAL_DATA", notSet);
                    _usable = tmp != notSet;
                    return;
                }

                _usable = true;
            }
            catch (Exception e)
            {
                _usable = false;
                Trace.WriteLine(e, "error");
            }
        }
        /// <summary>
        /// 是否正确设置了GDAL包
        /// </summary>
        public static bool Usable
        {
            get { return _usable; }
        }
        /// <summary>
        /// 配置编码
        /// </summary>
        private static void ConfigEncoding()
        {
            // 为了支持中文路径，如果默认编码非UTF8，请添加下面这句代码
            if (Encoding.Default.EncodingName != Encoding.UTF8.EncodingName || Encoding.Default.CodePage != Encoding.UTF8.CodePage)
            {
                var filenameConfig = Gdal.GetConfigOption("GDAL_FILENAME_IS_UTF8", string.Empty);
                if (filenameConfig!= "NO")
                {
                    Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
                }
            }
        }
        /// <summary>
        /// 配置OGR
        /// </summary>
        /// <remarks>请确保在使用Gdal/Ogr/Osr之前调用此函数</remarks>
        public static void ConfigureOgr()
        {
            if (!_usable) return;
            if (_configuredOgr) return;

            // Register drivers
            Ogr.RegisterAll();
            ConfigEncoding();
            //var shapeEncoding = Gdal.GetConfigOption("SHAPE_ENCODING", string.Empty);
            //if (shapeEncoding==string.Empty)
            //{
            //    Gdal.SetConfigOption("SHAPE_ENCODING", "");//网上有些说这里用NO，但我亲测必须用YES才能识别中文路径// 为了使属性表字段支持中文
            //    //Gdal.SetConfigOption("SHAPE_ENCODING", "GB18030");//网上说这里不填，反正我是没有测试成功，也填CP936，写SHP能行，但是读的话就就是问题，不全，如“张三”读取出来为“张？”后一位识别不了。
            //}
            _configuredOgr = true;

            PrintDriversOgr();
        }

        /// <summary>
        /// 配置GDAL
        /// </summary>
        /// <remarks>请确保在使用Gdal/Ogr/Osr之前调用此函数</remarks>
        public static void ConfigureGdal()
        {
            if (!_usable) return;
            if (_configuredGdal) return;

            // Register drivers
            Gdal.AllRegister();
            _configuredGdal = true;

            PrintDriversGdal();
        }


        /// <summary>
        /// 获取是x64还是x86系统
        /// </summary>
        private static string GetPlatform()
        {
            return Environment.Is64BitProcess ? "x64" : "x86";
        }

        /// <summary>
        /// 获取是否为Windows系统
        /// </summary>
        private static bool IsWindows
        {
            get
            {
                var res = !(Environment.OSVersion.Platform == PlatformID.Unix ||
                            Environment.OSVersion.Platform == PlatformID.MacOSX);

                return res;
            }
        }
        private static void PrintDriversOgr()
        {
#if DEBUG
            if (_usable)
            {
                var num = Ogr.GetDriverCount();
                for (var i = 0; i < num; i++)
                {
                    var driver = Ogr.GetDriver(i);
                    Trace.WriteLine($"OGR {i}: {driver.GetName()}", "Debug");
                }
            }
#endif
        }

        private static void PrintDriversGdal()
        {
#if DEBUG
            if (_usable)
            {
                var num = Gdal.GetDriverCount();
                for (var i = 0; i < num; i++)
                {
                    var driver = Gdal.GetDriver(i);
                    Trace.WriteLine($"GDAL {i}: {driver.ShortName}-{driver.LongName}");
                }
            }
#endif
        }
    }
}
