using EM.GIS.WPFControls;
using EM.IOC;
using EM.IOC.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace EM.WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 配置项
        /// </summary>
        public static IConfigurationRoot Configuration { get; }
        string[] _extensions = new[] { "dll", "exe" };
        static List<string> _privatePathes;
        static App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//todo 注册编码，注册gdal使用的GBK编码
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            var privatePathesSection = Configuration.GetSection("PrivatePathes");
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            _privatePathes = new List<string>() { directory };
            foreach (var item in privatePathesSection.GetChildren())
            {
                if (string.IsNullOrEmpty(item?.Value))
                {
                    continue;
                }
                _privatePathes.Add(Path.Combine(directory,item.Value));
            }
        }
        /// <summary>
        /// 实例化<seealso cref="App"/>
        /// </summary>
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            IocOptions iocOptions = new IocOptions();//ioc参数
            iocOptions.ServiceDirectories.AddRange(_privatePathes);
            if (IocManager.Default is MsIocManager iocManager)
            {
                iocManager.Initialize(iocOptions);
            }
            else
            {
                throw new Exception("容器管理器不能为空");
            }
            //此处可设置优先启动登录窗体
            MainWindow window = new MainWindow();//在主窗体中加载插件
            if (!(window.ShowDialog() ?? false))
            {
                Shutdown();
                return;
            }
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            base.OnStartup(e);
        }
        private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            Assembly? assembly = null;
            // check the installation directory
            if (_privatePathes != null)
            {
                var assemblyName = new AssemblyName(args.Name);
                var name = assemblyName.Name;
                if (string.IsNullOrEmpty(name))
                {
                    return assembly;
                }
                foreach (string directory in _privatePathes)
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
                    if (Directory.Exists(path))
                    {
                        foreach (string extension in _extensions)
                        {
                            var potentialFiles = Directory.GetFiles(path, $"{name}.{extension}", SearchOption.TopDirectoryOnly);
                            if (potentialFiles.Length > 0)
                            {
                                assembly = Assembly.LoadFrom(potentialFiles[0]);
                                if (assembly != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (assembly != null)
                    {
                        break;
                    }
                }
            }
            // assembly not found
            return assembly;
        }
    }
}
