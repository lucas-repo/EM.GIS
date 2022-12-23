using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 视图
    /// </summary>
    public class View:BaseCopy,IView
    {
        private readonly object _lockObj=new object();
        private int _extentChangedSuspensionCount;
        private bool _extentsChanged;

        private BackgroundWorker _bw;
        private int _busyCount;


        /// <inheritdoc/>
        public IFrame Frame { get; }
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

        private IExtent _extent;
        private bool disposedValue;

        /// <inheritdoc/>
        public virtual IExtent Extent
        {
            get
            {
                if (_extent == null)
                {
                    _extent = Frame?.Extent != null ? Frame.Extent.Copy() : new Extent(-180, -90, 180, 90);
                }
                return _extent;
            }
            set
            {
                if (_extent == value || value == null) return;
                IExtent ext = value.Copy();
               ExtentExtensions.ResetAspectRatio(ext,Width,Height);

                // 重新绘制背景图片
                SuspendExtentChanged();
                _extent = ext;
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

        /// <inheritdoc/>
        public Action<string, int> Progress { get; set; }
        public View(IFrame frame,int width, int height)
        {
            Frame = frame??throw new ArgumentNullException(nameof(frame));
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
            PropertyChanged += View_PropertyChanged;
        }
        private void View_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BackGround):
                    ResetBuffer();
                    break;
            }
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
                    if (Extent == null || Extent.IsEmpty() || Width <= 0 || Height <= 0 || CancellationPending())
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
                            Frame.Draw(g,Frame.Projection, Bound, Extent, selected,Progress, CancellationPending, resetBufferAction);
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
        /// 恢复ViewExtentsChanged
        /// </summary>
        protected void ResumeExtentChanged()
        {
            _extentChangedSuspensionCount--;
            if (_extentChangedSuspensionCount == 0)
            {
                if (_extentsChanged)
                {
                    if (_extentChangedSuspensionCount > 0) return;
                    OnPropertyChanged(nameof(Extent));
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
            Extent = env;
        }
        public IExtent BufferToProj(Rectangle rect)
        {
            Point tl = new Point(rect.X, rect.Y);
            Point br = new Point(rect.Right, rect.Bottom);

            var topLeft = BufferToProj(tl);
            var bottomRight = BufferToProj(br);
            return new Extent(topLeft.X, bottomRight.Y, bottomRight.X, topLeft.Y);
        }
        public ICoordinate BufferToProj(Point position)
        {
            ICoordinate coordinate = null;
            if (Extent != null)
            {
                double x = (position.X * Extent.Width / Width) + Extent.MinX;
                double y = Extent.MaxY - (position.Y * Extent.Height / Height);
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
            var maxExtent =Frame.GetMaxExtent(true);
            if (maxExtent != null)
            {
                Extent = maxExtent;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    if (_bw != null)
                    {
                        _bw.Dispose();
                        _bw=null;
                    }
                    if (BackBuffer != null)
                    {
                        BackBuffer.Dispose();
                        BackBuffer=null;
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
