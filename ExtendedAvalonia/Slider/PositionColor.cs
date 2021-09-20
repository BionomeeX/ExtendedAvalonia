using System.Drawing;

namespace ExtendedAvalonia.Slider
{
    public struct PositionColor
    {
        public Color Color
        {
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
            get
            {
                return Color.FromArgb(255, R, G, B);
            }
        }

        public byte R { set; get; }
        public byte G { set; get; }
        public byte B { set; get; }
        public double Position { init; get; }
    }
}
