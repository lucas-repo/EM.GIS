using BruTile;
using BruTile.Web;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;

namespace EM.GIS.Data
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public interface ITileSet : IRasterSet
    {
        /// <summary>
        /// 瓦片源
        /// </summary>
        ITileSource TileSource { get; }
        /// <summary>
        /// 缓存的瓦片集合
        /// </summary>
        ConcurrentDictionary<TileIndex, (IRasterSet Tile, bool IsNodata)> Tiles { get; }
        /// <summary>
        /// 获取瓦片位图
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="reloadTimes">重试次数</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <returns>瓦片位图</returns>
        Task<(Bitmap Bitmap, bool IsNodata)> GetBitmapAsync(TileInfo tileInfo, int reloadTimes = 1, Func<bool>? cancelFunc = null);
        /// <summary>
        /// 将指定的瓦片添加到<see cref="Tiles"/>
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="tileBitmap">瓦片缓存</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <returns>瓦片</returns>
        IRasterSet? AddTileToTiles(TileInfo tileInfo, (Bitmap Bitmap, bool IsNodata) tileBitmap, Func<bool>? cancelFunc = null);
    }
}