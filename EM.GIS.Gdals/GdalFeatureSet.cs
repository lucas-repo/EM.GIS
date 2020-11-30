using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projection;
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
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                }
                _dataSource = value;
            }
        }
        public Layer Layer => DataSource.GetLayerByIndex(0);
        public FeatureDefn FeatureDefn => Layer.GetLayerDefn();
        public override IExtent Extent => Layer.GetExtent();
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
                            DataSource = Ogr.Open(value, 1);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                            DataSource = Ogr.Open(value, 0);
                        }
                    }
                    else
                    {
                        DataSource = null;
                    }
                }
            }
        }
        public override ProjectionInfo Projection
        {
            get
            {
                SpatialReference spatialReference = null;
                if (Layer != null)
                {
                    spatialReference = Layer.GetSpatialRef();
                }
                if (base.Projection == null)
                {
                    base.Projection = new GdalProjectionInfo(spatialReference);
                }
                else if (base.Projection is GdalProjectionInfo gdalProjectionInfo)
                {
                    if (!gdalProjectionInfo.SpatialReference.Equals(spatialReference))//待测试
                    {
                        gdalProjectionInfo.SpatialReference = spatialReference;
                    }
                }
                return base.Projection; ;
            }
        }
        public override int FeatureCount => (int)Layer.GetFeatureCount(1);

        public override int FieldCount => FeatureDefn.GetFieldCount();

        public GdalFeatureSet(string filename, DataSource dataSource)
        {
            _ignoreChangeDataSource = true;
            Filename = filename;
            _dataSource = dataSource;
        }
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (DataSource != null)
                    {
                        DataSource.Dispose();
                        DataSource = null;
                    }
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

        public override bool RemoveFeature(int index)
        {
            return Layer.DeleteFeature(index) == 1;
        }

        public override IEnumerable<IFeature> GetFeatures()
        {
            var feature = Layer.GetNextFeature()?.ToFeature();
            while (feature != null)
            {
                yield return feature;
                feature = Layer.GetNextFeature()?.ToFeature();
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


        public override IGeometry GetSpatialFilter()
        {
            return Layer.GetSpatialFilter()?.ToGeometry();
        }

        public override void SetAttributeFilter(string expression)
        {
            if (Layer != null)
            {
                var ret = Layer.SetAttributeFilter(expression);
            }
        }

        public override void SetSpatialExtentFilter(IExtent extent)
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
            if (Layer != null)
            {
                Layer.SetSpatialFilter(geometry?.ToGeometry());
            }
        }
    }
}
