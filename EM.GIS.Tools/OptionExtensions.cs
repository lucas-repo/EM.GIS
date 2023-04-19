using EM.GIS.GdalExtensions;
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
                ["TILED"]= "YES",
                ["COMPRESS"] = "DEFLATE",
                ["BIGTIFF"] = "YES",
            };
            return ret;
        }
        /// <summary>
        /// 获取MBTiles的可选参数
        /// </summary>
        /// <returns>可选参数</returns>
        public static Dictionary<string, object>  GetMBTilesOptions(string name = "", string description = "", LayerType type = LayerType.overlay, string version = "1.1", int blockSize = 256, TileFormat tileFormat = TileFormat.PNG, int quality = 75, int zLevel = 6, YesNo dither = YesNo.NO, ZoomLevelStrategy zoomLevelStrategy = ZoomLevelStrategy.AUTO, Resampling resampling = Resampling.BILINEAR, YesNo writeBounds = YesNo.YES)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>()
            {
                ["NAME"] =name,
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
    }
}
