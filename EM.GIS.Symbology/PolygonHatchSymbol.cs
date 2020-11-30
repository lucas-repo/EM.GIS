using System;
using System.Drawing;



namespace EM.GIS.Symbology
{
    //todo 暂未实现
    public class PolygonHatchSymbol : PolygonSymbol, IPolygonHatchSymbol
    {
        public PolygonHatchSymbol() : base(PolygonSymbolType.Hatch)
        {
            throw new NotImplementedException();
        }
        public override Brush GetBrush()
        {
            return base.GetBrush();
        }
    }
}
