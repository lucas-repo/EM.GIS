using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 矢量数据接口
    /// </summary>
    public interface IVectorDriver : IFileDriver
    {
        new IFeatureSet Open(string fileName, bool update);
    }
}
