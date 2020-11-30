using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    [Serializable]
    public class LineCartographicSymbol : LineSimpleSymbol, ILineCartographicSymbol
    {
        public LineJoinType JoinType { get; set; }
        public float Offset { get; set; }
        public float[] CompoundArray { get; set; }
        public LineCap StartCap { get; set; }
        public LineCap EndCap { get; set; }
        public float[] DashPattern { get; set; } 
        public List<ILineDecoration> Decorations { get; } = new List<ILineDecoration>();
       
        public LineCartographicSymbol() : this(LineSymbolType.Cartographic)
        { }
        protected LineCartographicSymbol(LineSymbolType lineSymbolType) : base(lineSymbolType)
        {
        }
        public override Pen ToPen(float scale)
        {
            Pen myPen = base.ToPen(scale);
            myPen.EndCap = EndCap;
            myPen.StartCap = StartCap;
            if (CompoundArray != null) myPen.CompoundArray = CompoundArray;
            if (Offset != 0F)
            {
                float[] pattern = { 0, 1 };
                float w = (float)Width;
                if (w == 0) w = 1;
                w = (float)(scale * w);
                float w2 = (Math.Abs(Offset) + (w / 2)) * 2;
                if (CompoundArray != null)
                {
                    pattern = new float[CompoundArray.Length];
                    for (int i = 0; i < CompoundArray.Length; i++)
                    {
                        pattern[i] = CompoundArray[i];
                    }
                }

                for (int i = 0; i < pattern.Length; i++)
                {
                    if (Offset > 0)
                    {
                        pattern[i] = (w / w2) * pattern[i];
                    }
                    else
                    {
                        pattern[i] = 1 - (w / w2) + ((w / w2) * pattern[i]);
                    }
                }

                myPen.CompoundArray = pattern;
                myPen.Width = w2;
            }

            if (DashPattern != null)
            {
                myPen.DashPattern = DashPattern;
            }
            else
            {
                if (myPen.DashStyle == DashStyle.Custom)
                {
                    myPen.DashStyle = DashStyle.Solid;
                }
            }

            switch (JoinType)
            {
                case LineJoinType.Bevel:
                    myPen.LineJoin = LineJoin.Bevel;
                    break;
                case LineJoinType.Mitre:
                    myPen.LineJoin = LineJoin.Miter;
                    break;
                case LineJoinType.Round:
                    myPen.LineJoin = LineJoin.Round;
                    break;
            }

            return myPen;
        }
    }
}
