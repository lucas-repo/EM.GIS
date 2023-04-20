using BruTile;
using BruTile.Cache;
using EM.GIS.Data;
using EM.GIS.Data.MBTiles;
using EM.GIS.GdalExtensions;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// MBTiles下载类
    /// </summary>
    public class MBTilesDownloader : Downloader
    {
        private string OutPath;
        private string Extensions;
        private MBTilesDriver Driver { get; }
        public MBTilesDownloader(ITileSet tileSet, string tileDirectory, string tileExtensions, string outPath) : base(tileSet, tileDirectory, tileExtensions)
        {
            OutPath = outPath;
            var dataSetFactory = IocManager.Default.GetService<IDataSetFactory>();
            if (dataSetFactory == null)
            {
                throw new Exception("IDataSetFactory未注册");
            }
            IEnumerable<IRasterDriver> rasterDrivers = dataSetFactory.GetRasterDrivers();
            Extensions = Path.GetExtension(outPath);
            var drivers = rasterDrivers.Where(x => x.Name == Extensions);
            foreach (var item in drivers)
            {
                if (item is MBTilesDriver tilesDriver)
                {
                    Driver = tilesDriver;
                }
            }
            if (Driver == null)
            {
                throw new Exception($"不支持的格式{Extensions}");
            }
        }

        private void WriteTileToRasterSet(int level, List<TileInfo> tileInfos, MBTilesSet tilesSet, Action<string, int>? progressAction, CancellationToken cancellationToken)
        {
            if (HttpTileSource == null)
            {
                return;
            }
            int totalCount = tileInfos.Count();
            int successCount = 0;
            ParallelOptions options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1 /*Environment.ProcessorCount*/
            };
            Parallel.ForEach(tileInfos, options, (tileInfo) =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                var filename = FileCache.GetFileName(tileInfo.Index);
                byte[]? buffer = null;
                try
                {
                    var task = HttpTileSource.GetTileAsync(tileInfo);
                    task.ConfigureAwait(false);
                    buffer = task.Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                if (buffer != null)
                {
                    try
                    {
                        lock (LockObj)
                        {
                            tilesSet.Write(tileInfo.Index, buffer);
                            successCount++;
                        }
                        progressAction?.Invoke($"第{level}级写入缓存中", successCount * 100 / totalCount);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}下载失败：{e}");
                    }
                }
            });

            progressAction?.Invoke($"第{level}级写入缓存中", 99);
            tilesSet.Save();
        }
        private MBTilesSet? CreateMBtiles(List<TileInfo> tileInfos)
        {
            var minCol = tileInfos.Min(x => x.Index.Col);
            var minRow = tileInfos.Min(x => x.Index.Row);
            var maxCol = tileInfos.Max(x => x.Index.Col);
            var maxRow = tileInfos.Max(x => x.Index.Row);
            var minX = tileInfos.Min(x => x.Extent.MinX);
            var minY = tileInfos.Min(x => x.Extent.MinY);
            var maxX = tileInfos.Max(x => x.Extent.MaxX);
            var maxY = tileInfos.Max(x => x.Extent.MaxY);
            int tileWidth = 256, tileHeight = 256;
            int width = tileWidth * (maxCol - minCol + 1);
            int height = tileHeight * (maxRow - minRow + 1);
            MBTilesSet? ret = Driver.Create(OutPath, width, height, 3, RasterType.Byte) as MBTilesSet;
            return ret;
        }
        /// <inheritdoc/>
        public override void Download(Dictionary<int, List<TileInfo>> levelAndTileInfos, CancellationToken cancellationToken)
        {
            if (levelAndTileInfos.Count == 0)
            {
                return;
            }
            var name = Path.GetFileNameWithoutExtension(OutPath);
            var minLevel = levelAndTileInfos.Min(x => x.Key);
            var maxLevel = levelAndTileInfos.Max(x => x.Key);
            var extent = new Geometries.Extent();
            foreach (var item in levelAndTileInfos[maxLevel])
            {
                extent.ExpandToInclude(item.Extent.ToExtent());
            }
            var options = OptionExtensions.GetMBTilesOptions(name, extent, minLevel, maxLevel);
            var directory = Path.GetDirectoryName(OutPath);
            if (directory == null || name == null)
            {
                return;
            }
            var tilesSet = CreateMBtiles(levelAndTileInfos[maxLevel]);
            if (tilesSet == null)
            {
                return;
            }
            Action<string, int> downloadProgressAction = (txt, progress) => ProgressAction?.Invoke(txt, progress / 2);
            Action<string, int> writeProgressAction = (txt, progress) => ProgressAction?.Invoke(txt, 50 + progress / 2);
            foreach (var item in levelAndTileInfos)
            {
                var level = item.Key;
                var tileInfos = item.Value;
                if (tileInfos.Count == 0)
                {
                    continue;
                }
                DownloadTiles(item, cancellationToken);
                WriteTileToRasterSet(level, tileInfos, tilesSet, writeProgressAction, cancellationToken);
                //ProgressAction?.Invoke($"第{level.Item}级创建金字塔中", 99);
            }
        }
    }
}
