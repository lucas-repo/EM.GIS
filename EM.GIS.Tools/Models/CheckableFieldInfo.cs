using EM.Bases;
using EM.GIS.GdalExtensions;
using OSGeo.OGR;
using System;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 字段信息
    /// </summary>
    public class CheckableFieldInfo:NotifyClass
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
        /// 字段名
        /// </summary>
        public string FieldName { get; }
        /// <summary>
        /// 字段定义
        /// </summary>
        public FieldDefn FieldDefn { get; }

        public CheckableFieldInfo(FieldDefn fieldDefn)
        {
            if (fieldDefn==null)
            {
                throw new ArgumentNullException(nameof(fieldDefn));
            }
            FieldDefn=fieldDefn;
            FieldName =fieldDefn.GetNameUTF8();
        }

        public override string ToString()
        {
            return FieldName;
        }
    }
}
