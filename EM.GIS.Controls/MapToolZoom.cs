using EM.GIS.Data;
using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 缩放地图工具
    /// </summary>
    public class MapToolZoom : MapTool
    {
        #region Fields

        private int _direction;
        private Point _dragStart;
        private bool _isDragging;
        private bool _preventDrag;
        private double _sensitivity;
        private Rectangle _source;
        private int _timerInterval;
        private Timer _zoomTimer;

        #endregion

        #region  Constructors

        public MapToolZoom(IMap map)
            : base(map)
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

        public override void DoMouseDown(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Middle && !_preventDrag)
            {
                _dragStart = e.Location;
                _source = e.Map.MapFrame.ViewBounds;
                _isDragging = true;
            }

            base.DoMouseDown(e);
        }

        public override void DoMouseMove(GeoMouseArgs e)
        {
            if (_isDragging)
            {
                if (!BusySet)
                {
                    Map.IsBusy = true;
                    BusySet = true;
                }

                var dx = _dragStart.X - e.X;
                var dy = _dragStart.Y - e.Y;
                e.Map.MapFrame.ViewBounds = new Rectangle(_source.X + dx, _source.Y + dy, _source.Width, _source.Height);
            }

            base.DoMouseMove(e);
        }

        public override void DoMouseUp(GeoMouseArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _preventDrag = true;
                e.Map.MapFrame.ResetExtents();
                _preventDrag = false;
                Map.IsBusy = false;
                BusySet = false;
            }

            _dragStart = Point.Empty;
            _source = Rectangle.Empty;
            base.DoMouseUp(e);
        }

        public override void DoMouseWheel(GeoMouseArgs e)
        {
            // Fix this
            _zoomTimer.Stop(); // if the timer was already started, stop it.
            Rectangle r = e.Map.MapFrame.ViewBounds;

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
                double destX = e.X * ratioWidth + (1 - ratioWidth) * srcCenterX;
                double destY = e.Y * ratioHeight + (1 - ratioHeight) * srcCenterY;
                int xOff = Convert.ToInt32(e.X - destX);
                int yOff = Convert.ToInt32(e.Y - destY);
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
                double destX = e.X * ratioWidth + (1 - ratioWidth) * srcCenterX;
                double destY = e.Y * ratioHeight + (1 - ratioHeight) * srcCenterY;
                int xOff = Convert.ToInt32(e.X - destX);
                int yOff = Convert.ToInt32(e.Y - destY);
                r.X += xOff;
                r.Y += yOff;
            }

            Map.MapFrame.ViewBounds = r;
            _zoomTimer.Start();
            if (!BusySet)
            {
                Map.IsBusy = true;
                BusySet = true;
            }

            base.DoMouseWheel(e);
        }

        private void Configure()
        {
            MapToolMode = MapToolMode.Middle;
            _timerInterval = 100;
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
            Map.MapFrame.ResetExtents();
            Map.IsBusy = false;
            BusySet = false;
        }

        #endregion
    }
}
