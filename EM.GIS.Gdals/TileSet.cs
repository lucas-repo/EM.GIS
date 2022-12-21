using EM.GIS.Geometries;
using OSGeo.OGR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;

namespace EM.GIS.Data
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public class TileSet : RasterSet, ITileSet
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<BruTile.TileIndex, IRasterSet> Tiles { get; } = new ConcurrentDictionary<BruTile.TileIndex, IRasterSet>();
        /// <inheritdoc/>
        public override int ByteSize => GetByteSize(default(byte));

        /// <inheritdoc/>
        protected override void Dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    if (Tiles.Count > 0)
                    {
                        foreach (var item in Tiles)
                        {
                            item.Value?.Dispose();
                        }
                        Tiles.Clear();
                    }
                }
            }
            base.Dispose(disposeManagedResources);
        }
        public override Image GetImage(IExtent extent, Rectangle rectangle, Action<int> progressAction = null, Func<bool> cancelFunc = null)
        {
            Image ret = null;
            if (extent == null || extent.IsEmpty() || rectangle.IsEmpty || Projection == null)
            {
                return ret;
            }
            try
            {
                progressAction?.Invoke(10);
                if (cancelFunc?.Invoke() == true) return ret;

                // 计算范围
                Extent mapViewExtent = extent.Copy();
                if (Map.ExtendBuffer)
                {
                    mapViewExtent.ExpandBy(-mapViewExtent.Width / _extendBufferCoeff, -mapViewExtent.Height / _extendBufferCoeff); // 扩展范围
                }

                var xmin = mapViewExtent.MinX;
                var xmax = mapViewExtent.MaxX;
                var ymin = mapViewExtent.MinY;
                var ymax = mapViewExtent.MaxY;
                double[] z = { 0, 0 };
                Envelope geogEnv = new Envelope(xmin, xmax, ymin, ymax);

                if (cancelFunc?.Invoke() == true) return ret;
                // 若为投影坐标系，记录投影坐标范围
                Envelope spGeogEnv = new Envelope(xmin, xmax, ymin, ymax);
                if (Projection.Equals(ServiceProviderFactory.WebMercProj.Value))
                {
                    xmin = TileCalculator.Clip(xmin, TileCalculator.MinWebMercX, TileCalculator.MaxWebMercX);
                    xmax = TileCalculator.Clip(xmax, TileCalculator.MinWebMercX, TileCalculator.MaxWebMercX);
                    ymin = TileCalculator.Clip(ymin, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercY);
                    ymax = TileCalculator.Clip(ymax, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercY);
                    rectangle = GetRectangle(mapViewExtent, rectangle, xmin, ymin, xmax, ymax);
                    var clipExtent = new Extent(xmin, ymin, xmax, ymax);
                    geogEnv = clipExtent.Reproject(Map.Projection, ServiceProviderFactory.Wgs84Proj.Value).ToEnvelope();
                    spGeogEnv = clipExtent.ToEnvelope();
                }
                else if (Map.Projection.Equals(ServiceProviderFactory.Wgs84Proj.Value))
                {
                    xmin = TileCalculator.Clip(xmin, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);
                    xmax = TileCalculator.Clip(xmax, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);
                    ymin = TileCalculator.Clip(ymin, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
                    ymax = TileCalculator.Clip(ymax, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
                    rectangle = GetRectangle(mapViewExtent, rectangle, xmin, ymin, xmax, ymax);
                    geogEnv = new Envelope(xmin, xmax, ymin, ymax);
                    spGeogEnv = new Envelope(xmin, xmax, ymin, ymax);
                }

                progressAction?.Invoke(20);
                if (cancelFunc?.Invoke() == true) return ret;

                bool refreshMap = true; // 传入瓦片数据集不为空时，才刷新地图
                var tileInfos = TileManager.GetTileInfos(geogEnv, spGeogEnv, rectangle); // 计算要下载的瓦片
                if (ret == null)
                {
                    refreshMap = false;
                    ret = new TileSet() { Projection = TileManager.ServiceProvider.Projection };
                }
                if (tileInfos.Count == 0)
                {
                    return ret;
                }

                progressAction?.Invoke(30);
                if (cancelFunc?.Invoke() == true) return ret;

                if (TileManager.ServiceProvider is BrutileServiceProvider provider)
                {
                    int count = 0;
                    using (var parallelCts = new CancellationTokenSource())
                    {
                        ParallelOptions parallelOptions = new ParallelOptions()
                        {
                            CancellationToken = parallelCts.Token
                        };
                        var cancellationLock = _lockContainer.GetOrCreateLock("cancellationLock");
                        Parallel.ForEach(tileInfos, parallelOptions, (tileInfo) =>
                        {
                            if (cancelFunc?.Invoke() != true)
                            {
                                var newCancelFunc = (Func<bool>)(() =>
                                {
                                    bool isCancel = cancelFunc?.Invoke() == true;
                                    if (isCancel && !parallelCts.IsCancellationRequested)
                                    {
                                        lock (cancellationLock)
                                        {
                                            parallelCts.Cancel();
                                        }
                                    }
                                    return isCancel;
                                });

                                // 如果未包含该瓦片，则需进行下载至缓存
                                if (!ret.Tiles.ContainsKey(tileInfo.Index))
                                {
                                    using (var task = provider.GetBitmapAsync(tileInfo, provider, ReloadTimes, newCancelFunc).ContinueWith(
                                        (bitmapTask) =>
                                        {
                                            AddTileToTileSet(ret, tileInfo, bitmapTask.Result, refreshMap, newCancelFunc);
                                        }))
                                    {
                                        task.Wait(); // 等待任务完成
                                    }
                                }
                                else
                                {
                                    //if (refreshMap)
                                    //{
                                    //    var tileExtent = new Extent(tileInfo.Extent.MinX, tileInfo.Extent.MinY, tileInfo.Extent.MaxX, tileInfo.Extent.MaxY);
                                    //    RefreshMapFrame(tileExtent, newCancelFunc);
                                    //}
                                }
                                count++;
                                progressAction?.Invoke(30 + count * 70 / tileInfos.Count);
                            }
                        });
                    }

                    if (cancelFunc?.Invoke() == true)
                    {
                        return ret;
                    }

                    // 移除多余的缓存图片
                    for (int i = ret.Tiles.Count - 1; i >= 0; i--)
                    {
                        var existedTileInfo = ret.Tiles.ElementAt(i);
                        if (!tileInfos.Any(x => x.Index == existedTileInfo.Key))
                        {
                            if (ret.Tiles.TryRemove(existedTileInfo.Key, out InRamImageData inRamImageData))
                            {
                                inRamImageData?.Dispose();
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"已正常取消获取瓦片。"); // 不用管该异常
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(GetTileSetAsync)}失败，{ex}");
            }
        }
    }
}
