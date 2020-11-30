using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分组
    /// </summary>
    public interface IGroup : ILayer
    {
        /// <summary>
        /// 图层
        /// </summary>
        ILayerCollection Layers { get; }
        /// <summary>
        /// 图层个数
        /// </summary>
        int LayerCount { get; }
        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        ILayer GetLayer(int index);
        /// <summary>
        /// 获取图层集合
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILayer> GetLayers();
        /// <summary>
        /// 获取所有图层集合（包含子分组）
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILayer> GetAllLayers();
        /// <summary>
        /// 获取要素图层集合
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFeatureLayer> GetFeatureLayers();
        /// <summary>
        /// 获取所有要素图层集合（包含子分组）
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFeatureLayer> GetAllFeatureLayers();
        /// <summary>
        /// 获取所有栅格集合
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRasterLayer> GetRasterLayers();
        /// <summary>
        /// 获取所有栅格图层集合（包含子分组）
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRasterLayer> GetAllRasterLayers();
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool AddLayer(ILayer layer, int? index = null);
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        ILayer AddLayer(string filename, int? index = null);
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        ILayer AddLayer(IDataSet dataSet, int? index = null);
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        IFeatureLayer AddLayer(IFeatureSet featureSet, int? index = null);
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="rasterSet"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        IRasterLayer AddLayer(IRasterSet rasterSet, int? index = null);
        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        bool RemoveLayer(ILayer layer);
        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="index"></param>
        void RemoveLayerAt(int index);
        /// <summary>
        /// 清空图层
        /// </summary>
        void ClearLayers();
    }
}