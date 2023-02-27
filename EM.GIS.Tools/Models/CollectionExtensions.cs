using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 添加枚举
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="collection">集合</param>
        public static void AddEnums<T>(this ICollection<T> collection) where T : Enum
        {
            var array = Enum.GetValues(typeof(T));
            foreach (var item in array)
            {
                if (item is T t)
                {
                    collection.Add(t);
                }
            }
        }
        /// <summary>
        /// 添加枚举
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="collection">集合</param>
        public static void AddEnums<T>(this ICollection<Enum> collection) where T : Enum
        {
            var array = Enum.GetValues(typeof(T));
            foreach (var item in array)
            {
                if (item is T t)
                {
                    collection.Add(t);
                }
            }
        }
        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="items">待添加的元素</param>
        public static void AddRange<T>(this ICollection<T> collection,IEnumerable<T> items) 
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
