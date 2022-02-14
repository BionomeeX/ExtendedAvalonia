using ExtendedAvalonia.Slider;
using System;
using System.Linq;

namespace ExtendedAvalonia
{
    public class Gradient : IEquatable<Gradient>
    {
        public Gradient(PositionColor[] positionColors, double start, double end)
            => (_positionColors, Start, End) = (positionColors, start, end);

        public PositionColor[] PositionColors
        {
            set
            {
                if (value == null)
                {
                    _positionColors = Array.Empty<PositionColor>();
                }
                else
                {
                    _positionColors = value;
                }
            }
            get
            {
                return _positionColors;
            }
        }
        public double Start { set; get; }
        public double End { set; get; }

        private PositionColor[] _positionColors = Array.Empty<PositionColor>();

        public bool Equals(Gradient? other)
        {
            if (other is null)
            {
                return false;
            }
            return other.PositionColors.SequenceEqual(PositionColors) && other.Start == Start && other.End == End;
        }
    }
}
