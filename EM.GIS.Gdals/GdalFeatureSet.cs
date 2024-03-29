﻿using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using OSGeo.OGR;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EM.GIS.Gdals
{
    public class GdalFeatureSet : FeatureSet
    {
        private DataSource _dataSource;
        /// <summary>
        /// 数据源
        /// </summary>
        public DataSource DataSource
        {
            get { return _dataSource; }
            private set
            {
                SetProperty(ref _dataSource, value);
            }
        }
        private Layer? _layer;
        /// <summary>
        /// 要素图层
        /// </summary>
        public Layer? Layer
        {
            get { return _layer; }
            set
            {
                if (SetProperty(ref _layer, value))
                {
                    OnLayerChanged();
                }
            }
        }

        private void OnLayerChanged()
        {
            FeatureType featureType = FeatureType.Unknown;
            IExtent extent = null;
            GdalProjection projection = null;
            if (Layer != null)
            {
                featureType = Layer.GetGeomType().ToFeatureType();
                extent = Layer.GetExtent();
                SpatialReference spatialReference = Layer.GetSpatialRef();
                projection = new GdalProjection(spatialReference)
                {
                    SpatialReferenceDisposable = false
                };
            }
            FeatureType = featureType;
            Extent = extent;
            Projection = projection;
        }

        protected FeatureDefn FeatureDefn => Layer?.GetLayerDefn();
        private bool _ignoreChangeDataSource;
        public override string RelativeFilename
        {
            get => base.RelativeFilename;
            protected set
            {
                base.RelativeFilename = value;
                if (!_ignoreChangeDataSource)
                {
                    if (File.Exists(value))
                    {
                        try
                        {
                            DataSource = Ogr.Open(value, 0);
                            Layer = DataSource.GetLayerByIndex(0);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }
                    }
                    else
                    {
                        DataSource = null;
                    }
                }
            }
        }

        public override long FeatureCount => Layer.GetFeatureCount(1);

        public override int FieldCount => FeatureDefn.GetFieldCount();

        public GdalFeatureSet(string filename, DataSource dataSource, Layer? layer)
        {
            _ignoreChangeDataSource = true;
            Filename = filename;
            Name = Path.GetFileNameWithoutExtension(filename);
            _dataSource = dataSource;
            Layer = layer;
        }
        /// <summary>
        /// 初始化内存要素集
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="featureType">要素类型</param>
        public GdalFeatureSet(string name,FeatureType featureType)
        {
            _ignoreChangeDataSource = true;
            using var driver = Ogr.GetDriverByName("Memory");
            if (driver == null)
            {
                throw new Exception("创建内存要素集失败");
            }
            var geometryType = featureType.ToWkbGeometryType();
            if (geometryType == wkbGeometryType.wkbUnknown)
            {
                throw new Exception($"不支持的类型 {featureType}");
            }
            _dataSource = driver.CreateDataSource(name,null);
            _layer = _dataSource.CreateLayer(name, null, geometryType, null);
            Name = name;
            OnLayerChanged();
        }
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                    _dataSource = null;
                }
            }
            base.Dispose(disposing);
        }
        public override void Save()
        {
            DataSource.FlushCache();
        }
        public override void SaveAs(string filename, bool overwrite)
        {
            if (string.IsNullOrEmpty(filename) || (File.Exists(filename) && !overwrite))
            {
                return;
            }
            using var driver = DataSource.GetDriver();
            using var ds = driver.CopyDataSource(DataSource, filename, null);
        }
        public override IFeature AddFeature(IGeometry geometry)
        {
            IFeature destFeature = AddFeature(geometry, null);
            return destFeature;
        }

        public override IFeature AddFeature(IGeometry geometry, Dictionary<string, object> attribute)
        {
            IFeature destFeature = null;
            if (geometry != null)
            {
                return destFeature;
            }
            var fieldCount = FeatureDefn.GetFieldCount();
            FeatureDefn featureDefn = new FeatureDefn(null);
            var destAttribute = new Dictionary<FieldDefn, object>();
            for (int i = 0; i < fieldCount; i++)
            {
                var fieldDefn = FeatureDefn.GetFieldDefn(i);
                featureDefn.AddFieldDefn(fieldDefn);
                var fieldName = fieldDefn.GetName();
                if (attribute.ContainsKey(fieldName))
                {
                    destAttribute.Add(fieldDefn, attribute[fieldName]);
                }
                else
                {
                    fieldDefn.Dispose();
                }
            }
            using GeomFieldDefn geomFieldDefn = new GeomFieldDefn(null, FeatureDefn.GetGeomType());
            featureDefn.AddGeomFieldDefn(geomFieldDefn);
            Feature feature = new Feature(featureDefn);
            foreach (var item in destAttribute)
            {
                var fieldDefn = item.Key;
                feature.SetField(fieldDefn, item.Value);
                fieldDefn.Dispose();
            }
            var ret = Layer.CreateFeature(feature);
            if (ret == 1)
            {
                destFeature = new GdalFeature(feature);
            }
            return destFeature;
        }

        public override IFeature GetFeature(int index)
        {
            var feature = Layer.GetFeature(index)?.ToFeature();
            return feature;
        }

        /// <inheritdoc/>
        public override bool RemoveFeature(int index)
        {
            return Layer.DeleteFeature(index) == 1;
        }
        /// <inheritdoc/>
        public override IEnumerable<IFeature> GetFeatures()
        {
            var ogrFeature = Layer.GetNextFeature();
            while (ogrFeature != null)
            {
                var feature = ogrFeature.ToFeature();
                if (feature != null)
                {
                    yield return feature;
                }
                else
                {
                    Debug.WriteLine($"获取第 {ogrFeature.GetFID()} 个要素失败！");
                }
                ogrFeature = Layer.GetNextFeature();
            }
        }

        public override IFieldDefn GetFieldDefn(int index)
        {
            IFieldDefn destFieldDfn = null;
            var fieldDefn = FeatureDefn.GetFieldDefn(index);
            if (fieldDefn != null)
            {
                destFieldDfn = new GdalFieldDfn(fieldDefn);
            }
            return destFieldDfn;
        }


        /// <inheritdoc/>
        public override IGeometry? GetSpatialFilter()
        {
            return Layer.GetSpatialFilter()?.ToGeometry();
        }

        /// <inheritdoc/>
        public override void SetAttributeFilter(string expression)
        {
            if (Layer != null)
            {
                var ret = Layer.SetAttributeFilter(expression);
            }
        }

        /// <inheritdoc/>
        public override void SetSpatialExtentFilter(IExtent? extent)
        {
            if (Layer != null)
            {
                if (extent != null)
                {
                    Layer.SetSpatialFilterRect(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
                }
                else
                {
                    Layer.SetSpatialFilter(null);
                }
            }
        }

        public override void SetSpatialFilter(IGeometry geometry)
        {
            if (Layer != null && geometry != null)
            {
                Layer.SetSpatialFilter(geometry.ToGeometry());
            }
        }
    }
}
