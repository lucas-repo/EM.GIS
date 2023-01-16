﻿using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;

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
        public abstract void Draw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null);
    }
}