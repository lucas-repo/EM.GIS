using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EM.GIS
{
    /// <summary>
    /// 拷贝扩展
    /// </summary>
    public static class CopyExtensions
    {
        /// <summary>
        /// 拷贝一份副本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T Copy<T>(this T original)
           where T : class, ICloneable
        {
            return original?.Clone() as T;
        }

        /// <summary>
        /// 返回不同名称的属性信息数组
        /// </summary>
        /// <param name="allProperties">所有属性</param>
        /// <returns>属性数组</returns>
        public static PropertyInfo[] DistinctNames(IEnumerable<PropertyInfo> allProperties)
        {
            List<string> names = new List<string>();
            List<PropertyInfo> result = new List<PropertyInfo>();
            foreach (PropertyInfo property in allProperties)
            {
                if (names.Contains(property.Name)) continue;
                result.Add(property);
                names.Add(property.Name);
            }
            return result.ToArray();
        }

        /// <summary>
        /// 将源对象的属性和字段拷贝给目标对象
        /// </summary>
        /// <param name="src">源对象</param>
        /// <param name="copy">目标对象</param>
        /// <param name="bindingFlags">绑定标记</param>
        public static void CopyTo(this object src, object copy, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (src == null || copy == null)
            {
                return;
            }

            // This checks any property on copy, and if it is cloneable, it
            // creates a clone instead
            var srcType = src.GetType();
            Type copyType = copy.GetType();

            PropertyInfo[] copyProperties = DistinctNames(copyType.GetProperties(bindingFlags));
            PropertyInfo[] srcProperties = DistinctNames(srcType.GetProperties(bindingFlags));
            foreach (PropertyInfo p in copyProperties)
            {
                if (p.CanWrite == false) continue;
                if (!srcProperties.Any(x => x.Name == p.Name)) continue;
                PropertyInfo srcProperty = srcProperties.First(x => x.Name == p.Name);
                object srcValue = srcProperty.GetValue(src, null);
                if (srcProperty.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length > 0)
                {
                    // This property is marked as shallow, so skip cloning it
                    continue;
                }
                if (srcValue is ICloneable cloneable)
                {
                    p.SetValue(copy, cloneable.Clone(), null);
                }
                else
                {
                    p.SetValue(copy, srcValue, null);
                }
            }

            FieldInfo[] copyFields = copyType.GetFields(bindingFlags);
            FieldInfo[] srcFields = srcType.GetFields(bindingFlags);
            foreach (FieldInfo f in copyFields)
            {
                if (!srcFields.Any(x => x.Name == f.Name)) continue;
                FieldInfo myField = srcFields.First(x => x.Name == f.Name);
                object myValue = myField.GetValue(copy);

                if (myField.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length > 0)
                {
                    // This field is marked as shallow, so skip cloning it
                    continue;
                }

                ICloneable cloneable = myValue as ICloneable;
                if (cloneable == null) continue;
                f.SetValue(copy, cloneable.Clone());
            }
        }
    }
}
