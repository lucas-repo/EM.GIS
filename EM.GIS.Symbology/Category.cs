using System;
using System.Collections.Generic;


namespace EM.GIS.Symbology
{
    public abstract class Category : LegendItem, ICategory
    {
        public double? Maximum
        {
            get => Range?.Maximum;
            set
            {
                if (Range == null)
                {
                    Range = new Range(null, value);
                }
                Range.Maximum = value;
            }
        }
        public double? Minimum
        {
            get
            {
                return Range?.Minimum;
            }

            set
            {
                if (Range == null)
                {
                    Range = new Range(value, null);
                    return;
                }

                Range.Minimum = value;
            }
        }
        public Range Range { get; set; }
        public string Status { get; set; }
        public bool SelectionEnabled { get; set; } = true;
        public  object Tag { get; set; }
        public virtual void ApplyMinMax(EditorSettings settings)
        {
            Text = Range.ToString(settings.IntervalSnapMethod, settings.IntervalRoundingDigits);
        }
        public void ApplySnapping(IntervalSnapMethod method, int numDigits, List<double> values)
        {
            switch (method)
            {
                case IntervalSnapMethod.None: break;
                case IntervalSnapMethod.SignificantFigures:
                    if (Maximum != null)
                    {
                        Maximum = Utils.SigFig(Maximum.Value, numDigits);
                    }

                    if (Minimum != null)
                    {
                        Minimum = Utils.SigFig(Minimum.Value, numDigits);
                    }

                    break;
                case IntervalSnapMethod.Rounding:
                    if (Maximum != null)
                    {
                        Maximum = Math.Round((double)Maximum, numDigits);
                    }

                    if (Minimum != null)
                    {
                        Minimum = Math.Round((double)Minimum, numDigits);
                    }

                    break;
                case IntervalSnapMethod.DataValue:
                    if (Maximum != null)
                    {
                        Maximum = Utils.GetNearestValue((double)Maximum, values);
                    }

                    if (Minimum != null)
                    {
                        Minimum = Utils.GetNearestValue((double)Minimum, values);
                    }

                    break;
            }
        }
        public bool Contains(double value)
        {
            return Range == null || Range.Contains(value);
        }
        public virtual string ToString(IntervalSnapMethod method, int digits)
        {
            return Range.ToString(method, digits);
        }
        public override string ToString()
        {
            return Range.ToString();
        }
    }
}