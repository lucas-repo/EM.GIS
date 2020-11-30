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
        /// 打开栅格
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        new IRasterSet Open(string fileName, bool update);
        /// <summary>
        /// 创建栅格
        /// </summary>
        /// <param name="utf8_path"></param>
        /// <param name="xsize"></param>
        /// <param name="ysize"></param>
        /// <param name="bands"></param>
        /// <param name="eType"></param>
        /// <returns></returns>
        IRasterSet Create(string utf8_path, int xsize, int ysize, int bands, RasterType eType);
    }
}
