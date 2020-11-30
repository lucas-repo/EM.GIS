using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    [Serializable]
    public class LineMarkerSymbol : LineCartographicSymbol, ILineMarkerSymbol
    {
        public IPointSymbolizer Marker { get; set; }
        public LineMarkerSymbol() : base(LineSymbolType.Marker)
        {
            Marker = new PointSymbolizer();
            throw new NotImplementedException();//todo 待完善
        }
        public override Pen ToPen(float scale)
        {
            Pen pen = ToPen(scale, 0);
            return pen;
        }
        public Pen ToPen(float scale, float angle)
        {
            Pen pen = null;
            if (Marker != null)
            {
                pen = base.ToPen(scale);
            }
            else
            {
                float width = GetWidth(scale);
                Brush brush = null;
                if (Marker != null)
                {
                    SizeF size = Marker.Size;
                    int imgWidth = (int)Math.Ceiling(size.Width);
                    int imgHeight = (int)Math.Ceiling(size.Height);
                    Bitmap image = new Bitmap(imgWidth, imgHeight);
                    Rectangle rectangle = new Rectangle(0, 0, imgWidth, imgHeight);
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.RotateTransform(angle);//todo 测试角度
                        Marker.DrawLegend(g, rectangle);
                    }
                    brush = new TextureBrush(image);
                }
                pen = new Pen(brush, width)
                {
                    DashPattern = DashPattern
                };
            }
            return pen;
        }
        private float GetWidth(float scale)
        {
            float width = Width;
            if (Marker != null)
            {
                width = Marker.Size.Height;
            }
            width *= scale;
            return width;
        }
        public Pen ToPen(float scale, PointF startPoint, PointF endPoint)
        {
            Pen pen = null;
            if (Marker == null)
            {
                pen = base.ToPen(scale);
            }
            else
            {
                float width = GetWidth(scale);
                SizeF size = Marker.Size;
                int imgWidth = (int)Math.Ceiling(size.Width);
                int imgHeight = (int)Math.Ceiling(size.Height);
                Bitmap image = new Bitmap(imgWidth, imgHeight);
                Rectangle rectangle = new Rectangle(0, 0, imgWidth, imgHeight);
                float angle = DrawingHelper.GetAngle(startPoint, endPoint, true);
                using (Graphics g = Graphics.FromImage(image))
                {
                    Marker.DrawLegend(g, rectangle);
                }
                var brush = new TextureBrush(image);
                //brush.RotateTransform(angle);
                float[] dashPattern = ToDashPattern(DashPattern, startPoint, endPoint);
                pen = new Pen(brush, width);
                if (dashPattern != null)
                {
                    pen.DashStyle = DashStyle.Custom;
                    pen.DashPattern = new float[] { 1 };
                }
            }
            return pen;
        }
        public float[] ToDashPattern(float[] pattern, PointF startPoint, PointF endPoint)
        {
            float[] dashPattern = null;
            if (Marker == null)
            {
                dashPattern = pattern;
            }
            else
            {
                if (pattern != null)
                {
                    switch (pattern.Length)
                    {
                        case 0:
                            dashPattern = pattern;
                            break;
                        case 1:
                        default:
                            SizeF size = Marker.Size;
                            int imgWidth = (int)Math.Ceiling(size.Width);
                            int imgHeight = (int)Math.Ceiling(size.Height);
                            switch (pattern.Length)
                            {
                                case 1:
                                    float angle = DrawingHelper.GetAngle(startPoint, endPoint, false);
                                    float totalLength = Convert.ToSingle(DrawingHelper.Distance(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y));
                                    float centerX = (startPoint.X + endPoint.X) / 2;
                                    float centerY = (startPoint.Y + endPoint.Y) / 2;
                                    float dy = Convert.ToSingle(imgWidth / 2.0 * Math.Sin(angle));
                                    float dx = Convert.ToSingle(-imgWidth / 2.0 * Math.Cos(angle));
                                    float x = centerX - dx;
                                    float y = centerY - dy;
                                    float firstLength = Convert.ToSingle(DrawingHelper.Distance(startPoint.X, startPoint.Y, x, y));
                                    float secondLength = imgWidth;
                                    float thirdLength = totalLength - firstLength - secondLength;
                                    if (thirdLength > 0)
                                    {
                                        dashPattern = new float[]
                                        {
                                    0,firstLength,secondLength,thirdLength
                                        };
                                    }
                                    break;
                                default:
                                    dashPattern = new float[pattern.Length];
                                    for (int i = 0; i < pattern.Length; i++)
                                    {
                                        if (i % 2 == 0)
                                        {
                                            dashPattern[i] = pattern[i] * imgWidth;
                                        }
                                        else
                                        {
                                            dashPattern[i] = pattern[i];
                                        }
                                        float item = pattern[i];
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return dashPattern;
        }
        public override void DrawLine(Graphics context, float scale, GraphicsPath path)
        {
            if (Marker == null)
            {
                base.DrawLine(context, scale, path);
            }
            else
            {
                for (int i = 0; i < path.PointCount - 1; i++)
                {
                    PointF point0 = path.PathPoints[i];
                    PointF point1 = path.PathPoints[i + 1];
                    Pen pen = ToPen(scale, point0, point1);
                    context.DrawLine(pen, point0, point1);
                }

            }
        }
    }
}
