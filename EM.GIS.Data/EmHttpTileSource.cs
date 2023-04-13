using BruTile.Cache;
using BruTile.Web;
using BruTile;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EM.GIS.Data
{
    /// <summary>
    /// http瓦片源
    /// </summary>
    public class EmHttpTileSource : ITileSource, IRequest
    {
        /// <summary>
        /// 获取瓦片的方法
        /// </summary>
        public readonly Func<Uri, Task<byte[]>> FetchTile;
        /// <summary>
        /// 请求
        /// </summary>
        public readonly IRequest Request;
        private  HttpClient? _client;
        /// <summary>
        /// Http客户端
        /// </summary>
        public  HttpClient HttpClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient()
                    {
                        Timeout = new TimeSpan(0, 0, 5)
                    };
                    _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");
                }
                return _client;
            }
        }
        public EmHttpTileSource(ITileSchema tileSchema, string urlFormatter, IEnumerable<string>? serverNodes = null,
            string? apiKey = null, string? name = null, IPersistentCache<byte[]>? persistentCache = null,
            Func<Uri, Task<byte[]>>? tileFetcher = null, Attribution? attribution = null, string? userAgent = null)
            : this(tileSchema, new BasicRequest(urlFormatter, serverNodes, apiKey), name, persistentCache, tileFetcher, attribution, userAgent)
        {
        }

        public EmHttpTileSource(ITileSchema tileSchema, IRequest request, string? name = null,
            IPersistentCache<byte[]>? persistentCache = null, Func<Uri, Task<byte[]>>? tileFetcher = null,
            Attribution? attribution = null, string? userAgent = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            PersistentCache = persistentCache ?? new NullCache();
            FetchTile = tileFetcher ?? FetchTileAsync;
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent ?? "If you use BruTile please specify a user-agent specific to your app");
            Schema = tileSchema;
            Name = name ?? string.Empty;
            Attribution = attribution ?? new Attribution();
        }

        public IPersistentCache<byte[]> PersistentCache { get; set; }

        public Uri GetUri(TileInfo tileInfo)
        {
            return Request.GetUri(tileInfo);
        }

        /// <inheritdoc/>
        public ITileSchema Schema { get; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public Attribution Attribution { get; set; }

        /// <summary>
        /// 获取瓦片
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <returns>瓦片</returns>
        public virtual async Task<byte[]?> GetTileAsync(TileInfo tileInfo)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes != null) return bytes;
            bytes = await FetchTile(Request.GetUri(tileInfo)).ConfigureAwait(false);
            if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            return bytes;
        }

        private async Task<byte[]> FetchTileAsync(Uri arg)
        {
            return await HttpClient.GetByteArrayAsync(arg);
        }
        /// <summary>
        /// 获取瓦片
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>瓦片</returns>
        public virtual async Task<byte[]?> GetTileAsync(TileInfo tileInfo, CancellationToken cancellationToken)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes != null) return bytes;
            var uri = Request.GetUri(tileInfo);
            var httpResponseMessage = await HttpClient.GetAsync(uri, cancellationToken);
            bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            return bytes;
        }

    }
}
