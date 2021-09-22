using System;
using System.Drawing;

namespace ExtendedAvalonia.Slider
{
    public struct PositionColor : IEquatable<PositionColor>
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

        public bool Equals(PositionColor other)
        {
            return R == other.R && G == other.G && B == other.B && Position == other.Position;
        }

        public override string ToString()
        {
            return $"({R} {G} {B}) ; {Position}";
        }

        public byte R { set; get; }
        public byte G { set; get; }
        public byte B { set; get; }
        public double Position { init; get; }
    }
}
