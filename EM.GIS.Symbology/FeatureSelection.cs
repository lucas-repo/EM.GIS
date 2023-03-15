using EM.GIS.Data;
using EM.GIS.Geometries;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素选择
    /// </summary>
    public class FeatureSelection : Selection, IFeatureSelection
    {
        /// <summary>
        /// 要素集
        /// </summary>
        private IFeatureSet FeatureSet { get; }
        /// <summary>
        /// 选择的要素集合
        /// </summary>
        private List<IFeature> Features { get; }
        /// <inheritdoc/>
        public IFeature this[int index] { get => Features[index]; set => Features[index] = value; }

        /// <inheritdoc/>
        public override IExtent Extent
        {
            get
            {
                IExtent extent = new Extent();
                foreach (var item in this)
                {
                    extent.ExpandToInclude(item.Geometry.GetExtent());
                }
                return extent;
            }
        }
        /// <inheritdoc/>
        public int Count => Features.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public new IFeatureCategory? Category { get => base.Category as IFeatureCategory; set => base.Category = value; }
    
        public FeatureSelection(IFeatureSet featureSet)
        {
            FeatureSet = featureSet;
            Features = new List<IFeature>();
        }
        /// <inheritdoc/>
        public void Add(IFeature item)
        {
            Features.Add(item);
        }
        /// <inheritdoc/>
        public override bool AddRegion(IExtent extent, out IExtent affectedExtent)
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
                affectedExtent.ExpandToInclude(item.Geometry.GetExtent());
            }
            FeatureSet.SetSpatialExtentFilter(null);
            ret = true;
            return ret;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Features.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(IFeature item)
        {
            bool ret = Features.Contains(item);
            if (!ret && item != null)
            {
                ret = Features.Any(x => x.FId == item.FId);
            }
            return ret;
        }

        /// <inheritdoc/>
        public void CopyTo(IFeature[] array, int arrayIndex)
        {
            Features.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<IFeature> GetEnumerator()
        {
            return Features.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(IFeature item)
        {
            return Features.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, IFeature item)
        {
            Features.Insert(index, item);
        }

        /// <inheritdoc/>
        public override bool InvertSelection(IExtent extent, out IExtent affectedExtent)
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
                affectedExtent.ExpandToInclude(item.Geometry.GetExtent());
            }
            FeatureSet.SetSpatialExtentFilter(null);
            ret = true;
            return ret;
        }

        /// <inheritdoc/>
        public bool Remove(IFeature item)
        {
            return Features.Remove(item);
        }
        /// <inheritdoc/>
        public void RemoveRange( IEnumerable<IFeature> features)
        {
            SuspendChanges();
            foreach (IFeature f in features)
            {
                Remove(f);
            }
            ResumeChanges();
        }
        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            Features.RemoveAt(index);
        }

        /// <inheritdoc/>
        public override bool RemoveRegion(IExtent extent, out IExtent affectedExtent)
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
                affectedExtent.ExpandToInclude(item.Geometry.GetExtent());
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