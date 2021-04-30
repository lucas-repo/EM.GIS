using EM.GIS.Data;
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
        private IProgressHandler _progressHandler;
        public IProgressHandler ProgressHandler
        {
            get { return _progressHandler; }
            set
            {
                if (SetProperty(ref _progressHandler, value, nameof(ProgressHandler)))
                {
                    foreach (ILegendItem item in this)
                    {
                        item.ProgressHandler = _progressHandler;
                    }
                }
            }
        }
        public LegendItemCollection(ILegendItem parent) : base(parent)
        { }
    }
}