using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 元素集合
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    [Serializable]
    public abstract class ItemCollection< TParent, TChild> : ItemCollection<TChild>,  IItemCollection<TParent, TChild>
    {
        [NonSerialized]
        private TParent _parent;

        public TParent Parent { get => _parent; set => _parent = value; }
        public ItemCollection()
        {
            Items.CollectionChanged += Items_CollectionChanged;
        }
        public ItemCollection(TParent parent):this()
        {
            _parent = parent;
        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Action setOldItemsAction = new Action(() =>
            {
                foreach (var item in e.OldItems)
                {
                    if (item is IParentItem<TParent> t)
                    {
                        t.Parent = default;
                    }
                }
            });
            Action setNewItemsAction = new Action(() =>
            {
                foreach (var item in e.NewItems)
                {
                    if (item is IParentItem<TParent> t)
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
    /// <summary>
    /// 元素集合
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    public abstract class ItemCollection< TChild> : IItemCollection< TChild>
    {
        /// <summary>
        /// 元素集合
        /// </summary>
        protected ObservableCollection<TChild> Items { get; }
        public TChild this[int index] { get => Items[index]; set => Items[index] = value; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => Items.Count;

        public bool IsReadOnly => false;
        public ItemCollection()
        {
            Items = new ObservableCollection<TChild>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public virtual void Add(TChild item)
        {
            Items.Add(item);
        }

        public virtual void Clear()
        {
            Items.Clear();
        }

        public virtual bool Contains(TChild item)
        {
            return Items.Contains(item);
        }

        public virtual void CopyTo(TChild[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<TChild> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public virtual int IndexOf(TChild item)
        {
            return Items.IndexOf(item);
        }

        public virtual void Insert(int index, TChild item)
        {
            Items.Insert(index, item);
        }

        public void Move(int oldIndex, int newIndex)
        {
            Items.Move(oldIndex, newIndex);
        }

        public virtual bool Remove(TChild item)
        {
            return Items.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}