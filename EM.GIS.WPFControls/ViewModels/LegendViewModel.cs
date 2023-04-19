using EM.Bases;
using EM.GIS.Symbology;
using EM.IOC;
using EM.WpfBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private IIocManager IocManager { get; }
        public LegendViewModel(Legend t, IIocManager iocManager) : base(t)
        {
            IocManager = iocManager;
            LegendItems = new ObservableCollection<ITreeItem>();
            LegendItems.CollectionChanged += LegendItems_CollectionChanged;
            PropertyChanged += LegendViewModel_PropertyChanged;
        }

        private void LegendViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedLegendItemItem):
                    if (SelectedLegendItemItem !=null  && InitializedItems.ContainsKey(SelectedLegendItemItem))
                    {
                        foreach (var item in InitializedItems[SelectedLegendItemItem])
                        {
                            if (item is Command command)
                            {
                                command.RaiseCanExecuteChanged();
                            }
                        }
                    }
                    break;
            }
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
                        if (item is IGroup group)
                        {
                            group.Children.CollectionChanged -= LegendItems_CollectionChanged;
                            group.Children.CollectionChanged += LegendItems_CollectionChanged;
                            AddAddGroupCommand(group);
                            AddAddLayersCommand(group);
                        }
                        if (item is IRenderableItem renderableItem)
                        {
                            AddZoomCommand(renderableItem);
                            AddRemoveCommand(renderableItem);
                            AddPropertiesCommand(renderableItem);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加属性命令
        /// </summary>
        /// <param name="legendItem">图例元素</param>
        private void AddPropertiesCommand(ILegendItem legendItem)
        {
            //IContextCommand? cmd = null;

            //if (cmd != null)
            //{
            //    legendItem.ContextCommands.Add(cmd);
            //    InitializedItems[legendItem].Add(cmd);
            //}
        }
        /// <summary>
        /// 添加新建图层组命令
        /// </summary>
        /// <param name="group">分组</param>
        private void AddAddGroupCommand(IGroup group)
        {
            IContextCommand cmd = new AddNewGroupCommand(group);
            group.ContextCommands.Add(cmd);
            InitializedItems[group].Add(cmd);
        }
        /// <summary>
        /// 添加新建图层组命令
        /// </summary>
        /// <param name="group">分组</param>
        private void AddAddLayersCommand(IGroup group)
        {
            IContextCommand cmd = IocManager.GetService<ICommand, AddLayersCommand>();
            group.ContextCommands.Add(cmd);
            InitializedItems[group].Add(cmd);
        }
        /// <summary>
        /// 添加居中命令
        /// </summary>
        /// <param name="renderableItem">图例元素</param>
        private void AddZoomCommand(IRenderableItem renderableItem)
        {
            IContextCommand cmd = IocManager.GetService<ICommand, ZoomToItemCommand>();
            renderableItem.ContextCommands.Add(cmd);
            InitializedItems[renderableItem].Add(cmd);
        }
        /// <summary>
        /// 添加移除命令
        /// </summary>
        /// <param name="renderableItem">图例元素</param>
        private void AddRemoveCommand(IRenderableItem renderableItem)
        {
            switch (renderableItem)
            {
                case IFrame:
                    break;
                case ILayer:
                case IGroup:
                    var cmd = IocManager.GetService<ICommand, RemoveSelectedLayersCommand>();
                    renderableItem.ContextCommands.Add(cmd);
                    InitializedItems[renderableItem].Add(cmd);
                    break;
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
                            if (item is IGroup group)
                            {
                                group.Children.CollectionChanged -= LegendItems_CollectionChanged;
                            }
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
