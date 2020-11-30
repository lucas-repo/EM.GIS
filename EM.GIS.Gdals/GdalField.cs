using EM.GIS.Data;
using OSGeo.OGR;
using System;

namespace EM.GIS.Gdals
{
    internal class GdalField : IField
    {
        private Feature _feature;
        private int _index;
        private IFieldDefn _fieldDfn;
        public IFieldDefn FieldDfn
        {
            get
            {
                if (_fieldDfn == null)
                {
                    var fieldDefn = _feature?.GetFieldDefnRef(_index);
                    if (fieldDefn != null)
                    {
                        _fieldDfn = new GdalFieldDfn(fieldDefn);
                    }
                }
                return _fieldDfn;
            }
        }
        public bool IsNull
        {
            get
            {
                var value = false;
                if (_feature != null)
                {
                    value = _feature.IsFieldNull(_index);
                }
                return value;
            }
            set
            {
                if (_feature != null)
                {
                    _feature.SetFieldNull(_index);
                }
            }
        }

        public GdalField(Feature feature, int index)
        {
            _feature = feature;
            _index = index;
        }

        public string GetValueAsString()
        {
            var value = string.Empty;
            if (_feature != null)
            {
                value = _feature.GetFieldAsString(_index);
            }
            return value;
        }

        public int GetValueAsInteger()
        {
            var value = 0;
            if (_feature != null)
            {
                value = _feature.GetFieldAsInteger(_index);
            }
            return value;
        }

        public long GetValueAsLong()
        {
            var value = 0L;
            if (_feature != null)
            {
                value = _feature.GetFieldAsInteger64(_index);
            }
            return value;
        }

        public double GetValueAsDouble()
        {
            var value = 0.0;
            if (_feature != null)
            {
                value = _feature.GetFieldAsDouble(_index);
            }
            return value;
        }

        public DateTime GetValueAsDateTime()
        {
            var value = DateTime.MinValue;
            if (_feature != null)
            {
                _feature.GetFieldAsDateTime(_index, out int pnYear, out int pnMonth, out int pnDay, out int pnHour, out int pnMinute, out float pfSecond, out int pnTZFlag);
                value = new DateTime(pnYear, pnMonth, pnDay, pnHour, pnMinute, (int)pfSecond);
            }
            return value;
        }

        public int[] GetValueAsIntegerList()
        {
            int[] value = null;
            if (_feature != null)
            {
                value = _feature.GetFieldAsIntegerList(_index,out int count);
            }
            return value;
        }

        public string[] GetValueAsStringList()
        {
            string[] value = null;
            if (_feature != null)
            {
                value = _feature.GetFieldAsStringList(_index);
            }
            return value;
        }

        public void SetValue(string value)
        {
            _feature?.SetField(_index, value);
        }

        public void SetValue(long value)
        {
            _feature?.SetField(_index, value);
        }

        public void SetValue(int value)
        {
            _feature?.SetField(_index, value);
        }

        public void SetValue(double value)
        {
            _feature?.SetField(_index, value);
        }

        public void SetValue(DateTime value)
        {
            _feature?.SetField(_index,value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second,0);
        }

        public void SetValue(int[] value)
        {
            if (value != null)
            {
                _feature?.SetFieldIntegerList(_index, value.Length, value);
            }
        }

        public void SetValue(double[] value)
        {
            if (value != null)
            {
                _feature?.SetFieldDoubleList(_index, value.Length, value);
            }
        }

        public void SetValue(string[] value)
        {
            _feature?.SetFieldStringList(_index, value);
        }
    }
}