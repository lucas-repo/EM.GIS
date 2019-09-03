using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EMap
{
    /// <summary>
    /// 程序集扩展类
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// 根据指定路径获取程序集
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string fileName)
        {
            Assembly assembly = null;
            if (File.Exists(fileName))
            {
                assembly = Assembly.LoadFrom(fileName);
            }
            return assembly;
        }
        /// <summary>
        /// 在指定的私有路径查找程序集
        /// </summary>
        /// <param name="args">AssemblyResolve参数</param>
        /// <param name="privatePath">私有路径</param>
        /// <returns></returns>
        public static Assembly AssemblyResolve(ResolveEventArgs args, string privatePath)
        {
            Assembly assembly = null;
            if (string.IsNullOrEmpty(privatePath))
            {
                return assembly;
            }
            string[] directoryNames = privatePath.Split(';');
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string extension = Path.GetExtension(args.RequestingAssembly.CodeBase);
            string directory = null;
            string[] arry = args.Name.Split(',');
            string name = arry[0];
            string path = null;
            foreach (var directoryName in directoryNames)
            {
                directory = Path.Combine(baseDirectory, directoryName);
                path = Path.Combine(directory, $"{name}{extension}");
                if (File.Exists(path))
                {
                    assembly = GetAssembly(path);
                    if (assembly != null)
                    {
                        break;
                    }
                }
            }
            return assembly;
        }
    }
}
