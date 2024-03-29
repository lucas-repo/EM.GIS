﻿using EM.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 栅格分类集合
    /// </summary>
    public class RasterCategoryCollection : CategoryCollection, IRasterCategoryCollection
    {
        public new IRasterCategory this[int index]
        {
            get => Items[index] as IRasterCategory;
            set => Items[index] = value;
        }
        public new IRasterLayer Parent { get => base.Parent as IRasterLayer; set => base.Parent = value; }

        public RasterCategoryCollection(IRasterLayer parent) : base(parent)
        { }

        public override void Add(IBaseItem item)
        {
            if (item is IRasterCategory)
            {
                base.Add(item);
            }
        }
    }
}
