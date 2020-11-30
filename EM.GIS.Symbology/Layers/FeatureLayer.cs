using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素图层
    /// </summary>
    public abstract class FeatureLayer : Layer, IFeatureLayer
    {
        public override IExtent Extent
        {
            get => DataSet.Extent;
        }
        public new IFeatureCategory DefaultCategory { get => base.DefaultCategory as IFeatureCategory; set => base.DefaultCategory = value; }
        public FeatureLayer(IFeatureSet featureSet)
        {
            DataSet = featureSet;
            Selection = new FeatureSelection(DataSet);
        }

        public ILabelLayer LabelLayer { get; set; }
        public new IFeatureSet DataSet { get => base.DataSet as IFeatureSet; set => base.DataSet = value; }

        public new IFeatureCategoryCollection Categories => Items as IFeatureCategoryCollection;

        public new IFeatureSelection Selection
        {
            get => base.Selection as IFeatureSelection;
            set => base.Selection = value;
        }

        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, CancellationTokenSource cancellationTokenSource = null)
        {
            var polygon = extent.ToPolygon();
            DataSet.SetSpatialFilter(polygon);
            var features = new List<IFeature>();
            long featureCount = DataSet.FeatureCount;
            long drawnFeatureCount = 0;
            int threshold = 65536;
            int totalPointCount = 0;
            int percent;
            Action drawFeatuesAction = new Action(() =>
            {
                if (features.Count > 0)
                {
                    percent = (int)(drawnFeatureCount * 100 / featureCount);
                    ProgressHandler?.Progress(percent, "绘制要素中...");
                    MapArgs drawArgs = new MapArgs(rectangle, extent, graphics);
                    DrawFeatures(drawArgs, features, selected, ProgressHandler, cancellationTokenSource);
                    drawnFeatureCount += features.Count;
                    foreach (var item in features)
                    {
                        item.Dispose();
                    }
                    features.Clear();
                }
                totalPointCount = 0;
            });
            foreach (var feature in DataSet.GetFeatures())
            {
                features.Add(feature);
                int pointCount = feature.Geometry.PointCount;
                totalPointCount += pointCount;
                if (totalPointCount >= threshold)
                {
                    drawFeatuesAction();
                }
            }
            if (totalPointCount > 0)
            {
                drawFeatuesAction();
            }
            DataSet.SetSpatialFilter(null);
            ProgressHandler?.Progress(100, "绘制要素中...");
        }
        private Dictionary<IFeature, IFeatureCategory> GetFeatureAndCategoryDic(List<IFeature> features)
        {
            Dictionary<IFeature, IFeatureCategory> featureCategoryDic = new Dictionary<IFeature, IFeatureCategory>();
            foreach (var feature in features)
            {
                featureCategoryDic[feature] = DefaultCategory;
            }
            using (DataTable dataTable = GetAttribute(features))
            {
                foreach (IFeatureCategory featureCategory in Categories)
                {
                    DataRow[] rows = dataTable.Select(featureCategory.FilterExpression);
                    foreach (var row in rows)
                    {
                        int index = dataTable.Rows.IndexOf(row);
                        IFeature feature = features[index];
                        featureCategoryDic[feature] = featureCategory;
                    }
                }
            }
            return featureCategoryDic;
        }
        /// <summary>
        /// 绘制几何要素
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="symbolizer"></param>
        /// <param name="geometry"></param>
        protected abstract void DrawGeometry(MapArgs drawArgs, IFeatureSymbolizer symbolizer, IGeometry geometry);
        private void DrawFeatures(MapArgs drawArgs, List<IFeature> features, bool selected, IProgressHandler progressHandler, CancellationTokenSource cancellationTokenSource)
        {
            if (drawArgs == null || features == null || cancellationTokenSource?.IsCancellationRequested == true)
            {
                return;
            }
            var featureCategoryDic = GetFeatureAndCategoryDic(features);
            foreach (var item in featureCategoryDic)
            {
                if (cancellationTokenSource?.IsCancellationRequested == true)
                {
                    return;
                }
                IFeature feature = item.Key;
                var category = item.Value;
                var symbolizer = selected ? category.SelectionSymbolizer : category.Symbolizer;
                DrawGeometry(drawArgs, symbolizer, feature.Geometry);
            }
        }

        private DataTable GetSchema()
        {
            DataTable dataTable = new DataTable();
            int fieldCount = DataSet.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                var fieldDefn = DataSet.GetFieldDefn(i);

                FieldType fieldType = fieldDefn.FieldType;
                string name = fieldDefn.Name;
                Type type = null;
                switch (fieldType)
                {
                    case FieldType.Binary:
                        type = typeof(byte[]);
                        break;
                    case FieldType.DateTime:
                        type = typeof(DateTime);
                        break;
                    case FieldType.Int:
                        type = typeof(int);
                        break;
                    case FieldType.Long:
                        type = typeof(long);
                        break;
                    case FieldType.LongList:
                        type = typeof(long[]);//todo待测试
                        break;
                    case FieldType.IntList:
                        type = typeof(int[]);
                        break;
                    case FieldType.Double:
                        type = typeof(double);
                        break;
                    case FieldType.DoubleList:
                        type = typeof(double[]);
                        break;
                    case FieldType.String:
                        type = typeof(string);
                        break;
                    case FieldType.StringList:
                        type = typeof(string[]);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                DataColumn dataColumn = new DataColumn(name, type);
                dataTable.Columns.Add(dataColumn);
            }
            return dataTable;
        }
        private DataTable GetAttribute(List<IFeature> features)
        {
            DataTable dataTable = GetSchema();
            if (features == null || features.Count == 0)
            {
                return dataTable;
            }
            dataTable.BeginLoadData();
            foreach (var feature in features)
            {
                DataRow dataRow = dataTable.NewRow();
                dataTable.Rows.Add(dataRow);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    DataColumn column = dataTable.Columns[i];
                    var field = feature.GetField(i);
                    switch (field.FieldDfn.FieldType)
                    {
                        case FieldType.Int:
                            dataRow[column] = field.GetValueAsInteger();
                            break;
                        case FieldType.Long:
                            dataRow[column] = field.GetValueAsLong();
                            break;
                        case FieldType.Double:
                            dataRow[column] = field.GetValueAsDouble();
                            break;
                        case FieldType.DateTime:
                            dataRow[column] = field.GetValueAsDateTime();
                            break;
                        case FieldType.String:
                            dataRow[column] = field.GetValueAsString();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            dataTable.EndLoadData();
            return dataTable;
        }

    }
}