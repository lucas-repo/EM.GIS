using BruTile;
using BruTile.Cache;
using EM.Bases;
using EM.GIS.Data;
using EM.GIS.GdalExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 在线地图下载类
    /// </summary>
    public abstract class Downloader:NotifyClass
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        protected readonly object LockObj = new object();
        private static HttpClient? _client;
        /// <summary>
        /// Http客户端
        /// </summary>
        public static HttpClient HttpClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient()
                    {
                        Timeout = new TimeSpan(0, 3, 0)
                    };
                    _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");
                }
                return _client;
            }
        }
        private ITileSet tileSet;

        protected Downloader(ITileSet tileSet, string tileDirectory, string tileExtensions)
        {
            TileSet = tileSet;
            TileDirectory = tileDirectory;
            TileExtensions = tileExtensions;
            FileCache = GetFileCache(TileDirectory, TileExtensions);
        }

        /// <summary>
        /// 瓦片数据集
        /// </summary>
        public ITileSet TileSet
        {
            get { return tileSet; }
            set { SetProperty(ref tileSet, value); }
        }
        /// <summary>
        /// 瓦片源
        /// </summary>
        protected EmHttpTileSource HttpTileSource
        {
            get
            {
                if (TileSet.TileSource is EmHttpTileSource emHttpTileSource)
                {
                    return emHttpTileSource;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        /// <summary>
        /// 瓦片缓存目录
        /// </summary>
        public string TileDirectory { get; set; }
        /// <summary>
        /// 瓦片图片后缀(.jpg,.png)
        /// </summary>
        public string TileExtensions { get; set; }
        /// <summary>
        /// 进度委托
        /// </summary>
        public Action<string, int>? ProgressAction { get; set; }
        /// <summary>
        /// 获取瓦片缓存
        /// </summary>
        /// <param name="directory">目录</param>
        /// <param name="tileExtensions">瓦片图片后缀</param>
        /// <returns>瓦片缓存</returns>
        public static FileCache GetFileCache(string directory, string tileExtensions)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var fileCache = new FileCache(directory, tileExtensions.Replace(".", ""));
            return fileCache;
        }
        /// <summary>
        /// 瓦片缓存
        /// </summary>
        protected FileCache FileCache { get; }
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="levelAndTileInfos">级别和瓦片信息</param>
        /// <param name="cancellationToken">取消标记</param>
        public abstract void Download(Dictionary<int, List<TileInfo>> levelAndTileInfos, CancellationToken cancellationToken);
        /// <summary>
        /// 下载瓦片
        /// </summary>
        /// <param name="tileInfos">瓦片信息</param>
        /// <param name="cancellationToken">取消标记</param>
        protected void DownloadTiles(KeyValuePair<int, List<TileInfo>> tileInfos, CancellationToken cancellationToken)
        {
            if (TileSet == null)
            {
                return;
            }
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = cancellationToken
            };
            var totalCount = tileInfos.Value.Count;
            var level = tileInfos.Key;
            var successCount = 0;
            if (HttpTileSource != null)
            {
                Parallel.ForEach(tileInfos.Value, parallelOptions, (tileInfo) =>
                {
                    var uri = HttpTileSource.Request.GetUri(tileInfo);
                    if (uri != null)
                    {
                        try
                        {
                            var bytes = FileCache.Find(tileInfo.Index);
                            if (bytes == null)
                            {
                                var task = HttpClient.GetByteArrayAsync(uri);
                                task.ConfigureAwait(false);
                                bytes = task.Result;
                                if (bytes != null)
                                {
                                    FileCache.Add(tileInfo.Index, bytes);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"下载瓦片失败{uri}，{e}");
                        }
                    }
                    lock (LockObj)
                    {
                        successCount++;
                        ProgressAction?.Invoke($"第{level}级下载中", successCount * 100 / totalCount);
                    }
                });
            }
        }
    }
}
