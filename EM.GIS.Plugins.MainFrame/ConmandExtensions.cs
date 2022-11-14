using EM.GIS.Controls;
using EM.GIS.Resources;
using EM.GIS.Symbology;
using EM.GIS.WPFControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace EM.GIS.Plugins.MainFrame
{
    public static class ConmandExtensions
    {
        /// <summary>
        /// 添加图层工具
        /// </summary>
        /// <param name="commandFactory"></param>
        /// <param name="map"></param>
        /// <param name="layerCollection"></param>
        /// <returns></returns>
        public static IBaseCommand GetAddLayersCommand(this ICommandFactory commandFactory, IMap map, ILayerCollection layerCollection)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "addLayers";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand((obj) => AddLayers(map, layerCollection))
                    {
                        Header = "添加",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Add16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Add32.png"),
                        ToolTip = "添加图层"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IList<ILayer> AddLayers(IMap map, ILayerCollection layerCollection)
        {
            var layers = new List<ILayer>();
            OpenFileDialog dg = new OpenFileDialog()
            {
                Filter = "*.img,*.shp|*.img;*.shp",
                Multiselect = true
            };
            if (dg.ShowDialog(Window.GetWindow(map as DependencyObject)).HasValue)
            {
                foreach (var fileName in dg.FileNames)
                {
                    var layer = layerCollection.AddLayer(fileName);
                    if (layer != null)
                    {
                        layers.Add(layer);
                    }
                }
            }
            return layers;
        }
        /// <summary>
        /// 添加图层组工具
        /// </summary>
        /// <param name="commandFactory"></param>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static IBaseCommand GetAddGroupCommand(this ICommandFactory commandFactory, ILayerCollection layers)
        {
            IBaseCommand command = null;
            if (commandFactory != null && layers != null)
            {
                string name = "addGroup";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => layers.AddGroup())
                    {
                        Header = "添加分组",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Group16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Group32.png"),
                        ToolTip = "添加分组"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }

        public static IBaseCommand GetRemoveSelectedLayersCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "removeSelectedLayers";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => RemoveSelectedLayers(map))
                    {
                        Header = "移除",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Remove16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Remove32.png"),
                        ToolTip = "移除图层"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetRemoveLayerCommand(this ICommandFactory commandFactory, ILayer layer)
        {
            IBaseCommand command = null;
            if (commandFactory != null && layer != null)
            {
                string name = "removeLayer";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => RemoveLayer(layer))
                    {
                        Header = "移除",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Remove16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Remove32.png"),
                        ToolTip = "移除图层"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }

        private static void RemoveLayer(ILayer layer)
        {
            if (layer?.Parent != null)
            {
                layer.Parent.LegendItems.Remove(layer);
            }
        }

        public static IBaseCommand GetActivePanToolCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "activePanTool";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => ActivePanTool(map))
                    {
                        Header = "平移",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Pan16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Pan32.png"),
                        ToolTip = "平移工具"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetZoomToMaxExtentCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "zoomToMaxExtent";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => map?.ZoomToMaxExtent())
                    {
                        Header = "全图",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Global16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Global32.png"),
                        ToolTip = "缩放至全图"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetActiveZoomInToolCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "activeZoomInTool";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => ActiveZoomInTool(map))
                    {
                        Header = "放大",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("ZoomIn16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("ZoomIn32.png"),
                        ToolTip = "放大工具"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetZoomToPreviousViewCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "zoomToPreviousView";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => ZoomToPreviousView(map))
                    {
                        Header = "后退",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Pre16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Pre32.png"),
                        ToolTip = "后退至前一视图"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetActiveIdentifyToolCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "activeIdentifyTool";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => ActiveIdentifyTool(map))
                    {
                        Header = "识别",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Identify16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Identify32.png"),
                        ToolTip = "识别工具"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetActiveZoomOutToolCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "activeZoomOutTool";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => ActiveZoomOutTool(map))
                    {
                        Header = "缩小",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("ZoomOut16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("ZoomOut32.png"),
                        ToolTip = "缩小工具"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetZoomToNextViewCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "zoomToNextView";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => ZoomToNextView(map))
                    {
                        Header = "前进",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Next16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Next32.png"),
                        ToolTip = "前进至后一视图"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetSaveProjectCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "saveProject";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => SaveProject(map))
                    {
                        Header = "保存",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Save16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Save32.png"),
                        ToolTip = "保存工程"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetNewProjectCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "newProject";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => NewProject(map))
                    {
                        Header = "新建",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("New16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("New32.png"),
                        ToolTip = "新建工程"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }

        private static void NewProject(IMap map)
        {
        }

        public static IBaseCommand GetOpenProjectCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "openProject";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => OpenProject(map))
                    {
                        Header = "打开",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Open16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Open32.png"),
                        ToolTip = "打开工程"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }

        private static void OpenProject(IMap map)
        {
        }

        public static IBaseCommand GetUndoCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "undo";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => Undo(map))
                    {
                        Header = "撤销",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Undo16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Undo32.png"),
                        ToolTip = "撤销"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }
        public static IBaseCommand GetRedoCommand(this ICommandFactory commandFactory, IMap map)
        {
            IBaseCommand command = null;
            if (commandFactory != null && map != null)
            {
                string name = "redo";
                command = commandFactory.Commands?.FirstOrDefault(x => x.Name == name);
                if (command == null)
                {
                    command = new BaseCommand(() => Redo(map))
                    {
                        Header = "重做",
                        Name = name,
                        Icon = ResourcesHelper.GetBitmapImage("Redo16.png"),
                        LargeIcon = ResourcesHelper.GetBitmapImage("Redo32.png"),
                        ToolTip = "重做"
                    };
                    commandFactory.Commands.Add(command);
                }
            }
            return command;
        }

        private static void Redo(IMap map)
        {
        }

        private static void Undo(IMap map)
        {
        }

        private static void SaveProject(IMap map)
        {
        }

        private static void ZoomToNextView(IMap map)
        {
        }

        private static void ActiveZoomOutTool(IMap map)
        {
        }

        private static void ActiveIdentifyTool(IMap map)
        {
        }

        private static void ZoomToPreviousView(IMap map)
        {
        }

        private static void ActiveZoomInTool(IMap map)
        {
            //var panTool = map.MapTools.FirstOrDefault(x => x is MapToolZoom);
            //if (panTool != null)
            //{
            //    map.ActivateMapToolWithZoom(panTool);
            //}
        }

        private static void ActivePanTool(IMap map)
        {
            var panTool = map.MapTools.FirstOrDefault(x => x is MapToolPan);
            if (panTool != null)
            {
                map.ActivateMapToolWithZoom(panTool);
            }
        }

        private static bool IsLegendItemSelected(ILegendItemCollection legendItems)
        {
            bool ret = false;
            if (legendItems != null)
            {
                foreach (ILegendItem item in legendItems)
                {
                    if (item.IsSelected)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = IsLegendItemSelected(item.LegendItems);
                    }
                    if (ret)
                    {
                        break;
                    }
                }
            }
            return ret;
        }
        private static void RemoveSelectedLegendItems(ILegendItemCollection legendItems)
        {
            if (legendItems == null)
            {
                return;
            }
            for (int i = legendItems.Count - 1; i >= 0; i--)
            {
                if (legendItems[i].IsSelected)
                {
                    legendItems.RemoveAt(i);
                }
                else
                {
                    RemoveSelectedLegendItems(legendItems[i].LegendItems);
                }
            }
        }
        private static void RemoveSelectedLayers(IMap map)
        {
            if (map.MapFrame != null)
            {
                bool isSelected = IsLegendItemSelected(map.MapFrame.LegendItems);
                if (isSelected)
                {
                    if (MessageBox.Show(Window.GetWindow(map as DependencyObject), "是否移除选择的图层？", "删除确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        RemoveSelectedLegendItems(map.MapFrame.LegendItems);
                    }
                }
            }
        }
    }
}
