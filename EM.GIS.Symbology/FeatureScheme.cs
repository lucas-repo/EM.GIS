using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace EM.GIS.Symbology
{
    public abstract class FeatureScheme : Scheme, IFeatureScheme
    {
        public new FeatureEditorSettings EditorSettings
        {
            get
            {
                return base.EditorSettings as FeatureEditorSettings;
            }
            set
            {
                base.EditorSettings = value;
            }
        }

        //public new IFeatureLayer Parent { get => base.Parent as IFeatureLayer; set => base.Parent = value; }
        public new IFeatureCategoryCollection Categories { get => base.Categories as IFeatureCategoryCollection; set => base.Categories = value; }

        private static List<Break> GetUniqueValues(string fieldName, DataTable table)
        {
            HashSet<object> lst = new HashSet<object>();
            bool containsNull = false;
            foreach (DataRow dr in table.Rows)
            {
                object val = dr[fieldName];
                if (val == null || dr[fieldName] is DBNull || val.ToString() == string.Empty)
                {
                    containsNull = true;
                }
                else if (!lst.Contains(val))
                {
                    lst.Add(val);
                }
            }

            List<Break> result = new List<Break>();
            if (containsNull) result.Add(new Break("[NULL]"));
            foreach (object item in lst.OrderBy(o => o))
            {
                result.Add(new Break(string.Format("{0}", item)));
            }

            return result;
        }
        private static bool CheckFieldType(string fieldName, DataTable table)
        {
            return table.Columns[fieldName].DataType == typeof(string);
        }
        protected Color CreateRandomColor()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return CreateRandomColor(rnd);
        }
        private void CreateUniqueCategories(string fieldName, DataTable table)
        {
            Breaks = GetUniqueValues(fieldName, table);
            List<float> sizeRamp = GetSizeSet(Breaks.Count);
            List<Color> colorRamp = GetColorSet(Breaks.Count);
            string fieldExpression = "[" + fieldName.ToUpper() + "]";
            Clear();

            bool isStringField = CheckFieldType(fieldName, table);

            int colorIndex = 0;

            foreach (Break brk in Breaks)
            {
                // get the color for the category
                Color randomColor = colorRamp[colorIndex];
                float size = sizeRamp[colorIndex];
                IFeatureCategory cat = CreateNewCategory(randomColor, size) as IFeatureCategory;

                if (cat != null)
                {
                    cat.Text = brk.Name;

                    if (isStringField) cat.FilterExpression = fieldExpression + "= '" + brk.Name.Replace("'", "''") + "'";
                    else cat.FilterExpression = fieldExpression + "=" + brk.Name;
                    if (cat.FilterExpression != null)
                    {
                        if (cat.FilterExpression.Contains("=[NULL]"))
                        {
                            cat.FilterExpression = cat.FilterExpression.Replace("=[NULL]", " is NULL");
                        }
                        else if (cat.FilterExpression.Contains("= '[NULL]'"))
                        {
                            cat.FilterExpression = cat.FilterExpression.Replace("= '[NULL]'", " is NULL");
                        }
                    }

                    Add(cat);
                }

                colorIndex++;
            }
        }
        public void CreateCategories(DataTable table)
        {
            string fieldName = EditorSettings.FieldName;
            if (EditorSettings.ClassificationType == ClassificationType.Custom) return;

            if (EditorSettings.ClassificationType == ClassificationType.UniqueValues)
            {
                CreateUniqueCategories(fieldName, table);
            }
            else
            {
                if (table.Columns[fieldName].DataType == typeof(string))
                {
                    return;
                }

                if (GetUniqueValues(fieldName, table).Count <= EditorSettings.NumBreaks)
                {
                    CreateUniqueCategories(fieldName, table);
                }
                else
                {
                    GetValues(table);
                    CreateBreakCategories();
                }
            }

            Text = fieldName;
        }
        public void GetValues(DataTable table)
        {
            Values = new List<double>();
            string normField = EditorSettings.NormField;
            string fieldName = EditorSettings.FieldName;
            if (!string.IsNullOrEmpty(EditorSettings.ExcludeExpression))
            {
                DataRow[] rows = table.Select("NOT (" + EditorSettings.ExcludeExpression + ")");
                foreach (DataRow row in rows)
                {
                    if (rows.Length < EditorSettings.MaxSampleCount)
                    {
                        double val;
                        if (!double.TryParse(row[fieldName].ToString(), out val)) continue;
                        if (double.IsNaN(val)) continue;

                        if (normField != null)
                        {
                            double norm;
                            if (!double.TryParse(row[normField].ToString(), out norm) || double.IsNaN(val)) continue;

                            Values.Add(val / norm);
                            continue;
                        }

                        Values.Add(val);
                    }
                    else
                    {
                        Dictionary<int, double> randomValues = new Dictionary<int, double>();
                        int count = EditorSettings.MaxSampleCount;
                        int max = rows.Length;

                        // Specified seed is required for consistently recreating the break values
                        Random rnd = new Random(9999);

                        for (int i = 0; i < count; i++)
                        {
                            double val;
                            double norm = 1;
                            int index;
                            bool failed = false;
                            do
                            {
                                index = rnd.Next(max);
                                if (!double.TryParse(rows[index][fieldName].ToString(), out val)) failed = true;
                                if (normField == null) continue;

                                if (!double.TryParse(rows[index][normField].ToString(), out norm)) failed = true;
                            }
                            while (randomValues.ContainsKey(index) || double.IsNaN(val) || failed);

                            if (normField != null)
                            {
                                Values.Add(val / norm);
                            }
                            else
                            {
                                Values.Add(val);
                            }

                            randomValues.Add(index, val);
                        }
                    }
                }

                Values.Sort();
                Statistics.Calculate(Values);
                return;
            }

            if (table.Rows.Count < EditorSettings.MaxSampleCount)
            {
                // Simply grab all the values
                foreach (DataRow row in table.Rows)
                {
                    double val;
                    if (!double.TryParse(row[fieldName].ToString(), out val)) continue;
                    if (double.IsNaN(val)) continue;

                    if (normField == null)
                    {
                        Values.Add(val);
                        continue;
                    }

                    double norm;
                    if (!double.TryParse(row[normField].ToString(), out norm) || double.IsNaN(val)) continue;

                    Values.Add(val / norm);
                }
            }
            else
            {
                // Grab random samples
                Dictionary<int, double> randomValues = new Dictionary<int, double>();
                int count = EditorSettings.MaxSampleCount;
                int max = table.Rows.Count;

                // Specified seed is required for consistently recreating the break values
                Random rnd = new Random(9999);
                for (int i = 0; i < count; i++)
                {
                    double val;
                    double norm = 1;
                    int index;
                    bool failed = false;
                    do
                    {
                        index = rnd.Next(max);
                        if (!double.TryParse(table.Rows[index][fieldName].ToString(), out val)) failed = true;
                        if (normField == null) continue;

                        if (!double.TryParse(table.Rows[index][normField].ToString(), out norm)) failed = true;
                    }
                    while (randomValues.ContainsKey(index) || double.IsNaN(val) || failed);

                    if (normField != null)
                    {
                        Values.Add(val / norm);
                    }
                    else
                    {
                        Values.Add(val);
                    }

                    randomValues.Add(index, val);
                }
            }

            Values.Sort();
        }
        public abstract IFeatureCategory CreateRandomCategory(string filterExpression);

        public void Draw(Graphics context, IExtent envelope, Rectangle rectangle)
        {
            throw new NotImplementedException();
        }
    }
}