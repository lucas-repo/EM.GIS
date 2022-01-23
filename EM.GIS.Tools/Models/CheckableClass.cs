using EM.WpfBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 可选择的类
    /// </summary>
    public class CheckableClass<T> : NotifyClass
    {
        private bool _isChecked;
        /// <summary>
        /// 是否已选择
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }
        /// <summary>
        /// 对象
        /// </summary>
        public T Obj { get; }
        public CheckableClass(T t)
        {
            Obj = t;
        }
        public CheckableClass(bool isChecked, T t) : this(t)
        {
            _isChecked=isChecked;
        }
    }
}
