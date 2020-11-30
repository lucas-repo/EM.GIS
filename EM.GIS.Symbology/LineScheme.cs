using System.Collections.Generic;
using System.Drawing;

namespace EM.GIS.Symbology
{
    public class LineScheme:FeatureScheme,ILineScheme
    {
        public new ILineCategoryCollection Categories { get => base.Categories as ILineCategoryCollection; set => base.Categories = value; }

        public override int Count => Categories.Count;

        public LineScheme()
        {
        }

        public override ICategory CreateNewCategory(Color fillColor, float size)
        {
            ILineSymbolizer ls = EditorSettings.TemplateSymbolizer.Clone() as ILineSymbolizer;
            if (ls != null)
            {
                ls.Color = fillColor;
                ls.Width = size;
            }
            else
            {
                ls = new LineSymbolizer(fillColor, size);
            }

            return new LineCategory(ls);
        }

        public override IFeatureCategory CreateRandomCategory(string filterExpression)
        {
            LineCategory result = new LineCategory();
            var fillColor = CreateRandomColor();
            result.Symbolizer = new LineSymbolizer(fillColor, 2);
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
            if (item is ILineCategory lineCategory)
            {
                Categories.Add(lineCategory);
            }
        }

        public override void Clear()
        {
            Categories.Clear();
        }

        public override bool Contains(ICategory item)
        {
            if (item is ILineCategory lineCategory)
            {
                return Categories.Contains(lineCategory);
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
            return Categories.Remove(item as ILineCategory);
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
