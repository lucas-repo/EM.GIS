using BruTile;
using BruTile.Cache;
using BruTile.Web;
using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 谷歌瓦片下载类
    /// </summary>
    public class GoogleTileDownloader : Downloader
    {
        public GoogleTileDownloader(ITileSet tileSet, string tileDirectory, string tileExtensions) : base(tileSet, tileDirectory, tileExtensions)
        {
        }

        /// <inheritdoc/>
        public override void Download(Dictionary<int, List<TileInfo>> levelAndTileInfos, CancellationToken cancellationToken)
        {
            foreach (var item in levelAndTileInfos)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (item.Value.Count == 0)
                {
                    continue;
                }
                DownloadTiles(item, cancellationToken);
            }
        }
    }
}
