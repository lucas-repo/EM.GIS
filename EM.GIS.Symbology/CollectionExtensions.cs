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
        /// <param name="frame">地图框架</param>
        /// <returns>选择的元素</returns>
        public static IEnumerable<IRenderableItem> GetSelectedItems(this IFrame frame) 
        {
            if (frame != null)
            {
                if (frame.IsSelected)
                {
                    yield return frame;
                }
                if (frame.Children != null)
                {
                    foreach (var item in frame.Children.GetSelectedItems<IRenderableItem>())
                    {
                        yield return item;
                    };
                }
            }
        }
        /// <summary>
        /// 返回选择的元素
        /// </summary>
        /// <param name="items">元素集合</param>
        /// <returns>选择的元素</returns>
        public static IEnumerable<T> GetSelectedItems<T>(this IItemCollection<IBaseItem> items) where T : ITreeItem
        {
            foreach (var item in items)
            {
                if (item is T t)
                {
                    if (t.IsSelected)
                    {
                        yield return t;
                    }
                    if (t.Children != null)
                    {
                        foreach (var selectedChildItem in t.Children.GetSelectedItems<T>())
                        {
                            yield return selectedChildItem;
                        }
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
            var items = frame.GetSelectedItems();
            Extent extent = new Extent();
            foreach (var item in items)
            {
                switch (item)
                {
                    case IRenderableItem renderableItem:
                        extent.ExpandToInclude(renderableItem.Extent);
                        break;
                }
            }
            if (!extent.IsEmpty())
            {
                frame.ExpandExtent(extent);
                frame.View.ViewExtent =  extent;
            }
        }
        /// <summary>
        /// 获取集合中所有指定类型的元素
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="items">集合</param>
        /// <returns>指定类型的元素</returns>
        public static IEnumerable<T> GetAllItems<T>(this IEnumerable<IBaseItem> items) where T : ITreeItem
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item is T t)
                    {
                        yield return t;
                    }
                    if (item is ITreeItem treeItem)
                    {
                        foreach (var childItem in GetAllItems<T>(treeItem.Children))
                        {
                            yield return childItem;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取集合中所有可渲染元素
        /// </summary>
        /// <param name="items">集合</param>
        /// <returns>可渲染元素</returns>
        public static IEnumerable<IRenderableItem> GetAllRenderableItems(this IEnumerable<IBaseItem> items)
        {
            return GetAllItems<IRenderableItem>(items);
        }
        /// <summary>
        /// 获取集合中所有图层
        /// </summary>
        /// <param name="items">集合</param>
        /// <returns>图层</returns>
        public static IEnumerable<ILayer> GetAllLayers(this IEnumerable<IBaseItem> items)
        {
            return GetAllItems<ILayer>(items);
        }
        /// <summary>
        /// 获取集合中所有要素图层
        /// </summary>
        /// <param name="items">集合</param>
        /// <returns>要素图层</returns>
        public static IEnumerable<IFeatureLayer> GetAllFeatureLayers(this IEnumerable<IBaseItem> items)
        {
            return GetAllItems<IFeatureLayer>(items);
        }
        /// <summary>
        /// 获取集合中所有栅格图层
        /// </summary>
        /// <param name="items">集合</param>
        /// <returns>栅格图层</returns>
        public static IEnumerable<IRasterLayer> GetAllRasterLayers(this IEnumerable<IBaseItem> items)
        {
            return GetAllItems<IRasterLayer>(items);
        }
    }
}
