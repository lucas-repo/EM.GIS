using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
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
                IExtent destExtent = new Extent();
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
        public override Rectangle Draw(MapArgs mapArgs, bool onlyInitialized = false, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null)
        {
            var ret = Rectangle.Empty;
            if (mapArgs == null || mapArgs.Graphics == null || mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() || cancelFunc?.Invoke() == true || Children.Count == 0)
            {
                return ret;
            }

            List<IRenderableItem> visibleItems = new List<IRenderableItem>();
            foreach (var item in Children)
            {
                if (item is IRenderableItem renderableItem && renderableItem.IsVisible)
                {
                    if (item is IGroup group)
                    {
                        visibleItems.Add(group);
                    }
                    else
                    {
                        if (onlyInitialized)
                        {
                            if (renderableItem.IsDrawingInitialized(mapArgs, mapArgs.DestExtent))
                            {
                                visibleItems.Add(renderableItem);
                            }
                        }
                        else
                        {
                            visibleItems.Add(renderableItem);
                        }
                    }
                }
            }
            if (visibleItems.Count == 0)
            {
                return ret;
            }
            string progressStr = this.GetProgressString();
            //progressAction?.Invoke(progressStr, 0);
            double increment = 100.0 / visibleItems.Count;
            double totalProgress = 0;
            Action<string, int> newProgressAction = (txt, progress) =>
            {
                if (progressAction != null)
                {
                    var destProgress = (int)((double)progress / visibleItems.Count + totalProgress);
                    progressAction.Invoke(txt, destProgress);
                }
            };
            for (int i = visibleItems.Count - 1; i >= 0; i--)
            {
                var rect = Rectangle.Empty;
                switch (visibleItems[i])
                {
                    case ILayer layer:
                        rect = layer.Draw(mapArgs, onlyInitialized, selected, newProgressAction, cancelFunc, invalidateMapFrameAction);
                        break;
                    case IGroup group:
                        rect = group.Draw(mapArgs, onlyInitialized, selected, newProgressAction, cancelFunc, invalidateMapFrameAction);
                        break;
                }
                if (!rect.IsEmpty)
                {
                    ret = ret.ExpandToInclude(rect);
                    //invalidateMapFrameAction?.Invoke(rect);
                }
                totalProgress += increment;
            }
            //progressAction?.Invoke(progressStr, 100);
            return ret;
        }
        /// <inheritdoc/>
        public override bool IsDrawingInitialized(IProj proj, IExtent extent)
        {
            bool ret = true;
            foreach (var item in Children)
            {
                if (item is IRenderableItem renderableItem && !renderableItem.IsDrawingInitialized(proj,extent))
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
    }
}
