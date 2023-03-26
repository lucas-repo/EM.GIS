using EM.Bases;
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

        /// <inheritdoc/>
        public event EventHandler? SelectionChanged;
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
                if (item is IRenderableItem renderableItem && !renderableItem.IsDrawingInitialized(proj, extent))
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }

        private int _suspendLevel;
        private bool _selectionChanged;
        /// <inheritdoc/>
        public override bool SelectionChangesIsSuspended => _suspendLevel > 0;
        /// <inheritdoc/>
        public void SuspendSelectionChanges()
        {
            if (_suspendLevel == 0)
            {
                _selectionChanged = false;
            }

            _suspendLevel += 1;

            // using an integer allows many nested levels of suspension to exist,
            // resuming events only once all the nested resumes are called.
        }
        /// <inheritdoc />
        public override bool ClearSelection(out IExtent affectedAreas, bool force = false)
        {
            affectedAreas = new Extent();
            bool changed = false;
            SuspendSelectionChanges();

            foreach (var item in Children)
            {
                if (item is IRenderableItem renderableItem)
                {
                    if (renderableItem.ClearSelection(out var layerArea, force))
                    {
                        changed = true;
                        affectedAreas.ExpandToInclude(layerArea);
                    }
                }
            }

            ResumeSelectionChanges();
            return changed;
        }

        /// <inheritdoc />
        public override bool InvertSelection(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea)
        {
            affectedArea = new Extent();
            bool somethingChanged = false;
            SuspendSelectionChanges();
            foreach (var item in Children)
            {
                if (item is IRenderableItem renderableItem && renderableItem.SelectionEnabled && renderableItem.IsVisible)
                {
                    if (renderableItem.InvertSelection(tolerant, strict, mode, out var layerArea))
                    {
                        somethingChanged = true;
                        affectedArea.ExpandToInclude(layerArea);
                    }
                }
            }
            ResumeSelectionChanges();
            return somethingChanged;
        }
        /// <inheritdoc />
        public override bool Select(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea, ClearStates clear)
        {
            affectedArea = new Extent();
            bool somethingChanged = false;
            SuspendSelectionChanges();
            foreach (var item in Children)
            {
                if (item is IRenderableItem renderableItem && renderableItem.SelectionEnabled && renderableItem.GetVisible())
                {
                    IExtent layerArea;
                    IExtent destTolerant = tolerant;
                    IExtent destStrict = strict;
                    if (renderableItem is ILayer layer)
                    {
                        if (Frame != null && layer.DataSet != null && !Equals(layer.DataSet.Projection, Frame.Projection))
                        {
                            destTolerant = tolerant.Copy();
                            Frame.Projection.ReProject(layer.DataSet.Projection, destTolerant);
                            destStrict = strict.Copy();
                            Frame.Projection.ReProject(layer.DataSet.Projection, destStrict);
                        }
                    }
                    if (renderableItem.Select(destTolerant, destStrict, mode, out layerArea, clear))
                    {
                        somethingChanged = true;
                        affectedArea.ExpandToInclude(layerArea);
                    }
                }
            }
            ResumeSelectionChanges(); // fires only AFTER the individual layers have fired their events.
            return somethingChanged;
        }


        /// <inheritdoc />
        public override bool UnSelect(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea)
        {
            affectedArea = new Extent();
            bool somethingChanged = false;
            SuspendSelectionChanges();
            foreach (var item in Children)
            {
                if (item is IRenderableItem renderableItem && renderableItem.SelectionEnabled && renderableItem.GetVisible())
                {
                    IExtent layerArea;
                    if (renderableItem.UnSelect(tolerant, strict, mode, out layerArea))
                    {
                        somethingChanged = true;
                        affectedArea.ExpandToInclude(layerArea);
                    }
                }
            }
            ResumeSelectionChanges();
            return somethingChanged;
        }
        /// <inheritdoc/>
        public void ResumeSelectionChanges()
        {
            _suspendLevel -= 1;
            if (SelectionChangesIsSuspended == false)
            {
                if (_selectionChanged)
                {
                    OnSelectionChanged();
                }
            }

            if (_suspendLevel < 0) _suspendLevel = 0;
        }

        /// <summary>
        /// 触发SelectionChanged事件
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChangesIsSuspended)
            {
                _selectionChanged = true;
                return;
            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <inheritdoc/>
        protected override void OnChildrenItemAdded(ILegendItem t)
        {
            if (t is IFeatureLayer featureLayer)
            {
                featureLayer.Selection.Changed += LayerSelection_Changed;
            }
            base.OnChildrenItemAdded(t);
        }

        private void LayerSelection_Changed(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        /// <inheritdoc/>
        protected override void OnChildrenItemRemoved(ILegendItem t)
        {
            if (t is IFeatureLayer featureLayer)
            {
                featureLayer.Selection.Changed -= LayerSelection_Changed;
            }
            base.OnChildrenItemRemoved(t);
        }
    }
}
