using BruTile;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Plugins.WebMaps
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public class TileImageSet : DataSet, IGetImage
    {
        /// <summary>
        /// 瓦片集合
        /// </summary>
        public Dictionary<TileIndex, ImageSet> Tiles { get; }
        public override IExtent Extent
        {
            get
            {
                IExtent extent = new Geometries.Extent();
                foreach (var item in Tiles)
                {
                    if (item.Value?.Extent!=null)
                    {
                        extent.ExpandToInclude(item.Value.Extent);
                    }
                }
                return extent;
            }
        }

        public TileImageSet()
        {
            Tiles = new Dictionary<TileIndex, ImageSet>();
        }
        public Image GetImage()
        {
            return null;
        }

        public Image GetImage(IExtent envelope, Rectangle window, Action<int> progressAction = null)
        {
            Bitmap bitmap = null;
            if (Tiles.Count==0|| envelope==null|| window.Width == 0 || window.Height == 0)
            {
                return bitmap;
            }
            try
            {
                bitmap = new Bitmap(window.Width, window.Height);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (bitmap==null)
            {
                return bitmap;
            }
            using (var g = Graphics.FromImage(bitmap))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                double dx = window.Width / envelope.Width;
                double dy = window.Height / envelope.Height;
                foreach (var item in Tiles)
                {
                    try
                    {
                        using (Image tmpImg = item.Value.GetImage(envelope, window, progressAction))
                        {
                            if (tmpImg==null)
                            {
                                continue;
                            }
                            double[] a = item.Value.Bounds.AffineCoefficients;

                            // gets the affine scaling factors.
                            float m11 = Convert.ToSingle(a[1] * dx);
                            float m22 = Convert.ToSingle(a[5] * -dy);
                            float m21 = Convert.ToSingle(a[2] * dx);
                            float m12 = Convert.ToSingle(a[4] * -dy);
                            double l = a[0] - (.5 * (a[1] + a[2])); // Left of top left pixel
                            double t = a[3] - (.5 * (a[4] + a[5])); // top of top left pixel
                            float xShift = (float)((l - envelope.MinX) * dx);
                            float yShift = (float)((envelope.MaxY - t) * dy);
                            g.Transform = new Matrix(m11, m12, m21, m22, xShift, yShift);
                            if (m11 > 1 || m22 > 1) g.InterpolationMode = InterpolationMode.NearestNeighbor;
                            if (!g.VisibleClipBounds.IsEmpty) g.DrawImage(item.Value.Bitmap, new PointF(0, 0));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return bitmap;
        }

        public Image GetImage(IExtent envelope, Size size)
        {
            return GetImage(envelope, new Rectangle(new Point(0, 0), size));
        }

    }
}
