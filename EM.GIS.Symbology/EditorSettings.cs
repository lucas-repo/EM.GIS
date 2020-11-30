using System.Drawing;

namespace EM.GIS.Symbology
{
    public class EditorSettings : Descriptor
    {
        #region Properties
        
        public Color EndColor { get; set; } = SymbologyGlobal.ColorFromHsl(345, .8, .8);
        
        public string ExcludeExpression { get; set; }

        public bool HueSatLight { get; set; } = true;
        public int HueShift { get; set; }

        public IntervalMethod IntervalMethod { get; set; } = IntervalMethod.EqualInterval;

        public int IntervalRoundingDigits { get; set; }
        public IntervalSnapMethod IntervalSnapMethod{ get; set; } = IntervalSnapMethod.DataValue;

        public int MaxSampleCount { get; set; } = 10000;

        public int NumBreaks { get; set; } = 5;

        public bool RampColors { get; set; } = true;

        public Color StartColor { get; set; } = SymbologyGlobal.ColorFromHsl(5, .7, .7);

        public bool UseColorRange { get; set; } = true;

        #endregion
    }
}