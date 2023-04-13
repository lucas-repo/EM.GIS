using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 视图
    /// </summary>
    public class View : BaseCopy, IView
    {
        /// <summary>
        /// 锁容器
        /// </summary>
        private LockContainer LockContainer { get; } = new LockContainer();
        /// <summary>
        /// 视图范围改变次数
        /// </summary>
        private int viewExtentChangedCount;
        /// <summary>
        /// 视图范围是否改变
        /// </summary>
        private bool viewExtentChanged;
        private BackgroundWorker bw;
        private ViewCache? drawingViewCache;
        /// <summary>
        /// 正在绘制的缓存图片
        /// </summary>
        private ViewCache? DrawingViewCache
        {
            get => drawingViewCache;
            set
            {
                if (drawingViewCache != value)
                {
                    drawingViewCache = value;
                }
            }
        }
        private bool disposedValue;

        /// <summary>
        /// 暂停触发ViewExtentsChanged
        /// </summary>
        protected void SuspendExtentChanged()
        {
            if (viewExtentChangedCount == 0) viewExtentChanged = false;
            viewExtentChangedCount++;
        }
        /// <summary>
        /// 恢复ViewExtentsChanged
        /// </summary>
        protected void ResumeExtentChanged()
        {
            viewExtentChangedCount--;
            if (viewExtentChangedCount == 0)
            {
                if (viewExtentChanged)
                {
                    if (viewExtentChangedCount > 0) return;
                    OnPropertyChanged(nameof(ViewExtent));
                }
            }
        }
        /// <inheritdoc/>
        public int Width { get; private set; }
        /// <inheritdoc/>
        public int Height { get; private set; }
        private Color background;
        /// <inheritdoc/>
        public Color Background
        {
            get { return background; }
            set { SetProperty(ref background, value); }
        }

        ViewCache? backImage;
        /// <inheritdoc/>
        public ViewCache? BackImage
        {
            get
            {
                return backImage;
            }
            set
            {
                var lockObj = LockContainer.GetOrCreateLock(nameof(BackImage));
                lock (lockObj)
                {
                    if (backImage == value)
                    {
                        return;
                    }
                    backImage?.Dispose();
                    backImage = value;
                    OnPropertyChanged(nameof(BackImage));
                }
            }
        }
        /// <inheritdoc/>
        public Rectangle Bound
        {
            get => new Rectangle(0, 0, Width, Height);
        }
        /// <inheritdoc/>
        public IExtent Extent => ViewExtent;
        private RectangleF viewBound;
        /// <inheritdoc/>
        public RectangleF ViewBound
        {
            get
            {
                if (viewBound.IsEmpty)
                {
                    if (viewBound != Bound)
                    {
                        viewBound = Bound;
                    }
                }
                return viewBound;
            }
            set => SetViewBound(value, true);
        }
        /// <summary>
        /// 设置视图矩形
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="updateMap">是否更新地图</param>
        private void SetViewBound(RectangleF rect, bool updateMap)
        {
            SetProperty(ref viewBound, rect, nameof(ViewBound));
            if (updateMap && UpdateMapAction != null)
            {
                UpdateMapAction(Bound);//TODO需要调试范围
            }
        }
        private IExtent viewExtent;
        /// <inheritdoc/>
        public IExtent ViewExtent
        {
            get
            {
                if (viewExtent == null)
                {
                    viewExtent = Frame?.Extent != null ? Frame.Extent.Copy() : new Extent(-180, -90, 180, 90);
                }
                return viewExtent;
            }
            set
            {
                SetViewExtent(value, true, true);
            }
        }
        /// <summary>
        /// 设置视图范围
        /// </summary>
        /// <param name="extent">范围</param>
        /// <param name="resetAspectRatio">是否根据长宽比重设范围</param>
        /// <param name="resetBuffer">是否重设缓存</param>
        private void SetViewExtent(IExtent extent, bool resetAspectRatio, bool resetBuffer)
        {
            if (Equals(viewExtent, extent) || extent == null) return;

            IExtent ext = extent;
            if (resetAspectRatio)
            {
                ext = extent.Copy();
                ExtentExtensions.ResetAspectRatio(ext, Width, Height);
            }
            viewExtent = ext;
            if (resetBuffer)
            {
                ResetBuffer(Bound, ext, ext);
            }
            OnPropertyChanged(nameof(ViewExtent));
        }
        /// <inheritdoc/>
        public IFrame Frame { get; }
        /// <summary>
        /// 计数器
        /// </summary>
        private int _busyCount;
        /// <inheritdoc/>
        public bool IsWorking
        {
            get => _busyCount > 0;
            private set
            {
                var lockObj = LockContainer.GetOrCreateLock(nameof(IsWorking));
                lock (lockObj)
                {
                    if (value)
                    {
                        _busyCount++;
                    }
                    else
                    {
                        _busyCount--;
                    }
                    if (_busyCount < 0)
                    {
                        _busyCount = 0;
                    }
                }
            }
        }
        /// <inheritdoc/>
        public Action<string, int>? Progress { get; set; }
        /// <inheritdoc/>
        public Action<Rectangle>? UpdateMapAction { get; set; }
        /// <summary>
        /// 实例化<see cref="View"/>
        /// </summary>
        /// <param name="frame">地图框架</param>
        /// <exception cref="ArgumentNullException">地图框架为空的异常</exception>
        public View(IFrame frame)
        {
            Frame = frame ?? throw new ArgumentNullException(nameof(frame));
            Frame.FirstLayerAdded += Frame_FirstLayerAdded;
            Frame.Children.CollectionChanged += LegendItems_CollectionChanged;
            PropertyChanged += View_PropertyChanged;
            bw = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            bw.DoWork += BwDoWork;
            bw.RunWorkerCompleted += BwRunWorkerCompleted;
        }

        private void Frame_FirstLayerAdded(object sender, EventArgs e)
        {
            var extent = Frame.Extent.Copy();
            if (!extent.IsEmpty())
            {
                extent.ExpandBy(extent.Width / 10, extent.Height / 10);
            }
            SetViewExtent(extent, true, false);
        }

        private void LegendItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddLayerEvent(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveLayerEvent(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveLayerEvent(e.OldItems);
                    AddLayerEvent(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RemoveLayerEvent(e.OldItems);
                    break;
            }
            ResetBuffer(Bound, ViewExtent, ViewExtent);
        }
        private void Layer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ILegendItem.IsVisible):
                    if (sender is IRenderableItem)
                    {
                        ResetBuffer(Bound, ViewExtent, ViewExtent);
                    }
                    break;
            }
        }
        private void AddLayerEvent(IList? list)
        {
            if (list != null)
            {
                foreach (ILegendItem item in list)
                {
                    AddLayerEvent(item);
                }
            }
        }
        private void AddLayerEvent(ILegendItem layer)
        {
            if (layer != null)
            {
                layer.PropertyChanged += Layer_PropertyChanged;
                if (layer is IGroup group)
                {
                    group.Children.CollectionChanged += LegendItems_CollectionChanged;
                }
            }
        }
        private void RemoveLayerEvent(IList? list)
        {
            if (list != null)
            {
                foreach (ILegendItem item in list)
                {
                    RemoveLayerEvent(item);
                }
            }
        }
        private void RemoveLayerEvent(ILegendItem legendItem)
        {
            if (legendItem != null)
            {
                legendItem.PropertyChanged -= Layer_PropertyChanged;
                legendItem.Children.CollectionChanged -= LegendItems_CollectionChanged;
                if (legendItem is IRenderableItem renderableItem)
                {
                    ResetBuffer(Bound, ViewExtent, renderableItem.Extent);
                }
            }
        }
        private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sender is BackgroundWorker bw)
            {
                IsWorking = false;
                if (IsWorking)
                {
                    bw.RunWorkerAsync(DrawingViewCache);
                }
            }
        }

        private void DrawFrame(MapArgs mapArgs, Func<bool> cancelFunc, Action<Rectangle> updateMapAction)
        {
            mapArgs.Graphics.Clear(Background); //填充背景色

            #region 获取进度委托
            IEnumerable<ILayer> allVisibleLayers = Frame.Children.GetAllLayers().Where(x => x.GetVisible(mapArgs.DestExtent));
            List<ILabelLayer> visibleLabelLayers = new List<ILabelLayer>();
            foreach (var item in allVisibleLayers)
            {
                if (item is IFeatureLayer featureLayer && featureLayer.LabelLayer.GetVisible(mapArgs.DestExtent))
                {
                    visibleLabelLayers.Add(featureLayer.LabelLayer);
                }
            }
            var destRect = Rectangle.Empty;
            var totalCount = allVisibleLayers.Count() + visibleLabelLayers.Count;
            if (totalCount == 0)
            {
                destRect = mapArgs.ProjToPixel(mapArgs.DestExtent);
                updateMapAction(destRect);
                return;
            }
            double layerRatio = allVisibleLayers.Count() / totalCount;//可见图层占比
            double labelLayerRatio = visibleLabelLayers.Count / totalCount;//标注图层占比
            Action<string, int> drawLayerProgressAction = (txt, progress) =>
            {
                if (Progress != null)
                {
                    var destProgress = (int)(progress * layerRatio);
                    Progress.Invoke(txt, destProgress);
                }
            };
            Action<string, int> drawLabelProgressAction = (txt, progress) =>
            {
                if (Progress != null)
                {
                    var destProgress = (int)(100.0 * layerRatio + progress / 100 * labelLayerRatio);
                    Progress.Invoke(txt, destProgress);
                }
            };
            #endregion
            #region 绘制图层
            int count = 2;
            for (int i = 0; i < count; i++)
            {
                if (cancelFunc?.Invoke() == true)
                {
                    break;
                }
                bool selected = i == 1;
                Action<string, int>? destProgressAction = selected ? null : drawLayerProgressAction;//不更新绘制选择要素的进度
                var rect = Frame.Draw(mapArgs, selected, destProgressAction, cancelFunc, updateMapAction);
                if (!rect.IsEmpty)
                {
                    destRect = destRect.ExpandToInclude(rect);
                }
            }
            #endregion

            #region 绘制标注
            for (int i = visibleLabelLayers.Count - 1; i >= 0; i--)
            {
                if (cancelFunc?.Invoke() == true)
                {
                    break;
                }
                var rect = visibleLabelLayers[i].Draw(mapArgs, false, drawLabelProgressAction, cancelFunc, updateMapAction);
                if (!rect.IsEmpty)
                {
                    destRect = destRect.ExpandToInclude(rect);
                }
            }
            #endregion

            //Progress?.Invoke("已完成", 0);
            Progress?.Invoke(string.Empty, 0);
            //if (cancelFunc?.Invoke() != true)
            //{
            //    updateMapAction.Invoke(destRect);//更新地图控件
            //}
        }
        private void ResetViewCacheAndUpdateMap(ViewCache viewCache, Rectangle rect, bool copyView = true)
        {
            #region 设置缓存图片、视图矩形、试图范围
            ViewCache destViewCache = copyView ? viewCache.Copy() : viewCache;

            bool viewExtentChanged = false;
            if (viewExtent != viewCache.Extent)
            {
                viewExtent = viewCache.Extent;
                viewExtentChanged = true;
            }
            SetViewBound(viewCache.Bound, false);
            BackImage = destViewCache;
            if (viewExtentChanged) OnPropertyChanged(nameof(ViewExtent));
            #endregion

            UpdateMapAction?.Invoke(rect);//更新地图控件
        }

        private Action<Rectangle> GetNewUpdateMapAction(ViewCache viewCache)
        {
            Action<Rectangle> newUpdateMapAction = (rect) => ResetViewCacheAndUpdateMap(viewCache, rect);
            return newUpdateMapAction;
        }
        /// <summary>
        /// 设置绘制裁剪区域
        /// </summary>
        /// <param name="mapArgs">参数</param>
        private void SetClip(MapArgs mapArgs)
        {
            using GraphicsPath gp = new GraphicsPath();
            if (!Equals(mapArgs.Extent, mapArgs.DestExtent))
            {
                var rect = mapArgs.ProjToPixelF(mapArgs.DestExtent);
                gp.StartFigure();
                gp.AddRectangle(rect);
                mapArgs.Graphics.Clip = new Region(gp);
            }
        }
        /// <summary>
        /// 将当前绘制的缓存复制到视图缓存
        /// </summary>
        /// <param name="viewCache">正在绘制的视图缓存</param>
        /// <param name="rectangle">复制矩形的范围</param>
        /// <param name="copyCache">是否整体复制缓存</param>
        private void CopyDrawingViewCacheToBackImage(ViewCache viewCache, Rectangle rectangle, bool copyCache)
        {
            if (viewCache == null)
            {
                return;
            }
            if (BackImage == null || copyCache)
            {
                BackImage = viewCache.Copy();
            }
            else
            {
                if (viewCache.Bitmap == null)
                {
                    return;
                }
                try
                {
                    var lockObj = LockContainer.GetOrCreateLock(nameof(BackImage));
                    lock (lockObj)
                    {
                        if (BackImage.Bitmap != null && BackImage.Bitmap.Width == viewCache.Bitmap.Width && BackImage.Bitmap.Height == viewCache.Bitmap.Height)
                        {
                            using Graphics g = Graphics.FromImage(BackImage.Bitmap);
                            g.DrawImage(viewCache.Bitmap, rectangle, rectangle, GraphicsUnit.Pixel);//绘制有问题
                        }
                        else
                        {
                            BackImage.Bitmap = viewCache.Bitmap.Copy();
                        }
                        BackImage.Bound = viewCache.Bound;
                        if (!Equals(BackImage.Extent, viewCache.Extent))
                        {
                            BackImage.Extent = viewCache.Extent.Copy();
                        }
                        if (!Equals(BackImage.DrawingExtent, viewCache.DrawingExtent))
                        {
                            BackImage.DrawingExtent = viewCache.DrawingExtent.Copy();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{nameof(CopyDrawingViewCacheToBackImage)}失败，{e}");
                }
            }
        }
        private void BwDoWork(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker worker) || !(e.Argument is ViewCache viewCache) || Frame == null || Width == 0 || Height == 0)
            {
                return;
            }

            using var parallelCts = new CancellationTokenSource();
            Func<bool> cancelFunc = () =>
            {
                bool isCancel = worker.CancellationPending;
                if (isCancel && !e.Cancel)
                {
                    e.Cancel = true;
                }
                if (isCancel && !parallelCts.IsCancellationRequested)
                {
                    parallelCts.Cancel();
                }
                return isCancel;
            };
            DrawToViewCache(viewCache, cancelFunc);
            if (viewCache != DrawingViewCache)//如果不是正在绘制的视图缓存，则释放资源
            {
                viewCache.Dispose();
            }
        }
        /// <summary>
        /// 绘制地图至视图缓存
        /// </summary>
        /// <param name="viewCache">视图缓存</param>
        /// <param name="cancelFunc">取消委托</param>
        private void DrawToViewCache(ViewCache viewCache, Func<bool> cancelFunc)
        {
            using Graphics g = Graphics.FromImage(viewCache.Bitmap);
            MapArgs mapArgs = new MapArgs(viewCache.Bound, viewCache.Extent, g, viewCache.DrawingExtent);
            SetClip(mapArgs);
            //Action<Rectangle> newUpdateMapAction = GetNewUpdateMapAction(viewCache);//先设置缓存图片，再更新地图控件
            int count = 0;
            Action<Rectangle> newUpdateMapAction = (rect) =>
            {
                //BackImage = viewCache.Copy();
                CopyDrawingViewCacheToBackImage(viewCache, rect, count == 0);
                UpdateMapAction?.Invoke(rect);//更新局部地图控件
                count++;
            };
            DrawFrame(mapArgs, cancelFunc, newUpdateMapAction);//mapargs绘制冲突 
            //UpdateMapAction?.Invoke(Bound);//绘制完成后，更新整个地图控件
        }
        /// <inheritdoc/>
        public void ResetBuffer()
        {
            ResetBuffer(Bound, ViewExtent, ViewExtent);
        }
        /// <inheritdoc/>
        public void ResetBuffer(Rectangle rectangle, IExtent extent, IExtent drawingExtent)
        {
            if (rectangle.IsEmpty || extent == null || extent.IsEmpty() || drawingExtent == null || drawingExtent.IsEmpty())
            {
                return;
            }
            IsWorking = true;
            DrawingViewCache = new ViewCache(Width, Height, rectangle, extent, drawingExtent);
            //DrawingArgs drawingArgs = new DrawingArgs(rectangle, extent, drawingExtent);
            if (!bw.IsBusy)
                bw.RunWorkerAsync(DrawingViewCache);
            else
                bw.CancelAsync();
        }
        private void View_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Background):
                    ResetBuffer(Bound, ViewExtent, ViewExtent);
                    break;
                case nameof(ViewExtent):
                    ScaleFactor = GetScaleFactor();
                    break;
            }
        }
        private double GetScaleFactor()
        {
            double ret = double.NaN;
            try
            {
                const double metersPerInche = 0.0254;
                double widthInMeters;
                if (Frame.Projection.IsLatLon)
                {
                    widthInMeters = Frame.Projection.GetLengthOfMeters(0.0, ViewExtent.Center.Y, ViewExtent.Width, ViewExtent.Center.Y);
                }
                else
                {
                    widthInMeters = ViewExtent.Width * Frame.Projection.Unit.Meters;
                }
                double dpi = 96;
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    dpi = g.DpiX;
                }
                double dScreenWidthInMeters = Width / dpi * metersPerInche;
                ret = widthInMeters / dScreenWidthInMeters;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return ret;
        }
        /// <inheritdoc/>
        public void Draw(Graphics g, RectangleF rectangle)
        {
            if (BackImage?.Bitmap != null && g != null)
            {
                var srcRectangle = GetSrcRectangleToView(rectangle);
                try
                {
                    var lockObj = LockContainer.GetOrCreateLock(nameof(BackImage));
                    lock (lockObj)
                    {
                        g.DrawImage(BackImage.Bitmap, rectangle, srcRectangle, GraphicsUnit.Pixel);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"{nameof(Draw)}失败_{rectangle}，{e}");
                }
            }
        }
        /// <summary>
        /// 获得相对于背景图像的矩形范围
        /// </summary>
        /// <param name="destRect">需要绘制的矩形范围</param>
        /// <returns>相对于背景图像的矩形范围</returns>
        public RectangleF GetSrcRectangleToView(RectangleF destRect)
        {
            var result = new RectangleF
            {
                X = ViewBound.X + (destRect.X * ViewBound.Width / Width),
                Y = ViewBound.Y + (destRect.Y * ViewBound.Height / Height),
                Width = destRect.Width * ViewBound.Width / Width,
                Height = destRect.Height * ViewBound.Height / Height
            };
            return result;
        }
        /// <summary>
        /// 获得相对于背景图像的原有矩形范围
        /// </summary>
        /// <param name="destRectangle">目标矩形范围</param>
        /// <returns>原有矩形范围</returns>
        public Rectangle GetSrcRectangleToView(Rectangle destRectangle)
        {
            var dx = ViewBound.Width / Width;
            var dy = ViewBound.Height / Height;
            var left = (int)(ViewBound.Left + destRectangle.Left * dx);
            if (left < 0)
            {
                //left = 0;
            }
            var right = (int)(ViewBound.Left + destRectangle.Right * dx);
            var top = (int)(ViewBound.Top + destRectangle.Top * dy);
            if (top < 0)
            {
                //top = 0;
            }
            var bottom = (int)(ViewBound.Top + destRectangle.Bottom * dy);
            var result = Rectangle.FromLTRB(left, top, right, bottom);
            return result;
        }
        /// <inheritdoc/>
        public RectangleF GetDestRectangleOfView()
        {
            RectangleF destRect;
            if (BackImage == null)
            {
                destRect = RectangleF.Empty;
            }
            else
            {
                var srcRect = BackImage.ProjToPixelF(ViewExtent);//当前视图范围在缓存图片的像素范围
                destRect = new RectangleF
                {
                    X = (srcRect.X - ViewBound.X) * Width / ViewBound.Width,
                    Y = (srcRect.Y - ViewBound.Y) * Height / ViewBound.Height,
                    Width = srcRect.Width * Width / ViewBound.Width,
                    Height = srcRect.Height * Height / ViewBound.Height
                };
            }
            return destRect;
        }
        /// <inheritdoc/>
        public void ResetViewExtent()
        {
            if (Bound.IsEmpty || ViewExtent.IsEmpty())
            {
                return;
            }
            IExtent env = IProjExtensions.PixelToProj(ViewBound, Bound, ViewExtent);
            SetViewExtent(env, false, true);
            SetViewBound(Bound, false);
        }
        /// <inheritdoc/>
        public void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0||(width==Width&&height==Height))
            {
                return;
            }
            var dx = width - Width;
            var dy = height - Height;
            var destWidth = ViewBound.Width + dx;
            var destHeight = ViewBound.Height + dy;

            // Check for minimal size of view.
            if (destWidth < 5) destWidth = 5;
            if (destHeight < 5) destHeight = 5;

            var newViewBound = new RectangleF(ViewBound.X, ViewBound.Y, destWidth, destHeight);
            var newViewExtent= IProjExtensions.PixelToProj(newViewBound, Bound, ViewExtent);
            Width = width;
            Height = height;
            SetViewExtent(newViewExtent, false, true);
            SetViewBound(Bound, false);
        }
        /// <inheritdoc/>
        public void ZoomToMaxExtent()
        {
            var maxExtent = Frame?.GetMaxExtent(true);
            if (maxExtent != null)
            {
                ViewExtent = maxExtent;
            }
        }
        private double scaleFactor;
        /// <summary>
        /// 比例尺因子
        /// </summary>
        public double ScaleFactor
        {
            get { return scaleFactor; }
            set { SetProperty(ref scaleFactor, value); }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    PropertyChanged -= View_PropertyChanged;
                    bw?.Dispose();
                    BackImage?.Dispose();
                    if (Frame != null)
                    {
                        Frame.FirstLayerAdded -= Frame_FirstLayerAdded;
                        Frame.Children.CollectionChanged -= LegendItems_CollectionChanged;
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~View()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc/>
        public Bitmap? GetBitmap()
        {
            Bitmap? bmp = null;
            var lockObj = LockContainer.GetOrCreateLock(nameof(BackImage));
            lock (lockObj)
            {
                if (BackImage?.Bitmap == null)
                {
                    return bmp;
                }
                try
                {
                    var viewRect = BackImage.ProjToPixel(ViewExtent);
                    var srcRect = GetSrcRectangleToView(viewRect);
                    bmp = new Bitmap(Width, Height);//调试用哪个宽度
                    using Graphics g = Graphics.FromImage(bmp);
                    {
                        g.DrawImage(BackImage.Bitmap, Bound, srcRect, GraphicsUnit.Pixel);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"{nameof(GetBitmap)}失败，{e}");
                }
            }
            return bmp;
        }
    }
}
