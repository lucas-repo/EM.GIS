using EM.Bases;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 集合扩展类
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 返回集合中是否有元素被选择
        /// </summary>
        /// <param name="items">集合</param>
        /// <returns>是否有选择</returns>
        public static bool IsAnyItemSelected(this IItemCollection<IBaseItem> items)
        {
            bool ret = false;
            if (items == null)
            {
                return ret;
            }
            foreach (var item in items)
            {
                if (item is ILegendItem legendItem)
                {
                    if (legendItem.IsSelected)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = IsAnyItemSelected(legendItem.Children);
                    }
                    if (ret)
                    {
                        break;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 移除集合中所有选择的元素
        /// </summary>
        /// <param name="items">元素集合</param>
        public static void RemoveSelectedItems(this IItemCollection<IBaseItem> items)
        {
            if (items == null)
            {
                return;
            }
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is ISelectableItem selectableItem)
                {
                    if (selectableItem.IsSelected)
                    {
                        items.RemoveAt(i);
                    }
                    else if (selectableItem is ITreeItem treeItem)
                    {
                        RemoveSelectedItems(treeItem.Children);
                    }
                }
            }
        }

        /// <summary>
        /// 移除集合中所有选择的元素
        /// </summary>
        /// <param name="frame">地图框架</param>
        public static void RemoveSelectedItems(this IFrame frame)
        {
            frame.Children.RemoveSelectedItems();
        }
        /// <summary>
        /// 返回选择的元素
        /// </summary>
        /// <param name="items">元素集合</param>
        /// <returns>选择的元素</returns>
        public static IEnumerable<IBaseItem> GetSelectedItems(this IItemCollection<IBaseItem> items)
        {
            foreach (var item in items)
            {
                if (item is ISelectableItem selectableItem)
                {
                    if (selectableItem.IsSelected)
                    {
                        yield return item;
                    }
                }
                if (item is ITreeItem treeItem)
                {
                    foreach (var selectedChildItem in treeItem.Children.GetSelectedItems())
                    {
                        yield return selectedChildItem;
                    }
                }
            }
        }
        /// <summary>
        /// 缩放至选择的元素
        /// </summary>
        /// <param name="frame">地图框架</param>
        public static void ZoomToSelectedItems(this IFrame frame)
        {
            var items = frame.Children.GetSelectedItems();
            Extent extent = new Extent();
            foreach (var item in items)
            {
                switch (item)
                {
                    case ILayer layer:
                        extent.ExpandToInclude(layer.Extent);
                        break;
                    case IGroup group:
                        extent.ExpandToInclude(group.Extent);
                        break;
                }
            }
            if (!extent.IsEmpty())
            {
                frame.View.ViewExtent = extent;
            }
        }
    }
}
