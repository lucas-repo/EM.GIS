using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层接口
    /// </summary>
    public interface ILayer : IRenderableItem
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        new ICategoryCollection Children { get; }

        /// <summary>
        /// 数据集
        /// </summary>
        IDataSet? DataSet { get; set; }
    }
}
