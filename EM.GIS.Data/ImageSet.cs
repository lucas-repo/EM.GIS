using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 图片
    /// </summary>
    public class ImageSet : RasterSet
    {
        /// <summary>
        /// 位图
        /// </summary>
        public Image Bitmap { get; protected set; }
        public override int ByteSize => GetByteSize(default(byte));
        public override int NumRows => Bitmap==null ? 0 : Bitmap.Height;
        public override int NumColumns => Bitmap==null ? 0 : Bitmap.Width;
        public ImageSet(Image bitmap, IExtent extent)
        {
            Bitmap = bitmap??throw new ArgumentNullException(nameof(bitmap));
            Bounds=new RasterBounds(Bitmap.Height, Bitmap.Width, extent);
            Bands.Add(this);
            RasterType= RasterType.Byte;
        }
        public override void Draw(Graphics graphics, RectangleF rectangle, IExtent extent, Action<int> progressAction = null, Func<bool> cancelFunc = null)
        {
            if (graphics==null||rectangle.IsEmpty||extent==null||extent.IsEmpty()||cancelFunc?.Invoke()==true) return ;

            if (Bitmap==null|| Bounds == null || Bounds.Extent == null || Bounds.Extent.IsEmpty()) return ;

            // Gets the scaling factor for converting from geographic to pixel coordinates
            double dx = rectangle.Width / extent.Width;
            double dy = rectangle.Height / extent.Height;

            double[] a = Bounds.AffineCoefficients;

            // gets the affine scaling factors.
            float m11 = Convert.ToSingle(a[1] * dx);
            float m22 = Convert.ToSingle(a[5] * -dy);
            float m21 = Convert.ToSingle(a[2] * dx);
            float m12 = Convert.ToSingle(a[4] * -dy);
            double l = a[0] - (.5 * (a[1] + a[2])); // Left of top left pixel
            double t = a[3] - (.5 * (a[4] + a[5])); // top of top left pixel
            float xShift = (float)((l - extent.MinX) * dx);
            float yShift = (float)((extent.MaxY - t) * dy);

            try
            {
                graphics.Transform = new Matrix(m11, m12, m21, m22, xShift, yShift);
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                if (m11 > 1 || m22 > 1) graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                RectangleF destRect = Extent.ProjToPixelF(rectangle, extent);
                if (!graphics.VisibleClipBounds.IsEmpty) graphics.DrawImage(Bitmap, destRect, new Rectangle(0,0,Bitmap.Width,Bitmap.Height),GraphicsUnit.Pixel);
            }
            catch (OverflowException)
            {
                // Raised by g.DrawImage if the new images extent is to small
            }
        }
    }
}
