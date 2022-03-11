using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace EM.GIS.Data
{
    [Serializable]
    public abstract class RasterSet : DataSet, IRasterSet
    {
        public virtual int NumRows { get; }
        public virtual int NumColumns { get; }
        public IList<IRasterSet> Bands { get; }
        public abstract int ByteSize { get; }
        [Category("Data")]
        [Description("Gets or sets a  double showing the no-data value for this raster.")]
        public virtual double? NoDataValue { get; set; }
        public IRasterBounds Bounds { get; set; }
        public override IExtent Extent
        { 
            get => Bounds.Extent; 
            protected set => Bounds.Extent=value; 
        }

        public int PixelSpace { get; set; }
        public int LineSpace { get; set; }

        public RasterType RasterType { get; set; }

        public int BandCount => Bands.Count;

        public IRasterBounds RasterBounds { get; set; }
        public RasterSet()
        {
            Bands = new List<IRasterSet>();
        }
        public virtual Image GetImage()
        {
            return null;
        }

        public Image GetImage(IExtent envelope, Size size)
        {
            return GetImage(envelope, new Rectangle(new Point(0, 0), size));
        }

        public virtual Image GetImage(IExtent envelope, Rectangle window, Action<int> progressAction = null)
        {
            return null;
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Color[] CategoryColors()
        {
            return null;
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string[] CategoryNames()
        {
            return null;
        }

        public virtual Statistics GetStatistics()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取字节大小
        /// </summary>
        /// <typeparam name="TValue">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>字节大小</returns>
        public static int GetByteSize<TValue>(TValue value)
        {
            if (value is byte) return 1;
            if (value is short) return 2;
            if (value is int) return 4;
            if (value is long) return 8;
            if (value is float) return 4;
            if (value is double) return 8;

            if (value is sbyte) return 1;
            if (value is ushort) return 2;
            if (value is uint) return 4;
            if (value is ulong) return 8;

            if (value is bool) return 1;

            return 0;
        }
    }
}
