using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 矢量数据接口
    /// </summary>
    public interface IVectorDriver : IDriver
    {
        /// <summary>
        /// 打开要素数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>要素数据集</returns>
        new IFeatureSet? Open(string path);
    }
}
