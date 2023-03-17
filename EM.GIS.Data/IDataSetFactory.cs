using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据集工厂接口
    /// </summary>
    public interface IDataSetFactory
    {
        /// <summary>
        /// 数据驱动集合
        /// </summary>
        List<IDriver> Drivers { get; }
        /// <summary>
        /// 打开数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="progress">进度委托</param>
        /// <returns>数据集</returns>
        IDataSet? OpenDataSet(string path, ProgressDelegate? progress = null);
        /// <summary>
        /// 打开栅格数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="progress">进度委托</param>
        /// <returns>数据集</returns>
        IRasterSet? OpenRasterSet(string path, ProgressDelegate? progress = null);
        /// <summary>
        /// 打开矢量数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="progress">进度委托</param>
        /// <returns>数据集</returns>
        IFeatureSet? OpenFeatureSet(string path, ProgressDelegate? progress = null);
        /// <summary>
        /// 创建要素数据集
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="featureType">要素类型</param>
        /// <returns>要素数据集</returns>
        IFeatureSet CreateFeatureSet(string name, FeatureType featureType);
        /// <summary>
        /// 复制数据集
        /// </summary>
        /// <param name="srcFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>成功true反之false</returns>
        bool Copy(string srcFileName, string destFileName);
        /// <summary>
        /// 删除数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>成功与否</returns>
        bool Delete(string path);
        /// <summary>
        /// 重命名数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="newName">新的名称</param>
        /// <returns>成功与否</returns>
        bool RenameDataSet(string path, string newName);
    }
}
