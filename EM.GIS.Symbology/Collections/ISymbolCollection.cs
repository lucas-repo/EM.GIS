using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public interface ISymbolCollection: ILegendItemCollection
    {
        #region 需要重写的部分
        new ISymbol this[int index] { get; set; }
        new ISymbolizer Parent { get; set; }
        #endregion
    }
}