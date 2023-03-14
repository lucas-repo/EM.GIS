using EM.Bases;
using EM.GIS.Controls;
using EM.GIS.Geometries;
using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 选择工具
    /// </summary>
    public class MapToolSelect : MapTool
    {
        ICoordinate startCoord;
        public MapToolSelect(IMap map) : base(map)
        {
        }
        public override void DoMouseDown(GeoMouseButtonEventArgs e)
        {
            startCoord = e.GeographicLocation.Copy();
            base.DoMouseDown(e);
        }
        public override void DoMouseUp(GeoMouseButtonEventArgs e)
        {
            if (Map?.Frame != null)
            {
                Map.Frame.GetSelectedItems()
            }
            base.DoMouseUp(e);
        }
    }
}
