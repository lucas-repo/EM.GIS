using BruTile;
using System.Collections.Concurrent;

namespace EM.GIS.Data
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public interface ITileSet: IRasterSet
    {
        /// <summary>
        /// Gets 瓦片集合
        /// </summary>
         ConcurrentDictionary<TileIndex, IRasterSet> Tiles { get; } 
    }
}