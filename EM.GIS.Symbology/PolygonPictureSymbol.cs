using EM.Bases;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public class PolygonPictureSymbol : PolygonSymbol, IPolygonPictureSymbol
    {
        public PolygonPictureSymbol() : base(PolygonSymbolType.Picture)
        { }
        public WrapMode WrapMode { get; set; }
        public Bitmap Picture { get; set; }
        public float Angle { get; set; }
        public PointF Scale { get; set; } = new PointF(1, 1);
        private Bitmap GetPicture(Bitmap srcImg)
        {
            Bitmap destImg = null;
            if (srcImg != null)
            {
                destImg = srcImg.Copy();
                using (Graphics g = Graphics.FromImage(srcImg))
                {
                    g.ScaleTransform(Scale.X, Scale.Y);
                    g.RotateTransform(Angle);
                }
            }
            return destImg;
        }
        public override Brush GetBrush()
        {
            Brush brush = base.GetBrush();
            if (Picture == null) return brush;
            if (Scale.X == 0 || Scale.Y == 0) return brush;
            if (Scale.X * Picture.Width * Scale.Y * Picture.Height > 8000 * 8000) return brush; // The scaled image is too large, will cause memory exceptions.
            if (Picture != null)
            {
                Bitmap scaledBitmap = new Bitmap((int)(Picture.Width * Scale.X), (int)(Picture.Height * Scale.Y));
                Graphics scb = Graphics.FromImage(scaledBitmap);
                scb.DrawImage(Picture, new Rectangle(0, 0, scaledBitmap.Width, scaledBitmap.Height), new Rectangle(0, 0, Picture.Width, Picture.Height), GraphicsUnit.Pixel);

                TextureBrush tb = new TextureBrush(scaledBitmap, WrapMode);
                tb.RotateTransform(-Angle);
                brush = tb;
            }
            return brush;
        }
    }
}
