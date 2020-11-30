using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    [Serializable]
    public class LineSimpleSymbol : LineSymbol, ILineSimpleSymbol
    {

        public LineSimpleSymbol() : this(LineSymbolType.Simple)
        { }
        protected LineSimpleSymbol(LineSymbolType lineSymbolType) : base(lineSymbolType)
        { }
        public LineSimpleSymbol(Color color) : base(color, LineSymbolType.Simple)
        {
        }
        public LineSimpleSymbol(Color color, float width) : base(color, width, LineSymbolType.Simple)
        {
        }
        public DashStyle DashStyle { get; set; }

        public override Pen ToPen(float scale)
        {
            float width = scale * Width;
            Pen pen  = new Pen(Color, width)
            {
                DashStyle = DashStyle,
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            if (DashStyle == DashStyle.Custom)
            {
                pen.DashPattern = new[] { 1F };
            }
            return pen;
        }
        protected override void OnRandomize(Random generator)
        {
            Color = generator.NextColor();
            Opacity = generator.NextFloat();
            Width = generator.NextFloat(10);
            DashStyle = generator.NextEnum<DashStyle>();
            base.OnRandomize(generator);
        }
    }
}