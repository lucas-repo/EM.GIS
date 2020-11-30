using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;



namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素
    /// </summary>
    [Serializable]
    public abstract class LegendItem : Descriptor, ILegendItem
    {
        public event EventHandler ItemChanged;
        public event EventHandler RemoveItem;

        public bool IsVisible { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public string Text { get; set; }
        public ILegendItem Parent { get; set; }

        public List<SymbologyMenuItem> ContextMenuItems { get; set; }
        public LegendMode LegendSymbolMode { get ; set ; }
        public LegendType LegendType { get; set; }

        public ILegendItemCollection Items { get; protected set; }

        public LegendItem()
        {
        }
        public LegendItem(ILegendItem parent) : this()
        {
            Parent = parent;
        }

        public virtual void DrawLegend(Graphics g, Rectangle rectangle)
        {
            throw new NotImplementedException();
        }
    }
}