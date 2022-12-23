using EM.Bases;
using EM.GIS.Data;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素
    /// </summary>
    [Serializable]
    public abstract class LegendItem : TreeItem, ILegendItem
    {
        public ObservableCollection<IBaseCommand> ContextCommands { get; } = new ObservableCollection<IBaseCommand>();

        private ProgressDelegate _progress;
        /// <summary>
        /// 是否已释放
        /// </summary>
        protected bool IsDisposed { get;private set; }

        public LegendItem()
        {
        }
        public LegendItem(ILegendItem parent) : this()
        {
            Parent = parent;
        }

        public virtual void DrawLegend(Graphics g, Rectangle rectangle) { }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    if (Children.Count > 0)
                    {
                        foreach (var item in Children)
                        {
                            if (item is IDisposable disposable)
                            {
                                disposable.Dispose();
                            }
                        }
                        Children.Clear();
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

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}