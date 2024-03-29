﻿using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public class PointSymbolCollection :FeatureSymbolCollection, IPointSymbolCollection
    {
        public PointSymbolCollection(IPointSymbolizer parent):base(parent)
        {
        }

        public new IPointSymbol this[int index] { get => base[index] as IPointSymbol; set => base[index] = value; }

        public new IPointSymbolizer Parent { get => base.Parent as IPointSymbolizer; set => base.Parent = value; }

    }
}