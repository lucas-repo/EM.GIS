﻿using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.IOC;
using System.Drawing;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 拖动地图工具
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ITool))]
    public class MapToolPan : MapTool
    {
        #region Fields

        private PointD _startPoint;
        private bool _preventDrag;
        private RectangleF _source;
        private bool _isDragging;

        #endregion

        #region  Constructors

        public MapToolPan()
        {
            MapToolMode = MapToolMode.LeftButton;
            Name = "拖动";
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void DoMouseDown(GeoMouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && !_preventDrag)
            {
                _startPoint = e.Location;
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

                var dx = _startPoint.X - e.Location.X;
                var dy = _startPoint.Y - e.Location.Y;
                e.Map.Frame.View.ViewBound = new RectangleF((float)(_source.X + dx), (float)(_source.Y + dy), _source.Width, _source.Height);
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
            _startPoint.X = double.NaN;
            _startPoint.Y = double.NaN;
            _source = RectangleF.Empty;
            base.DoMouseUp(e);
        }

        #endregion
    }
}
