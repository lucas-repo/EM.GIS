using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Controls
{
    public interface IMapTool
    {
        event EventHandler FunctionActivated;
        event EventHandler FunctionDeactivated; 
        Image Icon { get; set; }
        Image Cursor { get; set; }
        bool Enabled { get; }
        IMap Map { get; set; }
        string Name { get; set; }
        YieldStyles  YieldStyle { get; set; }
        void Activate(); 
        void Deactivate();

        void DoKeyDown(KeyEventArgs e);

        void DoKeyUp(KeyEventArgs e);

        void DoMouseDoubleClick(GeoMouseArgs e);

        void DoMouseDown(GeoMouseArgs e);

        void DoMouseMove(GeoMouseArgs e);

        void DoMouseUp(GeoMouseArgs e);

        void DoMouseWheel(GeoMouseArgs e);

        void Draw(MapDrawArgs e);

    }
}
