using EM.GIS.Controls;
using EM.GIS.WPFControls;
using EM.IOC;
using System;
using System.Linq;
using System.Windows.Controls.Ribbon;

namespace EM.GIS.Downloads
{
    /// <summary>
    /// 下载器扩展
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IPlugin))]
    public class DownloaderPlugin : Plugin
    {
        IWpfAppManager WpfAppManager { get; }
        RibbonGroup RibbonGroup { get; set; }
        RibbonButton RibbonButton { get; set; }
        public DownloaderPlugin(IWpfAppManager appManager)
        {
            WpfAppManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
        }
        public override bool OnLoad()
        {
            if (RibbonGroup == null)
            {
                if (WpfAppManager.Ribbon != null)
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
                                        Label = "下载",
                                        ToolTip = "下载在线地图",
                                        Command = new Command(Dowload)
                                    };
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            if (RibbonGroup != null && !RibbonGroup.Items.Contains(RibbonButton))
            {
                RibbonGroup.Items.Add(RibbonButton);
            }
            return base.OnLoad();
        }

        private void Dowload(object? obj)
        {
            Downloader downloader = new Downloader();
            downloader.ShowDialog();
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
