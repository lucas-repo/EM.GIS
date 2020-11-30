using EM.GIS.Data;
using EM.GIS.Geometries;
using OSGeo.OGR;
using System;
using System.Collections.Generic;

namespace EM.GIS.Gdals
{
    internal class GdalFeature : IFeature
    {
        private bool disposedValue;

        public Feature Feature { get; set; }
        public IGeometry Geometry
        {
            get
            {
                IGeometry geometry = Feature?.GetGeometryRef().ToGeometry();
                return geometry;
            }
            set
            {
                if (Feature != null && value is Geometry geometry)
                {
                    Feature.SetGeometry(geometry.OgrGeometry);
                }
            }
        }
        public Dictionary<string, object> Attribute
        {
            get
            {
                Dictionary<string, object> attribute = new Dictionary<string, object>();
                if (Feature != null)
                {
                    var count = Feature.GetFieldCount();
                    for (int i = 0; i < count; i++)
                    {
                        using var fieldDefn = Feature.GetFieldDefnRef(i);
                        object value = Feature.GetFieldValue(i);
                        attribute.Add(fieldDefn.GetName(), value);
                    }
                }
                return attribute;
            }
            set
            {
                if (Feature != null && value != null)
                {
                    foreach (var item in value)
                    {
                        using var fieldDefn = Feature.GetFieldDefnRef(item.Key);
                        Feature.SetField(fieldDefn, item.Value);
                    }
                }
            }
        }

        public long FId
        {
            get
            {
                long value = -1;
                if (Feature != null)
                {
                    value = Feature.GetFID();
                }
                return value;
            }
            set
            {
                if (Feature != null)
                {
                    var ret = Feature.SetFID(value);
                }
            }
        }

        public int FieldCount
        {
            get
            {
                int value = -1;
                if (Feature != null)
                {
                    value = Feature.GetFieldCount();
                }
                return value;
            }
        }

        public GdalFeature(Feature feature)
        {
            Feature = feature;
        }

        public object Clone()
        {
            return new GdalFeature(Feature.Clone());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Feature != null)
                    {
                        Feature.Dispose();
                        Feature = null;
                    }
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~GdalFeature()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public override bool Equals(object obj)
        {
            bool ret = false;
            if (obj is GdalFeature gdalFeature)
            {
                ret = Feature == gdalFeature.Feature;
            }
            return ret;
        }

        public override int GetHashCode()
        {
            int hashCode = "GdalFeature".GetHashCode() ^ Feature.GetHashCode();
            return hashCode;
        }

        public IField GetField(int index)
        {
            IField value = null;
            if (Feature != null && index >= 0 && index < FieldCount)
            {
                value = new GdalField(Feature, index);
            }
            return value;
        }

        public IField GetField(string name)
        {
            IField value = null;
            if (Feature != null && !string.IsNullOrEmpty(name))
            {
                var fieldCount = Feature.GetFieldCount();
                for (int i = 0; i < fieldCount; i++)
                {
                    var fieldDefn = Feature.GetFieldDefnRef(i);
                    if (fieldDefn.GetName() == name)
                    {
                        value = new GdalField(Feature, i);
                        break;
                    }
                }
            }
            return value;
        }
    }
}