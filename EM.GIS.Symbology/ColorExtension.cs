using System;
using System.Drawing;

namespace EM.GIS.Symbology
{
    public static class ColorExtension
    {
        public static float GetOpacity(this Color color)
        {
            return color.A / 255F;
        }
        public static Color ToTransparent(this Color color, float opacity)
        {
            int a = Convert.ToInt32(opacity * 255);
            if (a > 255) a = 255;
            if (a < 0) a = 0;
            return Color.FromArgb(a, color.R, color.G, color.B);
        }
    }
}
