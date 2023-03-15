using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可渲染的元素
    /// </summary>
    public abstract class RenderableItem : LegendItem, IRenderableItem
    {
        /// <inheritdoc/>
        public new IGroup? Parent
        {
            get => base.Parent as IGroup;
            set => base.Parent = value;
        }
        /// <inheritdoc/>
        public virtual IExtent Extent => Geometries.Extent.Empty;
        /// <inheritdoc/>
        public bool UseDynamicVisibility { get; set; }
        /// <inheritdoc/>
        public double MaxInverseScale { get; set; }
        /// <inheritdoc/>
        public double MinInverseScale { get; set; }
        private string _progressMessage = string.Empty;
        /// <inheritdoc/>
        public event EventHandler? SelectionChanged;

        /// <summary>
        /// 进度消息文字
        /// </summary>
        protected string ProgressMessage
        {
            get
            {
                if (string.IsNullOrEmpty(_progressMessage))
                {
                    _progressMessage = this.GetProgressString();
                }
                return _progressMessage;
            }
        }
        /// <summary>
        /// 触发选择改变事件
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <inheritdoc/>
        public bool SelectionEnabled { get; set; }
        /// <inheritdoc/>
        public virtual bool SelectionChangesIsSuspended => false;

        /// <inheritdoc/>
        public bool GetVisible(IExtent extent, Rectangle rectangle)
        {
            bool ret = GetVisible();
            if (!ret)
            {
                return ret;
            }

            if (UseDynamicVisibility)
            {
                //todo compare the scalefactor 
            }

            return true;
        }
        /// <inheritdoc/>
        public bool GetVisible(IExtent extent)
        {
            bool ret = GetVisible();
            if (!ret)
            {
                return ret;
            }
            ret = Extent.Intersects(extent);
            return ret;
        }
        /// <inheritdoc/>
        public abstract Rectangle Draw(MapArgs mapArgs, bool onlyInitialized = false, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null);
        /// <inheritdoc/>
        public virtual bool IsDrawingInitialized(IProj proj, IExtent extent) => true;
        /// <inheritdoc/>
        public virtual void InitializeDrawing(IProj proj, IExtent extent, Func<bool>? cancelFunc = null) { }

        /// <inheritdoc/>
        public virtual bool ClearSelection(out IExtent affectedArea, bool force)
        {
            affectedArea = new Extent();
            return false;
        }

        /// <inheritdoc/>
        public virtual bool InvertSelection(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea)
        {
            affectedArea = new Extent();
            return false;
        }

        /// <inheritdoc/>
        public virtual bool Select(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea, ClearStates clear)
        {
            affectedArea = new Extent();
            return false;
        }

        /// <inheritdoc/>
        public virtual bool UnSelect(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea)
        {
            affectedArea = new Extent();
            return false;
        }

        /// <inheritdoc/>
        public virtual void ResumeSelectionChanges()
        {
        }

        /// <inheritdoc/>
        public virtual void SuspendSelectionChanges()
        {
        }
    }
}