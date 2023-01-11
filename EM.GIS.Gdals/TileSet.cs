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
using EM.Bases;
using EM.GIS.Data;
using System.Linq;
using BruTile;
using System.IO;
using System.Resources;
using BruTile.Web;
using EM.GIS.Gdals.Properties;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public class TileSet : DataSet, ITileSet
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<BruTile.TileIndex, IRasterSet> Tiles { get; } = new ConcurrentDictionary<BruTile.TileIndex, IRasterSet>();
        private LockContainer _lockContainer = new LockContainer();
        /// <inheritdoc/>
        public BruTile.ITileSource TileSource { get; set; }
        /// <summary>
        /// 会产生graphics异常的PixelFormat
        /// </summary>
        private PixelFormat[] _indexedPixelFormats =
            { PixelFormat.Undefined, PixelFormat.DontCare, PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };

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
        /// <inheritdoc/>
        public void Draw(MapArgs mapArgs, Action<int> progressAction = null, Func<bool> cancelFunc = null)
        {
            if (mapArgs == null || mapArgs.Graphics == null || mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() || cancelFunc?.Invoke() == true)
            {
                return;
            }
            try
            {
                progressAction?.Invoke(10);
                if (cancelFunc?.Invoke() == true) return;

                // 计算范围
                double[] z = { 0, 0 };

                if (cancelFunc?.Invoke() == true) return;
                // 若为投影坐标系，记录投影坐标范围
                IExtent geogExtent = mapArgs.DestExtent.Copy();//地理范围
                IExtent destExtent = mapArgs.DestExtent.Copy();//要下载的地图范围
                var destRectangle = mapArgs.ProjToPixel(mapArgs.DestExtent);
                if (Projection.Equals(TileCalculator.WebMercProj.Value))
                {
                    destExtent.MinX = TileCalculator.Clip(destExtent.MinX, TileCalculator.MinWebMercX, TileCalculator.MaxWebMercX);
                    destExtent.MaxX = TileCalculator.Clip(destExtent.MaxX, TileCalculator.MinWebMercX, TileCalculator.MaxWebMercX);
                    destExtent.MinY = TileCalculator.Clip(destExtent.MinY, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercY);
                    destExtent.MaxY = TileCalculator.Clip(destExtent.MaxY, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercY);
                    destRectangle = mapArgs.ProjToPixel(destExtent);
                    geogExtent = destExtent.Copy();
                    Projection.ReProject(TileCalculator.Wgs84Proj.Value, geogExtent);
                }
                else if (Projection.Equals(TileCalculator.Wgs84Proj.Value))
                {
                    destExtent.MinX = TileCalculator.Clip(destExtent.MinX, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);
                    destExtent.MinX = TileCalculator.Clip(destExtent.MaxX, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);
                    destExtent.MinY = TileCalculator.Clip(destExtent.MinY, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
                    destExtent.MaxY = TileCalculator.Clip(destExtent.MaxY, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
                    destRectangle = mapArgs.ProjToPixel(destExtent);
                    geogExtent = destExtent.Copy();
                }

                progressAction?.Invoke(20);
                if (cancelFunc?.Invoke() == true) return;

                var tileInfos = GetTileInfos(geogExtent, destExtent, destRectangle); // 计算要下载的瓦片
                if (tileInfos.Count > 0)
                {
                    progressAction?.Invoke(30);
                    if (cancelFunc?.Invoke() == true) return;

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
                                if (!Tiles.ContainsKey(tileInfo.Index))
                                {
                                    using (var task = GetBitmapAsync(tileInfo, 1, newCancelFunc).ContinueWith(
                                        (bitmapTask) =>
                                        {
                                            AddTileToTileSet(tileInfo, bitmapTask.Result, newCancelFunc);
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
                                progressAction?.Invoke((int)(30 + count * 60.0 / tileInfos.Count));
                            }
                        });
                    }

                    if (cancelFunc?.Invoke() == true)
                    {
                        return;
                    }
                }
                // 移除多余的缓存图片
                for (int i = Tiles.Count - 1; i >= 0; i--)
                {
                    var existedTileInfo = Tiles.ElementAt(i);
                    if (!tileInfos.Any(x => x.Index == existedTileInfo.Key))
                    {
                        if (Tiles.TryRemove(existedTileInfo.Key, out var rasterSet))
                        {
                            rasterSet?.Dispose();
                        }
                    }
                }

                #region 绘制相交的图片

                foreach (var tile in Tiles)
                {
                    if (cancelFunc?.Invoke() == true)
                    {
                        Debug.WriteLine($"{nameof(Draw)}取消_{tile.Key.Level}_{tile.Key.Col}_{tile.Key.Row}");
                        return;
                    }
                    if (tile.Value.Extent.Intersects(mapArgs.DestExtent))
                    {
                        tile.Value.Draw(mapArgs, progressAction, cancelFunc);
                    }
                    //var srcExtent = GetIntersectExtent(tile.Value.Extent, extent);
                    //if (srcExtent == null)
                    //{
                    //    continue;
                    //}
                    //DrawBmp(graphics,rectangle,extent, tile.Value, srcExtent, cancelFunc);
                }
                #endregion
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"已正常取消获取瓦片。"); // 不用管该异常
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"{nameof(Draw)}失败，{ex}");
            }
        }
        /// <summary>
        /// 绘制图片
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="rectangle">窗口大小</param>
        /// <param name="extent">窗口范围</param>
        /// <param name="rasterSet">图片</param>
        /// <param name="drawExtent">需要绘制的范围</param>
        /// <param name="cancelFunc">取消委托</param>
        private void DrawBmp(Graphics g, Rectangle rectangle, IExtent extent, IRasterSet rasterSet, IExtent drawExtent, Func<bool> cancelFunc = null)
        {
            if (g == null || g.VisibleClipBounds.IsEmpty || rasterSet?.Extent == null || drawExtent == null || drawExtent.Width == 0 || drawExtent.Height == 0 || cancelFunc?.Invoke() == true)
            {
                return;
            }
            Rectangle imgRect = new Rectangle(0, 0, rasterSet.NumColumns, rasterSet.NumRows);
            var srcRect = drawExtent.ProjToPixelF(imgRect, rasterSet.Extent);
            var srcExtent = srcRect.PixelToProj(imgRect, rasterSet.Extent);
            var destRect = srcExtent.ProjToPixelF(rectangle, extent);
            destRect.Inflate(0.5f, 0.5f);
            //var image = rasterSet.GetImage();
            //if (image != null)
            //{
            //    var oldInterpolationMode = g.InterpolationMode;
            //    var oldPixelOffsetMode = g.PixelOffsetMode;
            //    try
            //    {
            //        g.InterpolationMode = InterpolationMode.NearestNeighbor;
            //        g.PixelOffsetMode = PixelOffsetMode.Half;
            //        if (cancelFunc?.Invoke() == true)
            //        {
            //            return;
            //        }
            //        g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
            //    }
            //    catch (System.Exception e)
            //    {
            //        Debug.WriteLine(e);
            //    }
            //    finally
            //    {
            //        g.PixelOffsetMode = oldPixelOffsetMode;
            //        g.InterpolationMode = oldInterpolationMode;
            //    }
            //}
        }
        private IExtent GetIntersectExtent(IExtent extent0, IExtent extent1)
        {
            var tempExt = extent0.Intersection(extent1);
            if (tempExt == null) return null;

            return tempExt;
        }
        /// <summary>
        /// 判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        /// </summary>
        /// <param name="imgPixelFormat">像素格式</param>
        /// <returns>是索引模式则返回true</returns>
        private bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in _indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }

            return false;
        }
        /// <summary>
        /// 将瓦片缓存到瓦片数据集
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="image">瓦片缓存</param>
        /// <param name="cancelFunc">取消委托</param>
        private void AddTileToTileSet(BruTile.TileInfo tileInfo, Image image, Func<bool> cancelFunc = null)
        {
            if (tileInfo == null || tileInfo.Extent == null || tileInfo.Index == null || image == null || cancelFunc?.Invoke() == true)
            {
                return;
            }

            try
            {
                var extent = new Geometries.Extent(tileInfo.Extent.MinX, tileInfo.Extent.MinY, tileInfo.Extent.MaxX, tileInfo.Extent.MaxY);
                if (!Tiles.ContainsKey(tileInfo.Index))
                {
                    var destImage = image;
                    bool isIndex = IsPixelFormatIndexed(image.PixelFormat); // 是否为索引模式

                    // 需要把索引图片转成普通位图，才能正常绘制到地图上
                    if (isIndex)
                    {
                        var bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                        bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution); // 设置dpi，防止与屏幕dpi不一致导致拼接错位
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.DrawImage(image, 0, 0);
                        }

                        destImage = bmp;
                        if (cancelFunc?.Invoke() == true)
                        {
                            bmp.Dispose();
                            return;
                        }
                    }

                    var inRamImageData = GetInRamImageData(destImage, extent);
                    if (inRamImageData == null)
                    {
                        if (isIndex)
                        {
                            destImage.Dispose();
                        }
                        Debug.WriteLine($"{nameof(GetInRamImageData)}失败_{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}");
                        return;
                    }
                    Tiles.TryAdd(tileInfo.Index, inRamImageData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(AddTileToTileSet)}失败_{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row},{ex}");
            }
        }

        private ImageSet GetInRamImageData(BruTile.TileInfo tileInfo, Image image, Func<bool> cancelFunc = null)
        {
            ImageSet ret = null;
            if (tileInfo == null || tileInfo.Extent == null || tileInfo.Index == null || image == null || cancelFunc?.Invoke() == true)
            {
                return ret;
            }

            try
            {
                var extent = new Geometries.Extent(tileInfo.Extent.MinX, tileInfo.Extent.MinY, tileInfo.Extent.MaxX, tileInfo.Extent.MaxY);
                var destImage = image;
                bool isIndex = IsPixelFormatIndexed(image.PixelFormat); // 是否为索引模式

                // 需要把索引图片转成普通位图，才能正常绘制到地图上
                if (isIndex)
                {
                    var bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                    bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution); // 设置dpi，防止与屏幕dpi不一致导致拼接错位
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.DrawImage(image, 0, 0);
                    }

                    destImage = bmp;
                    if (cancelFunc?.Invoke() == true)
                    {
                        bmp.Dispose();
                        return ret;
                    }
                }

                ret = GetInRamImageData(destImage, extent);
                if (ret == null)
                {
                    if (isIndex)
                    {
                        destImage.Dispose();
                    }
                    Debug.WriteLine($"{nameof(GetInRamImageData)}失败_{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(GetInRamImageData)}失败_{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row},{ex}");
            }
            return ret;
        }
        private ImageSet GetInRamImageData(Image image, Geometries.Extent bmpExtent)
        {
            ImageSet tileImage = null;
            try
            {
                tileImage = new ImageSet(image, bmpExtent)
                {
                    Name = Name,
                    Projection = Projection,
                    Bounds = new RasterBounds(image.Height, image.Width, bmpExtent)
                };
                //if (Map != null && TileManager.ServiceProvider.Projection?.Equals(Map.Projection) == false)
                //{
                //    tileImage.Reproject(Map.Projection);
                //    tileImage.Projection = Map.Projection;
                //}
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                if (tileImage != null)
                {
                    tileImage.Dispose();
                    tileImage = null;
                }
            }
            return tileImage;
        }
        /// <summary>
        /// 获取瓦片位图
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="reloadTimes">重试次数</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <returns>瓦片位图</returns>
        public async Task<Bitmap> GetBitmapAsync(TileInfo tileInfo, int reloadTimes = 1, Func<bool> cancelFunc = null)
        {
            Bitmap bitmap = null;
            if (TileSource is HttpTileSource httpTileSource)
            {
                byte[] data = null;
                for (int i = 0; i < reloadTimes; i++)
                {
                    if (cancelFunc?.Invoke() == true)
                    {
                        break;
                    }
                    try
                    {
                        data = httpTileSource.PersistentCache?.Find(tileInfo.Index);
                        if (data == null)
                        {
                            data = await httpTileSource.GetTileAsync(tileInfo);
                            if (data != null)
                            {
                                httpTileSource.PersistentCache?.Add(tileInfo.Index, data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    if (data != null)
                    {
                        break;
                    }
                }

                try
                {
                    if (data != null)
                    {
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            bitmap = new Bitmap(ms);
                            ms.Close();
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"获取瓦片 {tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row} 失败");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"获取瓦片 {tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row} 失败,{e}");
                }
            }
            if (bitmap == null)
            {
                bitmap = Nodata;
            }
            return bitmap;
        }
        private Bitmap _nodata;
        /// <summary>
        /// 无数据的图片
        /// </summary>
        private Bitmap Nodata
        {
            get
            {
                if (_nodata == null)
                {
                    MemoryStream ms = new MemoryStream(Resources.nodata);
                    _nodata = new Bitmap(ms);
                }
                return _nodata;
            }
        }

        /// <summary>
        /// 根据范围获取范围内所有的瓦片信息集合
        /// </summary>
        /// <param name="geoExtent">地理坐标系范围</param>
        /// <param name="extent">所需下载的地图范围</param>
        /// <param name="rectangle">窗口大小</param>
        /// <returns>瓦片信息集合</returns>
        public List<BruTile.TileInfo> GetTileInfos(IExtent geoExtent, IExtent extent, RectangleF rectangle)
        {
            var ret = new List<BruTile.TileInfo>();
            int minZoom = 0, maxZoom = 18;
            if (TileSource is BruTile.Web.HttpTileSource httpTileSource)
            {
                var levels = httpTileSource.Schema.Resolutions.Keys;
                if (levels.Count > 0)
                {
                    minZoom = levels.First();
                    maxZoom = levels.Last();
                }

                var zoom = TileCalculator.DetermineZoomLevel(geoExtent, rectangle, minZoom, maxZoom);
                ret = httpTileSource.Schema.GetTileInfos(new BruTile.Extent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY), zoom)?.ToList();
            }

            return ret;
        }
    }
}
