using System;
using System.Collections.Generic;
using System.Text;

namespace EMap
{
    public static class ArrayExtension
    {
        public static T[] Remove<T>(this T[] array, T item)
        {
            List<T> list = null;
            if (array != null)
            {
                 list = new List<T>(array);
                list.Remove(item);
            }
            return list?.ToArray();
        }
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            List<T> list = null;
            if (array != null)
            {
                list = new List<T>(array);
                list.RemoveAt(index);
            }
            return list?.ToArray();
        }
    }
}
