using EM.WpfBase;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private string _fieldName;
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }
        private FieldType _fieldType;
        /// <summary>
        /// 字段类型
        /// </summary>
        public FieldType FieldType
        {
            get { return _fieldType; }
            set { SetProperty(ref _fieldType, value); }
        }

        public CheckableFieldInfo(FieldDefn fieldDefn)
        {
            if (fieldDefn==null)
            {
                throw new ArgumentNullException(nameof(fieldDefn));
            }
            _fieldName=fieldDefn.GetName();
            _fieldType=fieldDefn.GetFieldType();
        }

        public CheckableFieldInfo(string fieldName, FieldType fieldType)
        {
            _fieldName=fieldName;
            _fieldType=fieldType;
        }

        public override string ToString()
        {
            return FieldName;
        }
    }
}
