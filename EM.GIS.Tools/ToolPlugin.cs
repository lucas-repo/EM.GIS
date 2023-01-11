using EM.GIS.WPFControls;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Ribbon;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 工具扩展
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IPlugin))]
    public class ToolPlugin : Plugin
    {
        IWpfAppManager WpfAppManager { get; }
        RibbonGroup RibbonGroup { get; set; }
        RibbonButton RibbonButton { get; set; }
        public ToolPlugin(IWpfAppManager appManager)
        {
            WpfAppManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
        }
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
                                for (int j = ribbonTab.Items.Count - 1; j >= 0; j--)
                                {
                                    if (ribbonTab.Items[j] is RibbonGroup ribbonGroup)
                                    {
                                        RibbonGroup = ribbonGroup;
                                        RibbonButton = new RibbonButton()
                                        {
                                            Label = "工具",
                                            ToolTip = "常用工具",
                                            Command = new Command(OpenTools)
                                        };
                                        RibbonGroup.Items.Add(RibbonButton);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    });
                }
            }
            return base.OnLoad();
        }

        private void OpenTools(object? obj)
        {
            MainWindow window = new MainWindow();
            window.ShowDialog();
        }

        public override bool OnUnload()
        {
            if (RibbonGroup != null && RibbonButton != null)
            {
                RibbonGroup.Items.Remove(RibbonButton);
            }
            return base.OnUnload();
        }
    }
}
