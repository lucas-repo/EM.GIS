using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

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
            var ret = RectangleF.Empty;
            if ((selected && Selection.Count == 0) || cancelFunc?.Invoke() == true || DataSet == null)
            {
                return ret.ToRectangle();
            }
            IExtent filter = mapArgs.DestExtent;
            if (mapArgs.Projection != null && DataSet.Projection != null && !mapArgs.Projection.Equals(DataSet.Projection))
            {
                filter = mapArgs.DestExtent.Copy();
                mapArgs.Projection.ReProject(DataSet.Projection, filter);
            }

            DataSet.SetSpatialExtentFilter(filter);
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
                        var rect = DrawFeatures(mapArgs, features, selected, progressAction, cancelFunc);
                        if (!rect.IsEmpty)
                        {
                            ret = ret.ExpandToInclude(rect);
                            invalidateMapFrameAction?.Invoke(rect);
                        }
                        drawnFeatureCount += features.Count;
                    }
                    foreach (var item in features)
                    {
                        item.Dispose();
                    }
                    features.Clear();
                }
                totalPointCount = 0;
            };
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
                        feature.Dispose();
                        continue;
                    }
                }
                else
                {
                    if (Selection.Contains(feature))
                    {
                        feature.Dispose();
                        continue;
                    }
                }
                features.Add(feature);
                if (feature.Geometry.GeometryCount == 0)
                {
                    totalPointCount += feature.Geometry.CoordinateCount;
                }
                else
                {
                    for (int i = 0; i < feature.Geometry.GeometryCount; i++)
                    {
                        var geometry = feature.Geometry.GetGeometry(i);
                        totalPointCount += geometry.CoordinateCount;
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
            DataSet.SetSpatialFilter(null);
            return ret.ToRectangle();
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
        /// <returns>像素范围</returns>
        protected abstract Rectangle DrawGeometry(IProj proj, Graphics graphics, IFeatureSymbolizer symbolizer, IGeometry geometry);
        private Rectangle DrawFeatures(MapArgs mapArgs, List<IFeature> features, bool selected, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null)
        {
            var ret = Rectangle.Empty;
            if (features == null || cancelFunc?.Invoke() == true)
            {
                return ret;
            }
            var featureCategoryDic = GetFeatureAndCategoryDic(features);
            Func<IGeometry, IGeometry> getGeometryFunc = (geometry) =>
            {
                IGeometry ret = geometry;
                if (mapArgs.Projection != null && DataSet?.Projection != null && !mapArgs.Projection.Equals(DataSet.Projection))
                {
                    ret = geometry.Copy();
                    DataSet.Projection.ReProject(mapArgs.Projection, ret);
                }
                return ret;
            };
            foreach (var item in featureCategoryDic)
            {
                if (cancelFunc?.Invoke() == true)
                {
                    return ret;
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
                var geometry = getGeometryFunc(feature.Geometry);
                var rect = DrawGeometry(mapArgs, mapArgs.Graphics, symbolizer, geometry);
                ret = ret.ExpandToInclude(rect);
                if (geometry != feature.Geometry)
                {
                    geometry.Dispose();
                }
            }
            return ret;
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
        /// <inheritdoc/>
        public override bool ClearSelection(out IExtent? affectedArea, bool force)
        {
            if (!force && !SelectionEnabled)
            {
                affectedArea = new Extent();
                return false;
            }

            affectedArea = Selection.Extent;
            //if (!_drawnStatesNeeded)
            //{
            //    return false;
            //}

            Selection.SuspendChanges();

            bool changed = false;
            // we're clearing by force or all categorys are selection enabled, so a simple clear of the list is enough
            if (force || Children.All(x => x is IFeatureCategory category && category.SelectionEnabled))
            {
                if (Selection.Count > 0)
                {
                    changed = true;
                }

                Selection.Clear();
            }
            else
            {
                // we're clearing only the categories that are selection enabled
                var area = new Extent();
                foreach (var item in Children)
                {
                    if (item is IFeatureCategory category && category.SelectionEnabled)
                    {
                        IExtent categoryArea;
                        Selection.Category = category;
                        if (Selection.RemoveRegion(affectedArea, out categoryArea))
                        {
                            changed = true;
                            area.ExpandToInclude(categoryArea);
                        }
                        Selection.Category = null;
                    }
                }
                affectedArea = area;
            }
            Selection.ResumeChanges();
            return changed;
        }
        /// <inheritdoc/>
        public override bool InvertSelection(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent? affectedArea)
        {
            SelectAction action = (ISelection selection, IExtent region, out IExtent affectedRegion) => selection.InvertSelection(region, out affectedRegion);
            return DoSelectAction(tolerant, strict, mode, ClearStates.False, out affectedArea, action);
        }
        private bool DoSelectAction(IExtent tolerant, IExtent strict, SelectionMode selectionMode, ClearStates clear, out IExtent affectedArea, SelectAction action)
        {
            bool changed = false;
            affectedArea = new Extent();
            if ((!SelectionEnabled && clear != ClearStates.Force) || !GetVisible(Extent))
            {
                return changed;
            }

            Selection.SuspendChanges();

            if (clear != ClearStates.False)
            {
                if (Selection.Count > 0)
                {
                    affectedArea.ExpandToInclude(Selection.Extent);
                    Selection.Clear();
                    changed = true;
                }
            }

            var region = DataSet?.FeatureType == FeatureType.Polygon ? strict : tolerant;
            Selection.SelectionMode = selectionMode;

            // all categories are selection enabled, so a category independent action is enough
            if (Children.All(x => x is IFeatureCategory category && category.SelectionEnabled))
            {
                if (action(Selection, region, out var selectionExtent))
                {
                    changed = true;
                    affectedArea.ExpandToInclude(selectionExtent);
                }
            }
            else
            {
                foreach (IFeatureCategory cat in Children.Where(x => x is IFeatureCategory category && category.SelectionEnabled))
                {
                    Selection.Category = cat;
                    if (action(Selection, region, out var categoryArea))
                    {
                        changed = true;
                        affectedArea.ExpandToInclude(categoryArea);
                    }
                    Selection.Category = null;
                }
            }

            Selection.ResumeChanges();

            return changed;
        }
        /// <inheritdoc/>
        public override bool Select(IExtent tolerant, IExtent strict, SelectionMode selectionMode, out IExtent affectedArea, ClearStates clear)
        {
            SelectAction action = (ISelection selection, IExtent region, out IExtent affectedRegion) => selection.AddRegion(region, out affectedRegion);
            return DoSelectAction(tolerant, strict, selectionMode, clear, out affectedArea, action);
        }
        /// <summary>
        /// 取消选择
        /// </summary>
        /// <param name="featureIndices">索引</param>
        public void UnSelect(IEnumerable<int> featureIndices)
        {
            if (DataSet == null)
            {
                return;
            }
            var features = featureIndices.Select(index => DataSet.GetFeature(index));
            Selection.RemoveRange(features);
        }
        /// <inheritdoc/>
        public override bool UnSelect(IExtent tolerant, IExtent strict, SelectionMode selectionMode, out IExtent affectedArea)
        {
            SelectAction action = (ISelection selection, IExtent region, out IExtent affectedRegion) => selection.RemoveRegion(region, out affectedRegion);
            return DoSelectAction(tolerant, strict, selectionMode, ClearStates.False, out affectedArea, action);
        }
        private delegate bool SelectAction(ISelection selection, IExtent region, out IExtent affectedRegion);
    }
}