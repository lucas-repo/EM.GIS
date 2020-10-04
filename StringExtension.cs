using System;
using System.Linq;

namespace EM.GIS
{
    public static class StringExtension
    {
        public static bool ToPosition(this string str, out double x, out double y)
        {
            bool ret = false;
            x = 0;
            y = 0;
            if (string.IsNullOrWhiteSpace(str) || !str.Contains(' '))
            {
                return ret;
            }
            string[] array = str.Split(' ');
            if (array.Length != 2)
            {
                return ret;
            }
            ret = double.TryParse(array[0], out x);
            if (ret)
            {
                ret = double.TryParse(array[1], out y);
            }
            return ret;
        }
    }
}
