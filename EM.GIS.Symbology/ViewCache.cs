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
                        bitmap = value;
                    }
                }
            }
        }
        private IExtent extent;
        /// <inheritdoc/>
        public IExtent Extent
        {
            get { return extent; }
            set
            {
                if (!Equals(extent,value))
                {
                    extent = value;
                }
            }
        }
        private Rectangle bound;
        /// <inheritdoc/>
        public Rectangle Bound
        {
            get { return bound; }
            set 
            {
                if (bound != value)
                {
                    bound = value;
                }
            }
        }

        private IExtent drawingExtent;
        /// <summary>
        /// 绘制的范围
        /// </summary>
        public IExtent DrawingExtent
        {
            get { return drawingExtent; }
            set
            {
                if (!Equals(drawingExtent, value))
                {
                    drawingExtent = value;
                }
            }
        }
        public ViewCache(Bitmap bitmap,Rectangle rectangle,IExtent extent,IExtent drawingExtent)
        {
            bitmap = bitmap??throw new NullReferenceException(nameof(bitmap));
            bound=rectangle;
            this.extent = extent;
            this.drawingExtent = drawingExtent;
        }
        public ViewCache(DrawingArgs drawingArgs)
        {
            bound = drawingArgs.Bound;
            extent = drawingArgs.Extent;
            drawingExtent = drawingArgs.DrawingExtent;
        }
        public ViewCache(int imageWidth,int imageHeight, Rectangle rectangle, IExtent extent, IExtent drawingExtent)
        {
            if (imageWidth == 0 || imageHeight == 0)
            {
                throw new ArgumentException($"{nameof(imageWidth)}和{nameof(imageHeight)}不能为0");
            }
            bitmap = new Bitmap(imageWidth,imageHeight);
            bound = rectangle;
            this.extent = extent;
            this.drawingExtent = drawingExtent;
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
