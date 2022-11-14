using EM.GIS.Controls;
using EM.GIS.Symbology;
using Fluent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EM.GIS.Plugins.MainFrame
{
    /// <summary>
    /// 控件扩展
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// 获取按钮
        /// </summary>
        /// <param name="header">标题</param>
        /// <param name="command">命令</param>
        /// <param name="iconPath">图标地址</param>
        /// <param name="largeIconPath">大图标地址</param>
        /// <param name="toolTip">提示</param>
        /// <param name="size">控件大小</param>
        /// <returns></returns>
        public static Fluent.Button GetFluentButton(object header, ICommand command, string iconPath, string largeIconPath, object toolTip = null, Fluent.RibbonControlSize size = Fluent.RibbonControlSize.Large)
        {
            var button = new Fluent.Button()
            {
                Header = header,
                ToolTip = toolTip,
                Size = size,
                Command = command
            };
            if (!string.IsNullOrEmpty(iconPath))
            {
                button.Icon = new BitmapImage(new Uri(iconPath, UriKind.RelativeOrAbsolute));
            }
            if (!string.IsNullOrEmpty(largeIconPath))
            {
                button.LargeIcon = new BitmapImage(new Uri(largeIconPath, UriKind.RelativeOrAbsolute));
            }
            return button;
        }
        /// <summary>
        /// 获取按钮
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="size">尺寸</param>
        /// <param name="useIcon">使用图标</param>
        /// <returns>按钮</returns>
        public static Fluent.Button ToFluentButton(this IBaseCommand command, RibbonControlSize size = RibbonControlSize.Large, bool useIcon = true)
        {
            Button button = null;
            if (command != null)
            {
                button = new Button()
                {
                    Header = command.Header,
                    Name = command.Name,
                    ToolTip = command.ToolTip,
                    Size = size,
                    Command = command
                };
                if (useIcon)
                {
                    button.Icon = command.Icon;
                    button.LargeIcon = command.LargeIcon;
                }
            }
            return button;
        }
        /// <summary>
        /// 获取快速访问菜单按钮
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="size">尺寸</param>
        /// <returns>快速访问菜单按钮</returns>
        public static QuickAccessMenuItem ToQuickAccessMenuItem(this IBaseCommand command, RibbonControlSize size = RibbonControlSize.Large)
        {
            QuickAccessMenuItem item = null;
            if (command != null)
            {
                item = new QuickAccessMenuItem()
                {
                    IsChecked = true,
                    Icon = command.Icon,
                    Target = command.ToFluentButton(size, false)
                };
            }
            return item;
        }

        /// <summary>
        /// 获取快速访问菜单按钮
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="size">尺寸</param>
        /// <returns>快速访问菜单按钮</returns>
        public static BackstageTabItem ToBackstageTabItem(this IBaseCommand command, RibbonControlSize size = RibbonControlSize.Large)
        {
            BackstageTabItem item = null;
            if (command != null)
            {
                item = new BackstageTabItem()
                {
                    Header = command.Header,
                    Icon = command.Icon,
                    Content = command.ToFluentButton(size, false)
                };
            }
            return item;
        }
    }
}
