using BruTile;
using BruTile.Web;
using System.Collections.Concurrent;

namespace EM.GIS.Data
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public interface ITileSet: IDataSet, IDrawable
    {
        /// <summary>
        /// Gets 瓦片集合
        /// </summary>
         ConcurrentDictionary<TileIndex, IRasterSet> Tiles { get; }
        /// <summary>
        /// 瓦片源
        /// </summary>
        ITileSource TileSource { get; set; }
    }
}