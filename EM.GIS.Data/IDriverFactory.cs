using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据驱动工厂
    /// </summary>
    public interface IDriverFactory
    {
        /// <summary>
        /// 数据驱动集合
        /// </summary>
        IEnumerable<IDriver> Drivers { get; set; }
        /// <summary>
        /// 矢量数据驱动集合
        /// </summary>
        IEnumerable<IVectorDriver> VectorDrivers { get; }
        /// <summary>
        /// 栅格数据驱动集合
        /// </summary>
        IEnumerable<IRasterDriver> RasterDrivers { get; }
        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
        /// <summary>
        /// 打开数据集
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IDataSet Open(string path);
        /// <summary>
        /// 打开矢量数据集
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IFeatureSet OpenVector(string path);
        /// <summary>
        /// 打开栅格数据集
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IRasterSet OpenRaster(string path);
        /// <summary>
        /// 获取矢量可读文件扩展
        /// </summary>
        /// <returns></returns>
        List<string> GetVectorReadableFileExtensions();
        /// <summary>
        /// 获取矢量可写文件扩展
        /// </summary>
        /// <returns></returns>
        List<string> GetRasterWritableFileExtensions();
        /// <summary>
        /// 获取矢量可读文件扩展
        /// </summary>
        /// <returns></returns>
        List<string> GetRasterReadableFileExtensions();
        /// <summary>
        /// 获取矢量可写文件扩展
        /// </summary>
        /// <returns></returns>
        List<string> GetVectorWritableFileExtensions();
    }
}
