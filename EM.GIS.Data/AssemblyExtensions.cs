using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 程序集扩展
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="destType"></param>
        /// <returns></returns>
        public static object CreateInstance(Assembly assembly, Type destType)
        {
            object t = default;
            if (assembly != null && destType != null)
            {
                foreach (Type item in assembly.GetTypes())
                {
                    if (!item.IsAbstract && destType.IsAssignableFrom(item))
                    {
                        t = Activator.CreateInstance(item);
                    }
                }
            }
            return t;
        }
        /// <summary>
        /// 从文件中读取dll创建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(string directory)
        {
            Type destType = typeof(T);
            T t = default;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                t = (T)CreateInstance(assembly, destType);
                if (t != null)
                {
                    break;
                }
            }
            if (t == null)
            {
                string[] dllFiles = Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);
                foreach (string dllFileName in dllFiles)
                {
                    if (assemblies.Any(x => x.Location == dllFileName))
                    {
                        continue;
                    }
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(dllFileName);
                        t = (T)CreateInstance(assembly, destType);
                        if (t != null)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"CreateInterface Error!{ex}");
                    }
                }
            }
            return t;
        }

    }
}
