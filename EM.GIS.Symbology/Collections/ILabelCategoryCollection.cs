using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public interface ILabelCategoryCollection:ICategoryCollection
    {
        new ILabelCategory this[int index] { get; set; }
        new ILabelScheme Parent { get; set; }
        new IEnumerator<ILabelCategory> GetEnumerator();
    }
}