using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EM.GIS.Tools
{
    public class BoolToStartString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = "开始";
            if (value is bool isFree)
            {
                if (isFree)
                {
                    ret = "开始";
                }
                else
                {
                    ret = "取消";
                }
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
