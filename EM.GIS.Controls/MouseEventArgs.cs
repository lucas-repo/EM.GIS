using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EM.GIS.Controls
{
    [ComVisible(true)]
    public class MouseEventArgs : EventArgs
    {
        private readonly MouseButtons button;

        private readonly int clicks;

        private readonly int x;

        private readonly int y;

        private readonly int delta;

        public MouseButtons Button => button;

        public int Clicks => clicks;

        public int X => x;

        public int Y => y;

        public int Delta => delta;

        public Point Location => new Point(x, y);

        public MouseEventArgs(MouseButtons button, int clicks, int x, int y) : this(button, clicks, x, y, 0)
        {
        }
        public MouseEventArgs(int x, int y, int delta):this( MouseButtons.None,0,x,y,delta)
        {
        }
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
        }
    }
}