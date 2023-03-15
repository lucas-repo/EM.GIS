using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Geometries;
using EM.IOC;
using System;
using System.Drawing;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 选择工具
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IMapTool))]
    public class MapToolSelect : MapTool
    {
        ICoordinate? startCoord;
        PointF startPoint;
        RectangleF lastRect;
        public override void DoMouseDown(GeoMouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                startCoord = e.GeographicLocation.Copy();
                startPoint = new PointF((float)e.Location.X, (float)e.Location.Y);
            }
            base.DoMouseDown(e);
        }
        public override void DoMouseMove(GeoMouseEventArgs e)
        {
            if (startCoord != null)
            {

                RectangleF invalidateRect=
                if (!lastRect.IsEmpty)
                {
                    lastRect.
                    e.Map.Invalidate(lastRect)
                }
            }
            base.DoMouseMove(e);
        }
        public override void DoMouseUp(GeoMouseButtonEventArgs e)
        {
            if (startCoord == null|| e.Map.Frame == null)
            {
                return;
            }
            double minX = Math.Min(startCoord.X, e.GeographicLocation.X);
            double maxX = Math.Max(startCoord.X, e.GeographicLocation.X);
            double minY = Math.Min(startCoord.Y, e.GeographicLocation.Y);
            double maxY = Math.Max(startCoord.Y, e.GeographicLocation.Y);
            IExtent extent=new  Extent(minX, minY, maxX,  maxY);
            e.Map.Frame.Select(extent, extent, Symbology.SelectionMode.Intersects, out _, Symbology.ClearStates.Force);
            startCoord = null;
            base.DoMouseUp(e);
        }
    }
}
