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

        public new IRasterCategoryCollection Categories { get => LegendItems as IRasterCategoryCollection; }
        public RasterLayer()
        {
            LegendItems = new RasterCategoryCollection(this);
        }
        public RasterLayer(IRasterSet rasterSet) : this()
        {
            DataSet = rasterSet;
            //if (DataSet?.Bands.Count > 0)
            //{
            //    foreach (var item in DataSet.Bands)
            //    {
            //        IRasterCategory rasterCategory = new RasterCategory()
            //        {
            //            Text = item.Name
            //        };
            //        LegendItems.Add(rasterCategory);
            //    }
            //}
        }

        public new IRasterSet DataSet { get => base.DataSet as IRasterSet; set => base.DataSet = value; }

        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            if (selected || cancelFunc?.Invoke() == true)
            {
                return;
            }
            using (var bmp = DataSet.GetBitmap(extent, rectangle, ReportProgress))
            {
                if (bmp != null)
                {
                    graphics.DrawImage(bmp, rectangle);
                }
            }
        }
        private void ReportProgress(int progress)
        {
            int minProgress = 0;
            int maxProgress = 80;
            double progressD = progress / 100.0 * (maxProgress - minProgress);
            ProgressHandler?.Progress((int)progressD, ProgressMessage);
        }
    }
}