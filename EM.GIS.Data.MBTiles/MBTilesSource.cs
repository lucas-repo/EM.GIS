using BruTile;
using BruTile.Predefined;
using EM.GIS.MBTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EM.GIS.Data.MBTiles
{
    /// <summary>
    /// MBTiles数据源
    /// </summary>
    public class MBTilesSource : ITileSource
    {
        /// <inheritdoc/>
        public MBTilesContext Context { get; }

        /// <inheritdoc/>
        public ITileSchema Schema { get; }

        /// <inheritdoc/>
        public string Name { get; }
        /// <inheritdoc/>
        public Attribution Attribution { get; }

        public MBTilesSource(MBTilesContext context)
        {
            Context = context;
            var metadataInfo = context.Metadata.GetMetadataInfo();
            if (metadataInfo == null)
            {
                throw new Exception("元数据不能为空");
            }
            var extent = new Extent(metadataInfo.MinX, metadataInfo.MinY, metadataInfo.MaxX, metadataInfo.MaxY);
            List<int> zoomLevels=new List<int>();
            for (int i = metadataInfo.MinZoom; i < metadataInfo.MaxZoom; i++)
            {
                zoomLevels.Add(i);
            }
            Schema = new GlobalSphericalMercator(metadataInfo.Format.ToString(), YAxis.OSM, zoomLevels, metadataInfo.Name,extent);
            Name= metadataInfo.Name;
            Attribution = new Attribution();
        }

        /// <inheritdoc/>
        public Task<byte[]> GetTileAsync(TileInfo tileInfo)
        {
            byte[] ret = Array.Empty<byte>();
            var index = tileInfo.Index;
            var tile = Context.Tiles.GetObject(index.Level, index.Col, index.Row);
            if (tile != null)
            {
                ret = tile.Datas;
            }
            return new Task<byte[]>(() => ret);
        }
    }
}
