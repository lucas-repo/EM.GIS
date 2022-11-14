using EM.GIS.Controls;
using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private ILegendItem _selectedItem;
        /// <summary>
        /// 选择的元素
        /// </summary>
        public ILegendItem SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value, nameof(SelectedItem)); }
        }

        public Legend()
        {
            InitializeComponent();
            LegendItems = new LayerCollection(null);
            LegendItems.CollectionChanged += LegendItems_CollectionChanged; 
            DataContext = this;
            ItemsSource = LegendItems; 
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue as ILegendItem;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool SetProperty<T>(ref T t, T value, string propertyName, bool autoDisposeOldValue = false)
        {
            bool flag = SetProperty(ref t, value, autoDisposeOldValue);
            if (flag)
            {
                OnPropertyChanged(propertyName);
            }

            return flag;
        }

        public bool SetProperty<T>(ref T t, T value, bool autoDisposeOldValue = false)
        {
            bool result = false;
            if (!Equals(t, value))
            {
                T val = t;
                t = value;
                IDisposable disposable = default;
                int num;
                if (autoDisposeOldValue)
                {
                    disposable = val as IDisposable;
                    num = ((disposable != null) ? 1 : 0);
                }
                else
                {
                    num = 0;
                }

                if (num != 0)
                {
                    disposable.Dispose();
                }

                result = true;
            }

            return result;
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

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void LegendItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        public ILegendItemCollection LegendItems { get; }

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

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
    }
}
