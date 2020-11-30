using EM.GIS.Data;
using EM.GIS.Geometries;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    public class RasterLayer : Layer, IRasterLayer
    {
        public new IRasterCategory DefaultCategory
        {
            get => base.DefaultCategory as IRasterCategory;
            set => base.DefaultCategory = value;
        }

        public new IRasterCategoryCollection Categories { get=> Items as IRasterCategoryCollection; }
        public RasterLayer()
        {
            DefaultCategory = new RasterCategory();
            Items = new RasterCategoryCollection(this)
            {
                DefaultCategory
            };
        }
        public RasterLayer(IRasterSet rasterSet):this()
        {
            DataSet = rasterSet;
        }

        public new IRasterSet DataSet { get => base.DataSet as IRasterSet; set => base.DataSet = value; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DataSet?.Dispose();
                DataSet = null;
            }
            base.Dispose(disposing);
        }
        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, CancellationTokenSource cancellationTokenSource = null)
        {
            using (var bmp = DataSet.GetBitmap(extent, rectangle))
            {
                if (bmp != null)
                {
                    graphics.DrawImage(bmp, rectangle);
                }
            }
        }
    }
}