using ExtendedAvalonia.Slider;
using System;
using System.Linq;

namespace ExtendedAvalonia
{
    public class Gradient : IEquatable<Gradient>
    {
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

        private PositionColor[] _positionColors = Array.Empty<PositionColor>();

        public bool Equals(Gradient? other)
        {
            if (other is null)
            {
                return false;
            }
            return other.PositionColors.SequenceEqual(PositionColors);
        }
    }
}
