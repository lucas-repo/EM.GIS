using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EM.GIS
{
    public static class ResourceHelper
    {
        /// <summary>
        /// 根据项目中的资源名称获取其实际存放的路径
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static string GetFileName(string name, string assemblyName)
        {
            System.Reflection.Assembly asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assemblyName); 
            string fileName = GetFileName(name, asm);
            return fileName;
        }
        public static string GetFileName(string name, Assembly asm)
        {
            string fileName = null;
            if (string.IsNullOrEmpty(name) || asm == null)
            {
                return fileName;
            }
            string directory = Path.GetDirectoryName(asm.CodeBase).Replace("file:\\", "");
            fileName = Path.Combine(directory, name);
            return fileName;
        }
    }
}
