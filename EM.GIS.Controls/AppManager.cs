using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace EM.GIS.Controls
{
    /// <summary>
    /// app管理类
    /// </summary>
    [Export]
    public class AppManager : NotifyClass, IAppManager
    {
        private IMap _map;
        public IMap Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(nameof(Map)); }
        }
        private ILegend _legend;
        public ILegend Legend
        {
            get { return _legend; }
            set { _legend = value; OnPropertyChanged(nameof(Legend)); }
        }
        private IProgressHandler _progressHandler;
        public IProgressHandler ProgressHandler
        {
            get { return _progressHandler; }
            set { _progressHandler = value; OnPropertyChanged(nameof(ProgressHandler)); }
        }

        public IEnumerable<IPlugin> Plugins { get; }
        public List<string> Directories { get; }
        private string _baseDirectory;
        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set { _baseDirectory = value; OnPropertyChanged(nameof(BaseDirectory)); }
        }
        private AggregateCatalog Catalog { get; set; }

        private CompositionContainer CompositionContainer { get; set; }
        public AppManager()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
            Plugins = new List<IPlugin>();
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Directories = new List<string>
            {
                string.Empty,
                "Plugins"
            };
        }
        private AggregateCatalog GetCatalog()
        {
            var catalog = new AggregateCatalog();

            #region 添加主程序
            List<Assembly> assemblies = new List<Assembly>();
            Assembly mainExe = Assembly.GetEntryAssembly();
            if (mainExe != null)
            {
                assemblies.Add(mainExe);
            }
            assemblies.Add(typeof(DataFactory).Assembly);
            assemblies.Add(typeof(AppManager).Assembly);

            foreach (var assembly in assemblies)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                Trace.WriteLine("Cataloging: " + assembly.FullName);
            }
            #endregion

            //添加Directories及子目录的扩展
            foreach (string dir in GetDirectoriesNestedOneLevel())
            {
                if (!DirectoryCatalogExists(catalog, dir))
                    TryLoadingCatalog(catalog, new DirectoryCatalog(dir));
            }

            return catalog;
        }
        private static bool DirectoryCatalogExists(AggregateCatalog catalog, string dir)
        {
            return catalog.Catalogs.OfType<DirectoryCatalog>().Any(directoryCatalog => directoryCatalog.FullPath.Equals(dir, StringComparison.OrdinalIgnoreCase));
        }
        private static void TryLoadingCatalog(AggregateCatalog catalog, DirectoryCatalog cat)
        {
            try
            {
                // We call Parts.Count simply to load the dlls in this directory, so that we can determine whether they will load properly.
                if (cat.Parts.Any())
                {
                    Trace.WriteLine("Cataloging: " + cat.Path);
                    catalog.Catalogs.Add(cat);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Type type = ex.Types[0];
                string typeAssembly = type != null ? type.Assembly.ToString() : string.Empty;

                string message = string.Format("跳过扩展{0}.{1}", typeAssembly, ex.LoaderExceptions.First().Message);
                Trace.WriteLine(message);
            }
        }
        /// <summary>
        /// 获取Directories及子文件夹的目录
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetDirectoriesNestedOneLevel()
        {
            // Visit each directory in Directories Property (usually set by application)
            foreach (string directory in Directories)
            {
                string path = Path.Combine(BaseDirectory, directory);
                if (Directory.Exists(path))
                {
                    yield return path;
                    // Add all of the directories in here, nested one level deep.
                    var dirs = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
                    foreach (var dir in dirs)
                    {
                        yield return dir;
                    }
                }
            }
        }
        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var knownExtensions = new[] { "dll", "exe" };
            string assemblyName = new AssemblyName(args.Name).Name;

            // check the installation directory
            foreach (string directory in Directories)
            {
                string path = Path.Combine(BaseDirectory, directory);

                if (Directory.Exists(path))
                {
                    foreach (string extension in knownExtensions)
                    {
                        var potentialFiles = Directory.GetFiles(path, assemblyName + "." + extension, SearchOption.AllDirectories);
                        if (potentialFiles.Length > 0)
                            return Assembly.LoadFrom(potentialFiles[0]);
                    }
                }
            }

            // assembly not found
            return null;
        }
        public virtual void LoadPlugins()
        {
            if (Plugins.Any())
            {
                throw new InvalidOperationException("LoadExtensions()只应调用一次. 刷新应使用RefreshExtensions()");
            }

            //PackageManager.RemovePendingPackagesAndExtensions();

            Thread updateThread = new Thread(AppLoadExtensions);
            updateThread.Start();

            // Update splash screen's progress bar while thread is active.
            //while (updateThread.IsAlive)
            //{
            //    UpdateSplashScreen(_message);
            //}

            updateThread.Join();

            ActivateAllExtensions();
            //OnExtensionsActivated(EventArgs.Empty);

            DataFactory.Default.ProgressHandler = ProgressHandler;
        }
        private void ActivateAllExtensions()
        {
            //激活不允许冻结的
            foreach (var plugin in Plugins.Where(_ => !_.DeactivationAllowed).OrderBy(_ => _.Priority))
            {
                Activate(plugin);
            }

            // 激活剩余的
            foreach (var plugin in Plugins.Where(_ => _.DeactivationAllowed).OrderBy(_ => _.Priority))
            {
                Activate(plugin);
            }
        }
        private static void Activate(IPlugin extension)
        {
            if (!extension.IsActive)
            {
                try
                {
                    Trace.WriteLine("Activating: " + extension.AssemblyQualifiedName);
                    extension.Activate();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Error: {extension.AssemblyQualifiedName} {ex.Message} {ex.StackTrace}");
                }
            }
        }
        private void AppLoadExtensions()
        {
            Catalog = GetCatalog();

            CompositionContainer = new CompositionContainer(Catalog);

            try
            {
                CompositionContainer.ComposeParts(this, DataFactory.Default);
            }
            catch (CompositionException compositionException)
            {
                Trace.WriteLine(compositionException.Message);
                throw;
            }

        }
    }
}
