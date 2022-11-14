using EM.Bases;
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
    public abstract class CategoryCollection : ItemCollection<IBaseItem>, ICategoryCollection
    {
        public CategoryCollection(ILayer parent) 
        {
            _parent = parent;
        }
        #region 重写部分
        public new ICategory this[int index]
        { 
            get => base[index] as ICategory; 
            set => base[index] = value; 
        }

        [NonSerialized]
        private ILayer _parent;
        /// <inheritdoc/>
        public ILayer Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }
        #endregion
    }
}