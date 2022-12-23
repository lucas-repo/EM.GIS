using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 栅格图层
    /// </summary>
    public class RasterLayer : Layer, IRasterLayer
    {
        /// <inheritdoc/>
        public new IRasterCategory DefaultCategory
        {
            get => base.DefaultCategory as IRasterCategory;
            set => base.DefaultCategory = value;
        }

        public RasterLayer()
        {
            Children = new RasterCategoryCollection(this);
        }
        public RasterLayer(IDataSet rasterSet) : this()
        {
            DataSet = rasterSet ?? throw new ArgumentNullException(nameof(rasterSet));
            Projection = rasterSet.Projection.Copy();
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

        /// <inheritdoc/>
        public new IRasterCategoryCollection Children
        {
            get => base.Children as IRasterCategoryCollection;
            protected set => base.Children = value;
        }

        /// <inheritdoc/>
        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            if (selected || cancelFunc?.Invoke() == true)
            {
                return;
            }
            if (DataSet is IDrawable drawable)
            {
                IExtent destExtent = extent.Copy();
                if (!Equals(Projection, DataSet.Projection))
                {
                    DataSet.Projection.ReProject(Projection, destExtent);
                }
                drawable.Draw(graphics, rectangle, destExtent, ReportProgress, cancelFunc);
            }
        }
        private void ReportProgress(int progress)
        {
            int minProgress = 0;
            int maxProgress = 80;
            double progressD = progress / 100.0 * (maxProgress - minProgress);
            Progress?.Invoke((int)progressD, ProgressMessage);
        }
    }
}