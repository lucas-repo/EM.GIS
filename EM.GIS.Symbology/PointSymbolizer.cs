using System;
using System.Collections.Generic;
using System.Drawing;



namespace EM.GIS.Symbology
{
    public class PointSymbolizer : FeatureSymbolizer, IPointSymbolizer
    {
        public SizeF Size
        {
            get
            {
                SizeF size = new SizeF();
                foreach (var symbol in Symbols)
                {
                    SizeF bsize = symbol.Size;
                    size.Width = Math.Max(size.Width, bsize.Width);
                    size.Height = Math.Max(size.Height, bsize.Height);
                }
                return size;
            }
            set
            {
                SizeF oldSize = Size;
                float dX = value.Width / oldSize.Width;
                float dY = value.Height / oldSize.Height;
                foreach (var symbol in Symbols)
                {
                    var os = symbol.Size;
                    symbol.Size = new SizeF(os.Width * dX, os.Height * dY);
                }
            }
        }

        public new IPointSymbolCollection Symbols { get => base.Symbols as IPointSymbolCollection; set => base.Symbols = value; }

        public PointSymbolizer()
        {
            Configure();
        }
        public PointSymbolizer(bool selected) : this()
        {
            if (!selected) return;

            IPointSymbol pointSymbol = Symbols[0];
            if (pointSymbol != null)
            {
                pointSymbol.Color = Color.Cyan;
            }
        }
        public PointSymbolizer(IPointSymbol symbol) : this()
        {
            Symbols.Add(symbol);
        }
        public PointSymbolizer(IEnumerable<IPointSymbol> symbols) : this()
        {
            foreach (var item in symbols)
            {
                Symbols.Add(item);
            }
        }

        public PointSymbolizer(Color color, PointShape shape, float size) : this()
        {
            IPointSymbol ss = new PointSimpleSymbol(color, shape, size);
            Symbols.Add(ss);
        }

        private void Configure()
        {
            Symbols = new PointSymbolCollection();
            IPointSimpleSymbol ss = new PointSimpleSymbol
            {
                Color = SymbologyGlobal.RandomColor(),
                Opacity = 1F,
                PointShape = PointShape.Rectangle
            };
            Symbols.Add(ss);
        }

        public override void DrawLegend(Graphics context, Rectangle rectangle)
        {
            float scaleH = rectangle.Width / Size.Width;
            float scaleV = rectangle.Height / Size.Height;
            float scale = Math.Min(scaleH, scaleV);
            float dx = rectangle.X + (rectangle.Width / 2);
            float dy = rectangle.Y + (rectangle.Height / 2);
            PointF point = new PointF(dx, dy);
            DrawPoint(context, scale, point);
        }

        public void DrawPoint(Graphics context, float scale, PointF point)
        {
            foreach (var symbol in Symbols)
            {
                symbol.DrawPoint(context, scale, point);
            }
        }
    }
}