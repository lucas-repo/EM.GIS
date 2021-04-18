﻿using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public class LineLayer : FeatureLayer, ILineLayer
    {
        public new ILineCategory DefaultCategory
        {
            get => base.DefaultCategory as ILineCategory;
            set => base.DefaultCategory = value;
        }
        public new ILineCategoryCollection Categories { get => LegendItems as ILineCategoryCollection; }
        public LineLayer(IFeatureSet featureSet) : base(featureSet)
        {
            LegendItems = new LineCategoryCollection(this);
            DefaultCategory = new LineCategory()
            {
                Text = "默认"
            };
        }

        protected override void DrawGeometry(MapArgs drawArgs, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            if (drawArgs == null || !(symbolizer is ILineSymbolizer lineSymbolizer) || geometry == null)
            {
                return;
            }
            float scaleSize = (float)symbolizer.GetScale(drawArgs);
            using (GraphicsPath path = new GraphicsPath())
            {
                GetLines(drawArgs, geometry, path);
                lineSymbolizer.DrawLine(drawArgs.Device, scaleSize, path);
            }
        }
        private void GetLines(MapArgs drawArgs, IGeometry geometry, GraphicsPath path)
        {
            if (geometry.Geometries.Count == 0)
            {
                int pointCount = geometry.Coordinates.Count;
                PointF[] points = new PointF[pointCount];
                for (int j = 0; j < pointCount; j++)
                {
                    var coord = geometry.Coordinates[j];
                    PointF point = drawArgs.CoordinateToPointF(coord);
                    points[j] = point;
                }
                path.AddLines(points);
            }
            else
            {
                int geoCount = geometry.Geometries.Count;
                for (int i = 0; i < geoCount; i++)
                {
                    var partGeo = geometry.Geometries[i];
                    path.StartFigure();
                    GetLines(drawArgs, partGeo, path);
                }
            }
        }
    }
}