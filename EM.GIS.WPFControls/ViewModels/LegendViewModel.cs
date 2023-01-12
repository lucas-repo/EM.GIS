using EM.Bases;
using EM.GIS.Symbology;
using EM.WpfBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 图例视图模型
    /// </summary>
    public class LegendViewModel : ViewModel<Legend>
    {
        private IFrame? Frame
        {
            get
            {
                IFrame? ret = LegendItems.FirstOrDefault(x => x is IFrame) as IFrame;
                return ret;
            }
        }
        private ILegendItem? _selectedItem;
        /// <summary>
        /// 选择的元素
        /// </summary>
        public ILegendItem? SelectedLegendItemItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }
        /// <summary>
        /// 元素集合
        /// </summary>
        public ObservableCollection<ITreeItem> LegendItems { get; }
        public LegendViewModel(Legend t) : base(t)
        {
            LegendItems = new ObservableCollection<ITreeItem>();
            LegendItems.CollectionChanged += LegendItems_CollectionChanged;
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
        /// <summary>
        /// 已初始化的元素
        /// </summary>
        private Dictionary<ILegendItem, List<IContextCommand>> InitializedItems { get; } = new Dictionary<ILegendItem, List<IContextCommand>>();
        private void HandleNewItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ILegendItem legendItem)
                    {
                        if (InitializedItems.ContainsKey(legendItem))
                        {
                            continue;
                        }
                        legendItem.PropertyChanged += Item_PropertyChanged;
                        InitializedItems[legendItem] = new List<IContextCommand>();
                        AddZoomCommand(legendItem);
                        AddRemoveCommand(legendItem);
                    }
                }
            }
        }
        private void AddZoomCommand(ILegendItem legendItem)
        {
            IContextCommand? zoomCmd = null;
            switch (legendItem)
            {
                case ILayer layer:
                    zoomCmd = new ContextCommand((obj) =>
                    {
                        if (Frame != null && !layer.Extent.IsEmpty())
                        {
                            Frame.View.ViewExtent = layer.Extent;
                        }
                    });
                    break;
                case IGroup group:
                    zoomCmd = new ContextCommand((obj) =>
                    {
                        if (Frame != null && !group.Extent.IsEmpty())
                        {
                            Frame.View.ViewExtent = group.Extent;
                        }
                    })
                    {
                        Name="居中"
                    };
                    break;
            }
            if (zoomCmd != null)
            {
                legendItem.ContextCommands.Add(zoomCmd);
                InitializedItems[legendItem].Add(zoomCmd);
            }
        }
        private void AddRemoveCommand(ILegendItem legendItem)
        {
            IContextCommand? zoomCmd = null;
            switch (legendItem)
            {
                case ILayer layer:
                case IGroup group:
                    zoomCmd = new ContextCommand((obj) =>
                    {
                        if (legendItem.Parent != null)
                        {
                            legendItem.Parent.Children.Remove(legendItem);
                        }
                    })
                    {
                        Name="移除"
                    };
                    break;
            }
            if (zoomCmd != null)
            {
                legendItem.ContextCommands.Add(zoomCmd);
                InitializedItems[legendItem].Add(zoomCmd);
            }
        }
        private void HandleOldItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ILegendItem legendItem)
                    {
                        if (InitializedItems.ContainsKey(legendItem))
                        {
                            legendItem.PropertyChanged -= Item_PropertyChanged;
                            foreach (var contextCommand in InitializedItems[legendItem])
                            {
                                legendItem.ContextCommands.Remove(contextCommand);
                            }
                            InitializedItems.Remove(legendItem);
                        }
                    }
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
        /// <summary>
        /// 添加地图框架
        /// </summary>
        /// <param name="frame"></param>
        public void AddMapFrame(IFrame frame)
        {
            LegendItems.Add(frame);
        }
    }
}
