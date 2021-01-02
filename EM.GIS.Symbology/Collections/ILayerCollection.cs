using EM.GIS.Data;
using System;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层集合接口
    /// </summary>
    public interface ILayerCollection : ILegendItemCollection,IEnumerable<ILayer>
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置图层
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new ILayer this[int index] { get;set; }
        /// <summary>
        /// 父元素
        /// </summary>
        new IGroup Parent { get; set; }
        #endregion

        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        ILayer AddLayer(string path,bool isVisible=true);
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        ILayer AddLayer(IDataSet dataSet, bool isVisible = true);
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        IFeatureLayer AddLayer(IFeatureSet featureSet, bool isVisible = true);
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        IRasterLayer AddLayer(IRasterSet raster, bool isVisible = true);
    }
}
