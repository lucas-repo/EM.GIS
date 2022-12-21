using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 在线地图图层
    /// </summary>
    public interface IWebMapLayer : IRasterLayer
    {
        /// <summary>
        /// 栅格数据集
        /// </summary>
        new IRasterSet DataSet { get; set; }
    }
}
