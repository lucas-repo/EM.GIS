﻿using BruTile;
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
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public override IEnumerable<IRasterSet> Rasters => Tiles.Values.Select(x=>x.Tile);
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
        public void AddTileToTiles(TileInfo tileInfo, (Bitmap Bitmap, bool IsNodata) tileBitmap, Func<bool>? cancelFunc = null)
        {
            if (tileInfo == null || tileInfo.Extent == null || tileInfo.Index == null || tileBitmap.Bitmap == null || cancelFunc?.Invoke() == true)
            {
                return;
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
                    Tiles.TryAdd(tileInfo.Index, (inRamImageData, tileBitmap.IsNodata));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(AddTileToTiles)}失败_{tileInfo.Index.Level}_{tileInfo.Index.Col}_{tileInfo.Index.Row},{ex}");
            }
        }

        private ImageSet? GetInRamImageData(Image image, Geometries.Extent bmpExtent)
        {
            ImageSet? tileImage = null;
            try
            {
                tileImage = new ImageSet(image, bmpExtent)
                {
                    Name = Name,
                    Projection = Projection,
                    Bounds = new RasterBounds(image.Height, image.Width, bmpExtent)
                };
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
        public async Task<(Bitmap Bitmap, bool IsNodata)> GetBitmapAsync(TileInfo tileInfo, int reloadTimes = 1, Func<bool>? cancelFunc = null)
        {
            Bitmap? bitmap = null;
            bool isNodata = false;
            if (TileSource is HttpTileSource httpTileSource)
            {
                byte[]? data = null;
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
            if (TileSource is HttpTileSource httpTileSource)
            {
                var levels = httpTileSource.Schema.Resolutions.Keys;
                if (levels.Count > 0)
                {
                    minZoom = levels.First();
                    maxZoom = levels.Last();
                }

                var zoom = TileCalculator.DetermineZoomLevel(geoExtent, rectangle, minZoom, maxZoom);
                ret.AddRange(httpTileSource.Schema.GetTileInfos(new BruTile.Extent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY), zoom));
            }

            return ret;
        }
    }
}
