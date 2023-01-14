using EM.GIS.Data;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EM.Bases;
using System.Collections.Specialized;
using System;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层集合
    /// </summary>
    public class RenderableItemCollection : ItemCollection<IBaseItem>, IRenderableItemCollection
    {
        /// <inheritdoc/>
        public new IRenderableItem this[int index]
        {
            get
            {
                if (base[index] is IRenderableItem renderableItem)
                {
                    return renderableItem;
                }
                else
                {
                    throw new Exception($"第{index}个元素不为 {nameof(ICategoryCollection)} 类型");
                }
            }
            set => base[index] = value;
        }
        /// <inheritdoc/>
        public IGroup? AddGroup(string groupName = "")
        {
            string destGroupName = groupName;
            if (string.IsNullOrEmpty(groupName))
            {
                destGroupName = GetDifferenctName("分组");
            }
            IGroup? group = null;
            if (!string.IsNullOrEmpty(destGroupName))
            {
                group = new Group()
                {
                    Text = destGroupName,
                    IsVisible = true
                };
                Insert(0, group);
            }
            return group;
        }
        private string GetDifferenctName(string prefix, int i = 0)
        {
            string name = $"{prefix}{i}";
            foreach (ILegendItem item in this)
            {
                if (item.Text == name)
                {
                    name = GetDifferenctName(prefix, ++i);
                    break;
                }
            }
            return name;
        }
        /// <inheritdoc/>
        public ILayer? AddLayer(IDataSet dataSet, bool isVisible = true)
        {
            ILayer? layer = null;
            //var ss = dataSet as ISelfLoadSet;
            //if (ss != null) return Add(ss);
            switch (dataSet)
            {
                case IFeatureSet featureSet:
                    layer = AddLayer(featureSet, isVisible);
                    break;
                case IRasterSet rasterSet:
                    layer = AddLayer(rasterSet, isVisible);
                    break;
                default:
                    return layer;
            }
            if (layer != null)
            {
                layer.Text = dataSet.Name;
            }
            return layer;
        }

        /// <inheritdoc/>
        public IFeatureLayer? AddLayer(IFeatureSet featureSet, bool isVisible = true)
        {
            IFeatureLayer? layer = null;
            if (featureSet == null) return null;

            if (featureSet.FeatureType == FeatureType.Point || featureSet.FeatureType == FeatureType.MultiPoint)
            {
                layer = new PointLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polyline)
            {
                layer = new LineLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polygon)
            {
                layer = new PolygonLayer(featureSet);
            }

            if (layer != null)
            {
                layer.IsVisible = isVisible;
                Insert(0, layer);
            }

            return layer;
        }

        /// <inheritdoc/>
        public IRasterLayer? AddLayer(IRasterSet raster, bool isVisible = true)
        {
            IRasterLayer? rasterLayer = null;
            if (raster != null)
            {
                rasterLayer = new RasterLayer(raster)
                {
                    IsVisible = isVisible
                };
                Insert(0, rasterLayer);
            }
            return rasterLayer;
        }
        /// <inheritdoc/>
        public override void Add(IBaseItem item)
        {
            if (item is IRenderableItem)
            {
                base.Insert(0, item);
            }
        }
    }
}