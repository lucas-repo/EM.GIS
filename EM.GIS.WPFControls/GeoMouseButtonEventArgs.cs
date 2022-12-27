using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 带几何鼠标事件参数
    /// </summary>
    public class GeoMouseButtonEventArgs: MouseButtonEventArgs, IGeoMouseEventArgs
    {
        #region  Constructors
        /// <summary>
        /// 初始化<seealso cref="GeoMouseButtonEventArgs"/>
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        /// <param name="map">地图</param>
        /// <exception cref="ArgumentNullException">参数为空时</exception>
        public GeoMouseButtonEventArgs(MouseButtonEventArgs e, Map map) : base(e.MouseDevice, e.Timestamp,e.ChangedButton)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            var position = e.GetPosition(map);
            Location = new Coordinate(position.X, position.Y);
            GeographicLocation = map.View.PixelToProj(position.X, position.Y);
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public ICoordinate GeographicLocation { get; }
        /// <inheritdoc/>
        public IMap Map { get; }
        /// <inheritdoc/>
        public ICoordinate Location { get; }
        #endregion
    }
}
