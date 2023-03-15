using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Symbology;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 缩放地图工具
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IMapTool))]
    public class MapToolZoom : MapTool
    {
        #region Fields

        private int _direction;
        private PointD _dragStart;
        private bool _isDragging;
        private bool _preventDrag;
        private double _sensitivity;
        private RectangleF _source;
        private int _timerInterval;
        private Timer _zoomTimer;

        #endregion

        #region  Constructors

        public MapToolZoom()
        {
            Configure();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 滚轮前滚是否为放大
        /// </summary>
        public bool ForwardZoomsIn
        {
            get => _direction > 0;
            set => _direction = value ? 1 : -1;
        }

        /// <summary>
        /// 获取或设置滚轮缩放灵敏度，最大值是0.5，最小值是0.01
        /// </summary>
        public double Sensitivity
        {
            get => 1.0 / _sensitivity;
            set
            {
                if (value > 0.5)
                    value = 0.5;
                else if (value < 0.01)
                    value = 0.01;
                _sensitivity = 1.0 / value;
            }
        }

        /// <summary>
        /// 获取或设置以毫秒为单位的完整刷新超时值（默认100毫秒）
        /// </summary>
        public int TimerInterval
        {
            get
            {
                return _timerInterval;
            }

            set
            {
                _timerInterval = value;
                _zoomTimer.Interval = _timerInterval;
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void DoMouseDown(GeoMouseButtonEventArgs e)
        {
            if (e.MiddleButton ==  System.Windows.Input.MouseButtonState.Pressed && !_preventDrag&&!_isZooming)
            {
                _dragStart = e.Location;
                _source = e.Map.Frame.View.ViewBound;
                _isDragging = true;
            }

            base.DoMouseDown(e);
        }

        /// <inheritdoc/>
        public override void DoMouseMove(GeoMouseEventArgs e)
        {
            if (_isDragging)
            {
                if (!BusySet)
                {
                    e.Map.IsBusy = true;
                    BusySet = true;
                }

                var dx = _dragStart.X - e.Location.X;
                var dy = _dragStart.Y - e.Location.Y;
                e.Map.Frame.View.ViewBound = new RectangleF((float)(_source.X + dx), (float)((_source.Y + dy)), _source.Width, _source.Height);
            }

            base.DoMouseMove(e);
        }

        /// <inheritdoc/>
        public override void DoMouseUp(GeoMouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _preventDrag = true;
                e.Map.Frame.View.ResetViewExtent();
                _preventDrag = false;
                e.Map.IsBusy = false;
                BusySet = false;
            }

            _dragStart = PointD.Empty;
            _source = Rectangle.Empty;
            base.DoMouseUp(e);
        }
        private bool _isZooming;
        public override void DoMouseWheel(GeoMouseWheelEventArgs e)
        {
            _isZooming = true;
            // Fix this
            _zoomTimer.Stop(); // if the timer was already started, stop it.
            var r = e.Map.Frame.View.ViewBound;

            // For multiple zoom steps before redrawing, we actually
            // want the x coordinate relative to the screen, not
            // the x coordinate relative to the previously modified view.

            double w = r.Width;
            double h = r.Height;
            double srcCenterX = r.X + r.Width / 2.0;
            double srcCenterY = r.Y + r.Height / 2.0;
            if (_direction * e.Delta > 0)
            {
                double ratio = Sensitivity / 2;
                double dHalfWidth = -w * ratio;
                double dHalfHeight = -h * ratio;
                r.Inflate(Convert.ToInt32(dHalfWidth), Convert.ToInt32(dHalfHeight));
                double ratioWidth = r.Width / w;
                double ratioHeight = r.Height / h;
                double destX = e.Location.X * ratioWidth + (1 - ratioWidth) * srcCenterX;
                double destY = e.Location.Y * ratioHeight + (1 - ratioHeight) * srcCenterY;
                int xOff = Convert.ToInt32(e.Location.X - destX);
                int yOff = Convert.ToInt32(e.Location.Y - destY);
                r.X += xOff;
                r.Y += yOff;
            }
            else
            {
                double ratio = Sensitivity / (2 * (1 - Sensitivity));
                double dHalfWidth = w * ratio;
                double dHalfHeight = h * ratio;
                r.Inflate(Convert.ToInt32(dHalfWidth), Convert.ToInt32(dHalfHeight));
                double ratioWidth = r.Width / w;
                double ratioHeight = r.Height / h;
                double destX = e.Location.X * ratioWidth + (1 - ratioWidth) * srcCenterX;
                double destY = e.Location.Y * ratioHeight + (1 - ratioHeight) * srcCenterY;
                int xOff = Convert.ToInt32(e.Location.X - destX);
                int yOff = Convert.ToInt32(e.Location.Y - destY);
                r.X += xOff;
                r.Y += yOff;
            }

            e.Map.Frame.View.ViewBound = r;
            _zoomTimer.Start();
            if (!BusySet)
            {
                e.Map.IsBusy = true;
                BusySet = true;
            }

            base.DoMouseWheel(e);
        }

        private void Configure()
        {
            MapToolMode = MapToolMode.Middle;
            _timerInterval = 500;
            _zoomTimer = new Timer
            {
                Interval = _timerInterval
            };
            _zoomTimer.Elapsed += ZoomTimer_Elapsed;
            Sensitivity = 0.2;
            ForwardZoomsIn = true;
            Name = "缩放";
        }

        private void ZoomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _zoomTimer.Stop();
            Map.Frame.View.ResetViewExtent();
            Map.IsBusy = false;
            BusySet = false;
            _isZooming = false;
        }

        #endregion
    }
}
