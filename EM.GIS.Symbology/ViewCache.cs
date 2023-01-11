using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 视图缓存
    /// </summary>
    public class ViewCache : BaseCopy, IProj,IDisposable
    {
        /// <summary>
        /// 图片
        /// </summary>
        public Image Image { get; private set; }
        /// <inheritdoc/>
        public IExtent Extent { get; }
        /// <inheritdoc/>
        public Rectangle Bound { get; }
        /// <summary>
        /// 绘制的范围
        /// </summary>
        public IExtent DrawingExtent { get; }

        public ViewCache(Image image,Rectangle rectangle,IExtent extent,IExtent drawingExtent)
        {
            Image = image??throw new NullReferenceException(nameof(image));
            Bound=rectangle;
            Extent = extent;
            DrawingExtent = drawingExtent;
        }
        public ViewCache(int imageWidth,int imageHeight, Rectangle rectangle, IExtent extent, IExtent drawingExtent)
        {
            if (imageWidth == 0 || imageHeight == 0)
            {
                throw new ArgumentException($"{nameof(imageWidth)}和{nameof(imageHeight)}不能为0");
            }
            Image = new Bitmap(imageWidth,imageHeight);
            Bound = rectangle;
            Extent = extent;
            DrawingExtent = drawingExtent;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }
        }
    }
}
