using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 标注图层接口
    /// </summary>
    public interface ILabelLayer : ILayer
    {
        #region Properties

        /// <summary>
        /// 要素图层
        /// </summary>
        IFeatureLayer FeatureLayer { get; }
        /// <summary>
        /// 分类集合
        /// </summary>
        new ILabelCategoryCollection Categories { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        new ILabelCategory DefaultCategory { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 清空选择
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// 根据表达式创建标注
        /// </summary>
        void CreateLabels();

        /// <summary>
        /// 使缓存无效
        /// </summary>
        void Invalidate();

        /// <summary>
        /// 高亮标注
        /// </summary>
        /// <param name="extent"></param>
        /// <returns></returns>
        bool Select(IExtent extent);

        #endregion
    }
}
