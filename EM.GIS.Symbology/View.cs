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
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        private LockContainer lockContainer = new LockContainer();
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
                var lockObj = lockContainer.GetOrCreateLock(nameof(BackImage));
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
        public IExtent Extent => Frame.Extent;
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
        private void SetViewBound(RectangleF rect,bool updateMap) 
        {
            SetProperty(ref viewBound, rect,nameof(ViewBound));
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
                SetViewExtent(value, true,true);
            }
        }
        /// <summary>
        /// 设置视图范围
        /// </summary>
        /// <param name="extent">范围</param>
        /// <param name="resetAspectRatio">是否根据长宽比重设范围</param>
        /// <param name="resetBuffer">是否重设缓存</param>
        private void SetViewExtent(IExtent extent,bool resetAspectRatio,bool resetBuffer)
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
                var lockObj = lockContainer.GetOrCreateLock(nameof(IsWorking));
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
                    bw.RunWorkerAsync(DrawingViewCache);
                }
            }
        }

        private void DrawFrame(MapArgs mapArgs, bool onlyInitialized, Func<bool> cancelFunc, Action<Rectangle> updateMapAction)
        {
            mapArgs.Graphics.Clear(Background); //填充背景色

            #region 获取进度委托
            IEnumerable<ILayer> allVisibleLayers = Frame.Children.GetAllLayers().Where(x => x.GetVisible(mapArgs.DestExtent));
            if (onlyInitialized)
            {
                allVisibleLayers = allVisibleLayers.Where(x => x.IsDrawingInitialized(mapArgs, mapArgs.DestExtent));
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
            var destRect = Rectangle.Empty;
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
        /// <param name="rectangle">复制矩形的范围</param>
        private void CopyDrawingViewCacheToBackImage(Rectangle rectangle)
        {
            if (DrawingViewCache == null)
            {
                return;
            }
            if (BackImage == null)
            {
                BackImage = DrawingViewCache.Copy();
            }
            else
            {
                if (DrawingViewCache.Bitmap == null)
                {
                    return;
                }
                try
                {
                    var lockObj = lockContainer.GetOrCreateLock(nameof(BackImage.Bitmap));
                    lock (lockObj)
                    {
                        if (BackImage.Bitmap != null && BackImage.Bitmap.Width == DrawingViewCache.Bitmap.Width && BackImage.Bitmap.Height == DrawingViewCache.Bitmap.Height)
                        {
                            DrawingViewCache.Bitmap.CopyBitmapByMemory(BackImage.Bitmap);
                        }
                        else
                        {
                            BackImage.Bitmap = DrawingViewCache.Bitmap.Copy();
                        }
                        BackImage.Bound = DrawingViewCache.Bound;
                        BackImage.Extent = DrawingViewCache.Extent;
                        BackImage.DrawingExtent = DrawingViewCache.DrawingExtent;
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
            if (!(sender is BackgroundWorker worker) || !(e.Argument is DrawingArgs drawingArgs) || Frame == null|| Width==0||Height==0)
            {
                return;
            }

            //记录正在绘制的视图缓存
            if (DrawingViewCache == null)
            {
                DrawingViewCache = new ViewCache(drawingArgs);
            }
            else
            {
                DrawingViewCache.Bound = drawingArgs.Bound;
                DrawingViewCache.Extent = drawingArgs.Extent;
                DrawingViewCache.DrawingExtent = drawingArgs.Extent;
            }
            if (DrawingViewCache.Bitmap==null|| DrawingViewCache.Bitmap?.PixelFormat== PixelFormat.DontCare|| DrawingViewCache.Bitmap.Width!=Width|| DrawingViewCache.Bitmap.Height!=Height)
            {
                DrawingViewCache.Bitmap=new Bitmap(Width,Height);    
            }

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
            bool cancelDrawingInitializedLayers = false;//取消绘制已初始化图层的标记
            Func<bool> drawingInitializedLayersCancelFunc = () =>
            {
                return cancelFunc() || cancelDrawingInitializedLayers;
            };
           var layers = Frame.Children.GetAllLayers().Where(x => !x.IsDrawingInitialized(drawingArgs, drawingArgs.DrawingExtent));
            Task? initializeLayerTask = null;//初始化图层任务
            if (layers.Count() > 0)
            {
                //异步线程完成图层的初始化
                initializeLayerTask = Task.Run(() =>
                {
                    #region 初始化图层绘制
                    try
                    {
                        ParallelOptions parallelOptions = new ParallelOptions()
                        {
                            CancellationToken = parallelCts.Token
                        };
                        Parallel.ForEach(layers, parallelOptions, (layer) =>
                        {
                            layer.InitializeDrawing(drawingArgs, drawingArgs.DrawingExtent);
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine($"已正常取消{nameof(ILayer.InitializeDrawing)}。"); // 不用管该异常
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{nameof(ILayer.InitializeDrawing)}失败，{ex}");
                    }
                    #endregion
                }).ContinueWith((task) =>
                {
                    #region 再重新绘制所有图层
                    if (!cancelFunc())
                    {
                        cancelDrawingInitializedLayers = true;//先取消绘制已准备好的图层
                        DrawToViewCache(DrawingViewCache, cancelFunc);
                    }
                    #endregion
                });
                initializeLayerTask.ConfigureAwait(false);
            }
            #endregion
            DrawToViewCache(DrawingViewCache,  drawingInitializedLayersCancelFunc);
            if (!drawingInitializedLayersCancelFunc())//若取消绘制则释放图片
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
        private void DrawToViewCache(ViewCache viewCache,Func<bool> drawingInitializedLayersCancelFunc)
        {
            using Graphics g = Graphics.FromImage(viewCache.Bitmap);
            MapArgs mapArgs = new MapArgs(viewCache.Bound, viewCache.Extent, g, Frame.Projection, viewCache.DrawingExtent);
            SetClip(mapArgs);
            //Action<Rectangle> newUpdateMapAction = GetNewUpdateMapAction(viewCache);//先设置缓存图片，再更新地图控件
            Action<Rectangle> newUpdateMapAction = (rect) =>
            {
                CopyDrawingViewCacheToBackImage(rect);
                UpdateMapAction?.Invoke(rect);//更新地图控件
            };
            DrawFrame(mapArgs, true, drawingInitializedLayersCancelFunc, newUpdateMapAction);//mapargs绘制冲突
        }
        /// <inheritdoc/>
        public void ResetBuffer(Rectangle rectangle, IExtent extent, IExtent drawingExtent)
        {
            if (rectangle.IsEmpty || extent == null || extent.IsEmpty() || drawingExtent == null || drawingExtent.IsEmpty())
            {
                return;
            }
            IsWorking = true;
            DrawingArgs drawingArgs = new DrawingArgs(rectangle, extent, drawingExtent);
            if (!bw.IsBusy)
                bw.RunWorkerAsync(drawingArgs);
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
                    var lockObj = lockContainer.GetOrCreateLock(nameof(BackImage.Bitmap));
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
        /// <param name="rectangle">矩形范围</param>
        /// <returns>相对于背景图像的矩形范围</returns>
        public RectangleF GetSrcRectangleToView(RectangleF rectangle)
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
        /// <summary>
        /// 获得相对于背景图像的矩形范围
        /// </summary>
        /// <param name="rectangle">矩形范围</param>
        /// <returns>相对于背景图像的矩形范围</returns>
        public Rectangle GetSrcRectangleToView(Rectangle rectangle)
        {
            var dx = ViewBound.Width / Width;
            var dy = ViewBound.Height / Height;
            var left = (int)(ViewBound.Left + rectangle.Left * dx);
            if (left < 0)
            {
                //left = 0;
            }
            var right = (int)(ViewBound.Left + rectangle.Right * dx);
            var top = (int)(ViewBound.Top + rectangle.Top * dy);
            if (top < 0)
            {
                //top = 0;
            }
            var bottom = (int)(ViewBound.Top + rectangle.Bottom * dy);
            var result = Rectangle.FromLTRB(left,top,right,bottom);
            return result;
        }
        /// <inheritdoc/>
        public void ResetViewExtent()
        {
            if (Bound.IsEmpty|| ViewExtent.IsEmpty())
            {
                return;
            }
            IExtent env = IProjExtensions.PixelToProj(ViewBound, Bound, ViewExtent);
            SetViewExtent(env, false,true);
            ViewBound = Bound;
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
        /// <inheritdoc/>
        public Bitmap? GetBitmap()
        {
            Bitmap? bmp=null;
            var lockObj = lockContainer.GetOrCreateLock(nameof(BackImage.Bitmap));
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
