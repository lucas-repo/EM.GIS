using EM.GIS.Controls;
using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// Legend.xaml 的交互逻辑
    /// </summary>
    public partial class Legend : TreeView, ILegend, INotifyPropertyChanged
    {
        private ILegendItem? _selectedItem;
        /// <summary>
        /// 选择的元素
        /// </summary>
        public ILegendItem? SelectedLegendItemItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public Legend()
        {
            InitializeComponent();
            LegendItems = new LegendItemCollection(null);
            LegendItems.CollectionChanged += LegendItems_CollectionChanged; 
            DataContext = this;
            ItemsSource = LegendItems; 
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedLegendItemItem = e.NewValue as ILegendItem;
        }
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        public virtual void OnPropertyChanged(string? propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// 设置值并调用属性改变通知
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">变量</param>
        /// <param name="value">值</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="autoDisposeOldValue">自动释放旧值</param>
        /// <returns>成功true，反之false</returns>
        public bool SetProperty<T>(ref T t, T value, [CallerMemberName] string? propertyName = null, bool autoDisposeOldValue = false)
        {
            if (!Equals(t, value))
            {
                var oldValue = t;
                t = value;
                if (autoDisposeOldValue &&oldValue is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        private void HandleNewItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ILegendItem item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }
        private void HandleOldItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ILegendItem item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //if (sender is ILegendItem legendItem)
            //{
            //    switch (e.PropertyName)
            //    {
            //        case nameof(ILegendItem.IsSelected):
            //            if (legendItem.IsSelected)
            //            {
            //                SelectedItem = legendItem;
            //            }
            //            else
            //            {
            //                if (SelectedItem == legendItem)
            //                {
            //                    SelectedItem = null;
            //                }
            //            }
            //            break;
            //    }
            //}
        }

        private void LegendItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    HandleNewItems(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    HandleOldItems(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    HandleOldItems(e);
                    HandleNewItems(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    HandleOldItems(e);
                    break;
            }
        }
        /// <inheritdoc/>
        public ILegendItemCollection LegendItems { get; }

        /// <inheritdoc/>
        public void AddMapFrame(IFrame mapFrame)
        {
            LegendItems.Add(mapFrame);
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static DependencyObject? VisualUpwardSearch<T>(DependencyObject? source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
    }
}
