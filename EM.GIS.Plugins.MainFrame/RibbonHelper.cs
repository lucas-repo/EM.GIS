using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Resources;
using EM.GIS.Symbology;
using EM.GIS.WPFControls;
using Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace EM.GIS.Plugins.MainFrame
{
    public class RibbonHelper
    {
        public IWpfAppManager AppManager { get; }
        public IMap Map => AppManager.Map;
        public RibbonHelper(IWpfAppManager appManager)
        {
            AppManager = appManager;
        }
        #region StartScreen
        private object GetLeftContent()
        {
            Image image = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/EM.GIS.Resources;Component/Images/Home32.png", UriKind.RelativeOrAbsolute))
            };
            TextBlock textBlock = new TextBlock()
            {
                Text = "首页",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center
            };
            WrapPanel wrapPanel = new WrapPanel();
            wrapPanel.Children.Add(image);
            wrapPanel.Children.Add(textBlock);
            return wrapPanel;
        }
        private object GetRightContent()
        {
            TextBlock textBlock = new TextBlock()
            {
                Text = "欢迎，现在开始您的易图世界！",
                FontSize = 14
            };
            Fluent.Button button = new Fluent.Button()
            {
                Header = "进入",
                Content = "进入",
                FontSize = 14,
                IsDefinitive = true
            };
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(button);
            return stackPanel;
        }
        private StartScreen GetStartScreen()
        {
            StartScreenTabControl startScreenTabControl = new StartScreenTabControl()
            {
                LeftContent = GetLeftContent(),
                RightContent = GetRightContent()
            };
            StartScreen startScreen = new StartScreen()
            {
                IsOpen = true,
                Content = startScreenTabControl
            };
            return startScreen;
        }
        #endregion

        #region Menu
        private Backstage GetMenu()
        {
            BackstageTabControl backstageTabControl = new BackstageTabControl();
            var command = AppManager.CommandFactory.GetNewProjectCommand(Map);
            AddBackstageTabItem(command, backstageTabControl.Items);
            command = AppManager.CommandFactory.GetOpenProjectCommand(Map);
            AddBackstageTabItem(command, backstageTabControl.Items);
            command = AppManager.CommandFactory.GetSaveProjectCommand(Map);
            AddBackstageTabItem(command, backstageTabControl.Items);

            Backstage menu = new Backstage()
            {
                Content = backstageTabControl
            };
            return menu;
        }

        #endregion

        #region QuickAccessItems

        private void AddQuickAccessItems(Ribbon ribbon)
        {
            var command = AppManager.CommandFactory.GetSaveProjectCommand(Map);
            AddQuickAccessMenuItem(command, ribbon.QuickAccessItems);
            command = AppManager.CommandFactory.GetUndoCommand(Map);
            AddQuickAccessMenuItem(command, ribbon.QuickAccessItems);
            command = AppManager.CommandFactory.GetRedoCommand(Map);
            AddQuickAccessMenuItem(command, ribbon.QuickAccessItems);
        }

        private QuickAccessMenuItem GetQuickAccessMenuItem(object header, ICommand command, string iconPath, string largeIconPath, object toolTip = null, RibbonControlSize size = RibbonControlSize.Large)
        {
            QuickAccessMenuItem quickAccessMenuItem = new QuickAccessMenuItem()
            {
                IsChecked = true,
                Target = ControlExtensions.GetFluentButton(header, command, iconPath, largeIconPath, toolTip, size)
            };
            return quickAccessMenuItem;
        }

        #endregion

        #region Tabs
        private void AddTabs(Ribbon ribbon)
        {
            ribbon.Tabs.Add(GetMapRibbonTabItem());
        }

        private RibbonTabItem GetMapRibbonTabItem()
        {
            RibbonTabItem ribbonTabItem = new RibbonTabItem()
            {
                Header = "地图",

            };
            ribbonTabItem.Groups.Add(GetNavigateRibbonGoupBox());
            ribbonTabItem.Groups.Add(GetLayerRibbonGoupBox());
            return ribbonTabItem;
        }
        private RibbonGroupBox GetLayerRibbonGoupBox()
        {
            RibbonGroupBox ribbonGroupBox = new RibbonGroupBox()
            {
                Header = "图层"
            };
            var command = AppManager.CommandFactory.GetAddLayersCommand(Map, Map.Layers);
            AddButton(command, ribbonGroupBox.Items);
            command = AppManager.CommandFactory.GetRemoveSelectedLayersCommand(Map);
            AddButton(command, ribbonGroupBox.Items);
            return ribbonGroupBox;
        }

        private void AddButton(IBaseCommand command, IList itemCollection, RibbonControlSize size = RibbonControlSize.Large)
        {
            if (command != null && itemCollection != null)
            {
                var item = command.ToFluentButton(size);
                if (item != null)
                {
                    itemCollection.Add(item);
                }
            }
        }
        private void AddQuickAccessMenuItem(IBaseCommand command, IList itemCollection, RibbonControlSize size = RibbonControlSize.Large)
        {
            if (command != null && itemCollection != null)
            {
                var button = command.ToQuickAccessMenuItem(size);
                if (button != null)
                {
                    var item = command.ToQuickAccessMenuItem(size);
                    if (item != null)
                    {
                        itemCollection.Add(item);
                    }
                }
            }
        }
        private void AddBackstageTabItem(IBaseCommand command, IList itemCollection, RibbonControlSize size = RibbonControlSize.Large)
        {
            if (command != null && itemCollection != null)
            {
                var item = command.ToBackstageTabItem(size);
                if (item != null)
                {
                    itemCollection.Add(item);
                }
            }
        }
        private void AddCommandToControls(IEnumerable<IBaseCommand> commands, ItemCollection itemCollection)
        {
            if (commands != null)
            {
                foreach (var item in commands)
                {
                    AddButton(item, itemCollection);
                }
            }
        }


        private RibbonGroupBox GetNavigateRibbonGoupBox()
        {
            RibbonGroupBox ribbonGroupBox = new RibbonGroupBox()
            {
                Header = "导航"
            };
            RibbonControlSize size = RibbonControlSize.Middle;
            var command = AppManager.CommandFactory.GetActivePanToolCommand(Map);
            AddButton(command, ribbonGroupBox.Items);
            command = AppManager.CommandFactory.GetZoomToMaxExtentCommand(Map);
            AddButton(command, ribbonGroupBox.Items, size);
            command = AppManager.CommandFactory.GetActiveZoomInToolCommand(Map);
            AddButton(command, ribbonGroupBox.Items, size);
            command = AppManager.CommandFactory.GetZoomToPreviousViewCommand(Map);
            AddButton(command, ribbonGroupBox.Items, size);
            command = AppManager.CommandFactory.GetActiveIdentifyToolCommand(Map);
            AddButton(command, ribbonGroupBox.Items, size);
            command = AppManager.CommandFactory.GetActiveZoomOutToolCommand(Map);
            AddButton(command, ribbonGroupBox.Items, size);
            command = AppManager.CommandFactory.GetZoomToNextViewCommand(Map);
            AddButton(command, ribbonGroupBox.Items, size);
            return ribbonGroupBox;
        }

        #endregion
        public Ribbon GetRibbon()
        {
            Ribbon ribbon = new Ribbon()
            {
                //StartScreen = GetStartScreen(),
                Menu = GetMenu()
            };
            AddQuickAccessItems(ribbon);
            AddTabs(ribbon);
            return ribbon;
        }
        public StatusBar GetStatusBar()
        {
            StatusBar statusBar = new StatusBar();
            statusBar.Items.Add(GetProgressStatusBarItem());
            statusBar.Items.Add(GetCoordStatusBarItem());
            return statusBar;
        }

        #region StatusBar
        private object GetCoordStatusBarItem()
        {
            StatusBarItem statusBarItem = new StatusBarItem()
            {
                Title = "坐标",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 0, 0)
            };
            Map.GeoMouseMove += (sender, e) =>
            {
                statusBarItem.Content = $"{Math.Round(e.GeographicLocation.X, 3)},{Math.Round(e.GeographicLocation.Y, 3)}";
            };
            return statusBarItem;
        }
        private StatusBarItem GetProgressStatusBarItem()
        {
            WrapPanel wrapPanel = new WrapPanel();
            ProgressBar progressBar = new ProgressBar()
            {
                Width = 100,
                Margin = new Thickness(5, 0, 0, 0)
            };
            wrapPanel.Children.Add(progressBar);
            TextBlock textBlock = new TextBlock()
            {
                Margin = new Thickness(5, 0, 0, 0)
            };
            wrapPanel.Children.Add(textBlock);
            ProgressHandler progressHandler = new ProgressHandler()
            {
                Handler = (percent, message) =>
                {
                    var action = new Action(() =>
                    {
                        progressBar.Value = percent;
                        textBlock.Text = message;
                    });
                    AppManager.Window.Dispatcher.BeginInvoke(action);
                }
            };
            AppManager.ProgressHandler = progressHandler;
            StatusBarItem statusBarItem = new StatusBarItem()
            {
                Title = "进度",
                Content = wrapPanel,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            return statusBarItem;
        }
        #endregion

    }
}
