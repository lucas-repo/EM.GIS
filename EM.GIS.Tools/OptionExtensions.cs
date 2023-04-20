using EM.GIS.GdalExtensions;
using EM.GIS.Geometries;
using EM.GIS.MBTiles;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 可选项扩展
    /// </summary>
    public static class OptionExtensions
    {
        /// <summary>
        /// 获取tiff的可选参数
        /// </summary>
        /// <returns>可选参数</returns>
        public static Dictionary<string, object> GetGdalTiffOptions()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>
            {
                ["TILED"] = "YES",
                ["COMPRESS"] = "DEFLATE",
                ["BIGTIFF"] = "YES",
            };
            return ret;
        }
        /// <summary>
        /// 获取MBTiles的可选参数
        /// </summary>
        /// <returns>可选参数</returns>
        public static Dictionary<string, object> GetGdalMBTilesOptions(string name = "", string description = "", GdalExtensions.LayerType type = GdalExtensions.LayerType.overlay, string version = "1.1", int blockSize = 256, TileFormat tileFormat = TileFormat.PNG, int quality = 75, int zLevel = 6, YesNo dither = YesNo.NO, ZoomLevelStrategy zoomLevelStrategy = ZoomLevelStrategy.AUTO, Resampling resampling = Resampling.BILINEAR, YesNo writeBounds = YesNo.YES)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>()
            {
                ["NAME"] = name,
                ["DESCRIPTION"] = description,
                ["TYPE"] = type.ToString(),
                ["VERSION"] = version,
                ["BLOCKSIZE"] = blockSize,
                ["TILE_FORMAT"] = tileFormat.ToString(),
                ["QUALITY"] = quality,
                ["ZLEVEL"] = zLevel,
                ["DITHER"] = dither.ToString(),
                ["ZOOM_LEVEL_STRATEGY"] = zoomLevelStrategy.ToString(),
                ["RESAMPLING"] = resampling.ToString(),
                ["WRITE_BOUNDS"] = writeBounds.ToString(),
            };
            return ret;
        }
        /// <summary>
        /// 获取MBTiles的可选参数
        /// </summary>
        /// <returns>可选参数</returns>
        public static Dictionary<string, object> GetMBTilesOptions(string name, IExtent extent, int minZoom, int maxZoom, Format format = Format.jpg, string attribution = "", string description = "", MBTiles.LayerType type = MBTiles.LayerType.overlay, string version = "", string json = "")
        {
            Dictionary<string, object> ret = new Dictionary<string, object>()
            {
                ["Name"] = name,
                ["Format"] = format,
                ["MinX"] = extent.MinX,
                ["MinY"] = extent.MinY,
                ["MaxX"] = extent.MaxX,
                ["MaxY"] = extent.MaxY,
                ["CenterX"] = (extent.MinX + extent.MaxX) / 2,
                ["CenterY"] = (extent.MinY + extent.MaxY) / 2,
                ["MinZoom"] = minZoom,
                ["MaxZoom"] = maxZoom,
                ["Type"] = type
            };
            if (!string.IsNullOrEmpty(attribution))
            {
                ret["Attribution"] = attribution;
            }
            if (!string.IsNullOrEmpty(description))
            {
                ret["Description"] = description;
            }
            if (!string.IsNullOrEmpty(version))
            {
                ret["Version"] = version;
            }
            if (!string.IsNullOrEmpty(json))
            {
                ret["Json"] = json;
            }
            return ret;
        }
    }
}
