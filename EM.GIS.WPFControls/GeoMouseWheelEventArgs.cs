using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 鼠标滚轮事件参数
    /// </summary>
    public class GeoMouseWheelEventArgs : MouseWheelEventArgs, IGeoMouseEventArgs
    {
        #region  Constructors

        /// <summary>
        /// 初始化<seealso cref="GeoMouseWheelEventArgs"/>
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        /// <param name="map">地图</param>
        /// <exception cref="ArgumentNullException">参数为空时</exception>
        public GeoMouseWheelEventArgs(MouseWheelEventArgs e, Map map) : base(e.MouseDevice, e.Timestamp, e.Delta)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            var position = e.GetPosition(map);
            Location = new PointD(position.X, position.Y);
            GeographicLocation = map.View.PixelToProj(position.X, position.Y);
        }

        #endregion

        #region Properties
        /// <inheritdoc/>
        public ICoordinate GeographicLocation { get; }
        /// <inheritdoc/>
        public IMap Map { get; }
        /// <inheritdoc/>
        public PointD Location { get; }
        #endregion
    }
}
