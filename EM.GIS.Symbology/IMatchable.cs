using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public interface IMatchable
    {
        bool Matches(IMatchable other, out List<string> mismatchedProperties);
    }
}