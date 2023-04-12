using EM.GIS.Data;
using EM.GIS.GdalExtensions;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// GDAL栅格
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    [Serializable]
    public class GdalRasterSet<T> : RasterSet<T>
        where T : IEquatable<T>, IComparable<T>
    {
        private Dataset _dataset;
        /// <summary>
        /// 数据源
        /// </summary>
        public Dataset Dataset
        {
            get { return _dataset; }
            private set
            {
                if (SetProperty(ref _dataset, value))
                {
                    OnDatasetChanged();
                }
            }
        }
        /// <inheritdoc/>
        public override IProjection Projection 
        { 
            get => base.Projection;
            set
            {
                if (!Equals(base.Projection, value))
                {
                    base.Projection = value;
                    var wkt = base.Projection.ExportToWkt();
                    var err = _dataset.SetProjection(wkt);
                }
            }
        }
        private List<IRasterSet> rasters = new List<IRasterSet>();
        /// <inheritdoc/>
        public override IEnumerable<IRasterSet> Rasters => rasters;
        private void OnDatasetChanged()
        {
            GdalProjection projection = null;
            rasters.Clear();
            if (Dataset != null)
            {
                int numBands = Dataset.RasterCount;
                for (int i = 1; i <= numBands; i++)
                {
                    Band band = Dataset.GetRasterBand(i);
                    if (i == 1)
                    {
                        _band = band;
                    }
                    rasters.Add(new GdalRasterSet<T>(Filename, Dataset, band));
                }
                projection = new GdalProjection(Dataset.GetProjection());
            }
            Projection = projection;
            ReadHeader();
        }
        bool _ignoreChangeDataset;
        /// <inheritdoc/>
        public override string RelativeFilename
        {
            get => base.RelativeFilename;
            protected set
            {
                base.RelativeFilename = value;
                if (!_ignoreChangeDataset)
                {
                    if (File.Exists(value))
                    {
                        try
                        {
                            Dataset = Gdal.Open(value, Access.GA_Update);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                            Dataset = Gdal.Open(value, Access.GA_ReadOnly);
                        }
                    }
                    else
                    {
                        Dataset = null;
                    }
                }
            }
        }
        /// <inheritdoc/>
        public override int Height => _dataset.RasterYSize;
        /// <inheritdoc/>
        public override int Width => _dataset.RasterXSize;
        #region Fields
        /// <summary>
        /// 当前波段
        /// </summary>
        private Band? _band;
        private int _overviewCount;
        private ColorInterp _colorInterp;
        private int _overview;
        #endregion

        #region Constructors
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fromDataset">数据集</param>
        public GdalRasterSet(string fileName, Dataset fromDataset)
        {
            _ignoreChangeDataset = true;
            Filename = fileName;
            _ignoreChangeDataset = false;
            Name = Path.GetFileNameWithoutExtension(fileName);
            _dataset = fromDataset;
            OnDatasetChanged();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fromDataset">数据集</param>
        /// <param name="fromBand">波段</param>
        public GdalRasterSet(string fileName, Dataset fromDataset, Band fromBand)
        {
            _dataset = fromDataset;
            _band = fromBand;
            _ignoreChangeDataset = true;
            Filename = fileName;
            _ignoreChangeDataset = false;
            Name = Path.GetFileNameWithoutExtension(fileName);
            ReadHeader();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType GdalDataType => _band.DataType;
        /// <inheritdoc/>
        public override double? NoDataValue
        {
            get
            {
                return base.NoDataValue;
            }

            set
            {
                base.NoDataValue = value;
                if (_band != null && value.HasValue)
                {
                    _band.SetNoDataValue(value.Value);
                }
                else
                {
                    foreach (var raster in Rasters)
                    {
                        raster.NoDataValue = value;
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override Rectangle OnDraw(MapArgs mapArgs, Action<int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? graphicsUpdatedAction = null, Dictionary<string, object>? options = null)
        {
            var ret = Rectangle.Empty;

            progressAction?.Invoke(5);

            // Gets the scaling factor for converting from geographic to pixel coordinates
            double dx = mapArgs.Bound.Width / mapArgs.Extent.Width;
            double dy = mapArgs.Bound.Height / mapArgs.Extent.Height;

            double[] affine = Bounds.AffineCoefficients;

            // calculate inverse
            double p = 1 / ((affine[1] * affine[5]) - (affine[2] * affine[4])); 
            double[] aInv = new double[4];
            aInv[0] = affine[5] * p;
            aInv[1] = -affine[2] * p;
            aInv[2] = -affine[4] * p;
            aInv[3] = affine[1] * p;

            // estimate rectangle coordinates
            double tlx = ((mapArgs.DestExtent.MinX - affine[0]) * aInv[0]) + ((mapArgs.DestExtent.MaxY - affine[3]) * aInv[1]);
            double tly = ((mapArgs.DestExtent.MinX - affine[0]) * aInv[2]) + ((mapArgs.DestExtent.MaxY - affine[3]) * aInv[3]);
            double trx = ((mapArgs.DestExtent.MaxX - affine[0]) * aInv[0]) + ((mapArgs.DestExtent.MaxY - affine[3]) * aInv[1]);
            double trY = ((mapArgs.DestExtent.MaxX - affine[0]) * aInv[2]) + ((mapArgs.DestExtent.MaxY - affine[3]) * aInv[3]);
            double blx = ((mapArgs.DestExtent.MinX - affine[0]) * aInv[0]) + ((mapArgs.DestExtent.MinY - affine[3]) * aInv[1]);
            double bly = ((mapArgs.DestExtent.MinX - affine[0]) * aInv[2]) + ((mapArgs.DestExtent.MinY - affine[3]) * aInv[3]);
            double brx = ((mapArgs.DestExtent.MaxX - affine[0]) * aInv[0]) + ((mapArgs.DestExtent.MinY - affine[3]) * aInv[1]);
            double bry = ((mapArgs.DestExtent.MaxX - affine[0]) * aInv[2]) + ((mapArgs.DestExtent.MinY - affine[3]) * aInv[3]);

            // get absolute maximum and minimum coordinates to make a rectangle on projected coordinates
            // that overlaps all the visible area.
            double tLx = Math.Min(Math.Min(Math.Min(tlx, trx), blx), brx);
            double tLy = Math.Min(Math.Min(Math.Min(tly, trY), bly), bry);
            double bRx = Math.Max(Math.Max(Math.Max(tlx, trx), blx), brx);
            double bRy = Math.Max(Math.Max(Math.Max(tly, trY), bly), bry);

            // limit it to the available image
            // todo: why we compare NumColumns\Rows and X,Y coordinates??
            if (tLx > Bounds.NumColumns) tLx = Bounds.NumColumns;
            if (tLy > Bounds.NumRows) tLy = Bounds.NumRows;
            if (bRx > Bounds.NumColumns) bRx = Bounds.NumColumns;
            if (bRy > Bounds.NumRows) bRy = Bounds.NumRows;

            if (tLx < 0) tLx = 0;
            if (tLy < 0) tLy = 0;
            if (bRx < 0) bRx = 0;
            if (bRy < 0) bRy = 0;

            progressAction?.Invoke(10);

            // gets the affine scaling factors.
            float m11 = Convert.ToSingle(affine[1] * dx);
            float m22 = Convert.ToSingle(affine[5] * -dy);
            float m21 = Convert.ToSingle(affine[2] * dx);
            float m12 = Convert.ToSingle(affine[4] * -dy);
            double l = affine[0] - (.5 * (affine[1] + affine[2])); // Left of top left pixel
            double t = affine[3] - (.5 * (affine[4] + affine[5])); // top of top left pixel
            float xShift = (float)((l - mapArgs.Extent.MinX) * dx);
            float yShift = (float)((mapArgs.Extent.MaxY - t) * dy);
            mapArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            float xRatio = 1, yRatio = 1;
            if (_overviewCount > 0)
            {
                using (Band firstOverview = _band.GetOverview(0))
                {
                    xRatio = (float)firstOverview.XSize / _band.XSize;
                    yRatio = (float)firstOverview.YSize / _band.YSize;
                }
            }
            if (m11 > xRatio || m22 > yRatio)
            {
                // out of pyramids
                mapArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                _overview = -1; // don't use overviews when zooming behind the max res.
            }
            else
            {
                // estimate the pyramids that we need.
                // when using unreferenced images m11 or m22 can be negative resulting on inf logarithm.
                // so the Math.abs
                _overview = (int)Math.Min(Math.Log(Math.Abs(1 / m11), 2), Math.Log(Math.Abs(1 / m22), 2));

                // limit it to the available pyramids
                _overview = Math.Min(_overview, _overviewCount - 1);

                // additional test but probably not needed
                if (_overview < 0)
                {
                    _overview = -1;
                }
            }

            progressAction?.Invoke(15);

            var overviewPow = Math.Pow(2, _overview + 1);
            m11 *= (float)overviewPow;
            m12 *= (float)overviewPow;
            m21 *= (float)overviewPow;
            m22 *= (float)overviewPow;
            mapArgs.Graphics.Transform = new Matrix(m11, m12, m21, m22, xShift, yShift);

            int blockXsize = 0, blockYsize = 0;

            if (_overview >= 0 && _overviewCount > 0)
            {
                using (var overview = _band.GetOverview(_overview))
                {
                    overview.ComputeBlockSize(out blockXsize, out blockYsize);
                }
            }
            else
            {
                _band.ComputeBlockSize(out blockXsize, out blockYsize);
            }

            int nbX, nbY;

            // witdh and height of the image
            var w = (bRx - tLx) / overviewPow;
            var h = (bRy - tLy) / overviewPow;

            // limit the block size to the viewable image.
            if (w < blockXsize)
            {
                blockXsize = (int)Math.Ceiling(w);
                nbX = 1;
            }
            else if (w == blockXsize)
            {
                nbX = 1;
            }
            else
            {
                nbX = (int)Math.Ceiling(w / blockXsize);
            }

            if (h < blockYsize)
            {
                blockYsize = (int)Math.Ceiling(h);
                nbY = 1;
            }
            else if (h == blockYsize)
            {
                nbY = 1;
            }
            else
            {
                nbY = (int)Math.Ceiling(h / blockYsize);
            }
            int redundancy = (int)Math.Ceiling(Math.Abs(1 / Math.Min(m11, m22)));

            progressAction?.Invoke(20);

            var increment = 70.0 / nbX / nbY;
            double progressPercent = 20;

            for (var i = 0; i < nbX; i++)
            {
                for (var j = 0; j < nbY; j++)
                {
                    // The +1 is to remove the white stripes artifacts
                    double xOffsetD = (tLx / overviewPow) + (i * blockXsize);
                    double yOffsetD = (tLy / overviewPow) + (j * blockYsize);
                    int xOffsetI = (int)Math.Floor(xOffsetD);
                    int yOffsetI = (int)Math.Floor(yOffsetD);
                    int xSize = blockXsize + redundancy;
                    int ySize = blockYsize + redundancy;
                    using (var bitmap = GetBitmap(xOffsetI, yOffsetI, xSize, ySize))
                    {
                        if (bitmap != null)
                        {
                            mapArgs.Graphics.DrawImage(bitmap, xOffsetI, yOffsetI);
                        }
                    }
                    progressPercent += increment;

                    progressAction?.Invoke((int)progressPercent);
                }
            }
            progressAction?.Invoke(100);
            if (mapArgs.DestExtent.Intersects(Extent))
            {
                var destExtent = mapArgs.DestExtent.Intersection(Extent);//TODO 待测试
                ret = mapArgs.ProjToPixel(destExtent);
            }
            return ret;
        }
        private Bitmap ReadGrayIndex(int xOffset, int yOffset, int xSize, int ySize)
        {
            Bitmap result = null;
            Band firstBand;
            var disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                firstBand = _band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                firstBand = _band;
            }
            int width, height;
            firstBand.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, out width, out height);
            if (width > 0 && height > 0)
            {
                byte[] rBuffer = firstBand.ReadBand(xOffset, yOffset, width, height, width, height);
                result = GdalExtensions.GetBitmap(width, height, rBuffer, rBuffer, rBuffer, noDataValue: NoDataValue);
            }
            if (disposeBand)
            {
                firstBand.Dispose();
            }
            return result;
        }
        private Bitmap ReadRgb(int xOffset, int yOffset, int xSize, int ySize)
        {
            Bitmap result = null;
            if (rasters.Count < 3)
            {
                throw new Exception("RGB Format was indicated but there are only " + rasters.Count + " bands!");
            }
            Band rBand;
            Band gBand;
            Band bBand;
            var disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                rBand = (rasters[0] as GdalRasterSet<T>)._band.GetOverview(_overview);
                gBand = (rasters[1] as GdalRasterSet<T>)._band.GetOverview(_overview);
                bBand = (rasters[2] as GdalRasterSet<T>)._band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                rBand = (rasters[0] as GdalRasterSet<T>)._band;
                gBand = (rasters[1] as GdalRasterSet<T>)._band;
                bBand = (rasters[2] as GdalRasterSet<T>)._band;
            }

            int width, height;
            rBand.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, out width, out height);
            if (width > 0 && height > 0)
            {
                byte[] rBuffer = rBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] gBuffer = gBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] bBuffer = bBand.ReadBand(xOffset, yOffset, width, height, width, height);
                result = GdalExtensions.GetBitmap(width, height, rBuffer, gBuffer, bBuffer, noDataValue: NoDataValue);
            }
            if (disposeBand)
            {
                rBand.Dispose();
                gBand.Dispose();
                bBand.Dispose();
            }
            return result;
        }
        private Bitmap ReadRgba(int xOffset, int yOffset, int xSize, int ySize)
        {
            Bitmap result = null;
            if (rasters.Count < 4)
            {
                throw new Exception("ARGB Format was indicated but there are only " + rasters.Count + " bands!");
            }
            Band aBand;
            Band rBand;
            Band gBand;
            Band bBand;
            var disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                rBand = (rasters[0] as GdalRasterSet<T>)._band.GetOverview(_overview);
                gBand = (rasters[1] as GdalRasterSet<T>)._band.GetOverview(_overview);
                bBand = (rasters[2] as GdalRasterSet<T>)._band.GetOverview(_overview);
                aBand = (rasters[3] as GdalRasterSet<T>)._band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                rBand = (rasters[0] as GdalRasterSet<T>)._band;
                gBand = (rasters[1] as GdalRasterSet<T>)._band;
                bBand = (rasters[2] as GdalRasterSet<T>)._band;
                aBand = (rasters[3] as GdalRasterSet<T>)._band;
            }

            rBand.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, out int width, out int height);
            if (width > 0 && height > 0)
            {
                byte[] aBuffer = aBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] rBuffer = rBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] gBuffer = gBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] bBuffer = bBand.ReadBand(xOffset, yOffset, width, height, width, height);
                result = GdalExtensions.GetBitmap(width, height, rBuffer, gBuffer, bBuffer, aBuffer, NoDataValue);
            }
            if (disposeBand)
            {
                aBand.Dispose();
                rBand.Dispose();
                gBand.Dispose();
                bBand.Dispose();
            }
            return result;
        }

        private Bitmap ReadArgb(int xOffset, int yOffset, int xSize, int ySize)
        {
            Bitmap result = null;
            if (rasters.Count < 4)
            {
                throw new Exception("ARGB Format was indicated but there are only " + rasters.Count + " bands!");
            }
            Band aBand;
            Band rBand;
            Band gBand;
            Band bBand;
            var disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                aBand = (rasters[0] as GdalRasterSet<T>)._band.GetOverview(_overview);
                rBand = (rasters[1] as GdalRasterSet<T>)._band.GetOverview(_overview);
                gBand = (rasters[2] as GdalRasterSet<T>)._band.GetOverview(_overview);
                bBand = (rasters[3] as GdalRasterSet<T>)._band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                aBand = (rasters[0] as GdalRasterSet<T>)._band;
                rBand = (rasters[1] as GdalRasterSet<T>)._band;
                gBand = (rasters[2] as GdalRasterSet<T>)._band;
                bBand = (rasters[3] as GdalRasterSet<T>)._band;
            }

            rBand.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, out int width, out int height);
            if (width > 0 && height > 0)
            {
                byte[] aBuffer = aBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] rBuffer = rBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] gBuffer = gBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] bBuffer = bBand.ReadBand(xOffset, yOffset, width, height, width, height);
                result = GdalExtensions.GetBitmap(width, height, rBuffer, gBuffer, bBuffer, aBuffer, NoDataValue);
            }
            if (disposeBand)
            {
                aBand.Dispose();
                rBand.Dispose();
                gBand.Dispose();
                bBand.Dispose();
            }
            return result;
        }
        private Bitmap ReadPaletteBuffered(int xOffset, int yOffset, int xSize, int ySize)
        {
            Bitmap result = null;
            ColorTable ct = _band.GetRasterColorTable();
            if (ct == null)
            {
                throw new Exception("Image was stored with a palette interpretation but has no color table.");
            }

            if (ct.GetPaletteInterpretation() != PaletteInterp.GPI_RGB)
            {
                throw new Exception("Only RGB palette interpretation is currently supported by this " + " plug-in, " + ct.GetPaletteInterpretation() + " is not supported.");
            }

            int count = ct.GetCount();
            byte[][] colorTable = new byte[ct.GetCount()][];
            for (int i = 0; i < count; i++)
            {
                using (ColorEntry ce = ct.GetColorEntry(i))
                {
                    colorTable[i] = new[] { (byte)ce.c4, (byte)ce.c1, (byte)ce.c2, (byte)ce.c3 };
                }
            }
            ct.Dispose();

            Band firstBand;
            bool disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                firstBand = _band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                firstBand = _band;
            }

            int width, height;
            firstBand.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, out width, out height);
            if (width > 0 && height > 0)
            {
                byte[] indexBuffer = firstBand.ReadBand(xOffset, yOffset, width, height, width, height);
                byte[] rBuffer = new byte[indexBuffer.Length];
                byte[] gBuffer = new byte[indexBuffer.Length];
                byte[] bBuffer = new byte[indexBuffer.Length];
                byte[] aBuffer = new byte[indexBuffer.Length];
                for (int i = 0; i < indexBuffer.Length; i++)
                {
                    int index = indexBuffer[i];
                    aBuffer[i] = colorTable[index][0];
                    rBuffer[i] = colorTable[index][1];
                    gBuffer[i] = colorTable[index][2];
                    bBuffer[i] = colorTable[index][3];
                }
                result = GdalExtensions.GetBitmap(width, height, rBuffer, gBuffer, gBuffer, aBuffer, NoDataValue);
            }
            if (disposeBand)
            {
                firstBand.Dispose();
            }
            return result;
        }

        private Bitmap GetBitmap(int xOffset, int yOffset, int xSize, int ySize)
        {
            Bitmap result = null;
            Action action = new Action(() =>
            {
                switch (BandCount)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                        result = ReadGrayIndex(xOffset, yOffset, xSize, ySize);
                        break;
                    case 3:
                        result = ReadRgb(xOffset, yOffset, xSize, ySize);
                        break;
                    default:
                        switch (_colorInterp)
                        {
                            case ColorInterp.GCI_RedBand:
                                result = ReadRgba(xOffset, yOffset, xSize, ySize);
                                break;
                            case ColorInterp.GCI_AlphaBand:
                                result = ReadArgb(xOffset, yOffset, xSize, ySize);
                                break;
                        }
                        break;
                }
            });
            switch (_colorInterp)
            {
                case ColorInterp.GCI_PaletteIndex:
                    result = ReadPaletteBuffered(xOffset, yOffset, xSize, ySize);
                    break;
                case ColorInterp.GCI_GrayIndex:
                    result = ReadGrayIndex(xOffset, yOffset, xSize, ySize);
                    break;
                case ColorInterp.GCI_RedBand:
                    result = ReadRgb(xOffset, yOffset, xSize, ySize);
                    break;
                case ColorInterp.GCI_AlphaBand:
                    result = ReadArgb(xOffset, yOffset, xSize, ySize);
                    break;
                default:
                    action.Invoke();
                    break;
            }
            // data set disposed on disposing this image
            return result;
        }
        /// <inheritdoc/>
        public override Color[] CategoryColors()
        {
            Color[] colors = null;
            ColorTable table = GetColorTable();
            if (table != null)
            {
                int colorCount = table.GetCount();
                if (colorCount > 0)
                {
                    colors = new Color[colorCount];
                    for (int colorIndex = 0; colorIndex < colorCount; colorIndex += 1)
                    {
                        colors[colorIndex] = Color.DimGray;
                        ColorEntry entry = table.GetColorEntry(colorIndex);
                        switch (table.GetPaletteInterpretation())
                        {
                            case PaletteInterp.GPI_RGB:
                                colors[colorIndex] = Color.FromArgb(entry.c4, entry.c1, entry.c2, entry.c3);
                                break;
                            case PaletteInterp.GPI_Gray:
                                colors[colorIndex] = Color.FromArgb(255, entry.c1, entry.c1, entry.c1);
                                break;

                                // TODO: do any files use these types?
                                // case PaletteInterp.GPI_HLS
                                // case PaletteInterp.GPI_CMYK
                        }
                    }
                }
            }

            return colors;
        }

        /// <inheritdoc/>
        public override string[] CategoryNames()
        {
            if (_band != null)
            {
                return _band.GetCategoryNames();
            }

            foreach (GdalRasterSet<T> raster in Rasters)
            {
                return raster._band.GetCategoryNames();
            }

            return null;
        }

        /// <inheritdoc/>
        public override Statistics GetStatistics()
        {
            Statistics statistics = new Statistics();
            if (_band != null)
            {
                double min, max, mean, std;
                CPLErr err;
                try
                {
                    err = _band.GetStatistics(0, 1, out min, out max, out mean, out std);
                    if (err != CPLErr.CE_None)
                    {
                        err = _band.ComputeStatistics(false, out min, out max, out mean, out std, null, null);
                    }
                    statistics.Minimum = min;
                    statistics.Maximum = max;
                    statistics.Mean = mean;
                    statistics.StdDeviation = std;
                }
                catch (Exception ex)
                {
                    err = CPLErr.CE_Failure;
                    max = min = std = mean = 0;
                    Trace.WriteLine(ex);
                }

                // http://dotspatial.codeplex.com/workitem/22221
                // GetStatistics didn't return anything, so try use the raster default method.
                if (err != CPLErr.CE_None || (max == 0 && min == 0 && std == 0 && mean == 0)) base.GetStatistics();
            }
            else
            {
                // ?? doesn't this mean the stats get overwritten several times.
                foreach (IRasterSet raster in Rasters)
                {
                    statistics = raster.GetStatistics();
                }
            }
            return statistics;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)。
                    if (rasters.Count > 0)
                    {
                        foreach (IRasterSet raster in Rasters)
                        {
                            raster.Dispose();
                        }
                        rasters.Clear();
                        _dataset.Dispose();
                    }
                }
                // 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // 将大型字段设置为 null。
                if (_band != null)
                {
                    _band.Dispose();
                    _band = null;
                }
            }
            base.Dispose(disposing);
        }

        private ColorTable GetColorTable()
        {
            if (_band != null)
            {
                return _band.GetColorTable();
            }

            foreach (GdalRasterSet<T> raster in Rasters)
            {
                return raster._band.GetColorTable();
            }

            return null;
        }

        private void ReadHeader()
        {
            // Todo: look for prj file if GetProjection returns null.
            // Do we need to read this as an Esri string if we don't get a proj4 string?
            if (_band != null)
            {
                RasterType = _band.DataType.ToRasterType();
                double val;
                _band.GetNoDataValue(out val, out int hasVal);
                if (hasVal == 1)
                {
                    NoDataValue = val;
                }
                _overviewCount = _band.GetOverviewCount();
                _colorInterp = _band.GetColorInterpretation();
                int maxPixels = 2048 * 2048;
                //if (_overviewCount <= 0 && Width * Height > maxPixels)
                //{
                //    int ret = _dataset.CreateOverview();
                //    _overviewCount = _band.GetOverviewCount();
                //}
            }

            double[] affine = new double[6];
            _dataset.GetGeoTransform(affine);

            // in gdal (row,col) coordinates are defined relative to the top-left corner of the top-left cell
            // shift them by half a cell to give coordinates relative to the center of the top-left cell
            affine = new AffineTransform(affine).TransfromToCorner(0.5, 0.5);
            Bounds = new RasterBounds(Height, Width, affine);
            PixelSpace = Marshal.SizeOf(typeof(T));
        }
        /// <inheritdoc/>
        public override void SetGeoTransform(double[] affine)
        {
            _dataset.SetGeoTransform(affine);
        }
        #endregion
        /// <inheritdoc/>
        public override void WriteRaster(string filename, RasterArgs readArgs,RasterArgs writeArgs)
        {
            try
            {
                using var dataset = Gdal.Open(filename, Access.GA_ReadOnly);
                if (dataset == null)
                {
                    return;
                }
                byte[] buffer = new byte[readArgs.BufferXSize * readArgs.BufferYSize * readArgs.BandCount];
                var err = dataset.ReadRaster(readArgs.XOff, readArgs.YOff, readArgs.XSize, readArgs.YSize, buffer, readArgs.BufferXSize , readArgs.BufferYSize, readArgs.BandCount, readArgs.BandMap, readArgs.PixelSpace, readArgs.LineSpace, readArgs.BandSpace);
                if (err == CPLErr.CE_None)
                {
                    err = _dataset.WriteRaster(writeArgs.XOff, writeArgs.YOff, writeArgs.XSize, writeArgs.YSize, buffer, writeArgs.BufferXSize, writeArgs.BufferYSize, writeArgs.BandCount, writeArgs.BandMap, writeArgs.PixelSpace, writeArgs.LineSpace, writeArgs.BandSpace); 
                    if (err != CPLErr.CE_None)
                    {
                        Debug.WriteLine($"写入{filename}_destXOff:{writeArgs.XOff}_destYOff:{writeArgs.YOff}_destWidth:{writeArgs.XSize}_destHeight:{writeArgs.YSize}失败，{err}");
                    }
                }
                else
                {
                    Debug.WriteLine($"读取{filename}_srcXOff:{readArgs.XOff}_srcYOff:{readArgs.YOff}_srcWidth:{readArgs.BufferXSize}_srcHeight:{readArgs.BufferYSize}失败");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(WriteRaster)} 失败，{e}");
            }
        }
        /// <inheritdoc/>
        public override void Save()
        {
            _dataset.FlushCache();
            base.Save();
        }
        /// <inheritdoc/>
        public override void SaveAs(string filename, bool overwrite)
        {
            using var dataset = _dataset.GetDriver().CreateCopy(filename, _dataset, 1, null, null, null);
            base.SaveAs(filename, overwrite);
        }
        /// <inheritdoc/>
        public override void BuildOverviews(int minWidth = 2560, int minHeight = 2560)
        {
            var ret= _dataset.BuildOverviews(minWidth:minWidth,minHeight:minHeight);
        }
    }
}