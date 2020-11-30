using System;

namespace EM.GIS.Symbology
{
    public interface IDescriptor : IRandomizable, ICloneable, IMatchable
    {
        void CopyProperties(object other);
    }
}