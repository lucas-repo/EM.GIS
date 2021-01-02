using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 元素集合接口（带父元素）
    /// </summary>
    /// <typeparam name="TParent">父元素类型</typeparam>
    /// <typeparam name="TChild">元素类型</typeparam>
    public interface IItemCollection<TParent, TChild> : IItemCollection<TChild>, IParentItem<TParent>
    {
    }
    /// <summary>
    /// 元素集合
    /// </summary>
    /// <typeparam name="TChild">元素类型</typeparam>
    public interface IItemCollection<TChild> : IMoveable, INotifyCollectionChanged, IEnumerable
    {
        /// <summary>
        /// 个数
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 是否只读
        /// </summary>
        bool IsReadOnly { get; }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="item"></param>
        void Add(TChild item);
        /// <summary>
        /// 清空
        /// </summary>
        void Clear();
        /// <summary>
        /// 是否包含元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(TChild item);
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void CopyTo(TChild[] array, int arrayIndex);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Remove(TChild item);
        /// <summary>
        /// 获取或设置元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TChild this[int index] { get; set; }
        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int IndexOf(TChild item);
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void Insert(int index, TChild item);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="index"></param>
        void RemoveAt(int index);
    }
}