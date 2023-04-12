using EM.Bases;
using EM.GIS.Data;
using System;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层集合接口
    /// </summary>
    public interface IRenderableItemCollection : IItemCollection<IBaseItem>
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>元素</returns>
        new IRenderableItem this[int index] { get;set; }
        #endregion

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="dataSet">数据集</param>
        /// <param name="isVisible">是否可见</param>
        /// <returns>图层</returns>
        ILayer? AddLayer(IDataSet dataSet, bool isVisible = true);
        /// <summary>
        /// 添加要素图层
        /// </summary>
        /// <param name="dataset">要素数据集</param>
        /// <param name="isVisible">是否可见</param>
        /// <returns>要素图层</returns>
        IFeatureLayer? AddLayer(IFeatureSet dataset, bool isVisible = true);
        /// <summary>
        /// 添加栅格图层
        /// </summary>
        /// <param name="dataset">栅格数据集</param>
        /// <param name="isVisible">是否可见</param>
        /// <returns>栅格图层</returns>
        IRasterLayer? AddLayer(IRasterSet dataset, bool isVisible = true);
        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="groupName">分组名</param>
        /// <returns>分组</returns>
        IGroup? AddGroup(string groupName = "");
    }
}
