using System;
using System.Collections.Generic;
using System.Drawing;

namespace EM.GIS.Symbology
{
    public abstract class Scheme : LegendItem, IScheme
    {
        public EditorSettings EditorSettings { get; set; }
        public Statistics Statistics { get; protected set; } = new Statistics();
        public List<double> Values { get; protected set; }
        protected List<Break> Breaks { get; set; }
        public abstract int Count { get; }
        public bool IsReadOnly { get; } = false;
        public ICategoryCollection Categories { get; set; }

        protected virtual List<float> GetSizeSet(int count)
        {
            var result = new List<float>();
            for (var i = 0; i < count; i++)
            {
                result.Add(20);
            }
            return result;
        }
        private List<Color> CreateRandomColors(int numColors)
        {
            var result = new List<Color>(numColors);
            var rnd = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < numColors; i++)
            {
                result.Add(CreateRandomColor(rnd));
            }

            return result;
        }
        protected void ApplyBreakSnapping()
        {
            if (Values == null || Values.Count == 0) return;

            switch (EditorSettings.IntervalSnapMethod)
            {
                case IntervalSnapMethod.None: break;
                case IntervalSnapMethod.SignificantFigures:
                    foreach (var item in Breaks)
                    {
                        if (item.Maximum == null) continue;

                        var val = (double)item.Maximum;
                        item.Maximum = Utils.SigFig(val, EditorSettings.IntervalRoundingDigits);
                    }

                    break;
                case IntervalSnapMethod.Rounding:
                    foreach (var item in Breaks)
                    {
                        if (item.Maximum == null) continue;

                        item.Maximum = Math.Round((double)item.Maximum, EditorSettings.IntervalRoundingDigits);
                    }

                    break;
                case IntervalSnapMethod.DataValue:
                    foreach (var item in Breaks)
                    {
                        if (item.Maximum == null) continue;

                        item.Maximum = Utils.GetNearestValue((double)item.Maximum, Values);
                    }

                    break;
            }
        }
        protected List<Break> GetQuantileBreaks(int count)
        {
            var result = new List<Break>();
            var binSize = (int)Math.Ceiling(Values.Count / (double)count);
            for (var iBreak = 1; iBreak <= count; iBreak++)
            {
                if (binSize * iBreak < Values.Count)
                {
                    var brk = new Break { Maximum = Values[binSize * iBreak] };
                    result.Add(brk);
                }
                else
                {
                    // if num breaks is larger than number of members, this can happen
                    var brk = new Break { Maximum = null };
                    result.Add(brk);
                    break;
                }
            }

            return result;
        }
        protected List<Break> GetNaturalBreaks(int count)
        {
            var theOutput = new List<Break>(count);
            var natBreaks = new NaturalBreaks(Values, count);
            List<double> theBreakValues = natBreaks.GetResults();

            // remove the first break value since it is the lowest value
            theBreakValues.RemoveAt(0);
            foreach (double oneBreakValue in theBreakValues)
            {
                Break b = new Break();
                b.Maximum = oneBreakValue;
                theOutput.Add(b);
            }

            return theOutput;
        }
        protected List<Break> GetEqualBreaks(int count)
        {
            var result = new List<Break>();
            var min = Values[0];
            var dx = (Values[Values.Count - 1] - min) / count;

            for (var i = 0; i < count; i++)
            {
                var brk = new Break();

                // max
                if (i == count - 1)
                {
                    brk.Maximum = null;
                }
                else
                {
                    brk.Maximum = min + ((i + 1) * dx);
                }

                result.Add(brk);
            }

            return result;
        }
        protected static void SetBreakNames(IList<Break> breaks)
        {
            for (var i = 0; i < breaks.Count; i++)
            {
                var brk = breaks[i];
                if (breaks.Count == 1)
                {
                    brk.Name = "All Values";
                }
                else if (i == 0)
                {
                    brk.Name = "<= " + brk.Maximum;
                }
                else if (i == breaks.Count - 1)
                {
                    brk.Name = "> " + breaks[i - 1].Maximum;
                }
                else
                {
                    brk.Name = breaks[i - 1].Maximum + " - " + brk.Maximum;
                }
            }
        }
        protected void CreateBreakCategories()
        {
            var count = EditorSettings.NumBreaks;
            switch (EditorSettings.IntervalMethod)
            {
                case IntervalMethod.EqualFrequency:
                    Breaks = GetQuantileBreaks(count);
                    break;
                case IntervalMethod.NaturalBreaks:
                    Breaks = GetNaturalBreaks(count);
                    break;
                default:
                    Breaks = GetEqualBreaks(count);
                    break;
            }

            ApplyBreakSnapping();
            SetBreakNames(Breaks);
            var colorRamp = GetColorSet(count);
            var sizeRamp = GetSizeSet(count);
            Clear();
            var colorIndex = 0;
            Break prevBreak = null;
            foreach (var brk in Breaks)
            {
                // get the color for the category
                var randomColor = colorRamp[colorIndex];
                var randomSize = sizeRamp[colorIndex];
                var cat = CreateNewCategory(randomColor, randomSize);

                if (cat != null)
                {
                    // cat.SelectionSymbolizer = _selectionSymbolizer.Copy();
                    cat.Text = brk.Name;
                    if (prevBreak != null) cat.Minimum = prevBreak.Maximum;
                    cat.Maximum = brk.Maximum;
                    cat.Range.MaxIsInclusive = true;
                    cat.ApplyMinMax(EditorSettings);
                    Add(cat);
                }

                prevBreak = brk;

                colorIndex++;
            }
        }
        protected Color CreateRandomColor(Random rnd)
        {
            var startColor = EditorSettings.StartColor;
            var endColor = EditorSettings.EndColor;
            if (EditorSettings.HueSatLight)
            {
                double hLow = startColor.GetHue();
                var dH = endColor.GetHue() - hLow;
                double sLow = startColor.GetSaturation();
                var ds = endColor.GetSaturation() - sLow;
                double lLow = startColor.GetBrightness();
                var dl = endColor.GetBrightness() - lLow;
                var aLow = startColor.A / 255.0;
                var da = (endColor.A - aLow) / 255.0;
                return SymbologyGlobal.ColorFromHsl((rnd.NextDouble() * dH) + hLow, (rnd.NextDouble() * ds) + sLow, (rnd.NextDouble() * dl) + lLow).ToTransparent((float)((rnd.NextDouble() * da) + aLow));
            }

            int rLow = Math.Min(startColor.R, endColor.R);
            int rHigh = Math.Max(startColor.R, endColor.R);
            int gLow = Math.Min(startColor.G, endColor.G);
            int gHigh = Math.Max(startColor.G, endColor.G);
            int bLow = Math.Min(startColor.B, endColor.B);
            int bHigh = Math.Max(startColor.B, endColor.B);
            int iaLow = Math.Min(startColor.A, endColor.A);
            int aHigh = Math.Max(startColor.A, endColor.A);
            return Color.FromArgb(rnd.Next(iaLow, aHigh),rnd.Next(rLow, rHigh), rnd.Next(gLow, gHigh), rnd.Next(bLow, bHigh));
        }
        private static List<Color> CreateRampColors(int numColors, float minSat, float minLight, int minHue, float maxSat, float maxLight, int maxHue, int hueShift, int minAlpha, int maxAlpha)
        {
            var result = new List<Color>(numColors);
            var ds = (maxSat - (double)minSat) / numColors;
            var dh = (maxHue - (double)minHue) / numColors;
            var dl = (maxLight - (double)minLight) / numColors;
            var dA = (maxAlpha - (double)minAlpha) / numColors;
            for (var i = 0; i < numColors; i++)
            {
                var h = (minHue + (dh * i)) + (hueShift % 360);
                var s = minSat + (ds * i);
                var l = minLight + (dl * i);
                var a = (float)(minAlpha + (dA * i)) / 255f;
                result.Add(SymbologyGlobal.ColorFromHsl(h, s, l).ToTransparent(a));
            }

            return result;
        }
        private static List<Color> CreateRampColors(int numColors, Color startColor, Color endColor)
        {
            var result = new List<Color>(numColors);
            var dR = (endColor.R - (double)startColor.R) / numColors;
            var dG = (endColor.G - (double)startColor.G) / numColors;
            var dB = (endColor.B - (double)startColor.B) / numColors;
            var dA = (endColor.A - (double)startColor.A) / numColors;
            for (var i = 0; i < numColors; i++)
            {
                result.Add(Color.FromArgb((int)(startColor.A + (dA * i)),(int)(startColor.R + (dR * i)), (int)(startColor.G + (dG * i)), (int)(startColor.B + (dB * i))));
            }

            return result;
        }
        protected virtual List<Color> GetDefaultColors(int count)
        {
            return EditorSettings.RampColors ? CreateUnboundedRampColors(count) : CreateUnboundedRandomColors(count);
        }
        private static List<Color> CreateUnboundedRandomColors(int numColors)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var result = new List<Color>(numColors);
            for (var i = 0; i < numColors; i++)
            {
                result.Add(rnd.NextColor());
            }

