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
        public RasterLayer(IDataSet rasterSet):base(rasterSet)
        {
            DataSet = rasterSet ?? throw new ArgumentNullException(nameof(rasterSet));
            Children = new RasterCategoryCollection(this);
            Text = rasterSet.Name;
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
        protected override void OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null)
        {
            if (selected || cancelFunc?.Invoke() == true)
            {
                return;
            }
            if (DataSet is IDrawable drawable)
            {
                Action<int> newProgressAction = (progress) => progressAction?.Invoke(ProgressMessage, progress);
                drawable.Draw(mapArgs, newProgressAction, cancelFunc);
            }
        }
    }
}