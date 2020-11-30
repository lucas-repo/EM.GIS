using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private object _lockObject = new object();
        public CancellationTokenSource CancellationTokenSource { get; set; }
        private int _width;
        public int Width => _width;
        private int _height;
        public int Height => _height;

        private Color _backGround = Color.Transparent;
        public Color BackGround
        {
            get { return _backGround; }
            set { _backGround = value; OnBackGroundChanged(); }
        }
        private Rectangle _viewBounds;
        public Rectangle ViewBounds
        {
            get { return _viewBounds; }
            set
            {
                if (_viewBounds == value)
                {
                    return;
                }
                _viewBounds = value;
                OnViewBoundsChanged();
            }
        }
        public event EventHandler BufferChanged;
        private void OnViewBoundsChanged()
        {
            OnBufferChanged();
        }

        private void OnBufferChanged()
        {
            BufferChanged?.Invoke(this, new EventArgs());
        }

        private Image _backBuffer;
        public Image BackBuffer
        {
            get { return _backBuffer; }
            set
            {
                if (_backBuffer == value)
                {
                    return;
                }
                lock (_lockObject)
                {
                    _backBuffer?.Dispose();
                    _backBuffer = value;
                }
                OnBackBufferChanged();
            }
        }

        private void OnBackBufferChanged()
        {
            _viewBounds = new Rectangle(0, 0, _width, _height);
            OnBufferChanged();
        }

        public virtual IExtent ViewExtents
        {
            get
            {
                return _viewExtents ?? (_viewExtents = Extent != null ? Extent.Copy() : new Extent(-180, -90, 180, 90));
            }

            set
            {
                if (value == null) return;
                IExtent ext = value.Copy();
                ResetAspectRatio(ext);
                _viewExtents = value;
                if (_extentChangedSuspensionCount == 0)
                {
                    OnExtentsChanged(_viewExtents);
                }
            }
        }

        private Rectangle _bounds;
        public Rectangle Bounds
        {
            get => _bounds;
            private set
            {
                if (_bounds != value)
                {
                    _bounds = value;
                }
            }
        }
        private int _isBusyIndex;
        public bool IsBusy
        {
            get
            {
                return _isBusyIndex > 0;
            }
            set
            {
                if (value) _isBusyIndex++;
                else _isBusyIndex--;
                if (_isBusyIndex <= 0)
                {
                    _isBusyIndex = 0;
                }
            }
        }
        public Frame(int width, int height)
        {
            _width = width;
            _height = height;
            _bounds = new Rectangle(0, 0, _width, _height);
            _viewBounds = new Rectangle(0, 0, _width, _height);
            DrawingLayers = new LayerCollection();
            Items.CollectionChanged += Layers_CollectionChanged;
        }

        protected void ResetAspectRatio(IExtent newEnv)
        {
            // Aspect Ratio Handling
            if (newEnv == null) return;

            // It isn't exactly an exception, but rather just an indication not to do anything here.
            if (_height == 0 || _width == 0) return;

            double controlAspect = (double)_width / _height;
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
                    ViewExtents = Extent;
                    return;
                }
            }
            ResetBuffer();
        }

        protected void OnViewExtentChanged()
        {
            ResetBuffer();
        }

        private void OnBackGroundChanged()
        {
            ResetBuffer();
        }

        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, CancellationTokenSource cancellationTokenSource = null)
        {
            base.OnDraw(graphics, rectangle, extent, selected, cancellationTokenSource);
            var visibleDrawingFeatureLayers = new List<IFeatureLayer>();
            if (DrawingLayers != null)
            {
                foreach (var item in DrawingLayers)
                {
                    if (CancellationTokenSource?.IsCancellationRequested == true)
                    {
                        break;
                    }
                    if (item.GetVisible(extent, rectangle))
                    {
                        item.Draw(graphics, rectangle, extent, selected, cancellationTokenSource);
                        if (item is IFeatureLayer featureLayer)
                        {
                            visibleDrawingFeatureLayers.Add(featureLayer);
                        }
                    }
                }
            }

            var featureLayers = GetFeatureLayers().Where(x=>x.GetVisible(extent, rectangle)).Union(visibleDrawingFeatureLayers);
            var labelLayers = featureLayers.Where(x => x.LabelLayer?.GetVisible(extent, rectangle) == true).Select(x => x.LabelLayer);
            foreach (var layer in labelLayers)
            {
                if (CancellationTokenSource?.IsCancellationRequested == true)
                {
                    break;
                }
                layer.Draw(graphics, rectangle, extent, selected, CancellationTokenSource);
            }
        }
        public Point ProjToBuffer(Coordinate location)
        {
            if (_width == 0 || _height == 0) return new Point(0, 0);
            int x = (int)((location.X - ViewExtents.MinX) * (_width / ViewExtents.Width)) + ViewBounds.X;
            int y = (int)((ViewExtents.MaxY - location.Y) * (_height / ViewExtents.Height)) + ViewBounds.Y;
            return new Point(x, y);
        }
        public Rectangle ProjToBuffer(IExtent extent)
        {
            Coordinate tl = new Coordinate(extent.MinX, extent.MaxY);
            Coordinate br = new Coordinate(extent.MaxX, extent.MinY);
            Point topLeft = ProjToBuffer(tl);
            Point bottomRight = ProjToBuffer(br);
            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }

        /// <summary>
        /// 根据指定范围重置缓存图片
        /// </summary>
        /// <param name="extent"></param>
        public async Task ResetBuffer(IExtent extent = null)
        {
            await Task.Run((Action)(() =>
            {
                if (extent == null)
                {
                    extent = ViewExtents;
                }
                Bitmap tmpBuffer = null;
                if (Width > 0 && Height > 0)
                {
                    tmpBuffer = new Bitmap(Width, Height);
                    #region 绘制MapFrame
                    Rectangle rectangle = ProjToBuffer(extent);
                    if (rectangle.Width * rectangle.Height != 0)
                    {
                        using (Graphics g = Graphics.FromImage(tmpBuffer))
                        {
                            using (Brush brush = new SolidBrush(BackGround))
                            {
                                g.FillRectangle(brush, rectangle);
                            }

                            int count = 2;
                            var visibleLayers = GetLayers().Where(x => x.GetVisible(extent, rectangle));
                            for (int i = 0; i < count; i++)
                            {
                                if (CancellationTokenSource?.IsCancellationRequested == true)
                                {
                                    break;
                                }
                                bool selected = i == 1;
                                Draw(g, rectangle, extent, selected, CancellationTokenSource);
                            }
                        }
                    }
                    #endregion
                }
                BackBuffer = tmpBuffer;
            }));
        }
        public void Draw(Graphics g, Rectangle rectangle)
        {
            if (BackBuffer != null && g != null)
            {
                g.DrawImage(BackBuffer, rectangle, Bounds, GraphicsUnit.Pixel);
            }
        }


        public void ResetExtents()
        {
            IExtent env = BufferToProj(ViewBounds);
            ViewExtents = env;
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
            double x = Convert.ToDouble(position.X);
            double y = Convert.ToDouble(position.Y);
            x = (x * ViewExtents.Width / _width) + ViewExtents.MinX;
            y = ViewExtents.MaxY - (y * ViewExtents.Height / _height);

            return new Coordinate(x, y, 0.0);
        }

        public void Resize(int width, int height)
        {
            var diff = new Point
            {
                X = width - _width,
                Y = height - _height
            };
            var newView = new Rectangle(ViewBounds.X, ViewBounds.Y, ViewBounds.Width + diff.X, ViewBounds.Height + diff.Y);

            // Check for minimal size of view.
            if (newView.Width < 5) newView.Width = 5;
            if (newView.Height < 5) newView.Height = 5;

            ViewBounds = newView;
            ResetExtents();

            _width = width;
            _height = height;
            Bounds = new Rectangle(0, 0, _width, _height);
        }

        public void ZoomToMaxExtent()
        {
            ViewExtents = GetMaxExtent(true);
        }
        public IExtent GetMaxExtent(bool expand = false)
        {
            // to prevent exception when zoom to map with one layer with one point
            const double Eps = 1e-7;
            var maxExtent = Extent.Width < Eps || Extent.Height < Eps ? new Extent(Extent.MinX - Eps, Extent.MinY - Eps, Extent.MaxX + Eps, Extent.MaxY + Eps) : Extent;
            if (expand) maxExtent.ExpandBy(maxExtent.Width / 10, maxExtent.Height / 10);
            return maxExtent;
        }
        private IExtent _viewExtents;
        private int _extentChangedSuspensionCount;
        public event EventHandler UpdateMap;
        public event EventHandler<ExtentArgs> ViewExtentsChanged;

        public bool ExtentsInitialized { get; set; }

        public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.AntiAlias;


        public ILayerCollection DrawingLayers { get; }

        /// <summary>
        /// Fires the ExtentsChanged event
        /// </summary>
        /// <param name="ext">The new extent.</param>
        protected virtual void OnExtentsChanged(IExtent ext)
        {
            if (_extentChangedSuspensionCount > 0) return;
            ViewExtentsChanged?.Invoke(this, new ExtentArgs(ext));
        }

    }
}
