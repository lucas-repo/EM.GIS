using EM.GIS.Data;
using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;

namespace EM.GIS.Controls
{
    public class MapToolZoom : MapTool
    {
        #region Fields

        private Rectangle _client;
        private int _direction;
        private Point _dragStart;
        private bool _isDragging;
        private IFrame _mapFrame;
        private bool _preventDrag;
        private double _sensitivity;
        private Rectangle _source;
        private int _timerInterval;
        private Timer _zoomTimer;

        #endregion

        #region  Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapToolZoom"/> class.
        /// </summary>
        /// <param name="inMap">The map the tool should work on.</param>
        public MapToolZoom(IMap inMap)
            : base(inMap)
        {
            Configure();
            BusySet = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the map function is currently interacting with the map.
        /// </summary>
        public bool BusySet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether forward zooms in. This controls the sense (direction) of zoom (in or out) as you roll the mouse wheel.
        /// </summary>
        public bool ForwardZoomsIn
        {
            get
            {
                return _direction > 0;
            }

            set
            {
                _direction = value ? 1 : -1;
            }
        }

        /// <summary>
        /// Gets or sets the wheel zoom sensitivity. Increasing makes it more sensitive. Maximum is 0.5, Minimum is 0.01
        /// </summary>
        public double Sensitivity
        {
            get
            {
                return 1.0 / _sensitivity;
            }

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
        /// Gets or sets the full refresh timeout value in milliseconds
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

        /// <summary>
        /// Handles the actions that the tool controls during the OnMouseDown event
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseDown(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Middle && !_preventDrag)
            {
                _dragStart = e.Location;
                _source = e.Map.MapFrame.ViewBounds;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Handles the mouse move event, changing the viewing extents to match the movements
        /// of the mouse if the left mouse button is down.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseMove(GeoMouseArgs e)
        {
            if (_dragStart != Point.Empty && !_preventDrag)
            {
                if (!BusySet)
                {
                    Map.IsBusy = true;
                    BusySet = true;
                }

                _isDragging = true;
                Point diff = new Point
                {
                    X = _dragStart.X - e.X,
                    Y = _dragStart.Y - e.Y
                };
                e.Map.MapFrame.ViewBounds = new Rectangle(_source.X + diff.X, _source.Y + diff.Y, _source.Width, _source.Height);
                Map.Invalidate();
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Mouse Up
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseUp(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Middle && _isDragging)
            {
                _isDragging = false;
                _preventDrag = true;
                e.Map.MapFrame.ResetExtents();
                _preventDrag = false;
                Map.IsBusy = false;
                BusySet = false;
            }

            _dragStart = Point.Empty;

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Mouse Wheel
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseWheel(GeoMouseArgs e)
        {
            // Fix this
            _zoomTimer.Stop(); // if the timer was already started, stop it.
            Rectangle r = e.Map.MapFrame.ViewBounds;

            // For multiple zoom steps before redrawing, we actually
            // want the x coordinate relative to the screen, not
            // the x coordinate relative to the previously modified view.
            if (_client == Rectangle.Empty) _client = r;

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

            e.Map.MapFrame.ViewBounds = r;
            _zoomTimer.Start();
            _mapFrame = e.Map.MapFrame;
            if (!BusySet)
            {
                Map.IsBusy = true;
                BusySet = true;
            }

            base.OnMouseWheel(e);
        }

        private void Configure()
        {
            YieldStyle = YieldStyles.Scroll;
            _timerInterval = 100;
            _zoomTimer = new Timer
            {
                Interval = _timerInterval
            };
            _zoomTimer.Elapsed += ZoomTimer_Elapsed;
            _client = Rectangle.Empty;
            Sensitivity = 0.2;
            ForwardZoomsIn = true;
            Name = "ScrollZoom";
        }

        private void ZoomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _zoomTimer.Stop();
            if (_mapFrame == null) return;
            _client = Rectangle.Empty;
            _mapFrame.ResetExtents();
            Map.IsBusy = false;
            BusySet = false;
        }

        #endregion
    }
}
