using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 元数据类型
    /// </summary>
    public enum MetadataTypes
    {
        /// <summary>
        /// 名称（字符串）
        /// </summary>
        name,
        /// <summary>
        /// 格式（字符串，pbf、jpg、png、webp）
        /// </summary>
        format,
        /// <summary>
        /// WGS84经纬度范围（字符串，-180.0,-85,180,85）
        /// </summary>
        bounds,
        /// <summary>
        /// 地图默认视图的经度、纬度和缩放级别（字符串，-122.1906,37.7599,11）
        /// </summary>
        center,
        /// <summary>
        /// 最低缩放级别（数字）
        /// </summary>
        minzoom,
        /// <summary>
        /// 最高缩放级别（数字）
        /// </summary>
        maxzoom,
        /// <summary>
        /// 地图数据和样式（HTML字符串）
        /// </summary>
        attribution,
        /// <summary>
        /// 描述  （字符串）
        /// </summary>
        description,
        /// <summary>
        /// 类型  （字符串，overlay或baselayer）
        /// </summary>
        type,
        /// <summary>
        /// 瓦片版本  （数字）
        /// </summary>
        version,
        /// <summary>
        /// 列出矢量切片中显示的图层以及显示在这些图层中要素的属性（json字符串）
        /// </summary>
        json
    }
}
