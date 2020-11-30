using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace EM.GIS.Symbology
{
    public class FeatureSelection : Selection, IFeatureSelection
    {
        private IFeatureSet FeatureSet { get; }
        private List<IFeature> Features { get; }
        public IFeature this[int index] { get => Features[index]; set => Features[index] = value; }

        public override IExtent IExtent
        {
            get
            {
                IExtent extent = new Extent();
                foreach (var item in this)
                {
                    extent.ExpandToInclude(item.Geometry.Extent);
                }
                return extent;
            }
        }

        public int Count => Features.Count;

        public bool IsReadOnly => false;

        public new IFeatureCategory Category { get => base.Category as IFeatureCategory; set => base.Category = value; }
        public FeatureSelection(IFeatureSet featureSet)
        {
            FeatureSet = featureSet;
            Features = new List<IFeature>();
        }
        public void Add(IFeature item)
        {
            Features.Add(item);
        }

        public bool AddRegion(IExtent extent, out IExtent affectedExtent)
        {
            bool ret = false;
            affectedExtent = extent;
            if (extent == null)
            {
                return ret;
            }
            FeatureSet.SetSpatialExtentFilter(extent);
            foreach (var item in FeatureSet.GetFeatures())
            {
                Features.Add(item);
                affectedExtent.ExpandToInclude(item.Geometry.Extent);
            }
            FeatureSet.SetSpatialExtentFilter(null);
            ret = true;
            return ret;
        }

        public void Clear()
        {
            Features.Clear();
        }

        public bool Contains(IFeature item)
        {
            return Features.Contains(item);
        }

        public void CopyTo(IFeature[] array, int arrayIndex)
        {
            Features.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return Features.GetEnumerator();
        }

        public int IndexOf(IFeature item)
        {
            return Features.IndexOf(item);
        }

        public void Insert(int index, IFeature item)
        {
            Features.Insert(index, item);
        }

        public bool InvertSelection(IExtent extent, out IExtent affectedExtent)
        {
            bool ret = false;
            affectedExtent = extent;
            if (extent == null)
            {
                return ret;
            }
            FeatureSet.SetSpatialExtentFilter(extent);
            foreach (var item in FeatureSet.GetFeatures())
            {
                if (Features.Contains(item))
                {
                    Features.Remove(item);
                }
                else
                {
                    Features.Add(item);
                }
                affectedExtent.ExpandToInclude(item.Geometry.Extent);
            }
            FeatureSet.SetSpatialExtentFilter(null);
            ret = true;
            return ret;
        }

        public bool Remove(IFeature item)
        {
            return Features.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Features.RemoveAt(index);
        }

        public bool RemoveRegion(IExtent extent, out IExtent affectedExtent)
        {
            bool ret = false;
            affectedExtent = extent;
            if (extent == null)
            {
                return ret;
            }
            FeatureSet.SetSpatialExtentFilter(extent);
            foreach (var item in FeatureSet.GetFeatures())
            {
                if (Features.Contains(item))
                {
                    Features.Remove(item);
                }
                affectedExtent.ExpandToInclude(item.Geometry.Extent);
            }
            FeatureSet.SetSpatialExtentFilter(null);
            ret = true;
            return ret;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}