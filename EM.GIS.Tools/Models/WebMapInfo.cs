using EM.Bases;
using System.Collections.Generic;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 在线地图信息
    /// </summary>
    public class WebMapInfo:NotifyClass
    {
        public WebMapInfo(string name, int minLevel, int maxLevel, string urlFormatter, IEnumerable<string>? serverNodes=null, string? apiKey=null)
        {
            Name=name;
            MinLevel=minLevel;
            MaxLevel=maxLevel;
            UrlFormatter=urlFormatter;
            ServerNodes=serverNodes;
            ApiKey=apiKey;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 最小级别
        /// </summary>
        public int MinLevel { get; set; }
        /// <summary>
        /// 最大级别
        /// </summary>
        public int MaxLevel { get; set; }
        /// <summary>
        /// 地址格式
        /// </summary>
        public string UrlFormatter { get; set; } = string.Empty;
        /// <summary>
        /// 服务节点
        /// </summary>
        public IEnumerable<string>? ServerNodes { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public string? ApiKey { get; set; } = string.Empty;

    }
}
