using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 视图缓存
    /// </summary>
    public class ViewCache : BaseCopy, IProj,IDisposable
    {
        private bool isDisposed;
        private readonly object lockObj=new object();
        private Bitmap? bitmap;
        /// <summary>
        /// 图片
        /// </summary>
        public Bitmap? Bitmap
        {
            get { return bitmap; }
            set 
            {
                lock (lockObj)
                {
                    if (bitmap != value)
                    {
                        if (bitmap != null)
                        {
                            bitmap.Dispose();
                        }
                        bitmap = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IExtent Extent { get; set; }
        /// <inheritdoc/>
        public Rectangle Bound { get; set; }
        /// <summary>
        /// 绘制的范围
        /// </summary>
        public IExtent DrawingExtent { get; set; }
        public ViewCache(Bitmap bitmap,Rectangle rectangle,IExtent extent,IExtent drawingExtent)
        {
            Bitmap = bitmap??throw new NullReferenceException(nameof(bitmap));
            Bound=rectangle;
            Extent = extent;
            DrawingExtent = drawingExtent;
        }
        public ViewCache(DrawingArgs drawingArgs)
        {
            Bound = drawingArgs.Bound;
            Extent=drawingArgs.Extent;
            DrawingExtent=drawingArgs.Extent;
        }
        public ViewCache(int imageWidth,int imageHeight, Rectangle rectangle, IExtent extent, IExtent drawingExtent)
        {
            if (imageWidth == 0 || imageHeight == 0)
            {
                throw new ArgumentException($"{nameof(imageWidth)}和{nameof(imageHeight)}不能为0");
            }
            Bitmap = new Bitmap(imageWidth,imageHeight);
            Bound = rectangle;
            Extent = extent;
            DrawingExtent = drawingExtent;
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            if (!isDisposed)
            {
                if (Bitmap != null)
                {
                    Bitmap.Dispose();
                    Bitmap = null;
                }
                isDisposed = true;
            }
        }
        /// <inheritdoc/>
        protected override void OnCopy(object copy)
        {
            if (isDisposed)
            {
                throw new Exception("不允许复制已释放的实例");
            }
            base.OnCopy(copy);
        }
    }
}
