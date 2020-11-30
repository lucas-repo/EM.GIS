using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分类集合
    /// </summary>
    public abstract class CategoryCollection : LegendItemCollection, ICategoryCollection
    {
        public CategoryCollection()
        { }
        public CategoryCollection(ILayer parent) : base(parent)
        { }
        #region 重写部分
        public new ICategory this[int index] { get => base[index] as ICategory; set => base[index] = value; }

        public new ILayer Parent { get => base.Parent as ILayer; set => base.Parent = value; }
        public new IEnumerator<ICategory> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as ICategory;
            }
        }

        #endregion
    }
}