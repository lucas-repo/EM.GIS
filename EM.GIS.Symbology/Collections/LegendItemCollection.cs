using EM.Bases;
using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Transactions;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素集合
    /// </summary>
    public class LegendItemCollection:ItemCollection<IBaseItem>, ILegendItemCollection
    {
        [NonSerialized]
        private ILegendItem _parent;
        /// <inheritdoc/>
        public ILegendItem Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }

        private ProgressDelegate _progress;
        /// <inheritdoc/>
        public ProgressDelegate Progress
        {
            get { return _progress; }
            set
            {
                if (SetProperty(ref _progress, value, nameof(Progress)))
                {
                    foreach (var item in this)
                    {
                        if (item is IProgressHandler progressHandler)
                        {
                            progressHandler.Progress = _progress;
                        }
                    }
                }
            }
        }
        /// <inheritdoc/>
        public new ILegendItem this[int index] { get => base[index] as ILegendItem; set => base[index] =value; }

        public LegendItemCollection(ILegendItem parent) 
        {
            _parent = parent;
            CollectionChanged += LegendItemCollection_CollectionChanged;
        }

        private void LegendItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Action setOldItemsAction = new Action(() =>
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ILegendItem t)
                    {
                        t.Parent = default;
                    }
                }
            });
            Action setNewItemsAction = new Action(() =>
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ILegendItem t)
                    {
                        t.Parent = Parent;
                    }
                }
            });
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    setNewItemsAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    setOldItemsAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    setOldItemsAction.Invoke();
                    setNewItemsAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    setOldItemsAction.Invoke();
                    break;
            }
        }
    }
}