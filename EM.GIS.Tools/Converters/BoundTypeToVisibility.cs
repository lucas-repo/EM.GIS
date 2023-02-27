using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 将<see cref="BoundType"/>转为<see cref="Visibility"/>
    /// </summary>
    public class BoundTypeToVisibility : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility ret = Visibility.Visible;
            if (value is BoundType boundType)
            {
                bool inverse = false;
                if (bool.TryParse(parameter?.ToString(), out inverse))
                {
                    if (inverse)
                    {
                        if (boundType == BoundType.SelectedFeatures)
                        {
                            ret = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        if (boundType != BoundType.SelectedFeatures)
                        {
                            ret = Visibility.Hidden;
                        }
                    }
                }
            }
            return ret;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
