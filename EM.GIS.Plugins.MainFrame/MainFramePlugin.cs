using AvalonDock;
using EM.GIS.Controls;
using EM.GIS.Symbology;
using EM.GIS.WPFControls;
using Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EM.GIS.Plugins.MainFrame
{
    /// <summary>
    /// 主框架扩展
    /// </summary>
    public class MainFramePlugin : WpfPlugin
    {
        public override int Priority => -1000;
        public override void Activate()
        {
            if (App.Window != null && App.Window.Content is Grid grid && grid.Children.Count == 0 && App.Ribbon == null)
            {
                //添加行
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                //初始化AppManager
                InitializeAppManager();

                //添加ribbon
                RibbonHelper ribbonHelper = new RibbonHelper(App);
                DockHelper dockHelper = new DockHelper(App);
                Ribbon ribbon = ribbonHelper.GetRibbon();
                DockingManager dockingManager = dockHelper.GetDockingManager();
                StatusBar statusBar = ribbonHelper.GetStatusBar();
                grid.Children.Add(ribbon);
                grid.Children.Add(dockingManager);
                grid.Children.Add(statusBar);
                Grid.SetRow(dockingManager, 1);
                Grid.SetRow(statusBar, 2);
                App.Ribbon = ribbon;
                App.DockingManager = dockingManager;
                App.StatusBar = statusBar;
            }
            base.Activate();
        }
        private void InitializeAppManager()
        {
            if (App.Legend == null)
            {
                ILegend legend = new Legend();
                App.Legend = legend;
            }
            if (App.Map == null)
            {
                App.Map = new Map();
                App.Map.PropertyChanged += Map_PropertyChanged;
                AddFrameContextCommands(App.Map.MapFrame);
            }
            if (App.Map.Legend != App.Legend)
            {
                App.Map.Legend = App.Legend;
            }
        }
        private void AddFrameContextCommands(IFrame frame)
        {
            AddGroupContextCommands(frame);
            frame.Layers.CollectionChanged -= Layers_CollectionChanged;
            frame.Layers.CollectionChanged += Layers_CollectionChanged;
        }
        private void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is IMap map)
            {
                switch (e.PropertyName)
                {
                    case nameof(IMap.MapFrame):
                        AddFrameContextCommands(map.MapFrame);
                        break;
                }
            }
        }
        private void AddGroupContextCommands(IGroup group)
        {
            var command = App.CommandFactory.GetAddLayersCommand(App.Map, group.Layers);
            AddCommandToLegendItem(group.ContextCommands, command);
            command = App.CommandFactory.GetAddGroupCommand(group.Layers);
            AddCommandToLegendItem(group.ContextCommands, command);
            group.Layers.CollectionChanged -= Layers_CollectionChanged;
            group.Layers.CollectionChanged += Layers_CollectionChanged;
        }
        private void AddLayerContextCommands(ILayer layer)
        {
            var command = App.CommandFactory.GetRemoveLayerCommand(layer);
            AddCommandToLegendItem(layer.ContextCommands, command);
        }
        private void AddCommandToLegendItem(IList commands, IBaseCommand command)
        {
            if (commands != null && command != null && !commands.Contains(command))
            {
                commands.Add(command);
            }
        }
        private void HandleNewItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ILegendItem item in e.NewItems)
                {
                    if (item is IGroup group)
                    {
                        AddGroupContextCommands(group);
                    }
                    if (item is ILayer layer)
                    {
                        AddLayerContextCommands(layer);
                    }
                }
            }
        }
        private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    HandleNewItems(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    HandleNewItems(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (ILegendItem item in e.OldItems)
                    {
                        //item.ContextCommands.
                    }
                    break;
            }
        }

        public override void Deactivate()
        {
            App.Map = null;
            App.Ribbon = null;
            App.StatusBar = null;
            App.DockingManager = null;
            base.Deactivate();
        }
    }
}
