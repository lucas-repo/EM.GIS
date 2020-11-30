namespace EM.GIS.Data
{
    /// <summary>
    /// 获取字段定义
    /// </summary>
    public interface IGetFieldDefn
    {
        /// <summary>
        /// 字段数量
        /// </summary>
        int FieldCount { get; }
        /// <summary>
        /// 获取字段定义
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IFieldDefn GetFieldDefn(int index);
    }
}