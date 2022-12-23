using EM.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 瓦片地图
    /// </summary>
    public class TileMap
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }=string.Empty;
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// 服务器节点（以","隔开）
        /// </summary>
        public string[]? Servers { get; set; } 
        /// <summary>
        /// 最小级别
        /// </summary>
        public int MinLevel { get; set; } =0;
        /// <summary>
        /// 最小级别
        /// </summary>
        public int MaxLevel { get; set; } = 18;
        /// <summary>
        /// 坐标系编号
        /// </summary>
        public int EPSG { get; set; } = 3857;
    }
}
