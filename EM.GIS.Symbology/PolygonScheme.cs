using System;
using System.Collections.Generic;
using System.Drawing;



namespace EM.GIS.Symbology
{
    public class PolygonScheme : FeatureScheme, IPolygonScheme
    {
        public new IPolygonCategoryCollection Categories { get => base.Categories as IPolygonCategoryCollection; set => base.Categories = value; }

        public override int Count => Categories.Count;

        public PolygonScheme()
        {
            Categories = new PolygonCategoryCollection();
        }


        public override ICategory CreateNewCategory(Color fillColor, float size)
        {
            throw new NotImplementedException();
        }

        public override IFeatureCategory CreateRandomCategory(string filterExpression)
        {
            throw new NotImplementedException();
        }


        public override void DrawCategory(int index, Graphics context, Rectangle bounds)
        {
            Categories[index].Symbolizer.DrawLegend(context, bounds);
        }

        public override void Add(ICategory item)
        {
            if (item is IPolygonCategory polygonCategory)
            {
                Categories.Add(polygonCategory);
            }
        }

        public override void Clear()
        {
            Categories.Clear();
        }

        public override bool Contains(ICategory item)
        {
            if (item is IPolygonCategory polygonCategory)
            {
                return Categories.Contains(polygonCategory);
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
            return Categories.Remove(item as IPolygonCategory );
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
