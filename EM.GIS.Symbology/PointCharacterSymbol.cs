using System;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace EM.GIS.Symbology
{
    public class PointCharacterSymbol : PointSymbol, IPointCharacterSymbol
    {
        public PointCharacterSymbol() : base(PointSymbolType.Character)
        { }

        public UnicodeCategory Category => char.GetUnicodeCategory(Character);

        public char Character { get; set; }
        public string FontFamilyName { get; set; }

        public FontStyle FontStyle { get; set; }
        protected override void OnRandomize(Random generator)
        {
            Color = generator.NextColor();
            Opacity = generator.NextFloat();
            Character = (char)generator.Next(0, 255);
            FontFamilyName = FontFamily.Families[generator.Next(0, FontFamily.Families.Length - 1)].Name;
            FontStyle = generator.NextEnum<FontStyle>();
            base.OnRandomize(generator);
        }

        protected override void OnDrawPoint(Graphics g, float scale)
        {
            using (Brush b = new SolidBrush(Color))
            {
                string txt = new string(new[] { Character });
                float fontPointSize = Size.Height * scale;
                Font fnt = new Font(FontFamilyName, fontPointSize, FontStyle, GraphicsUnit.Pixel);
                SizeF fSize = g.MeasureString(txt, fnt);
                float x = -fSize.Width / 2;
                float y = -fSize.Height / 2;
                if (fSize.Height > fSize.Width * 5)
                {
                    // Defective fonts sometimes are created with a bad height.
                    // Use the width instead
                    y = -fSize.Width / 2;
                }
                g.DrawString(txt, fnt, b, new PointF(x, y));
                using (var path = fSize.ToPath())
                {
                    DrawOutLine(g, scale, path);
                }
            }
        }
    }
}
