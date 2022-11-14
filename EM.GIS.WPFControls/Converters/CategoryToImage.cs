using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 将图层分类转成图片
    /// </summary>
    public class CategoryToImage: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage ret = null;
            if (value is ICategory category)
            {
                int width = 15, height = 11;
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        category.DrawLegend(g, new Rectangle(0, 0, width, height));
                    }
                    ret = BitmapHelper.BitmapToBitmapImage(bmp, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
