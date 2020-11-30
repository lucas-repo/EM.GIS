using System;

namespace EM.GIS.Data
{
    /// <summary>
    /// 字段定义
    /// </summary>
    public interface IFieldDefn
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        FieldType FieldType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        int Length { get; set; }
        /// <summary>
        /// 精度
        /// </summary>
        int Precision { get; set; }
        /// <summary>
        /// 获取或设置获取要素时是否忽略此字段
        /// </summary>
        bool IsIgnored { get; set; }
        /// <summary>
        /// 获取或设置此字段是否可为空
        /// </summary>
        bool IsNullable { get; set; }
    }
}