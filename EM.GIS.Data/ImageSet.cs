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
        public Bitmap Bitmap { get; protected set; }
        public override int ByteSize => GetByteSize(default(byte));
        public override int NumRows => Bitmap==null ? 0 : Bitmap.Height;
        public override int NumColumns => Bitmap==null ? 0 : Bitmap.Width;
        public ImageSet(Bitmap bitmap, IExtent extent)
        {
            Bitmap = bitmap??throw new ArgumentNullException(nameof(bitmap));
            Bounds=new RasterBounds(Bitmap.Height, Bitmap.Width, extent);
            Bands.Add(this);
            RasterType= RasterType.Byte;

        }
        public override Image GetImage()
        {
            return Bitmap;
        }

        public override Image GetImage(IExtent envelope, Rectangle window, Action<int> progressAction = null)
        {
            if (window.Width == 0 || window.Height == 0) return null;

            if (Bounds == null || Bounds.Extent == null || Bounds.Extent.IsEmpty()) return null;

            // Gets the scaling factor for converting from geographic to pixel coordinates
            double dx = window.Width / envelope.Width;
            double dy = window.Height / envelope.Height;

            double[] a = Bounds.AffineCoefficients;

            // gets the affine scaling factors.
            float m11 = Convert.ToSingle(a[1] * dx);
            float m22 = Convert.ToSingle(a[5] * -dy);
            float m21 = Convert.ToSingle(a[2] * dx);
            float m12 = Convert.ToSingle(a[4] * -dy);
            double l = a[0] - (.5 * (a[1] + a[2])); // Left of top left pixel
            double t = a[3] - (.5 * (a[4] + a[5])); // top of top left pixel
            float xShift = (float)((l - envelope.MinX) * dx);
            float yShift = (float)((envelope.MaxY - t) * dy);

            Bitmap tempResult = null;
            Bitmap result = null;
            Graphics g = null;
            try
            {
                tempResult = new Bitmap(window.Width, window.Height);
                g = Graphics.FromImage(tempResult);
                g.Transform = new Matrix(m11, m12, m21, m22, xShift, yShift);
                g.PixelOffsetMode = PixelOffsetMode.Half;
                if (m11 > 1 || m22 > 1) g.InterpolationMode = InterpolationMode.NearestNeighbor;
                if (!g.VisibleClipBounds.IsEmpty) g.DrawImage(Bitmap, new PointF(0, 0));
                result = tempResult;
                tempResult = null;
            }
            catch (OverflowException)
            {
                // Raised by g.DrawImage if the new images extent is to small
            }
            finally
            {
                tempResult?.Dispose();
                g?.Dispose();
            }

            return result;
        }
    }
}
