﻿using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    internal class PolygonSymbolizer : FeatureSymbolizer, IPolygonSymbolizer
    {
        public new IPolygonSymbolCollection Symbols { get => base.Symbols as IPolygonSymbolCollection; set => base.Symbols = value; }
      
        public PolygonSymbolizer()
        {
            Symbols = new PolygonSymbolCollection(this);
            PolygonSimpleSymbol polygonSymbol = new PolygonSimpleSymbol();
            Symbols.Add(polygonSymbol);
        }

        public PolygonSymbolizer(bool selected) 
        {
            Symbols = new PolygonSymbolCollection(this);
            IPolygonSymbol polygonSymbol = new PolygonSimpleSymbol();
            if (selected)
            {
                polygonSymbol.Color = Color.Cyan;
            }
            Symbols.Add(polygonSymbol);
        }
        public override void DrawLegend(Graphics context, Rectangle rectangle)
        {
            PointF[] points = new PointF[]
            {
                new PointF(rectangle.Left,rectangle.Top),
                new PointF(rectangle.Left,rectangle.Bottom),
                new PointF(rectangle.Right,rectangle.Bottom),
                new PointF(rectangle.Right,rectangle.Top),
                new PointF(rectangle.Left,rectangle.Top)
            };
            DrawPolygon(context, 1, points.ToPath());
        }
        public void DrawPolygon(Graphics context, float scale, GraphicsPath path)
        {
            for (int i = Symbols.Count - 1; i >= 0; i--)
            {
                Symbols[i].DrawPolygon(context, scale, path);
            }
        }
    }
}