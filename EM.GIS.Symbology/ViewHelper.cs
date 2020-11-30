using EM.GIS.Geometries;
using System.Drawing;

namespace EM.GIS.Symbology
{
    public static class ViewHelper
    {
        /// <summary>
        /// 通过指定的视图计算视图几何范围
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="view"></param>
        public static void GetViewEnvelope(IExtent envelope,Rectangle view)
        {
            // Aspect Ratio Handling
            if (envelope == null) return;
            double h = view.Height;
            double w = view.Width;

            // It isn't exactly an exception, but rather just an indication not to do anything here.
            if (h == 0 || w == 0) return;

            double controlAspect = w / h;
            double envWidth = envelope.Width;
            double envHeight = envelope.Height;
            double envelopeAspect = envWidth / envHeight;
            var center = envelope.Center;

            if (controlAspect > envelopeAspect)
            {
                // The Control is proportionally wider than the envelope to display.
                // If the envelope is proportionately wider than the control, "reveal" more width without
                // changing height If the envelope is proportionately taller than the control,
                // "hide" width without changing height
                envelope.SetCenter(center, envHeight * controlAspect, envHeight);
            }
            else
            {
                // The control is proportionally taller than the content is
                // If the envelope is proportionately wider than the control,
                // "hide" the extra height without changing width
                // If the envelope is proportionately taller than the control, "reveal" more height without changing width
                envelope.SetCenter(center, envWidth, envWidth / controlAspect);
            }
        }
    }
}
