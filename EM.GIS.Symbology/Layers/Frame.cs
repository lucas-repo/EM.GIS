using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架
    /// </summary>
    public class Frame : Group, IFrame
    {
        private int _extentChangedSuspensionCount;
        private bool _extentsChanged;
        private BackgroundWorker _bw;
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
        public CancellationTokenSource CancellationTokenSource { get; set; }
        /// <inheritdoc/>
        public int Width { get; private set; }
        /// <inheritdoc/>
        public int Height { get; private set; }

        private Color _backGround = Color.Transparent;
        /// <inheritdoc/>
        public Color BackGround
        {
            get { return _backGround; }
            set { SetProperty(ref _backGround, value); }
        }

        private Rectangle _viewBound;
        /// <inheritdoc/>
        public Rectangle ViewBound
        {
            get { return _viewBound; }
            set { SetProperty(ref _viewBound, value); }
        }
        private Image _backBuffer;
        /// <inheritdoc/>
        public Image BackBuffer
        {
            get { return _backBuffer; }
            set
            {
                lock (_lockObj)
                {
                    if (_backBuffer == value)
                    {
                        return;
                    }
                    _backBuffer?.Dispose();
                    _backBuffer = value;
                    //更改视图矩形
                    _viewBound = new Rectangle(0, 0, Width, Height);
                    OnPropertyChanged(nameof(BackBuffer));
                }
            }
        }

        private IExtent _viewExtents;
        /// <inheritdoc/>
        public virtual IExtent ViewExtent
        {
            get
            {
                return _viewExtents ?? (_viewExtents = Extent != null ? Extent.Copy() : new Extent(-180, -90, 180, 90));
            }

            set
            {
                if (_viewExtents == value || value == null) return;
                IExtent ext = value.Copy();
                ResetAspectRatio(ext);

                // 重新绘制背景图片
                SuspendExtentChanged();
                _viewExtents = ext;
                _extentsChanged = true;
                ResetBuffer();
                ResumeExtentChanged(); // fires the needed event.
            }
        }
        /// <inheritdoc/>
        public Rectangle Bound
        {
            get => new Rectangle(0, 0, Width, Height);
        }
        public Frame(int width, int height)
        {
            Width = width;
            Height = height;
            _viewBound = new Rectangle(0, 0, Width, Height);
            _bw = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _bw.DoWork += BwDoWork;
            _bw.RunWorkerCompleted += BwRunWorkerCompleted;
            _bw.ProgressChanged += BwProgressChanged;

            DrawingLayers = new LayerCollection(this, this);
            Children.CollectionChanged += Layers_CollectionChanged;
            PropertyChanged += Frame_PropertyChanged;
        }

        private void Frame_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BackGround):
                    ResetBuffer();
                    break;
            }
        }

        private void BwProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress?.Invoke(e.ProgressPercentage, "绘制中 ...");
        }
        private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sender is BackgroundWorker bw)
            {
                IsWorking = false;
                if (IsWorking)
                {
                    bw.RunWorkerAsync();
                    return;
                }
                Progress?.Invoke(0, string.Empty);
            }
        }
        private void BwDoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (ViewExtent == null || ViewExtent.IsEmpty() || Width <= 0 || Height <= 0 || CancellationPending())
                    {
                        return;
                    }
                    Image tmpBuffer = new Bitmap(Width, Height);
                    Action resetBufferAction = () =>
                    {
                        if (!CancellationPending())
                        {
                            if (BackBuffer != tmpBuffer)
                            {
                                BackBuffer = tmpBuffer;
                            }
                            else
                            {
                                OnPropertyChanged(nameof(BackBuffer));
                            }
                        }
                    };
                    #region 绘制BackBuffer
                    using (Graphics g = Graphics.FromImage(tmpBuffer))
                    {
                        g.Clear(BackGround); //填充背景色
                        int count = 2;
                        for (int i = 0; i < count; i++)
                        {
                            if (CancellationPending())
                            {
                                break;
                            }
                            bool selected = i == 1;
                            Draw(g, Bound, ViewExtent, selected, CancellationPending, resetBufferAction);
                        }
                    }
                    #endregion
                    resetBufferAction();
                    if (BackBuffer != tmpBuffer && CancellationPending())//若取消绘制则释放图片
                    {
                        tmpBuffer.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 是否取消绘制
        /// </summary>
        /// <returns></returns>
        private bool CancellationPending()
        {
            bool ret = CancellationTokenSource?.IsCancellationRequested == true || _bw.CancellationPending;
            return ret;
        }
        /// <summary>
        /// 根据当前长宽比，重设新范围的长宽比
        /// </summary>
        /// <param name="newEnv">新范围</param>
        protected void ResetAspectRatio(IExtent newEnv)
        {
            // Aspect Ratio Handling
            if (newEnv == null) return;

            // It isn't exactly an exception, but rather just an indication not to do anything here.
            if (Height == 0 || Width == 0) return;

            double controlAspect = (double)Width / Height;
            double envelopeAspect = newEnv.Width / newEnv.Height;
            var center = newEnv.Center;

            if (controlAspect > envelopeAspect)
            {
                // The Control is proportionally wider than the envelope to display.
                // If the envelope is proportionately wider than the control, "reveal" more width without
                // changing height If the envelope is proportionately taller than the control,
                // "hide" width without changing height
                newEnv.SetCenter(center, newEnv.Height * controlAspect, newEnv.Height);
            }
            else
            {
                // The control is proportionally taller than the content is
                // If the envelope is proportionately wider than the control,
                // "hide" the extra height without changing width
                // If the envelope is proportionately taller than the control, "reveal" more height without changing width
                newEnv.SetCenter(center, newEnv.Width, newEnv.Width / controlAspect);
            }

        }

        bool firstLayerAdded;
        private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!firstLayerAdded)
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                {
                    firstLayerAdded = true;
                }
                if (firstLayerAdded)
                {
                    var extent = Extent.Copy();
                    if (!extent.IsEmpty())
                    {
                        extent.ExpandBy(extent.Width / 10, extent.Height / 10);
                    }
                    ViewExtent = extent;
                    return;
                }
            }
            ResetBuffer();
        }

        protected void OnViewExtentChanged()
        {
            ResetBuffer();
        }

        public override void Draw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            base.Draw(graphics, rectangle, extent, selected, cancelFunc, invalidateMapFrameAction);
            var visibleDrawingFeatureLayers = new List<IFeatureLayer>();
            if (DrawingLayers != null)
            {
                foreach (ILayer item in DrawingLayers)
                {
                    if (cancelFunc?.Invoke() == true)
                    {
                        break;
                    }
                    if (item.GetVisible(extent, rectangle))
                    {
                        item.Draw(graphics, rectangle, extent, selected, cancelFunc);
                        if (item is IFeatureLayer featureLayer)
                        {
                            visibleDrawingFeatureLayers.Add(featureLayer);
                        }
                    }
                }
            }

            var featureLayers = GetFeatureLayers().Where(x => x.GetVisible(extent, rectangle)).Union(visibleDrawingFeatureLayers);
            var labelLayers = featureLayers.Where(x => x.LabelLayer?.GetVisible(extent, rectangle) == true).Select(x => x.LabelLayer);
            foreach (var layer in labelLayers)
            {
                if (CancellationTokenSource?.IsCancellationRequested == true)
                {
                    break;
                }
                layer.Draw(graphics, rectangle, extent, selected, cancelFunc);
            }
        }

        /// <inheritdoc/>
        public void ResetBuffer()
        {
            IsWorking = true;
            if (!_bw.IsBusy)
                _bw.RunWorkerAsync();
            else
                _bw.CancelAsync();
        }
        /// <inheritdoc/>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            if (BackBuffer != null && g != null)
            {
                Rectangle srcRectangle = GetRectangleToView(rectangle);
                try
                {
                    lock (_lockObj)
                    {
                        g.DrawImage(BackBuffer, rectangle, srcRectangle, GraphicsUnit.Pixel);
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
        public Rectangle GetRectangleToView(Rectangle rectangle)
        {
            Rectangle result = new Rectangle
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
            IExtent env = BufferToProj(ViewBound);
            ViewExtent = env;
        }
        public IExtent BufferToProj(Rectangle rect)
        {
            Point tl = new Point(rect.X, rect.Y);
            Point br = new Point(rect.Right, rect.Bottom);

            Coordinate topLeft = BufferToProj(tl);
            Coordinate bottomRight = BufferToProj(br);
            return new Extent(topLeft.X, bottomRight.Y, bottomRight.X, topLeft.Y);
        }
        public Coordinate BufferToProj(Point position)
        {
            Coordinate coordinate = null;
            if (ViewExtent != null)
            {
                double x = (position.X * ViewExtent.Width / Width) + ViewExtent.MinX;
                double y = ViewExtent.MaxY - (position.Y * ViewExtent.Height / Height);
                coordinate = new Coordinate(x, y, 0.0);
            }
            return coordinate;
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

            _viewBound = new Rectangle(ViewBound.X, ViewBound.Y, destWidth, destHeight);
            ResetViewExtent();

            Width = width;
            Height = height;
        }

        public void ZoomToMaxExtent()
        {
            var maxExtent = GetMaxExtent(true);
            if (maxExtent != null)
            {
                ViewExtent = maxExtent;
            }
        }
        public IExtent GetMaxExtent(bool expand = false)
        {
            // to prevent exception when zoom to map with one layer with one point
            IExtent maxExtent = null;
            if (Extent == null)
            {
                return maxExtent;
            }
            const double Eps = 1e-7;
            maxExtent = Extent.Width < Eps || Extent.Height < Eps ? new Extent(Extent.MinX - Eps, Extent.MinY - Eps, Extent.MaxX + Eps, Extent.MaxY + Eps) : Extent.Copy();
            if (expand) maxExtent.ExpandBy(maxExtent.Width / 10, maxExtent.Height / 10);
            return maxExtent;
        }
        /// <inheritdoc/>
        public event EventHandler<ExtentArgs> ViewExtentsChanged;

        //public bool ExtentsInitialized { get; set; }

        //public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.AntiAlias;
        /// <inheritdoc/>
        public ILayerCollection DrawingLayers { get; }

        /// <summary>
        /// 恢复ViewExtentsChanged
        /// </summary>
        protected void ResumeExtentChanged()
        {
            _extentChangedSuspensionCount--;
            if (_extentChangedSuspensionCount == 0)
            {
                if (_extentsChanged)
                {
                    OnViewExtentsChanged(_viewExtents);
                }
            }
        }

        /// <summary>
        /// 暂停触发ViewExtentsChanged
        /// </summary>
        protected void SuspendExtentChanged()
        {
            if (_extentChangedSuspensionCount == 0) _extentsChanged = false;
            _extentChangedSuspensionCount++;
        }
        /// <summary>
        /// 触发视图范围改变事件
        /// </summary>
        /// <param name="ext">范围</param>
        protected virtual void OnViewExtentsChanged(IExtent ext)
        {
            if (_extentChangedSuspensionCount > 0) return;
            ViewExtentsChanged?.Invoke(this, new ExtentArgs(ext));
        }
    }
}
