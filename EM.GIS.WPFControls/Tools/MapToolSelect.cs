using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.IOC;
using System;
using System.Drawing;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 选择工具
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ITool))]
    public class MapToolSelect : MapTool
    {
        ICoordinate? startCoord;
        PointF startPoint;
        RectangleF lastRect;
        public MapToolSelect()
        {
            MapToolMode = MapToolMode.LeftButton;
            Name = "选择";
        }
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
            InvalidateRect(e);
            base.DoMouseMove(e);
        }
        public override void DoDraw(MapEventArgs e)
        {
            if (startCoord != null)
            {
                e.Device.DrawRectangle(Pens.Blue, lastRect);
            }
            base.DoDraw(e);
        }
        public override void DoMouseUp(GeoMouseButtonEventArgs e)
        {
            if (startCoord == null || e.Map.Frame == null)
            {
                return;
            }
            var minX = Math.Min(startCoord.X, e.GeographicLocation.X);
            var maxX = Math.Max(startCoord.X, e.GeographicLocation.X);
            var minY = Math.Min(startCoord.Y, e.GeographicLocation.Y);
            var maxY = Math.Max(startCoord.Y, e.GeographicLocation.Y);
            IExtent extent = new Extent(minX, minY, maxX, maxY);
            e.Map.Frame.Select(extent, extent, Symbology.SelectionMode.Intersects, out _, Symbology.ClearStates.Force);
            //InvalidateRect(e);
            Map.Frame.View.ResetBuffer(Map.Frame.View.Bound, Map.Frame.View.ViewExtent, Map.Frame.View.ViewExtent);
            startCoord = null;
            lastRect = RectangleF.Empty;
            base.DoMouseUp(e);
        }
        private void InvalidateRect(IGeoMouseEventArgs e)
        {
            if (startCoord != null)
            {
                var currentPoint = new PointF((float)e.Location.X, (float)e.Location.Y);
                var left = Math.Min(startPoint.X, currentPoint.X);
                var right = Math.Max(startPoint.X, currentPoint.X);
                var top = Math.Min(startPoint.Y, currentPoint.Y);
                var bottom = Math.Max(startPoint.Y, currentPoint.Y);
                RectangleF rect = RectangleF.FromLTRB(left, top, right, bottom);
                RectangleF invalidateRect = lastRect.Union(rect);
                lastRect = rect;
                e.Map.Invalidate(invalidateRect);
            }
        }
    }
}
