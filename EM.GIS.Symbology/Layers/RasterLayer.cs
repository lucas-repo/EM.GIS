using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
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

        public new IRasterCategoryCollection Categories { get=> LegendItems as IRasterCategoryCollection; }
        public RasterLayer()
        {
            LegendItems = new RasterCategoryCollection(this);
        }
        public RasterLayer(IRasterSet rasterSet):this()
        {
            DataSet = rasterSet;
            if (DataSet?.Bands.Count > 0)
            {
                foreach (var item in DataSet.Bands)
                {
                    IRasterCategory rasterCategory = new RasterCategory()
                    {
                        Text = item.Name
                    };
                    LegendItems.Add(rasterCategory);
                }
            }
        }

        public new IRasterSet DataSet { get => base.DataSet as IRasterSet; set => base.DataSet = value; }

        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null)
        {
            if (selected || cancelFunc?.Invoke() == true)
            {
                return;
            }
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