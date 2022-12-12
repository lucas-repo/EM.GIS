using EM.Bases;
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
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsAnyItemSelected(this IItemCollection<IBaseItem> items)
        {
            bool ret = false;
            if(items==null)
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
                if (items[i] is ILegendItem legendItem)
                {
                    if (legendItem.IsSelected)
                    {
                        items.RemoveAt(i);
                    }
                    else if (items[i] is IGroup)
                    {
                        RemoveSelectedItems(legendItem.Children);
                    }
                }
            }
        }
    }
}
