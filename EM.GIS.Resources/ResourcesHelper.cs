using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;

namespace EM.GIS.Resources
{
    /// <summary>
    /// 资源辅助类
    /// </summary>
    public static class ResourcesHelper
    {
        /// <summary>
        /// 获取图片资源地址
        /// </summary>
        /// <param name="name">图片名称</param>
        /// <returns>图片资源地址</returns>
        public static string GetImageUri(string name)
        {
            Assembly assembly = typeof(ResourcesHelper).Assembly;
            string uri = $"pack://application:,,,/{assembly.GetName().Name};Component/Images/{name}";
            return uri;
        }
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="uriKind">标识符类型</param>
        /// <returns>图片</returns>
        public static BitmapImage GetBitmapImage(string name, UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            BitmapImage image = null;
            if (string.IsNullOrEmpty(name))
            {
                return image;
            }
            string uriStr = GetImageUri(name);
            try
            {
                Uri uri = new Uri(uriStr, uriKind);
                image = new BitmapImage(uri);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return image;
        }
    }
}
