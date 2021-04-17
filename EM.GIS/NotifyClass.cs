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
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="t">字段</param>
        /// <param name="value">值</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="autoDisposeOldValue">是否自动释放</param>
        /// <returns>成功返回true，反之false</returns>
        public bool SetProperty<T>(ref T t, T value, string propertyName, bool autoDisposeOldValue = false)
        {
            bool ret = SetProperty(ref t, value, autoDisposeOldValue);
            if (ret)
            {
                OnPropertyChanged(propertyName);
            }
            return ret;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">字段</param>
        /// <param name="value">值</param>
        /// <param name="autoDisposeOldValue">是否自动释放</param>
        /// <returns>成功返回true，反之false</returns>
        public bool SetProperty<T>(ref T t, T value, bool autoDisposeOldValue = false)
        {
            bool ret = false;
            if (!Equals(t, value))
            {
                var old = t;
                t = value;
                if (autoDisposeOldValue && old is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                ret = true;
            }
            return ret;
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
    }
}
