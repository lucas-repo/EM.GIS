using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 带几何鼠标事件参数
    /// </summary>
    public class GeoMouseButtonEventArgs: MouseButtonEventArgs, IGeoMouseEventArgs
    {
        #region  Constructors

        public GeoMouseButtonEventArgs(MouseButtonEventArgs e, Map map) : base(e.MouseDevice, e.Timestamp,e.ChangedButton)
        {
            if (map == null) return;

            var position = e.GetPosition(map);
            Location = new Point((int)position.X, (int)position.Y);
            GeographicLocation = map.PointFToCoordinate(Location);
            Map = map;
        }

        #endregion

        #region Properties
        public ICoordinate GeographicLocation { get; }

        public IMap Map { get; }

        public Point Location { get; }

        #endregion
    }
}
