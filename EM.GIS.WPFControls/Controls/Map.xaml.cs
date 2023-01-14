using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Symbology;
using EM.IOC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private bool disposedValue;
        private IFrame frame;
        /// <inheritdoc/>
        public IFrame Frame
        {
            get { return frame; }
            set
            {
                if (frame != value)
                {
                    var oldFrame = frame;
                    frame = value;
                    if (oldFrame != null)
                    {
                        oldFrame.Dispose();
                    }
                    OnPropertyChanged(nameof(Frame));
                }
            }
        }

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
        public IRenderableItemCollection Layers => Frame.Children;

        /// <inheritdoc/>
        public List<ITool> MapTools { get; }

        /// <inheritdoc/>
        public event EventHandler<IGeoMouseEventArgs> GeoMouseMove;
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        public Map(IFrame frame)
        {
            InitializeComponent();
            PropertyChanged += Map_PropertyChanged;
            Frame = frame;
            Frame.View.PropertyChanged += View_PropertyChanged;
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

        private void Map_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Background):
                    break;
            }
        }

        private void View_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(View.Background):
                    Background = Frame.View.Background.ToBrush();
                    break;
                case nameof(View.BackImage):
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
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (drawingContext != null && Frame.View.Width > 0 && Frame.View.Height > 0)
            {
                BitmapSource bitmapSource;
                Rectangle bound = new Rectangle(0, 0, Frame.View.Width, Frame.View.Height);
                using (Bitmap bmp = new(Frame.View.Width, Frame.View.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Frame.View.Draw(g, bound);
                    }
                    bitmapSource = bmp.ToBitmapImage();
                }
                var rect = bound.ToRect();
                double offsetX = (ActualWidth - bound.Width) / 2.0;
                double offsetY = (ActualHeight - bound.Height) / 2.0;
                Transform transform = new TranslateTransform(offsetX, offsetY);
                drawingContext.PushTransform(transform);
                drawingContext.DrawImage(bitmapSource, rect);
            }
            base.OnRender(drawingContext);
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
        /// <inheritdoc/>
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
                Frame.View.Resize((int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Frame.View.PropertyChanged -= View_PropertyChanged;
                    // TODO: 释放托管状态(托管对象)
                    if (Frame != null)
                    {
                        Frame.Dispose();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Map()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        /// <inheritdoc/>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
        /// <summary>
        /// 设置值并通知属性改变
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="t">字段</param>
        /// <param name="value">值</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>成功为true</returns>
        public bool SetProperty<T>(ref T t, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(t, value))
            {
                t = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
        private void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
