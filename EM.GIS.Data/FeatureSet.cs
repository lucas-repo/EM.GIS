using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 要素集
    /// </summary>
    public abstract class FeatureSet : DataSet, IFeatureSet
    {
        public FeatureType FeatureType { get;protected set; }
        public abstract int FeatureCount { get; }
        public abstract int FieldCount { get; }

        public abstract IFeature AddFeature(IGeometry geometry);
        public abstract IFeature AddFeature(IGeometry geometry, Dictionary<string, object> attribute);
        public abstract IFeature GetFeature(int index);
        public abstract IEnumerable<IFeature> GetFeatures();
        public abstract IFieldDefn GetFieldDefn(int index);
        public abstract IGeometry GetSpatialFilter();
        public abstract bool RemoveFeature(int index);
        public abstract void SetAttributeFilter(string expression);
        public abstract void SetSpatialExtentFilter(IExtent extent);
        public abstract void SetSpatialFilter(IGeometry geometry);
    }
}
