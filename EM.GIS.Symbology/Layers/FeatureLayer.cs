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
        /// <inheritdoc/>
        public new IFeatureCategoryCollection Children
        {
            get
            {
                if (base.Children is IFeatureCategoryCollection collection)
                {
                    return collection;
                }
                else
                {
                    throw new Exception($"{nameof(Children)}类型必须为{nameof(IFeatureCategoryCollection)}");
                }
            }
            protected set => base.Children = value;
        }
        /// <inheritdoc/>
        public IFeatureCategory DefaultCategory
        {
            get
            {
                if (Children.LastOrDefault() is IFeatureCategory category)
                {
                    return category;
                }
                else
                {
                    throw new Exception($"{nameof(Children)} 必须包含一个 {nameof(IFeatureCategory)}");
                }
            }
            set
            {
                if (value != null)
                {
                    if (Children.Count > 0)
                    {
                        Children[Children.Count - 1] = value;
                    }
                    else
                    {
                        Children.Add(value);
                    }
                }
            }
        }
        /// <summary>
        /// 实例化<seealso cref="FeatureLayer"/>
        /// </summary>
        /// <param name="featureSet">要素集</param>
        public FeatureLayer(IFeatureSet featureSet) : base(featureSet)
        {
            Selection = new FeatureSelection(featureSet);
            LabelLayer = new LabelLayer(this);
        }

        /// <inheritdoc/>
        public ILabelLayer LabelLayer { get; }
        /// <inheritdoc/>
        public new IFeatureSet? DataSet
        {
            get
            {
                if (base.DataSet is IFeatureSet featureSet)
                {
                    return featureSet;
                }
                else
                {
                    throw new Exception($"{nameof(DataSet)}类型必须为{nameof(IFeatureSet)}");
                }
            }
            set
            {
                base.DataSet = value;
                DrawnStates.Clear();
            }
        }
        /// <inheritdoc/>
        public IFeatureSelection Selection { get; }

        /// <inheritdoc/>
        public Dictionary<long, IFeatureCategory> DrawnStates { get; } = new Dictionary<long, IFeatureCategory>();
        /// <inheritdoc/>
        protected override Rectangle OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null)
        {
            var ret = Rectangle.Empty;
            if (selected && Selection.Count == 0 || cancelFunc?.Invoke() == true || DataSet == null)
            {
                return ret;
            }
            DataSet.SetSpatialExtentFilter(mapArgs.DestExtent);
            long featureCount = DataSet.FeatureCount;
            progressAction?.Invoke(ProgressMessage, 5);
            var features = new List<IFeature>();
            long drawnFeatureCount = 0;
            int threshold = 262144;
            int totalPointCount = 0;
            int percent;
            Action drawFeatuesAction = () =>
            {
                if (features.Count > 0)
                {
                    if (cancelFunc?.Invoke() != true)
                    {
                        percent = (int)(drawnFeatureCount * 90 / featureCount);
                        progressAction?.Invoke(ProgressMessage, percent);
                        DrawFeatures(mapArgs, mapArgs.Graphics, features, selected, progressAction, cancelFunc);
                        drawnFeatureCount += features.Count;
                        if (features.Count > 0)
                        {
                            IExtent extent = new Extent();
                            foreach (var feature in features)
                            {
                                extent.ExpandToInclude(feature.Geometry.GetExtent());
                            }
                            var rect = mapArgs.ProjToPixel(extent);
                            if (!rect.IsEmpty)
                            {
                                ret = ret.ExpandToInclude(rect);
                                invalidateMapFrameAction?.Invoke(rect);
                            }
                        }
                    }
                    foreach (var item in features)
                    {
                        item.Dispose();
                    }
                    features.Clear();
                }
                totalPointCount = 0;
            };
            List<IFeature> ignoreFeatures = new List<IFeature>();
            foreach (var feature in DataSet.GetFeatures())
            {
                if (cancelFunc?.Invoke() == true)
                {
                    break;
                }
                if (selected)
                {
                    if (!Selection.Contains(feature))
                    {
                        ignoreFeatures.Add(feature);
                        continue;
                    }
                }
                features.Add(feature);
                if (feature.Geometry.Geometries.Count == 0)
                {
                    int pointCount = feature.Geometry.Coordinates.Count;
                    totalPointCount += pointCount;
                }
                else
                {
                    foreach (var geometry in feature.Geometry.Geometries)
                    {
                        int pointCount = geometry.Coordinates.Count;
                        totalPointCount += pointCount;
                    }
                }
                if (totalPointCount >= threshold)
                {
                    drawFeatuesAction();
                }
            }
            if (totalPointCount > 0)
            {
                drawFeatuesAction();
            }
            foreach (var item in ignoreFeatures)
            {
                item.Dispose();
            }
            DataSet.SetSpatialFilter(null);
            return ret;
        }
        private Dictionary<IFeature, IFeatureCategory> GetFeatureAndCategoryDic(List<IFeature> features)
        {
            Dictionary<IFeature, IFeatureCategory> featureCategoryDic = new Dictionary<IFeature, IFeatureCategory>();
            var cachedFeatures = features.Where(x => DrawnStates.Keys.Contains(x.FId));
            foreach (var item in cachedFeatures)
            {
                featureCategoryDic[item] = DrawnStates[item.FId];
            }
            var otherFeatures = features.Except(cachedFeatures).ToList(); ;
            using (DataTable dataTable = GetAttribute(otherFeatures))
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];
                    IFeature feature = otherFeatures.ElementAt(i);
                    for (int j = Children.Count - 1; j >= 0; j--)
                    {
                        IFeatureCategory featureCategory = Children[j];
                        if (string.IsNullOrEmpty(featureCategory.FilterExpression))
                        {
                            DrawnStates[feature.FId] = featureCategory;
                            featureCategoryDic[feature] = featureCategory;
                        }
                        else
                        {
                            DataRow[] rows = dataTable.Select(featureCategory.FilterExpression);
                            if (rows.Contains(row))
                            {
                                DrawnStates[feature.FId] = featureCategory;
                                featureCategoryDic[feature] = featureCategory;
                            }
                        }
                    }
                }
            }
            return featureCategoryDic;
        }
        /// <summary>
        /// 绘制几何要素
        /// </summary>
        /// <param name="proj">投影参数</param>
        /// <param name="graphics">画布</param>
        /// <param name="symbolizer">要素符号</param>
        /// <param name="geometry">几何体</param>
        protected abstract void DrawGeometry(IProj proj, Graphics graphics, IFeatureSymbolizer symbolizer, IGeometry geometry);
        private void DrawFeatures(IProj proj, Graphics graphics, List<IFeature> features, bool selected, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null)
        {
            if (proj == null || proj.Bound.IsEmpty || proj.Extent == null || proj.Extent.IsEmpty() || graphics == null || features == null || cancelFunc?.Invoke() == true)
            {
                return;
            }
            var featureCategoryDic = GetFeatureAndCategoryDic(features);
            foreach (var item in featureCategoryDic)
            {
                if (cancelFunc?.Invoke() == true)
                {
                    return;
                }
                IFeature feature = item.Key;
                if (feature.Geometry == null)
                {
                    continue;
                }
                var category = item.Value;
                var symbolizer = selected ? category.SelectionSymbolizer : category.Symbolizer;
                if (symbolizer == null)
                {
                    continue;
                }
                DrawGeometry(proj, graphics, symbolizer, feature.Geometry);
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
                Type type;
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
        private DataTable GetAttribute(IEnumerable<IFeature> features)
        {
            DataTable dataTable = GetSchema();
            if (features == null || features.Count() == 0)
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