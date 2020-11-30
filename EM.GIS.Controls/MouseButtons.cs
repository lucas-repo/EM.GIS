using System;
using System.Runtime.InteropServices;

namespace EM.GIS.Controls
{
    [Flags]
    [ComVisible(true)]
    public enum MouseButtons
    {
        Left = 0x100000,
        None = 0x0,
        Right = 0x200000,
        Middle = 0x400000,
        XButton1 = 0x800000,
        XButton2 = 0x1000000
    }
}