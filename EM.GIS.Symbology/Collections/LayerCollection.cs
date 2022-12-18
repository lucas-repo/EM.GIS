using EM.GIS.Data;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EM.Bases;
using System.Collections.Specialized;
using System;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层集合
    /// </summary>
    public class LayerCollection : ItemCollection<IBaseItem>, ILayerCollection
    {
        public new ILayer this[int index]
        {
            get => base[index] as ILayer;
            set => base[index] = value;
        }
        [NonSerialized]
        private IGroup _parent;
        /// <inheritdoc/>
        public IGroup Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }
        private IFrame _frame;
        /// <inheritdoc/>
        public IFrame Frame
        {
            get { return _frame; }
            set { SetProperty(ref _frame, value); }
        }

        private ProgressDelegate _progress;
        public ProgressDelegate Progress
        {
            get { return _progress; }
            set
            {
                if (SetProperty(ref _progress, value, nameof(Progress)))
                {
                    foreach (var item in this)
                    {
                        if (item is ILayer layer)
                        {
                            layer.Progress = _progress;
                        }
                    }
                }
            }
        }
        public LayerCollection(IFrame frame, IGroup parent):this(parent)
        {
            _frame = frame;
        }
        public LayerCollection(IGroup parent)
        {
            _parent = parent;
            CollectionChanged += LegendItemCollection_CollectionChanged;
        }
        private void LegendItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Action setOldItemsAction = new Action(() =>
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is ILayer t)
                        {
                            t.Parent = default;
                        }
                    }
                }
            });
            Action setNewItemsAction = new Action(() =>
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is ILayer t)
                        {
                            t.Parent = Parent;
                        }
                    }
                }
            });
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    setNewItemsAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    setOldItemsAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    setOldItemsAction.Invoke();
                    setNewItemsAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    setOldItemsAction.Invoke();
                    break;
            }
        }

        public IGroup AddGroup(string groupName = null)
        {
            string destGroupName = groupName;
            if (string.IsNullOrEmpty(groupName))
            {
                destGroupName = GetDifferenctName("分组");
            }
            IGroup group = new Group()
            {
                Text = destGroupName,
                IsVisible = true,
                Parent = Parent
            };
            Insert(0, group);
            return group;
        }
        private string GetDifferenctName(string prefix, int i = 0)
        {
            string name = $"{prefix}{i}";
            foreach (ILegendItem item in this)
            {
                if (item.Text == name)
                {
                    name = GetDifferenctName(prefix, ++i);
                    break;
                }
            }
            return name;
        }
        public ILayer AddLayer(IDataSet dataSet, bool isVisible = true)
        {
            ILayer layer = null;
            //var ss = dataSet as ISelfLoadSet;
            //if (ss != null) return Add(ss);

            if (dataSet is IFeatureSet fs)
            {
                layer = AddLayer(fs, isVisible);
            }
            else if (dataSet is IRasterSet r)
            {
                layer = AddLayer(r, isVisible);
            }
            if (dataSet != null)
            {
                layer.Text = dataSet.Name;
            }
            return layer;
        }

        public IFeatureLayer AddLayer(IFeatureSet featureSet, bool isVisible = true)
        {
            IFeatureLayer layer = null;
            if (featureSet == null) return null;

            if (featureSet.FeatureType == FeatureType.Point || featureSet.FeatureType == FeatureType.MultiPoint)
            {
                layer = new PointLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polyline)
            {
                layer = new LineLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polygon)
            {
                layer = new PolygonLayer(featureSet);
            }

            if (layer != null)
            {
                layer.IsVisible = isVisible;
                layer.Parent = Parent;
                Insert(0, layer);
            }

            return layer;
        }

        public IRasterLayer AddLayer(IRasterSet raster, bool isVisible = true)
        {
            IRasterLayer rasterLayer = null;
            if (raster != null)
            {
                rasterLayer = new RasterLayer(raster)
                {
                    IsVisible = isVisible,
                    Parent = Parent
                };
                Insert(0, rasterLayer);
            }
            return rasterLayer;
        }

        //public ILayer AddLayer(string path, bool isVisible = true)
        //{
        //    IDataSet dataSet = DataFactory.Default.DriverFactory.Open(path);
        //    return AddLayer(dataSet, isVisible);
        //}
        public override void Add(IBaseItem item)
        {
            if (item is ILayer)
            {
                base.Add(item);
            }
        }
    }
}