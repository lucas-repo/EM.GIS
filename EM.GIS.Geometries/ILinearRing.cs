namespace EM.GIS.Geometries
{
    /// <summary>
    /// 线性环接口
    /// </summary>
    public interface ILinearRing : ILineString
    {
        /// <summary>
        /// 是否是闭环
        /// </summary>
        bool IsRing { get;  }
        /// <summary>
        /// 闭合环
        /// </summary>
        void CloseRings();
    }
}