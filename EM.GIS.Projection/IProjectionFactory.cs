using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Projections
{
    /// <summary>
    /// 投影工厂接口
    /// </summary>
    public interface  IProjectionFactory
    {
        /// <summary>
        /// 根据epsg创建投影
        /// </summary>
        /// <param name="epsg">投影编号</param>
        /// <returns>投影</returns>
        IProjection GetProjection(int epsg);
    }
}
