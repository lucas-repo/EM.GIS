using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Symbology;
using EM.IOC;
using EM.WpfBases;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// Legend.xaml 的交互逻辑
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ILegend))]
    public partial class Legend : TreeView, ILegend
    {
        private LegendViewModel ViewModel { get; }
        public Legend(IIocManager iocManager)
        {
            InitializeComponent();
            SelectedItemChanged += Legend_SelectedItemChanged;
            ViewModel = new LegendViewModel(this,iocManager);
            DataContext = this;
            ItemsSource = LegendItems;
        }

        private void Legend_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedLegendItemItem = e.NewValue as ILegendItem;
        }

        /// <inheritdoc/>
        public ObservableCollection<ITreeItem> LegendItems => ViewModel.LegendItems;

        /// <inheritdoc/>
        public void AddMapFrame(IFrame frame)=>ViewModel.AddMapFrame(frame);

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
