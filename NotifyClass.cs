using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS
{
    /// <summary>
    /// 可通知的类
    /// </summary>
    [Serializable]
    public class NotifyClass : INotifyPropertyChanged
    {
        /// <summary>
        /// 设置值并调用属性改变通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        public void SetProperty<T>(ref T t, T value, string propertyName)
        {
            if (!Equals(t, value))
            {
                t = value;
                OnPropertyChanged(propertyName);
            }
        }
        private Type _type;
        private Type Type
        {
            get
            {
                if (_type == null)
                {
                    _type = GetType();
                }
                return _type;
            }
        }
        /// <summary>
        /// 属性改变事件
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变方法
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        protected bool SetField<T>(string fieldName, T value)
        {
            bool ret = false;
            if (string.IsNullOrEmpty(fieldName))
            {
                return ret;
            }
            var field = Type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var oldValue = field.GetValue(this);
                if (!Equals(oldValue, value))
                {
                    try
                    {
                        field.SetValue(this, value);
                        ret = true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="fieldName">字段名称</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        protected bool SetProperty<T>(string fieldName, string propertyName, T value)
        {
            bool ret = false;
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(propertyName))
            {
                return ret;
            }
            if (SetField(fieldName, value))
            {
                OnPropertyChanged(propertyName);
                ret = true;
            }
            return ret;
        }
    }
}
