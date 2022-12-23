using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    public class LabelLayer : Layer, ILabelLayer
    {
        public IFeatureLayer FeatureLayer { get; }
        public new ILabelCategory DefaultCategory { get => base.DefaultCategory as ILabelCategory; set => base.DefaultCategory = value; }

        public new ILabelCategoryCollection Children { get =>base.Children as ILabelCategoryCollection; protected set=> base.Children = value; }
        public LabelLayer(IFeatureLayer featureLayer)
        {
            FeatureLayer = featureLayer;
            Children = new LabelCategoryCollection(this);
        }
        public void ClearSelection()
        {
            throw new NotImplementedException();
        }

        public void CreateLabels()
        {
            throw new NotImplementedException();
        }
        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Action<string, int> progressAction = null, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            throw new NotImplementedException();
        }
        public bool Select(IExtent region)
        {
            throw new NotImplementedException();
        }

        public void Invalidate()
        {
            throw new NotImplementedException();
        }
    }
}
