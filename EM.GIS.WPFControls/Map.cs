using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Geometries;
using EM.GIS.Symbology;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 地图控件
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IMap))]
    public class Map : FrameworkElement, IMap
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
            PropertyChanged += Map_PropertyChanged;
            Frame = frame;
            Frame.View.PropertyChanged += View_PropertyChanged;
            Frame.View.UpdateMapAction = Invalidate;
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
        private (ViewCache? ViewCache, BitmapSource? BitmapSource, Rect Rect) imageCache = new(null, null, Rect.Empty);
        /// <summary>
        /// 使地图控件无效，以重新绘制
        /// </summary>
        /// <param name="rectangle">范围</param>
        private void Invalidate(Rectangle rectangle)
        {
            if (rectangle.IsEmpty)
            {
                return;
            }
            Dispatcher.Invoke(() =>
            {
                //var image = Frame.View.GetBitmap(rectangle);
                //ImageCache.Key = rectangle.ToRect();
                //var image = Frame.View.GetBitmap(Frame.View.Bound);
                //if (imageCache.ViewCache != Frame.View.BackImage && Frame.View.BackImage.Image is Bitmap bitmap&& bitmap.PixelFormat!= System.Drawing.Imaging.PixelFormat.DontCare)
                //{
                //    imageCache.ViewCache = Frame.View.BackImage;
                //    imageCache.BitmapSource = bitmap.ToBitmapSource(true);
                //    imageCache.Rect = Frame.View.Bound.ToRect();
                //    if (imageCache.BitmapSource != null)
                //    {
                //        InvalidateVisual();
                //    }
                //}
                //ImageCache.Key = Frame.View.Bound.ToRect();
                //ImageCache.Value = image.Bmp?.ToBitmapSource();
                InvalidateVisual();
            });
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    if (imageCache.BitmapSource != null)
        //    {
        //        //double offsetX = (ActualWidth - Frame.View.Width) / 2.0;
        //        //double offsetY = (ActualHeight - Frame.View.Height) / 2.0;
        //        double scaleX = Frame.View.ViewBound.Width / Frame.View.Width;
        //        double scaleY = Frame.View.ViewBound.Height / Frame.View.Height;
        //        if (scaleX != 1 || scaleY != 1)
        //        {
        //            var transform = new ScaleTransform(scaleX, scaleY);
        //            drawingContext.PushTransform(transform);
        //        }
        //        double offsetX = (ActualWidth - Frame.View.Width) / 2.0 + Frame.View.ViewBound.X / scaleX;
        //        double offsetY = (ActualHeight - Frame.View.Height) / 2.0 + Frame.View.ViewBound.Y / scaleY;
        //        if (offsetX != 0 || offsetY != 0)
        //        {
        //            var transform = new TranslateTransform(offsetX, offsetY);
        //            drawingContext.PushTransform(transform);
        //        }
        //        drawingContext.DrawImage(imageCache.BitmapSource, imageCache.Rect);
        //    }
        //    base.OnRender(drawingContext);
        //}
        private KeyValueClass<IExtent, BitmapSource?> bitmapCache = new KeyValueClass<IExtent, BitmapSource?>(null,null);
        private BitmapSource? GetBitmapSource()
        {
            BitmapSource? ret = null;
           var bitmap = Frame.View.GetBitmap();
            if (bitmap != null)
            {
                ret = bitmap.ToBitmapSource();
                bitmap.Dispose();
            }
            return ret;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            Stopwatch sw=new Stopwatch();
            sw.Start();
            base.OnRender(drawingContext);
            if (!Equals(bitmapCache.Key,Frame.View.ViewExtent))
            {
                bitmapCache.Key = Frame.View.ViewExtent;
                bitmapCache.Value = GetBitmapSource();
            }
            else
            {
                if (bitmapCache.Value == null)
                {
                    bitmapCache.Value = GetBitmapSource();
                }
            }
            //var bitmap = Frame.View.GetBitmap();
            //if (bitmap!=null)
            {
                //var bitmapSource = bitmap.ToBitmapSource();
                var bitmapSource = bitmapCache.Value;
                if (bitmapSource != null)
                {
                    double offsetX = (ActualWidth - Frame.View.Width) / 2.0;
                    double offsetY = (ActualHeight - Frame.View.Height) / 2.0;
                    if (offsetX != 0 || offsetY != 0)
                    {
                        var transform = new TranslateTransform(offsetX, offsetY);
                        drawingContext.PushTransform(transform);
                    }
                    //var rect1 = (Frame.View as View).GetSrcRectangleToView(Frame.View.Bound).ToRect();
               
                    Rect rect = Frame.View.GetDestRectangleToView(Frame.View.Bound).ToRect();
                    drawingContext.DrawImage(bitmapSource, rect);
                }
            }
            sw.Stop();
            Debug.WriteLine($"绘制背景图 {sw.ElapsedMilliseconds} 毫秒");
        }
        /// 使地图控件无效，以重新绘制
        /// </summary>
        private void Invalidate()
        {
            Invalidate(Frame.View.Bound);
        }

        private void Map_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(Background):
            //        break;
            //}
        }

        private void View_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(View.Background):
            //        Background = Frame.View.Background.ToBrush();
            //        break;
            //}
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
        private KeyValueClass<ViewCache?, BitmapSource?> catchBitmap = new KeyValueClass<ViewCache?, BitmapSource?>(null, null);

        /// <inheritdoc/>
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
        /// <inheritdoc/>
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

        ///// <inheritdoc/>
        //protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        //{
        //    if (e.Handled)
        //    {
        //        return;
        //    }
        //    var args = new GeoMouseButtonEventArgs(e, this);
        //    foreach (IMapTool tool in MapTools.Where(_ => _.IsActivated))
        //    {
        //        tool.DoMouseDoubleClick(args);
        //        if (args.Handled) break;
        //    }
        //    base.OnMouseDoubleClick(e);
        //}
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
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">释放托管资源</param>
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
