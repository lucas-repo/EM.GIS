using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素集合
    /// </summary>
    public abstract class LegendItemCollection:ItemCollection<ILegendItem, ILegendItem>, ILegendItemCollection
    {
        public LegendItemCollection() 
        {
        }
        public LegendItemCollection(ILegendItem parent) : base(parent)
        { }
    }
}