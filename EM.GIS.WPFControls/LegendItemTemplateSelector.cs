using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 图例元素模板选择器
    /// </summary>
    public class LegendItemTemplateSelector: DataTemplateSelector
    {
        /// <summary>
        /// 分组模板
        /// </summary>
        public DataTemplate GroupTemplate { get; set; }
        /// <summary>
        /// 图层模板
        /// </summary>
        public DataTemplate LayerTemplate { get; set; }
        /// <summary>
        /// 分类模板
        /// </summary>
        public DataTemplate CategoryTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case IGroup grou:
                    return GroupTemplate;
                case ILayer layer:
                    return LayerTemplate;
                case ICategory category:
                    return CategoryTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
