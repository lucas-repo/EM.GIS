using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    ///栅格数据驱动接口
    /// </summary>
    public interface IRasterDriver : IDriver
    {
        /// <summary>
        /// 打开要素数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>要素数据集</returns>
        new IRasterSet? Open(string path);
        /// <summary>
        /// 创建栅格
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="bandCount">波段数</param>
        /// <param name="rasterType">类型</param>
        /// <param name="options">可选项</param>
        /// <returns>栅格</returns>
        IRasterSet? Create(string filename, int width, int height, int bandCount, RasterType rasterType, Dictionary<string,object>? options = null);
    }
}
