﻿using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace EM.GIS.Symbology
{
    public class LineLayer : FeatureLayer, ILineLayer
    {
        public new ILineCategory DefaultCategory
        {
            get => base.DefaultCategory as ILineCategory;
            set => base.DefaultCategory = value;
        }
        public new ILineCategoryCollection Children { get => base.Children as ILineCategoryCollection; protected set => base.Children = value; }
        public LineLayer(IFeatureSet featureSet) : base(featureSet)
        {
            Children = new LineCategoryCollection(this);
            DefaultCategory = new LineCategory()
            {
                Text = "默认"
            };
        }

        protected override void DrawGeometry(IProj proj, Graphics graphics, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            if (!(symbolizer is ILineSymbolizer lineSymbolizer))
            {
                return;
            }
            float scaleSize = (float)symbolizer.GetScale(proj);
            using (GraphicsPath path = new GraphicsPath())
            {
                GetLines(proj, geometry, path);
                lineSymbolizer.DrawLine(graphics, scaleSize, path);
            }
        }
        private void GetLines(IProj proj, IGeometry geometry, GraphicsPath path)
        {
            if (geometry.Geometries.Count == 0)
            {
                int pointCount = geometry.Coordinates.Count;
                PointF[] points = new PointF[pointCount];
                for (int j = 0; j < pointCount; j++)
                {
                    var coord = geometry.Coordinates[j];
                    PointF point = proj.ProjToPixelF(coord);
                    points[j] = point;
                }

                //去重
                var intPoints = DuplicationPreventer.Clean(points).ToArray();
                if (intPoints.Length >= 2)
                {
                    path.AddLines(intPoints);
                }
            }
            else
            {
                int geoCount = geometry.Geometries.Count;
                for (int i = 0; i < geoCount; i++)
                {
                    var partGeo = geometry.Geometries[i];
                    path.StartFigure();
                    GetLines(proj, partGeo, path);
                }
            }
        }
    }
}