using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 视图
    /// </summary>
    public class View : BaseCopy, IView
    {
        /// <summary>
        /// 锁
        /// </summary>
        private readonly object _lockObj = new object();
        /// <summary>
        /// 视图范围改变次数
        /// </summary>
        private int viewExtentChangedCount;
        /// <summary>
        /// 视图范围是否改变
        /// </summary>
        private bool viewExtentChanged;
        private BackgroundWorker bw;
        /// <summary>
        /// 正在绘制的缓存图片
        /// </summary>
        private ViewCache drawingViewCache;
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

        ViewCache backImage;
        /// <inheritdoc/>
        public ViewCache BackImage
        {
            get
            {
                return backImage;
            }
            set
            {
                lock (_lockObj)
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
        public IExtent Extent => Frame.Extent;
        private RectangleF viewBound;
        /// <inheritdoc/>
        public RectangleF ViewBound
        {
            get
            {
                if (viewBound.IsEmpty)
                {
                    return Bound;
                }
                else
                {
                    return viewBound;
                }
            }
            set { SetProperty(ref viewBound, value); }
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
                if (Equals(viewExtent, value) || value == null) return;

                IExtent ext = value.Copy();
                ExtentExtensions.ResetAspectRatio(ext, Width, Height);

                ResetBuffer(Bound, ext, ext);
                //OnPropertyChanged(nameof(ViewExtent));
            }
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
                lock (_lockObj)
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
        public Action<RectangleF>? UpdateMapAction { get; set; }
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
            ViewExtent = extent;
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
                    ResetBuffer(Bound, ViewExtent, ViewExtent);
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
        private void RemoveLayerEvent(ILegendItem layer)
        {
            if (layer != null)
            {
                layer.PropertyChanged -= Layer_PropertyChanged;
                layer.Children.CollectionChanged -= LegendItems_CollectionChanged;
            }
        }
        private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sender is BackgroundWorker bw)
            {
                IsWorking = false;
                if (IsWorking)
                {
                    bw.RunWorkerAsync(drawingViewCache);
                }
            }
        }

        private void DrawFrame(MapArgs mapArgs, bool onlyInitialized, Func<bool> cancelFunc, Action<RectangleF> updateMapAction)
        {
            mapArgs.Graphics.Clear(Background); //填充背景色

            #region 获取进度委托
            IEnumerable<ILayer> allVisibleLayers = Frame.Children.GetAllLayers().Where(x => x.GetVisible(mapArgs.DestExtent));
            if (onlyInitialized)
            {
                allVisibleLayers = allVisibleLayers.Where(x => x.IsDrawingInitialized(mapArgs));
            }
            List<ILabelLayer> visibleLabelLayers = new List<ILabelLayer>();
            foreach (var item in allVisibleLayers)
            {
                if (item is IFeatureLayer featureLayer && featureLayer.LabelLayer.GetVisible(mapArgs.DestExtent))
                {
                    visibleLabelLayers.Add(featureLayer.LabelLayer);
                }
            }
            var totalCount = allVisibleLayers.Count() + visibleLabelLayers.Count;
            if (totalCount == 0)
            {
                return;
            }
            double layerRatio = allVisibleLayers.Count() / totalCount;//可见图层占比
            double labelLayerRatio = visibleLabelLayers.Count / totalCount;//标注图层占比
            Action<string, int> drawLayerProgressAction = (txt, progress) =>
            {
                if (Progress != null)
                {
                    var destProgress = (int)(progress / 100 * layerRatio);
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
            RectangleF destRect = RectangleF.Empty;
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
                var rect = Frame.Draw(mapArgs, onlyInitialized, selected, destProgressAction, cancelFunc, updateMapAction);
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
                var rect = visibleLabelLayers[i].Draw(mapArgs, onlyInitialized, false, drawLabelProgressAction, cancelFunc, updateMapAction);
                if (!rect.IsEmpty)
                {
                    destRect = destRect.ExpandToInclude(rect);
                }
            }
            #endregion

            //if (cancelFunc?.Invoke() != true)
            //{
            //    updateMapAction.Invoke(destRect);//更新地图控件
            //}
        }
        private void ResetViewCacheAndUpdateMap(ViewCache viewCache, RectangleF rect, bool copyView = true)
        {
            #region 设置缓存图片、视图矩形、试图范围
            ViewCache destViewCache = copyView ? viewCache.Copy() : viewCache;

            bool viewExtentChanged = false;
            bool viewBoundChanged = false;
            if (viewExtent != viewCache.Extent)
            {
                viewExtent = viewCache.Extent;
                viewExtentChanged = true;
            }
            if (viewBound != viewCache.Bound)
            {
                viewBound = viewCache.Bound;
                viewBoundChanged = true;
            }
            BackImage = destViewCache;
            if (viewExtentChanged) OnPropertyChanged(nameof(ViewExtent));
            if (viewBoundChanged) OnPropertyChanged(nameof(ViewBound));
            #endregion

            UpdateMapAction?.Invoke(rect);//更新地图控件
        }
        private Action<RectangleF> GetNewUpdateMapAction(ViewCache viewCache)
        {
            Action<RectangleF> newUpdateMapAction = (rect) => ResetViewCacheAndUpdateMap(viewCache, rect);
            return newUpdateMapAction;
        }
        private void BwDoWork(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker worker) || !(e.Argument is ViewCache viewCache) || Frame == null)
            {
                return;
            }

            using Graphics g = Graphics.FromImage(viewCache.Image);
            MapArgs mapArgs = new MapArgs(viewCache.Bound, viewCache.Extent, g, Frame.Projection, viewCache.DrawingExtent);
            using GraphicsPath gp = new GraphicsPath();

            #region 设置绘制裁剪区域
            if (!Equals(mapArgs.Extent, mapArgs.DestExtent))
            {
                var rect = mapArgs.ProjToPixelF(mapArgs.DestExtent);
                gp.StartFigure();
                gp.AddRectangle(rect);
                g.Clip = new Region(gp);
            }
            #endregion

            Action<RectangleF> newUpdateMapAction = GetNewUpdateMapAction(viewCache);//先设置缓存图片，再更新地图控件

            #region 初始化绘制图层
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
            var layers = Frame.Children.GetAllLayers().Where(x => !x.IsDrawingInitialized(mapArgs));
            Task? initializeLayerTask = null;//初始化图层任务
            if (layers.Count() > 0)
            {
                //异步线程完成图层的初始化
                initializeLayerTask = Task.Run(() =>
                {
                    #region 初始化图层绘制
                    ParallelOptions parallelOptions = new ParallelOptions()
                    {
                        CancellationToken = parallelCts.Token
                    };
                    Parallel.ForEach(layers, parallelOptions, (layer) =>
                    {
                        layer.InitializeDrawing(mapArgs);
                    });
                    #endregion
                }).ContinueWith((task) =>
                {
                    #region 重新绘制所有图层
                    if (!cancelFunc())
                    {
                        var viewCacheCopy = viewCache.Copy();
                        var ret = viewCacheCopy.Image == viewCache.Image;
                        DrawFrame(mapArgs, false, cancelFunc, newUpdateMapAction);
                    }
                    #endregion
                });
                initializeLayerTask.ConfigureAwait(false);
            }
            #endregion
            DrawFrame(mapArgs, true, cancelFunc, newUpdateMapAction);
            if (cancelFunc())//若取消绘制则释放图片
            {
                viewCache.Dispose();
            }
            else
            {
                if (initializeLayerTask != null)
                {
                    initializeLayerTask.Wait(); //等待初始化线程及重绘完成
                }
                //TODO 待测试，后台绘制完成后，重设缓存以及刷新地图控件
                //var rect = mapArgs.ProjToPixelF(mapArgs.DestExtent);
                //ResetViewCacheAndUpdateMap(viewCache, rect, false);
            }
        }

        /// <inheritdoc/>
        public void ResetBuffer(Rectangle rectangle, IExtent extent, IExtent drawingExtent)
        {
            if (rectangle.IsEmpty || extent == null || extent.IsEmpty() || drawingExtent == null || drawingExtent.IsEmpty())
            {
                return;
            }
            IsWorking = true;
            //记录正在绘制的视图缓存
            if (BackImage?.Image != null && BackImage.Bound.Equals(rectangle) && BackImage.Extent.Equals(extent))
            {
                var bitmap = BackImage.Image.Copy();
                drawingViewCache = new ViewCache(bitmap, rectangle, extent, drawingExtent);
            }
            else
            {
                drawingViewCache = new ViewCache(rectangle.Width, rectangle.Height, rectangle, extent, drawingExtent);
            }
            if (!bw.IsBusy)
                bw.RunWorkerAsync(drawingViewCache);
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
            if (BackImage?.Image != null && g != null)
            {
                var srcRectangle = GetRectangleToView(rectangle);
                try
                {
                    lock (_lockObj)
                    {
                        g.DrawImage(BackImage.Image, rectangle, srcRectangle, GraphicsUnit.Pixel);
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
        /// <param name="rectangle">矩形范围</param>
        /// <returns>相对于背景图像的矩形范围</returns>
        public RectangleF GetRectangleToView(RectangleF rectangle)
        {
            var result = new RectangleF
            {
                X = ViewBound.X + (rectangle.X * ViewBound.Width / Width),
                Y = ViewBound.Y + (rectangle.Y * ViewBound.Height / Height),
                Width = rectangle.Width * ViewBound.Width / Width,
                Height = rectangle.Height * ViewBound.Height / Height
            };
            return result;
        }
        public void ResetViewExtent()
        {
            IExtent env = IProjExtensions.PixelToProj(ViewBound, Bound, ViewExtent);
            ResetBuffer(Bound, env, env);
        }
        /// <inheritdoc/>
        public void Resize(int width, int height)
        {
            var dx = width - Width;
            var dy = height - Height;
            var destWidth = ViewBound.Width + dx;
            var destHeight = ViewBound.Height + dy;

            // Check for minimal size of view.
            if (destWidth < 5) destWidth = 5;
            if (destHeight < 5) destHeight = 5;

            viewBound = new RectangleF(ViewBound.X, ViewBound.Y, destWidth, destHeight);
            ResetViewExtent();

            Width = width;
            Height = height;
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
    }
}
