using EM.GIS.Controls;
using EM.GIS.WPFControls;
using EM.GIS.Symbology;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 工具扩展
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IPlugin))]
    public class ToolPlugin : Plugin
    {
        IWpfAppManager WpfAppManager => IocManager.Default.GetService<IAppManager, IWpfAppManager>();
        RibbonGroup? RibbonGroup { get; set; }
        RibbonTab? RibbonTab { get; set; }
        public override bool OnLoad()
        {
            if (RibbonGroup == null)
            {
                if (WpfAppManager.Ribbon != null)
                {
                    WpfAppManager.Ribbon.Dispatcher.Invoke(() =>
                    {
                        for (int i = WpfAppManager.Ribbon.Items.Count - 1; i >= 0; i--)
                        {
                            if (WpfAppManager.Ribbon.Items[i] is RibbonTab ribbonTab)
                            {
                                RibbonTab = ribbonTab;
                                RibbonGroup = new RibbonGroup()
                                {
                                    Header = "在线地图"
                                };
                                AddRectangleTool();
                                AddPolygonTool();
                                AddDownloadTool();
                                AddTools();
                                ribbonTab.Items.Add(RibbonGroup);
                                break;
                            }
                        }
                    });
                }
            }
            return base.OnLoad();
        }

        private void AddPolygon(object? obj)
        {
        }

        private void AddRectangle(object? obj)
        {

        }
        Command? DownloadCmd { get; set; }
        private void AddDownloadTool()
        {
            if (RibbonGroup != null)
            {
                if (WpfAppManager?.Map?.Frame != null)
                {
                    WpfAppManager.Map.Frame.SelectionChanged += Frame_SelectionChanged;
                }
                DownloadCmd = new Command(OpenDownloadWindow, OpenDownloadWindowIsEnable);
                var ribbonButton = new RibbonButton()
                {
                    Label = "下载",
                    ToolTip = "下载在线地图",
                    Command = DownloadCmd
                };
                RibbonGroup.Items.Add(ribbonButton);
            }
        }

        private void Frame_SelectionChanged(object? sender, EventArgs e)
        {
            DownloadCmd?.RaiseCanExecuteChanged();
        }

        private bool OpenDownloadWindowIsEnable(object? arg)
        {
            var ret = false;
            if (WpfAppManager?.Map?.Frame != null)
            {
                var layers = WpfAppManager.Map.Frame.Children.GetAllFeatureLayers();
                ret = layers.Any(x => x.Selection.Count > 0);
            }
            return ret;
        }

        private void OpenDownloadWindow(object? obj)
        {
            DownloadWebMapWindow window = new DownloadWebMapWindow();
            window.ShowDialog();
        }

        private void AddPolygonTool()
        {
            if (RibbonGroup != null)
            {
                var ribbonButton = new RibbonButton()
                {
                    Label = "多边形",
                    ToolTip = "添加多边形",
                    Command = new Command(AddPolygon)
                };
                RibbonGroup.Items.Add(ribbonButton);
            }
        }

        private void AddRectangleTool()
        {
            if (RibbonGroup != null)
            {
                var ribbonButton = new RibbonButton()
                {
                    Label = "矩形",
                    ToolTip = "添加矩形框",
                    Command = new Command(AddRectangle)
                };
                RibbonGroup.Items.Add(ribbonButton);
            }
        }


        private void AddTools()
        {
            if (RibbonGroup != null)
            {
                var ribbonButton = new RibbonButton()
                {
                    Label = "工具",
                    ToolTip = "常用工具",
                    Command = new Command(OpenTools)
                };
                RibbonGroup.Items.Add(ribbonButton);
            }
        }
        private void OpenTools(object? obj)
        {
            MainWindow window = new MainWindow(WpfAppManager.Map.Frame);
            window.ShowDialog();
        }
        /// <inheritdoc/>
        public override bool OnUnload()
        {
            if (RibbonTab != null && RibbonGroup != null)
            {
                RibbonTab.Items.Remove(RibbonGroup);
                if (WpfAppManager?.Map?.Frame != null)
                {
                    WpfAppManager.Map.Frame.SelectionChanged -= Frame_SelectionChanged;
                }
            }
            return base.OnUnload();
        }
    }
}
