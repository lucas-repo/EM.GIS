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
        protected override void OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null)
        {
        }
        public bool Select(IExtent region)
        {
            return true;
        }

        public void Invalidate()
        {
        }
    }
}
