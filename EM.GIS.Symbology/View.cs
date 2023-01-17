using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
        public Action<string, int> Progress { get; set; }
        public View(IFrame frame)
        {
            Frame = frame ?? throw new ArgumentNullException(nameof(frame));
            Frame.FirstLayerAdded += Frame_FirstLayerAdded;
            Frame.Children.CollectionChanged += LegendItems_CollectionChanged;
            PropertyChanged += View_PropertyChanged;
            bw = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            bw.ProgressChanged += BwProgressChanged;
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
                layer.Children.CollectionChanged += LegendItems_CollectionChanged;
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
        private void BwProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (Progress != null)
            {
                string msg = string.Empty;
                if (e.UserState is string str)
                {
                    msg = str;
                }
                Progress.Invoke(msg, e.ProgressPercentage);
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
                else if (e.Result is ViewCache viewCache)
                {
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
                    BackImage = viewCache;
                    if (viewExtentChanged) OnPropertyChanged(nameof(ViewExtent));
                    if (viewBoundChanged) OnPropertyChanged(nameof(ViewBound));
                }
            }
        }
        private void BwDoWork(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker worker) || !(e.Argument is ViewCache viewCache) || Frame == null)
            {
                return;
            }
            Func<bool> cancelFunc = () =>
            {
                bool ret = worker.CancellationPending;
                if (ret && !e.Cancel)
                {
                    e.Cancel = true;
                }
                return ret;
            };
            #region 绘制BackBuffer
            using (Graphics g = Graphics.FromImage(viewCache.Image))
            {
                MapArgs mapArgs = new MapArgs(viewCache.Bound, viewCache.Extent, g, Frame.Projection, viewCache.DrawingExtent);
                using (GraphicsPath gp = new GraphicsPath())
                {
                    #region 设置绘制裁剪区域
                    if (!Equals(mapArgs.Extent, mapArgs.DestExtent))
                    {
                        var rect = mapArgs.ProjToPixelF(mapArgs.DestExtent);
                        gp.StartFigure();
                        gp.AddRectangle(rect);
                        g.Clip = new Region(gp);
                    }
                    #endregion

                    g.Clear(Background); //填充背景色
                    Action<string, int> progressAction = (txt, progress) =>
                    {
                        worker.ReportProgress(progress, txt);
                    };
                    int count = 2;
                    for (int i = 0; i < count; i++)
                    {
                        if (cancelFunc())
                        {
                            break;
                        }
                        bool selected = i == 1;
                        Action<string, int>? destProgressAction = selected ? null : progressAction;//不更新绘制选择要素的进度
                        Frame.Draw(mapArgs, selected, destProgressAction, cancelFunc);
                    }
                }
            }
            #endregion
            if (cancelFunc())//若取消绘制则释放图片
            {
                viewCache.Dispose();
            }
            else
            {
                e.Result = viewCache;
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

                    break;
            }
        }
        private double GetScale()
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

                // Get the number of pixels in one screen inch.
                // get resolution, most screens are 96 dpi, but you never know...
                double dpi = 96;
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    dpi = g.DpiX;
                }
                double dScreenWidthInMeters = (Convert.ToDouble(Width) / dpi) * metersPerInche;
                ret= widthInMeters / dScreenWidthInMeters;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return double.NaN;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    PropertyChanged -= View_PropertyChanged;
                    if (bw != null)
                    {
                        bw.Dispose();
                        bw = null;
                    }
                    if (BackImage != null)
                    {
                        BackImage.Dispose();
                        BackImage = null;
                    }
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
