using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分组
    /// </summary>
    public interface IGroup : IRenderableItem
    {
        /// <summary>
        /// 图层集合
        /// </summary>
        new IRenderableItemCollection Children { get; }

        /// <summary>
        /// 图层个数
        /// </summary>
        int LayerCount { get; }
        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        ILayer? GetLayer(int index);
        /// <summary>
        /// 获取图层集合
        /// </summary>
        /// <returns>图层集合</returns>
        IEnumerable<ILayer> GetLayers();
        /// <summary>
        /// 获取图层组集合
        /// </summary>
        /// <returns>图层组集合</returns>
        IEnumerable<IGroup> GetGroups();
        /// <summary>
        /// 获取所有图层集合（包含子分组）
        /// </summary>
        /// <returns>所有图层集合</returns>
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
    }
}