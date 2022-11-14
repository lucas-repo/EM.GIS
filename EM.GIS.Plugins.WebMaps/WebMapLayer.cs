using EM.GIS.Geometries;
using EM.GIS.Symbology;
using System.Drawing;

namespace EM.GIS.Plugins.WebMaps
{
    /// <summary>
    /// 在线地图图层
    /// </summary>
    public class WebMapLayer : Layer
    {
        public new TileImageSet? DataSet
        {
            get=>base.DataSet as TileImageSet;
            set=>base.DataSet = value;
        }
        public override IExtent Extent { get => base.Extent; set => base.Extent=value; }
        public WebMapLayer(IFrame frame)
        {
            
        }
        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null)
        {
            if (selected || cancelFunc?.Invoke() == true)
            {
                return;
            }
            using (var bmp = DataSet.GetImage(extent, rectangle, ReportProgress))
            {
                if (bmp != null)
                {
                    graphics.DrawImage(bmp, rectangle);
                }
            }
        }
        private void ReportProgress(int progress)
        {
            int minProgress = 0;
            int maxProgress = 80;
            double progressD = progress / 100.0 * (maxProgress - minProgress);
            ProgressHandler?.Progress((int)progressD, ProgressMessage);
        }
    }
}