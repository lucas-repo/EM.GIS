using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Symbology;
using EM.IOC;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// Map.xaml 的交互逻辑
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IMap))]
    public partial class Map : UserControl, IMap
    {
        private IFrame _frame;
        /// <inheritdoc/>
        public IFrame Frame
        {
            get { return _frame; }
            set
            {
                if (_frame != value)
                {
                    _frame = value;
                }
            }
        }
        /// <summary>
        /// 地图视图
        /// </summary>
        public IView View => Frame.MapView;
        /// <inheritdoc/>
        public bool IsBusy { get; set; }
        private ILegend _legend;

        /// <inheritdoc/>
        public ILegend Legend
        {
            get { return _legend; }
            set
            {
                _legend = value;
                _legend?.AddMapFrame(Frame);
            }
        }

        /// <inheritdoc/>
        public IExtent Extent { get => Frame.Extent; }
        /// <inheritdoc/>
        public Rectangle Bound { get => View.Bound; }

        /// <inheritdoc/>
        public ILayerCollection Layers => Frame.Children;

        /// <inheritdoc/>
        public List<ITool> MapTools { get; }
        /// <inheritdoc/>
        public ProgressDelegate Progress
        {
            get => Frame.Progress;
            set => Frame.Progress = value;
        }

        /// <inheritdoc/>
        public event EventHandler<IGeoMouseEventArgs> GeoMouseMove;
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        public Map()
        {
            InitializeComponent(); 
            Frame = new Symbology.Frame((int)ActualWidth, (int)ActualHeight)
            {
                Text = "地图框"
            };
            Frame.PropertyChanged += MapFrame_PropertyChanged;
            Frame.Children.CollectionChanged += LegendItems_CollectionChanged;
            var pan = new MapToolPan(this);
            var zoom = new MapToolZoom(this);
            ITool[] mapTools = { pan, zoom };
            MapTools = new List<ITool>();
            MapTools.AddRange(mapTools);
            foreach (var mapTool in MapTools)
            {
                mapTool.Activated += MapTool_Activated;
            }
            ActivateMapToolWithZoom(pan);
        }

        private void MapFrame_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(View.BackBuffer):
                    Invalidate();
                    break;
                case nameof(View.ViewBound):
                    Invalidate();
                    break;
            }
        }

        private void MapTool_Activated(object? sender, EventArgs e)
        {
            if (sender is IMapTool mapTool)
            {
                if (mapTool.Cursor != null)
                {
                    Cursor = new Cursor(mapTool.Cursor);
                }
                else if (mapTool is MapToolPan)
                {
                    Cursor = Cursors.SizeAll;
                }
                else
                {
                    if (Cursor != Cursors.Arrow)
                    {
                        Cursor = Cursors.Arrow;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Invalidate(RectangleF rectangle)
        {
            Invalidate();
        }
        /// <inheritdoc/>
        public void Invalidate()
        {
            Action action = () => InvalidateVisual();
            Dispatcher.BeginInvoke(action);
        }
        /// <inheritdoc/>
        public void ZoomToMaxExtent()
        {
            View.ZoomToMaxExtent();
        }
        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (drawingContext != null && Bound.Width > 0 || Bound.Height > 0)
            {
                BitmapSource bitmapSource;
                using (Bitmap bmp = new(Bound.Width, Bound.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        View.Draw(g, Bound);
                    }
                    bitmapSource = bmp.ToBitmapImage();
                }
                var rect = Bound.ToRect();
                double offsetX = (ActualWidth - Bound.Width) / 2.0;
                double offsetY = (ActualHeight - Bound.Height) / 2.0;
                Transform transform = new TranslateTransform(offsetX, offsetY);
                drawingContext.PushTransform(transform);
                drawingContext.DrawImage(bitmapSource, rect);
            }
            base.OnRender(drawingContext);
        }
        private void Layer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ILegendItem.IsVisible):
                    View.ResetBuffer();
                    break;
            }
        }
        private void LegendItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ILegendItem item in e.NewItems)
                    {
                        AddLayerEvent(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ILegendItem item in e.OldItems)
                    {
                        RemoveLayerEvent(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (ILegendItem item in e.OldItems)
                    {
                        RemoveLayerEvent(item);
                    }
                    foreach (ILegendItem item in e.NewItems)
                    {
                        AddLayerEvent(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (ILegendItem item in e.OldItems)
                    {
                        RemoveLayerEvent(item);
                    }
                    break;

            }
        }

        private void AddLayerEvent(ILegendItem layer)
        {
            if (layer != null)
            {
                layer.PropertyChanged += Layer_PropertyChanged;
                layer.Children.CollectionChanged += LegendItems_CollectionChanged;
            }
        }

        private void RemoveLayerEvent(ILegendItem layer)
        {
            if (layer != null)
            {
                layer.PropertyChanged -= Layer_PropertyChanged;
                layer.Children.CollectionChanged -= LegendItems_CollectionChanged;
            }
        }

        public void ActivateMapToolWithZoom(ITool tool)
        {
            if (tool == null)
            {
                return;
            }
            if (!(tool is MapToolZoom))
            {
                var mapToolZoom = MapTools.FirstOrDefault(x => x is MapToolZoom);
                if (mapToolZoom != null)
                {
                    ActivateMapTool(mapToolZoom);
                }
            }
            ActivateMapTool(tool);
        }
        public void ActivateMapTool(ITool tool)
        {
            if (tool == null)
            {
                return;
            }
            if (!MapTools.Contains(tool))
            {
                MapTools.Add(tool);
            }

            foreach (var f in MapTools)
            {
                if ((f.MapToolMode & MapToolMode.AlwaysOn) == MapToolMode.AlwaysOn) continue;
                int test = (int)(f.MapToolMode & tool.MapToolMode);
                if (test > 0) f.Deactivate();
            }
            tool.Activate();
        }

        public void DeactivateAllMapTools()
        {
            foreach (var f in MapTools)
            {
                if ((f.MapToolMode & MapToolMode.AlwaysOn) != MapToolMode.AlwaysOn) f.Deactivate();
            }
        }
        /// <inheritdoc/>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (Frame != null && sizeInfo.NewSize.Width > 0 && sizeInfo.NewSize.Height > 0)
            {
                int width = (int)Math.Ceiling(sizeInfo.NewSize.Width);
                int height = (int)Math.Ceiling(sizeInfo.NewSize.Height);
                View.Resize(width, height);
            }
            base.OnRenderSizeChanged(sizeInfo);
        }
        #region 鼠标事件
        /// <inheritdoc/>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            var args = new GeoMouseButtonEventArgs(e, this);
            foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
            {
                tool.DoMouseDoubleClick(args);
                if (args.Handled) break;
            }
            base.OnMouseDoubleClick(e);
        }
        /// <inheritdoc/>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            var args = new GeoMouseButtonEventArgs(e, this);
            foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
            {
                tool.DoMouseDown(args);
                if (args.Handled) break;
            }
            base.OnMouseDown(e);
        }
        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            var args = new GeoMouseEventArgs(e, this);
            foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
            {
                tool.DoMouseMove(args);
                if (args.Handled) break;
            }
            GeoMouseMove?.Invoke(this, args);
            base.OnMouseMove(e);
        }
        /// <inheritdoc/>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            var args = new GeoMouseButtonEventArgs(e, this);
            foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
            {
                tool.DoMouseUp(args);
                if (args.Handled) break;
            }
            base.OnMouseUp(e);
        }
        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            var args = new GeoMouseWheelEventArgs(e, this);
            foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
            {
                tool.DoMouseWheel(args);
                if (args.Handled) break;
            }
            base.OnMouseWheel(e);
        }
        /// <inheritdoc/>
        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
            {
                tool.DoKeyUp(e);
                if (e.Handled) break;
            }
            base.OnKeyUp(e);//todo 待完成
        }
        /// <inheritdoc/>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
            {
                tool.DoKeyDown(e);
                if (e.Handled) break;
            }
            base.OnKeyDown(e);
        }

        /// <inheritdoc/>
        public IGroup AddGroup(string groupName )
        {
            return Layers.AddGroup(groupName);
        }

        #endregion
    }
}
