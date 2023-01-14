using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分组
    /// </summary>
    public class Group : RenderableItem, IGroup
    {
        /// <inheritdoc/>
        public new IRenderableItemCollection Children
        {
            get
            {
                if (base.Children is IRenderableItemCollection collection)
                {
                    return collection;
                }
                else
                {
                    throw new Exception($"{nameof(Children)}类型必须为{nameof(IRenderableItemCollection)}");
                }
            }
            set => base.Children = value;
        }
        /// <inheritdoc/>
        public int LayerCount => Children.Count();

        /// <inheritdoc/>
        public override IExtent Extent
        {
            get
            {
                IExtent destExtent =new Extent();
                foreach (var item in Children)
                {
                    if (item is IRenderableItem renderableItem)
                    {
                        destExtent.ExpandToInclude(renderableItem.Extent);
                    }
                }
                return destExtent;
            }
        }

        /// <summary>
        /// 实例化<seealso cref="Group"/>
        /// </summary>
        public Group()
        {
            Children = new RenderableItemCollection();
        }
        /// <inheritdoc/>
        public override void Draw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null)
        {
            if (mapArgs == null || mapArgs.Graphics == null || mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() || cancelFunc?.Invoke() == true || Children.Count == 0)
            {
                return;
            }

            var allVisibleLayers = GetAllLayers().Where(x => x.GetVisible(mapArgs.DestExtent));
            string progressStr = this.GetProgressString();
            progressAction?.Invoke(progressStr, 0);
            double increment = 100.0 / allVisibleLayers.Count();
            double totalProgress = 0;
            Action<string, int> newProgressAction = (txt, progress) =>
            {
                if (progressAction != null)
                {
                    var destProgress = (int)((double)progress / allVisibleLayers.Count() + totalProgress);
                    progressAction.Invoke(txt, destProgress);
                }
            };
            var visibleChildren = Children.Where(x => x is LegendItem legendItem && legendItem.IsVisible).ToList();
            for (int i = visibleChildren.Count-1; i >=0; i--)
            {
                var item = visibleChildren[i];
                if (item is ILegendItem legendItem && legendItem.IsVisible)
                {
                    if (item is ILayer layer)
                    {
                        layer.Draw(mapArgs, selected, newProgressAction, cancelFunc, invalidateMapFrameAction);
                    }
                    else if (item is IGroup group)
                    {
                        group.Draw(mapArgs, selected, newProgressAction, cancelFunc, invalidateMapFrameAction);
                    }
                }
                totalProgress += increment;
                invalidateMapFrameAction?.Invoke();
            }
            progressAction?.Invoke(progressStr, 100);
        }
        /// <inheritdoc/>
        public ILayer? GetLayer(int index)
        {
            ILayer? layer = null;
            if (index >= 0 && index < LayerCount)
            {
                layer = GetLayers().ElementAt(index);
            }
            return layer;
        }

        /// <inheritdoc/>
        public IEnumerable<ILayer> GetLayers()
        {
            foreach (var item in Children)
            {
                if (item is ILayer layer)
                {
                    yield return layer;
                }
            }
        }
        /// <inheritdoc/>
        public IEnumerable<IGroup> GetGroups()
        {
            foreach (var item in Children)
            {
                if (item is IGroup group)
                {
                    yield return group;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IFeatureLayer> GetAllFeatureLayers()
        {
            return GetItems<IFeatureLayer>(Children, true);
        }
       
        private IEnumerable<T> GetItems<T>(IEnumerable<IBaseItem> items, bool searchChildren) where T : ITreeItem
        {
            foreach (var item in items)
            {
                if (item is T t)
                {
                    yield return t;
                }
                if (searchChildren && item is ITreeItem treeItem)
                {
                    foreach (var childItem in GetItems<T>(treeItem.Children, searchChildren))
                    {
                        yield return childItem;
                    }
                }
            }
        }
        /// <inheritdoc/>
        public IEnumerable<IRasterLayer> GetAllRasterLayers()
        {
            return GetItems<IRasterLayer>(Children, true);
        }

        /// <inheritdoc/>
        public bool AddLayer(ILayer layer, int? index = null)
        {
            bool ret = false;
            if (layer == null)
            {
                return ret;
            }
            if (index.HasValue)
            {
                if (index.Value < 0 || index.Value > Children.Count)
                {
                    return ret;
                }
                Children.Insert(index.Value, layer);
            }
            else
            {
                Children.Add(layer);
            }
            ret = true;
            return ret;
        }

        /// <inheritdoc/>
        public IEnumerable<ILayer> GetAllLayers()
        {
            return GetItems<ILayer>(Children, true);
        }

        /// <inheritdoc/>
        public IEnumerable<IFeatureLayer> GetFeatureLayers()
        {
            return GetItems<IFeatureLayer>(Children, false);
        }

        /// <inheritdoc/>
        public IEnumerable<IRasterLayer> GetRasterLayers()
        {
            return GetItems<IRasterLayer>(Children, false);
        }
    }
}
