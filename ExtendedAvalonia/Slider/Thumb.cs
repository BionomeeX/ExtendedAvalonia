using System.Drawing;

namespace ExtendedAvalonia.Slider
{
    public record Thumb
    {
        private double _x, _y;

        public double X
        {
            set
            {
                _x = Math.Clamp(value, 0.0, 1.0);
            }
            get => _x;
        }
        public double Y
        {
            set
            {
                _y = Math.Clamp(value, 0.0, 1.0);
            }
            get => _y;
        }
        public Color Color { set; get; }
    }
}
