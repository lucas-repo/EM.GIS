using System;
using System.Drawing;



namespace EM.GIS.Symbology
{
    [Serializable]
    public abstract class FeatureSymbol : Descriptor, IFeatureSymbol
    {
        private Color _color;
        public virtual Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        public virtual float Opacity
        {
            get
            {
                return _color.GetOpacity();
            }
            set
            {
                _color = Color.ToTransparent(value);
            }
        }
        public FeatureSymbol()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            _color = random.NextColor();
        }
        public FeatureSymbol(Color color)
        {
            _color = color;
        }
    }
}
