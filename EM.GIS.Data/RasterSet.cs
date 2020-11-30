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
        public virtual int NumColumns { get;  }
        public IList<IRasterSet> Bands { get;  }
        public abstract int ByteSize { get; }
        [Category("Data")]
        [Description("Gets or sets a  double showing the no-data value for this raster.")]
        public virtual double NoDataValue { get; set; }
        public IRasterBounds Bounds { get; set; }
        public int PixelSpace { get; set; }
        public int LineSpace { get; set; }
        public override IExtent Extent
        {
            get => Bounds?.Extent;
        }

        public RasterType RasterType { get; set; }

        public int BandCount => Bands.Count;

        public IRasterBounds RasterBounds { get; set; }
        public RasterSet()
        {
            Bands = new List<IRasterSet>();
        }
        public virtual Bitmap GetBitmap()
        {
            return null;
        }

        public  Bitmap GetBitmap(IExtent envelope, Size size)
        {
            return GetBitmap(envelope, new Rectangle(new Point(0, 0), size));
        }

        public virtual Bitmap GetBitmap(IExtent envelope, Rectangle window)
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
    }
}
