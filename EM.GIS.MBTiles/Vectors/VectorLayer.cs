using System.Collections.Generic;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 矢量图层
    /// </summary>
    public class VectorLayer
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; } = string.Empty;
        /// <summary>
        /// 最小级别
        /// </summary>
        public int minzoom { get; set; }
        /// <summary>
        /// 最大级别
        /// </summary>
        public int maxzoom { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        public Dictionary<string, string> fields { get; }=new Dictionary<string, string>();
    }
}
