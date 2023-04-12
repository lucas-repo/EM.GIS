using BruTile;
using BruTile.Web;
using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 栅格图层
    /// </summary>
    public class TileLayer : RasterLayer, ITileLayer
    {
        private LockContainer _lockContainer = new LockContainer();
        /// <inheritdoc/>
        public new ITileSet? DataSet
        {
            get
            {
                if (base.DataSet is ITileSet dataset)
                {
                    return dataset;
                }
                else
                {
                    throw new Exception($"{nameof(DataSet)}类型必须为{nameof(ITileSet)}");
                }
            }
            set => base.DataSet = value;
        }

        public TileLayer(ITileSet rasterSet) : base(rasterSet)
        {
        }
        /// <summary>
        /// 添加瓦片
        /// </summary>
        /// <param name="tileSet">瓦片数据集</param>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <returns>任务</returns>
        private async Task<IRasterSet?> AddTile(ITileSet tileSet,TileInfo tileInfo, Func<bool>? cancelFunc)
        {
            IRasterSet? ret = null;
            if (!tileSet.Tiles.ContainsKey(tileInfo.Index))// 如果未包含该瓦片，则需进行下载至缓存
            {
                await tileSet.GetBitmapAsync(tileInfo, 1, cancelFunc).ContinueWith((bitmapTask) =>
                {
                    ret = tileSet.AddTileToTiles(tileInfo, bitmapTask.Result, cancelFunc);
                });
            }
            else
            {
                if (tileSet.Tiles.TryGetValue(tileInfo.Index, out var oldTleInfo)) // 重新下载nodata的瓦片
                {
                    ret = oldTleInfo.Tile;
                    if (oldTleInfo.IsNodata)
                    {
                        await tileSet.GetBitmapAsync(tileInfo, 1, cancelFunc).ContinueWith((bitmapTask) =>
                        {
                            var bitmap = bitmapTask.Result.Bitmap;
                            if (bitmap != null)
                            {
                                ret = new ImageSet(bitmap, oldTleInfo.Tile.Extent)
                                {
                                    Name = tileSet.Name,
                                    Projection = tileSet.Projection,
                                    Bounds = new RasterBounds(bitmap.Height, bitmap.Width, oldTleInfo.Tile.Extent)
                                };
                                oldTleInfo.Tile.Dispose();
                                tileSet.Tiles[tileInfo.Index] = (ret, bitmapTask.Result.IsNodata);
                            }
                        });
                    }
                }
            }
            return ret;
        }
        /// <inheritdoc/>
        protected override Rectangle OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null)
        {
            var ret = Rectangle.Empty;
            if (selected || DataSet == null|| cancelFunc?.Invoke() == true)
            {
                return ret;
            }
            try
            {
                if (cancelFunc?.Invoke() == true) return ret;

                var tileInfos = DataSet.GetTileInfos(mapArgs, mapArgs.DestExtent); // 计算要下载的瓦片
                progressAction?.Invoke(ProgressMessage, 5);
                #region 绘制瓦片
                if (tileInfos.Count > 0)
                {
                    object lockObj = new object();
                    double increment = 90.0 / tileInfos.Count;
                    double progress = 5;

                    if (cancelFunc?.Invoke() == true) return ret;

                    using var parallelCts = new CancellationTokenSource();
                    Func<bool> newCancelFunc = () =>
                    {
                        bool isCancel = cancelFunc?.Invoke() == true;
                        if (isCancel && !parallelCts.IsCancellationRequested)
                        {
                            parallelCts.Cancel();
                        }
                        return isCancel;
                    };

                    ParallelOptions parallelOptions = new ParallelOptions()
                    {
                        CancellationToken = parallelCts.Token
                    };
                    //var cancellationLock = _lockContainer.GetOrCreateLock("cancellationLock");
                    Parallel.ForEach(tileInfos, parallelOptions, (tileInfo) =>
                    {
                        if (newCancelFunc?.Invoke() == true) return;
                        using var task = AddTile(DataSet, tileInfo, newCancelFunc);
                        task.ConfigureAwait(false);
                        var tile = task.Result;// 等待任务完成
                        if (tile == null || newCancelFunc?.Invoke() == true) return;
                        lock (lockObj)
                        {
                            if (tile.Extent.Intersects(mapArgs.DestExtent))
                            {
                                var rect = tile.Draw(mapArgs, null, cancelFunc);
                                if (!rect.IsEmpty)
                                {
                                    ret = ret.ExpandToInclude(rect);
                                    invalidateMapFrameAction?.Invoke(rect);
                                }
                                progress += increment;
                                progressAction?.Invoke(ProgressMessage, (int)progress);
                            }
                        }
                    });
                }
                #endregion

                #region 超过缓存数后，移除多余的缓存图片
                if (DataSet.Tiles.Count > 1000)
                {
                    for (int i = DataSet.Tiles.Count - 1; i >= 0; i--)
                    {
                        var existedTileInfo = DataSet.Tiles.ElementAt(i);
                        if (!tileInfos.Any(x => x.Index == existedTileInfo.Key))
                        {
                            if (DataSet.Tiles.TryRemove(existedTileInfo.Key, out var tileInfo))
                            {
                                tileInfo.Tile?.Dispose();
                            }
                        }
                    }
                }
                #endregion
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"已正常取消获取瓦片。"); // 不用管该异常
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(OnDraw)}失败，{ex}");
            }
            return ret;
        }
    }
}