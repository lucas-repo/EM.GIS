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

            RectangleF destRect = Extent.ProjToPixelF(rectangle, extent);
            destRect = RectangleF.FromLTRB((float)Math.Floor(destRect.Left), (float)Math.Floor(destRect.Top), (float)Math.Ceiling(destRect.Right), (float)Math.Ceiling(destRect.Bottom));
            if (!graphics.VisibleClipBounds.IsEmpty) graphics.DrawImage(Bitmap, destRect, new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), GraphicsUnit.Pixel);

        }
    }
}
