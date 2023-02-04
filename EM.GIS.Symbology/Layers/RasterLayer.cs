using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 栅格图层
    /// </summary>
    public class RasterLayer : Layer, IRasterLayer
    {
        /// <inheritdoc/>
        public new IRasterSet? DataSet
        {
            get
            {
                if (base.DataSet is IRasterSet rasterSet)
                {
                    return rasterSet;
                }
                else
                {
                    throw new Exception($"{nameof(DataSet)}类型必须为{nameof(IRasterSet)}");
                }
            }
            set=> base.DataSet = value;
        }

        /// <inheritdoc/>
        public new IRasterCategoryCollection Children
        {
            get
            {
                if (base.Children is IRasterCategoryCollection categories)
                {
                    return categories;
                }
                else
                {
                    throw new Exception($"{nameof(DataSet)}类型必须为{nameof(IRasterCategoryCollection)}");
                }
            }
            set => base.Children = value;
        }
        public RasterLayer(IRasterSet rasterSet):base(rasterSet)
        {
            Children = new RasterCategoryCollection(this);
        }

        /// <inheritdoc/>
        protected override Rectangle OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null)
        {
            var ret = Rectangle.Empty;
            if (selected || cancelFunc?.Invoke() == true)
            {
                return ret;
            }
            if (DataSet is IDrawable drawable)
            {
                Action<int> newProgressAction = (progress) => progressAction?.Invoke(ProgressMessage, progress);
                ret= drawable.Draw(mapArgs, newProgressAction, cancelFunc);
            }
            return ret;
        }
    }
}