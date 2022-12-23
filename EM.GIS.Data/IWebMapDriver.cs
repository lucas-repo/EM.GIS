using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 在线地图驱动
    /// </summary>
    public interface IWebMapDriver:IDriver
    {
        /// <summary>
        /// 打开WMTS数据集
        /// </summary>
        /// <param name="capabilitiesUrl">元数据地址</param>
        /// <returns>多个数据集</returns>
        IEnumerable<ITileSet> OpenWmts(string capabilitiesUrl);
        /// <summary>
        /// 打开xyz数据集
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="urlFormatter">格式化字符串地址</param>
        /// <param name="serverNodes">服务节点</param>
        /// <param name="minLevel">最小级别</param>
        /// <param name="maxLevel">最大级别</param>
        /// <param name="pixelFormat">像素格式</param>
        /// <returns>数据集</returns>
        ITileSet OpenXYZ(string name, string urlFormatter, IEnumerable<string> serverNodes = null, int minLevel = 0, int maxLevel = 18, PixelFormat pixelFormat = PixelFormat.Format24bppRgb);
    }
}
