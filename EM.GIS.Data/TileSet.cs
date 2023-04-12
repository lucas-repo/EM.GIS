using BruTile;
using BruTile.Web;
using EM.Bases;
using EM.GIS.Data.Properties;
using EM.GIS.Geometries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Data
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public class TileSet : RasterSet, ITileSet
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<TileIndex, (IRasterSet Tile, bool IsNodata)> Tiles { get; } = new ConcurrentDictionary<TileIndex, (IRasterSet Tile, bool IsNodata)>();
        private LockContainer _lockContainer = new LockContainer();
        /// <inheritdoc/>
        public ITileSource TileSource { get; }
        /// <summary>
        /// 会产生graphics异常的PixelFormat
        /// </summary>
        private readonly PixelFormat[] IndexedPixelFormats =
            { PixelFormat.Undefined, PixelFormat.DontCare, PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };
        /// <inheritdoc/>
        public override IEnumerable<IRasterSet> Rasters => Tiles.Values.Select(x => x.Tile);
        private IExtent extent;
        /// <inheritdoc/>
        public override IExtent Extent
        {
            get => extent;
            set => extent = value;
        }
        /// <summary>
        /// 初始化<seealso cref="Tiles"/>
        /// </summary>
        /// <param name="tileSource">瓦片源</param>
        public TileSet(ITileSource tileSource)
        {
            TileSource = tileSource;
            extent = TileSource.Schema.Extent.ToExtent();
            Name = tileSource.Name;
            RasterType = RasterType.Byte;
        }
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
                            item.Value.Tile?.Dispose();
                        }
                        Tiles.Clear();
                    }
                    Nodata?.Dispose();
                }
            }
            base.Dispose(disposeManagedResources);
        }
        /// <summary>
        /// 判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        /// </summary>
        /// <param name="imgPixelFormat">像素格式</param>
        /// <returns>是索引模式则返回true</returns>
        private bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in IndexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }

            return false;
        }
        /// <inheritdoc/>
        public IRasterSet? AddTileToTiles(TileInfo tileInfo, (Bitmap Bitmap, bool IsNodata) tileBitmap, Func<bool>? cancelFunc = null)
        {
            IRasterSet? ret = null;
            if (tileInfo == null || tileInfo.Extent == null || tileInfo.Index == null || tileBitmap.Bitmap == null || cancelFunc?.Invoke() == true)
            {
                return ret;
            }

            try
            {
                var extent = new Geometries.Extent(tileInfo.Extent.MinX, tileInfo.Extent.MinY, tileInfo.Extent.MaxX, tileInfo.Extent.MaxY);
                if (!Tiles.ContainsKey(tileInfo.Index))
                {
                    var destImage = tileBitmap.Bitmap;
                    bool isIndex = IsPixelFormatIndexed(tileBitmap.Bitmap.PixelFormat); // 是否为索引模式

                    // 需要把索引图片转成普通位图，才能正常绘制到地图上
                    if (isIndex)
                    {
                        var bmp = new Bitmap(tileBitmap.Bitmap.Width, tileBitmap.Bitmap.Height, PixelFormat.Format24bppRgb);
                        bmp.SetResolution(tileBitmap.Bitmap.HorizontalResolution, tileBitmap.Bitmap.VerticalResolution); // 设置dpi，防止与屏幕dpi不一致导致拼接错位
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.DrawImage(tileBitmap.Bitmap, 0, 0);
                        }

                        destImage = bmp;
                        if (cancelFunc?.Invoke() == true)
                        {
                            bmp.Dispose();
                            return ret;
                        }
                    }

                    ret = new ImageSet(destImage, extent)
                    {
                        Name = Name,
                        Projection = Projection,
                        Bounds = new RasterBounds(destImage.Height, destImage.Width, extent)
                    };
                    Tiles.TryAdd(tileInfo.Index, (ret, tileBitmap.IsNodata));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(AddTileToTiles)}失败_{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row},{ex}");
            }
            return ret;
        }

        /// <summary>
        /// 获取瓦片位图
        /// </summary>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="reloadTimes">重试次数</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <returns>瓦片位图</returns>
        public async Task<(Bitmap Bitmap, bool IsNodata)> GetBitmapAsync(TileInfo tileInfo, int reloadTimes = 1, Func<bool>? cancelFunc = null)
        {
            Bitmap? bitmap = null;
            bool isNodata = false;
            byte[]? data = null;
            for (int i = 0; i < reloadTimes; i++)
            {
                if (cancelFunc?.Invoke() == true)
                {
                    break;
                }
                bool catchedException = false;//是否已捕捉异常
                try
                {
                    //data = httpTileSource.PersistentCache?.Find(tileInfo.Index);
                    //if (data == null)
                    //{
                    data = await TileSource.GetTileAsync(tileInfo);
                    //if (data != null)
                    //{
                    //    httpTileSource.PersistentCache?.Add(tileInfo.Index, data);
                    //}
                    //}
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"已取消第{i}次下载瓦片 {tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row} ");
                    catchedException = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"获取瓦片 {tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row} 失败,{e}");
                    catchedException = true;
                }
                if (data != null)
                {
                    break;
                }
                else
                {
                    if (!catchedException)
                    {
                        Debug.WriteLine($"第{i}次下载瓦片 {tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row} 超时");
                    }
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
            }
            catch (Exception e)
            {
                Debug.WriteLine($"瓦片 {tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row} 生成位图失败,{e}");
            }
            if (bitmap == null)
            {
                bitmap = Nodata.Copy();
                isNodata = true;
            }
            return (bitmap, isNodata);
        }
        private Bitmap? _nodata;
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
        /// <param name="geoExtent">所需下载的地理坐标系范围</param>
        /// <param name="extent">所需下载的范围</param>
        /// <param name="rectangle">窗口大小</param>
        /// <returns>瓦片信息集合</returns>
        public List<TileInfo> GetTileInfos(IExtent geoExtent, IExtent extent, RectangleF rectangle)
        {
            var ret = new List<TileInfo>();
            int minZoom = 0, maxZoom = 18;
            var levels = TileSource.Schema.Resolutions.Keys;
            if (levels.Count > 0)
            {
                minZoom = levels.First();
                maxZoom = levels.Last();
            }

            var zoom = TileCalculator.DetermineZoomLevel(geoExtent, rectangle, minZoom, maxZoom);
            ret.AddRange(TileSource.Schema.GetTileInfos(extent.ToExtent(), zoom));
            return ret;
        }
        /// <inheritdoc/>
        public List<TileInfo> GetTileInfos(IProj proj, IExtent extent)
        {
            if (proj == null || extent == null)
            {
                return new List<TileInfo>();
            }
            // 若为投影坐标系，记录投影坐标范围
            IExtent geogExtent = extent.Copy();//地理范围
            IExtent destExtent = extent.Copy();//要下载的地图范围
            var destRectangle = proj.ProjToPixel(extent);
            switch (Projection.EPSG)
            {
                case 3857:
                    destExtent.MinX = TileCalculator.Clip(destExtent.MinX, TileCalculator.MinWebMercX, TileCalculator.MaxWebMercX);
                    destExtent.MaxX = TileCalculator.Clip(destExtent.MaxX, TileCalculator.MinWebMercX, TileCalculator.MaxWebMercX);
                    destExtent.MinY = TileCalculator.Clip(destExtent.MinY, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercY);
                    destExtent.MaxY = TileCalculator.Clip(destExtent.MaxY, TileCalculator.MinWebMercY, TileCalculator.MaxWebMercY);
                    destRectangle = proj.ProjToPixel(destExtent);
                    geogExtent = destExtent.Copy();
                    Projection.ReProject(4326, geogExtent);
                    break;
                case 4326:
                    destExtent.MinX = TileCalculator.Clip(destExtent.MinX, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);
                    destExtent.MinX = TileCalculator.Clip(destExtent.MaxX, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);
                    destExtent.MinY = TileCalculator.Clip(destExtent.MinY, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
                    destExtent.MaxY = TileCalculator.Clip(destExtent.MaxY, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
                    destRectangle = proj.ProjToPixel(destExtent);
                    geogExtent = destExtent.Copy();
                    break;
                default:
                    throw new Exception($"不支持的坐标系 {Projection.EPSG}");
            }
            var tileInfos = GetTileInfos(geogExtent, destExtent, destRectangle); // 计算要下载的瓦片
            return tileInfos;
        }
        /// <inheritdoc/>
        public List<TileInfo> GetTileInfos(int level, IExtent extent)
        {
            List<TileInfo> ret = new List<TileInfo>();
            if (level < 0 || extent == null)
            {
                return ret;
            }
            var destExtent = extent.ToExtent();
            ret.AddRange(TileSource.Schema.GetTileInfos(destExtent, level));
            return ret;
        }
        /// <inheritdoc/>
        public List<TileInfo> GetTileInfos(int level, IGeometry geometry)
        {
            List<TileInfo> ret = new List<TileInfo>();
            if (level < 0 || geometry == null)
            {
                return ret;
            }
            var destExtent = geometry.GetExtent().ToExtent();
            var tileInfos = TileSource.Schema.GetTileInfos(destExtent, level);
            foreach (var tileInfo in tileInfos)
            {
                var tileGeo = tileInfo.Extent.ToPolygon();
                if (geometry.Intersects(tileGeo))
                {
                    ret.Add(tileInfo);
                }
            }
            return ret;
        }
        /// <summary>
        /// 添加瓦片
        /// </summary>
        /// <param name="tileSet">瓦片数据集</param>
        /// <param name="tileInfo">瓦片信息</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <returns>任务</returns>
        private async Task<IRasterSet?> AddTile(ITileSet tileSet, TileInfo tileInfo, Func<bool>? cancelFunc)
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
        protected override Rectangle OnDraw(MapArgs mapArgs, Action<int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? graphicsUpdatedAction = null, Dictionary<string, object>? options = null)
        {
            var ret = Rectangle.Empty;
            try
            {
                if (cancelFunc?.Invoke() == true) return ret;

                var tileInfos = GetTileInfos(mapArgs, mapArgs.DestExtent); // 计算要下载的瓦片
                progressAction?.Invoke( 5);
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
                        using var task = AddTile(this, tileInfo, newCancelFunc);
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
                                    graphicsUpdatedAction?.Invoke(rect);
                                }
                                progress += increment;
                                progressAction?.Invoke( (int)progress);
                            }
                        }
                    });
                }
                #endregion

                #region 超过缓存数后，移除多余的缓存图片
                if (Tiles.Count > 1000)
                {
                    for (int i = Tiles.Count - 1; i >= 0; i--)
                    {
                        var existedTileInfo = Tiles.ElementAt(i);
                        if (!tileInfos.Any(x => x.Index == existedTileInfo.Key))
                        {
                            if (Tiles.TryRemove(existedTileInfo.Key, out var tileInfo))
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
