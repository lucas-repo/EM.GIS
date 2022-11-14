using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EM.WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
            _privatePathes = new List<string>() { string.Empty };
            foreach (var item in privatePathesSection.GetChildren())
            {
                _privatePathes.Add(item.Value);
            }
        }
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }
        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            // check the installation directory
            if (_privatePathes != null)
            {
                var assemblyName = new AssemblyName(args.Name);
                string name = assemblyName.Name;
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
