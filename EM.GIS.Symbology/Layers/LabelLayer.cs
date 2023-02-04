using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    public class LabelLayer : Layer, ILabelLayer
    {
        public IFeatureLayer FeatureLayer { get; }

        public new ILabelCategoryCollection Children { get =>base.Children as ILabelCategoryCollection; protected set=> base.Children = value; }
        public LabelLayer( IFeatureLayer featureLayer)
        {
            FeatureLayer = featureLayer;
            Children = new LabelCategoryCollection(this);
        }
        public void ClearSelection()
        {
        }

        public void CreateLabels()
        {
        }
        /// <inheritdoc/>
        protected override Rectangle OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null)
        {
            return Rectangle.Empty;//TODO 待完善标注绘制
        }
        /// <inheritdoc/>
        public bool Select(IExtent region)
        {
            return true;
        }
        /// <inheritdoc/>
        public void Invalidate()
        {
        }
    }
}
