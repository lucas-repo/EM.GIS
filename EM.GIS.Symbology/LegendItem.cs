using EM.Bases;
using EM.GIS.Data;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素
    /// </summary>
    [Serializable]
    public abstract class LegendItem : TreeItem, ILegendItem
    {
        private IFrame? frame;
        /// <inheritdoc/>
        public IFrame? Frame
        {
            get { return frame; }
            set { SetProperty(ref frame, value); }
        }
        /// <inheritdoc/>
        public ObservableCollection<IContextCommand> ContextCommands { get; } = new ObservableCollection<IContextCommand>();

        /// <summary>
        /// 是否已释放
        /// </summary>
        protected bool IsDisposed { get; private set; }
        /// <inheritdoc/>
        public override IItemCollection<IBaseItem> Children
        {
            get => base.Children;
            protected set
            {
                if (base.Children != value)
                {
                    var oldChildren = base.Children;
                    base.Children = value;
                    base.Children.CollectionChanged += Children_CollectionChanged;
                    if (oldChildren != null)
                    {
                        oldChildren.CollectionChanged -= Children_CollectionChanged;
                    }
                }
            }
        }
        /// <summary>
        /// 实例化<seealso cref="LegendItem"/>
        /// </summary>
        public LegendItem()
        {
            IsVisible = true;
            PropertyChanged += LegendItem_PropertyChanged;
        }
        /// <summary>
        /// 实例化<seealso cref="LegendItem"/>
        /// </summary>
        /// <param name="parent">父元素</param>
        public LegendItem(ILegendItem parent) : this()
        {
            Parent = parent;
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Action setOldItemsAction = new Action(() =>
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ILegendItem t)
                    {
                        t.Parent = default;
                        t.Frame = default;
                    }
                }
            });
            Action setNewItemsAction = new Action(() =>
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ILegendItem t)
                    {
                        t.Parent = this;
                        t.Frame = Frame;
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
            }
        }

        private void LegendItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Frame):
                    if (Children != null)
                    {
                        foreach (var item in Children)
                        {
                            if (item is ILegendItem legendItem)
                            {
                                legendItem.Frame = Frame;
                            }
                        }
                    }
                    break;
            }
        }
        /// <inheritdoc/>
        public virtual void DrawLegend(Graphics g, Rectangle rectangle) { }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    PropertyChanged -= LegendItem_PropertyChanged;
                    Children.CollectionChanged -= Children_CollectionChanged;
                    if (Children is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                IsDisposed = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~LegendItem()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        /// <inheritdoc/>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc/>
        public bool GetVisible()
        {
            bool ret = IsVisible;
            if (!IsVisible)
            {
                return ret;
            }
            ITreeItem? parent = Parent;
            while (parent != null)
            {
                if (!parent.IsVisible)
                {
                    ret = false;
                    break;
                }
                else
                {
                    parent = parent.Parent;
                }
            }
            return ret;
        }
    }
}