            return result;
        }
        private static List<Color> CreateUnboundedRampColors(int numColors)
        {
            return CreateRampColors(numColors, .25f, .25f, 0, .75f, .75f, 360, 0, 255, 255);
        }
        protected List<Color> GetColorSet(int count)
        {
            List<Color> colorRamp;
            if (EditorSettings.UseColorRange)
            {
                if (!EditorSettings.RampColors)
                {
                    colorRamp = CreateRandomColors(count);
                }
                else if (!EditorSettings.HueSatLight)
                {
                    colorRamp = CreateRampColors(count, EditorSettings.StartColor, EditorSettings.EndColor);
                }
                else
                {
                    var cStart = EditorSettings.StartColor;
                    var cEnd = EditorSettings.EndColor;
                    colorRamp = CreateRampColors(count, cStart.GetSaturation(), cStart.GetBrightness(), (int)cStart.GetHue(), cEnd.GetSaturation(), cEnd.GetBrightness(), (int)cEnd.GetHue(), EditorSettings.HueShift, cStart.A, cEnd.A);
                }
            }
            else
            {
                colorRamp = GetDefaultColors(count);
            }
            return colorRamp;
        }

        public abstract ICategory CreateNewCategory(Color fillColor, float size);
        public abstract void DrawCategory(int index, Graphics context, Rectangle bounds);
        public abstract void Add(ICategory item);
        public abstract void Clear();
        public abstract bool Contains(ICategory item);
        public abstract void CopyTo(ICategory[] array, int arrayIndex);
        public abstract bool Remove(ICategory item);
        public abstract IEnumerator<ICategory> GetEnumerator();
        public abstract void Move(int oldIndex, int newIndex);
    }

}