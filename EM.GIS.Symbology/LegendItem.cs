using EM.Bases;
using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素
    /// </summary>
    [Serializable]
    public abstract class LegendItem : BaseCopy, ILegendItem
    {
        public event EventHandler ItemChanged;
        public event EventHandler RemoveItem;

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value, nameof(IsVisible)); }
        }
        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value, nameof(IsExpanded)); }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value, nameof(IsSelected)); }
        }

        public string Text { get; set; }
        public ILegendItem Parent { get; set; }

        public ObservableCollection<IBaseCommand> ContextCommands { get; } = new ObservableCollection<IBaseCommand>();
        public LegendMode LegendSymbolMode { get; set; }
        public LegendType LegendType { get; set; }

        private IProgressHandler _progressHandler;
        public IProgressHandler ProgressHandler
        {
            get { return _progressHandler; }
            set
            {
                if (SetProperty(ref _progressHandler, value, nameof(ProgressHandler)))
                {
                    if (LegendItems != null)
                    {
                        LegendItems.ProgressHandler = _progressHandler;
                    }
                }
            }
        }
        public ILegendItemCollection LegendItems { get; protected set; }

        public LegendItem()
        {
        }
        public LegendItem(ILegendItem parent) : this()
        {
            Parent = parent;
        }

        public virtual void DrawLegend(Graphics g, Rectangle rectangle)
        {
        }
    }
}