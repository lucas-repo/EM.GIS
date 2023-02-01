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
        /// <inheritdoc/>
        public override int Height => Bitmap == null ? 0 : Bitmap.Height;
        /// <inheritdoc/>
        public override int Width => Bitmap == null ? 0 : Bitmap.Width;
        private List<IRasterSet> rasters = new List<IRasterSet>();
        /// <inheritdoc/>
        public override IEnumerable<IRasterSet> Rasters => rasters;
        /// <summary>
        /// 位图数据集
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <param name="extent">范围</param>
        /// <exception cref="ArgumentNullException">空参数异常</exception>
        public ImageSet(Image bitmap, IExtent extent)
        {
            Bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            Bounds = new RasterBounds(Bitmap.Height, Bitmap.Width, extent);
            rasters.Add(this);
            RasterType = RasterType.Byte;
        }
        /// <inheritdoc/>
        public override void Draw(MapArgs mapArgs, Action<int>? progressAction = null, Func<bool>? cancelFunc = null)
        {
            if (mapArgs == null || mapArgs.Graphics == null || mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() || cancelFunc?.Invoke() == true|| Bitmap == null || Bounds == null || Bounds.Extent == null || Bounds.Extent.IsEmpty())
            {
                return;
            }

            var destExtent = mapArgs.DestExtent.Intersection(Extent);
            if (destExtent == null || destExtent.IsEmpty())
            {
                return;
            }
            RectangleF srcRect = destExtent.ProjToPixelF(new Rectangle(0, 0, Bounds.NumColumns, Bounds.NumRows), Extent);
            RectangleF destRect = mapArgs.ProjToPixelF(destExtent);
            destRect = RectangleF.FromLTRB((float)Math.Floor(destRect.Left), (float)Math.Floor(destRect.Top), (float)Math.Ceiling(destRect.Right), (float)Math.Ceiling(destRect.Bottom));
            if (!mapArgs.Graphics.VisibleClipBounds.IsEmpty) mapArgs.Graphics.DrawImage(Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                Bitmap?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
