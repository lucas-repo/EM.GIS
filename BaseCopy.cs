using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS
{
    /// <summary>
    /// 可拷贝基类
    /// </summary>
    [Serializable]
    public abstract class BaseCopy : NotifyClass, ICloneable
    {
        #region Methods
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            object copy = MemberwiseClone();
            OnCopy(copy);
            return copy;
        }

        #endregion


        /// <summary>
        /// 将当前对象拷贝到目标对象
        /// </summary>
        /// <param name="copy">目标对象</param>
        protected virtual void OnCopy(object copy)
        {
            this.CopyTo(copy);
        }
        /// <summary>
        /// 拷贝属性
        /// </summary>
        /// <param name="source"></param>
        public void CopyProperties(object source)
        {
            source.CopyTo(this);
        }
    }
}
