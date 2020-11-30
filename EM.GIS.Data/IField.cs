using System;

namespace EM.GIS.Data
{
    /// <summary>
    /// 字段接口
    /// </summary>
    public interface IField
    {
        /// <summary>
        /// 字段定义
        /// </summary>
        IFieldDefn FieldDfn { get;  }

        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsNull { get; set; }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <returns></returns>
        string GetValueAsString();
        /// <summary>
        /// 获取整数值
        /// </summary>
        /// <returns></returns>
        int GetValueAsInteger();
        /// <summary>
        /// 获取长整型值
        /// </summary>
        /// <returns></returns>
        long GetValueAsLong();
        /// <summary>
        /// 获取双精度值
        /// </summary>
        /// <returns></returns>
        double GetValueAsDouble();
        /// <summary>
        /// 获取时间值
        /// </summary>
        /// <returns></returns>
        DateTime GetValueAsDateTime();
        /// <summary>
        /// 获取整数数组
        /// </summary>
        /// <returns></returns>
        int[] GetValueAsIntegerList();
        /// <summary>
        /// 获取字符串数组
        /// </summary>
        /// <returns></returns>
        string[] GetValueAsStringList();

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(string value);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(long value);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(int value);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(double value);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(DateTime value);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(int[] value);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(double[] value);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(string[] value);
    }
}