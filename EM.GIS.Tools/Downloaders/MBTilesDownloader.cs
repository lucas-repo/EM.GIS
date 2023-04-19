using BruTile;
using BruTile.Cache;
using EM.GIS.Data;
using EM.GIS.GdalExtensions;
using EM.IOC;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private IRasterDriver rasterDriver;
        public MBTilesDownloader(ITileSet tileSet, string tileDirectory, string tileExtensions,string outPath) : base(tileSet, tileDirectory, tileExtensions)
        {
            OutPath=outPath;    
            var dataSetFactory = IocManager.Default.GetService<IDataSetFactory>();
            if (dataSetFactory == null)
            {
                throw new Exception("IDataSetFactory未注册");
            }
            IEnumerable<IRasterDriver> rasterDrivers = dataSetFactory.GetRasterDrivers();
            Extensions = Path.GetExtension(outPath);
            IRasterDriver? driver = rasterDrivers.FirstOrDefault(x => x.Name == Extensions);
            if (driver == null)
            {
                throw new Exception($"不支持的格式{Extensions}");
            }
            else
            {
                rasterDriver = driver;
            }
        }
        private IRasterSet? CreateRasterset(string path, double minX, double minY, double maxX, double maxY, int width, int height, int bandCount = 3, Dictionary<string, object>? options = null)
        {
            IRasterSet? ret = null;
            if (TileSet == null || width == 0 || height == 0 || bandCount == 0)
            {
                return ret;
            }

            if (width == 0 || height == 0)
            {
                return ret;
            }
            var directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
            {
                return ret;
            }
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            DriverExtensions.DeleteDataSource(path);
            ret = rasterDriver.Create(path, width, height, bandCount, RasterType.Byte, options);
            if (ret != null)
            {
                ret.Projection = TileSet.Projection.Copy();
                double destXResolution = (maxX - minX) / width;
                double destYResolution = (maxY - minY) / height;
                double[] affine = { minX, destXResolution, 0, maxY, 0, -destYResolution };
                ret.SetGeoTransform(affine);
            }
            return ret;
        }

        private void WriteTileToRasterSet( int tileWidth, int tileHeight, int bandCount, int[] bandMap, int level, List<TileInfo> tileInfos, int minCol, int minRow, IRasterSet rasterSet, Action<string, int>? progressAction, CancellationToken cancellationToken)
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
                if (!File.Exists(filename))
                {
                    try
                    {
                        var task = HttpTileSource.GetTileAsync(tileInfo);
                        task.ConfigureAwait(false);
                        task.Wait();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    if (!File.Exists(filename))
                    {
                        return;
                    }
                }
                var xOff = (tileInfo.Index.Col - minCol) * tileWidth;
                var yOff = (tileInfo.Index.Row - minRow) * tileHeight;
                try
                {
                    lock (LockObj)
                    {
                        RasterArgs readArgs = new RasterArgs(0, 0, tileWidth, tileHeight, tileWidth, tileHeight, bandCount, bandMap);
                        RasterArgs writeArgs = new RasterArgs(xOff, yOff, tileWidth, tileHeight, tileWidth, tileHeight, bandCount, bandMap);
                        rasterSet.WriteRaster(filename, readArgs, writeArgs);
                        successCount++;
                    }
                    progressAction?.Invoke($"第{level}级写入缓存中", successCount * 100 / totalCount);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}下载失败：{e}");
                }
            });

            progressAction?.Invoke($"第{level}级写入缓存中", 99);
            rasterSet.Save();
        }
        /// <inheritdoc/>
        public override void Download(Dictionary<int, List<TileInfo>> levelAndTileInfos, CancellationToken cancellationToken)
        {
            if (levelAndTileInfos.Count == 0)
            {
                return;
            }
            int tileWidth = 256, tileHeight = 256;
            int bandCount = 3;
            int[] bandMap = { 1, 2, 3 };
            var options = OptionExtensions.GetMBTilesOptions();
            var directory = Path.GetDirectoryName(OutPath);
            var name = Path.GetFileNameWithoutExtension(OutPath);
            if (directory == null || name == null)
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
                DownloadTiles(item,  cancellationToken);

                var minCol = tileInfos.Min(x => x.Index.Col);
                var minRow = tileInfos.Min(x => x.Index.Row);
                var maxCol = tileInfos.Max(x => x.Index.Col);
                var maxRow = tileInfos.Max(x => x.Index.Row);
                var minX = tileInfos.Min(x => x.Extent.MinX);
                var minY = tileInfos.Min(x => x.Extent.MinY);
                var maxX = tileInfos.Max(x => x.Extent.MaxX);
                var maxY = tileInfos.Max(x => x.Extent.MaxY);
                int width = tileWidth * (maxCol - minCol + 1);
                int height = tileHeight * (maxRow - minRow + 1);
                var path = Path.Combine(directory, $"{name}{level}{Extensions}");
                using var rasterSet = CreateRasterset( path, minX, minY, maxX, maxY, width, height, bandCount, options);
                if (rasterSet == null)
                {
                    continue;
                }
                WriteTileToRasterSet( tileWidth, tileHeight, bandCount, bandMap, level, tileInfos, minCol, minRow, rasterSet, writeProgressAction, cancellationToken);
                //ProgressAction?.Invoke($"第{level.Item}级创建金字塔中", 99);
                rasterSet.BuildOverviews();//创建金字塔
            }
        }
    }
}
