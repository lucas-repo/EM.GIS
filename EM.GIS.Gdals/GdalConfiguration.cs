/******************************************************************************
 *
 * Name:     GdalConfiguration.cs.pp
 * Project:  GDAL CSharp Interface
 * Purpose:  A static configuration utility class to enable GDAL/OGR.
 * Author:   Felix Obermaier
 *
 ******************************************************************************
 * Copyright (c) 2012-2018, Felix Obermaier
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

using EM.GIS.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Gdal = OSGeo.GDAL.Gdal;
using Ogr = OSGeo.OGR.Ogr;

namespace EM.GIS.Gdals
{
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
        static string _executingDirectory;
        public static string GdalPath { get; }
        static string _nativePath;
        static string _driverPath;
        static string[] _directories;
        /// <summary>
        /// Construction of Gdal/Ogr
        /// </summary>
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

                string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
                _executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

                if (string.IsNullOrEmpty(_executingDirectory))
                    throw new InvalidOperationException("cannot get executing directory");


                // modify search place and order
                SetDefaultDllDirectories(DllSearchFlags);

                GdalPath = Path.Combine(_executingDirectory, "gdal");
                _nativePath = GdalPath;
                if (!Directory.Exists(_nativePath))
                    throw new DirectoryNotFoundException($"GDAL native directory not found at '{_nativePath}'");
                if (!File.Exists(Path.Combine(_nativePath, "gdal_wrap.dll")))
                    throw new FileNotFoundException(
                        $"GDAL native wrapper file not found at '{Path.Combine(_nativePath, "gdal_wrap.dll")}'");

                // Add directories
                AddDllDirectory(_nativePath);
                _driverPath = Path.Combine(_nativePath, "plugins");
                AddDllDirectory(_driverPath);

                // Set the additional GDAL environment variables.
                string gdalData = Path.Combine(GdalPath, "data");
                Environment.SetEnvironmentVariable("GDAL_DATA", gdalData);
                Gdal.SetConfigOption("GDAL_DATA", gdalData);

                Environment.SetEnvironmentVariable("GDAL_DRIVER_PATH", _driverPath);
                Gdal.SetConfigOption("GDAL_DRIVER_PATH", _driverPath);

                Environment.SetEnvironmentVariable("GEOTIFF_CSV", gdalData);
                Gdal.SetConfigOption("GEOTIFF_CSV", gdalData);

                string projSharePath = Path.Combine(GdalPath, "share");
                Environment.SetEnvironmentVariable("PROJ_LIB", projSharePath);
                Gdal.SetConfigOption("PROJ_LIB", projSharePath);

                _usable = true;

                _directories = new string[] { GdalPath, _driverPath };
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
            }
            catch (Exception e)
            {
                _usable = false;
                Trace.WriteLine(e, "error");
                Trace.WriteLine($"Executing directory: {_executingDirectory}", "error");
                Trace.WriteLine($"gdal directory: {GdalPath}", "error");
                Trace.WriteLine($"native directory: {_nativePath}", "error");

                //throw;
            }
        }

        private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            string assemblyName = new AssemblyName(args.Name).Name;
            foreach (string directory in _directories)
            {
                assembly = AssemblyExtensions.GetAssembly(directory, assemblyName);
                if (assembly != null)
                {
                    break;
                }
            }
            return assembly;
        }

        /// <summary>
        /// Gets a value indicating if the GDAL package is set up properly.
        /// </summary>
        public static bool Usable
        {
            get { return _usable; }
        }

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
        public static void ConfigureOgr()
        {
            if (!_usable) return;
            if (_configuredOgr) return;

            // Register drivers
            Ogr.RegisterAll();
            _configuredOgr = true;

            PrintDriversOgr();
        }

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
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
        /// Function to determine which platform we're on
        /// </summary>
        private static string GetPlatform()
        {
            return Environment.Is64BitProcess ? "x64" : "x86";
        }

        /// <summary>
        /// Gets a value indicating if we are on a windows platform
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