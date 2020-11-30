using System;
using System.Collections.Generic;
using System.Drawing;



namespace EM.GIS.Symbology
{
    public class PointScheme : FeatureScheme, IPointScheme
    {
        public new IPointCategoryCollection Categories { get => base.Categories as IPointCategoryCollection; set => base.Categories = value; }

        public override int Count => Categories.Count;

        public PointScheme()
        {
            Categories = new PointCategoryCollection();
        }

        public override ICategory CreateNewCategory(Color fillColor, float size)
        {
            IPointSymbolizer ps = EditorSettings.TemplateSymbolizer.Clone() as IPointSymbolizer ?? new PointSymbolizer(fillColor, PointShape.Ellipse, size);
            ps.Symbols[0].Color = fillColor;
            SizeF oSize = ps.Size;
            float rat = size / Math.Max(oSize.Width, oSize.Height);
            ps.Size = new SizeF(rat * oSize.Width, rat * oSize.Height);
            return new PointCategory(ps);
        }

        public override IFeatureCategory CreateRandomCategory(string filterExpression)
        {
            PointCategory result = new PointCategory();
            var fillColor = CreateRandomColor();
            result.Symbolizer = new PointSymbolizer(fillColor, PointShape.Ellipse, 10);
            result.FilterExpression = filterExpression;
            result.Text = filterExpression;
            return result;
        }


        public override void DrawCategory(int index, Graphics context, Rectangle bounds)
        {
            Categories[index].Symbolizer.DrawLegend(context, bounds);
        }

        public override void Add(ICategory item)
        {
            if (item is IPointCategory pointCategory)
            {
                Categories.Add(pointCategory);
            }
        }

        public override void Clear()
        {
            Categories.Clear();
        }

        public override bool Contains(ICategory item)
        {
            if (item is IPointCategory pointCategory)
            {
                return Categories.Contains(pointCategory);
            }
            else
            {
                return false;
            }

        }

        public override void CopyTo(ICategory[] array, int arrayIndex)
        {
            int k = 0;
            for (int i = arrayIndex; i < Categories.Count; i++)
            {
                array[i] = Categories[i];
                k++;
            }
        }

        public override bool Remove(ICategory item)
        {
            return  Categories.Remove(item as IPointCategory);
        }

        public override IEnumerator<ICategory> GetEnumerator()
        {
            return Categories.GetEnumerator();
        }

        public override void Move(int oldIndex, int newIndex)
        {
            Categories.Move(oldIndex, newIndex);
        }
    }
}
