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

            var allVisibleLayers = Children.GetAllLayers().Where(x => x.GetVisible(mapArgs.DestExtent));
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
    }
}
