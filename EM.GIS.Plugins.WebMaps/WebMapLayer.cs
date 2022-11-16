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
        /// <summary>
        /// 瓦片数据集
        /// </summary>
        public new TileImageSet? DataSet
        {
            get=>base.DataSet as TileImageSet;
            set=>base.DataSet = value;
        }
        /// <inheritdoc/>
        public override IExtent Extent { get => base.Extent; set => base.Extent=value; }
        public WebMapLayer(IFrame frame)
        {
            
        }
        /// <inheritdoc/>
        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null)
        {
            if (DataSet==null|| selected || cancelFunc?.Invoke() == true)
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
            Progress?.Invoke((int)progressD, ProgressMessage);
        }
    }
}