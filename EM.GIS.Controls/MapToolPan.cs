using EM.GIS.Controls.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 拖动地图工具
    /// </summary>
    public class MapToolPan : MapTool
    {
        #region Fields

        private Point _dragStart;
        private bool _preventDrag;
        private Rectangle _source;

        #endregion

        #region  Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapToolPan"/> class.
        /// </summary>
        /// <param name="inMap">The map the tool should work on.</param>
        public MapToolPan(IMap inMap)
            : base(inMap)
        {
            MapToolMode = MapToolMode.LeftButton;
            BusySet = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the map function is currently interacting with the map.
        /// </summary>
        public bool BusySet { get; set; }

        /// <summary>
        /// Gets a value indicating whether this tool is currently being used.
        /// </summary>
        public bool IsDragging { get; private set; }

        #endregion

        #region Methods

        public override void DoMouseDown(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Left && _preventDrag == false)
            {
                _dragStart = e.Location;
                _source = e.Map.MapFrame.ViewBounds;
                IsDragging = true;
            }

            base.DoMouseDown(e);
        }

        public override void DoMouseMove(GeoMouseArgs e)
        {
            if (IsDragging)
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
            if (IsDragging)
            {
                IsDragging = false;

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

        #endregion
    }
}
