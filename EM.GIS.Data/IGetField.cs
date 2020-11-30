namespace EM.GIS.Data
{
    /// <summary>
    /// 获取字段值
    /// </summary>
    public interface IGetField
    {
        /// <summary>
        /// 字段数量
        /// </summary>
        int FieldCount { get; }
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IField GetField(int index);
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IField GetField(string name);
    }
}