using BruTile;
using BruTile.Web;
using EM.GIS.Geometries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
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
        /// <param name="cancellationToken">取消标记</param>
        /// <param name="reloadTimes">重试次数</param>
        /// <returns>瓦片位图</returns>
        Task<(Bitmap Bitmap, bool IsNodata)> GetBitmapAsync(TileInfo tileInfo, CancellationToken cancellationToken, int reloadTimes = 1);
        /// <summary>
        /// 将指定的瓦片添加到<see cref="Tiles"/>
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="tileBitmap">瓦片缓存</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>瓦片</returns>
        IRasterSet? AddTileToTiles(TileInfo tileInfo, (Bitmap Bitmap, bool IsNodata) tileBitmap, CancellationToken cancellationToken);
        /// <summary>
        /// 根据指定的窗口范围，计算所需瓦片集合
        /// </summary>
        /// <param name="proj">窗口参数</param>
        /// <param name="extent">要下载的范围</param>
        /// <returns>瓦片集合</returns>
        List<TileInfo> GetTileInfos(IProj proj, IExtent extent);
        /// <summary>
        /// 根据指定的级别和范围，计算所需瓦片集合
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="extent">要下载的范围</param>
        /// <returns>瓦片集合</returns>
        List<TileInfo> GetTileInfos(int level, IExtent extent);
        /// <summary>
        /// 根据指定的级别和几何体，计算所需瓦片集合
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="geometry">几何体</param>
        /// <returns>瓦片集合</returns>
        List<TileInfo> GetTileInfos(int level, IGeometry geometry);
    }
}