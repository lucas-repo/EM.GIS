using EM.GIS.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    public abstract class MapTool:Tool, IMapTool
    {
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<GeoMouseButtonEventArgs> MouseDoubleClick;
        public event EventHandler<GeoMouseButtonEventArgs> MouseDown;
        public event EventHandler<GeoMouseEventArgs> MouseMove;
        public event EventHandler<GeoMouseButtonEventArgs> MouseUp;
        public event EventHandler<GeoMouseWheelEventArgs> MouseWheel;
        public MapTool(IMap map) : base(map)
        { }
        public virtual void DoKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        public virtual void DoKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        public virtual void DoMouseDoubleClick(GeoMouseButtonEventArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }

        public virtual void DoMouseDown(GeoMouseButtonEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        public virtual void DoMouseMove(GeoMouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        public virtual void DoMouseUp(GeoMouseButtonEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        public virtual void DoMouseWheel(GeoMouseWheelEventArgs e)
        {
            MouseWheel?.Invoke(this, e);
        }

    }
}
