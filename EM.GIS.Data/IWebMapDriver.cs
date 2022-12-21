using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 在线地图驱动
    /// </summary>
    public interface IWebMapDriver:IDriver
    {
        /// <summary>
        /// 打开wmts数据集
        /// </summary>
        /// <param name="capabilitiesUrl">元数据地址</param>
        /// <returns>数据集</returns>
        ITileSet OpenWmts(string capabilitiesUrl);
        /// <summary>
        /// 打开xyz数据集
        /// </summary>
        /// <param name="xyzUrl">地址</param>
        /// <param name="serverNodes">服务节点</param>
        /// <returns>数据集</returns>
        ITileSet OpenXYZ(string xyzUrl, IEnumerable<string> serverNodes=null);
    }
}
