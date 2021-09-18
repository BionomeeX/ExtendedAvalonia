using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Drawing;
using System.Linq;

namespace ExtendedAvalonia.Slider
{
    // https://github.com/funwaywang/WpfRangeSlider
    public partial class GradientSlider : UserControl
    {
        public GradientSlider()
        {
            InitializeComponent();
        }

        private bool _isInit;

        private Color GetColorFromPosition(double position)
        {
            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            Thumb min, max;

            if (downSlider.Thumbs.Any(x => x.Position <= position)) // We found a slider lower than our value
            {
                min = downSlider.Thumbs.Where(x => x.Position <= position).OrderByDescending(x => x.Position).ToArray()[0]; // Closest to our left
            }
            else
            {
                return downSlider.Thumbs.OrderBy(x => x.Position).ToArray()[0].Color; // Else we can just return the smaller one
            }

            // Then we do the same for max
            if (downSlider.Thumbs.Any(x => x.Position >= position))
            {
                max = downSlider.Thumbs.Where(x => x.Position >= position).OrderBy(x => x.Position).ToArray()[0];
            }
            else
            {
                return downSlider.Thumbs.OrderByDescending(x => x.Position).ToArray()[0].Color;
            }

            if (min.Color == max.Color) // Nothing to do since min and max are same color
            {
                return min.Color;
            }

            var percent = (position - min.Position) / (max.Position - min.Position); // Percent between 0 and 1

            return Color.FromArgb(
                alpha: 255,
                red: (int)(percent * min.Color.R + (1 - percent) * max.Color.R),
                green: (int)(percent * min.Color.G + (1 - percent) * max.Color.G),
                blue: (int)(percent * min.Color.B + (1 - percent) * max.Color.B)
            );
        }

        public override void Render(Avalonia.Media.DrawingContext context)
        {
            if (!_isInit)
            {
                _isInit = true;

                var upSlider = this.FindControl<ExtendedSlider>("SliderUp");
                upSlider.AddThumb(new Thumb() { Position = 0.0, Color = Color.Transparent });
                upSlider.AddThumb(new Thumb() { Position = 1.0, Color = Color.Transparent });

                var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
                downSlider.AddThumb(new Thumb() { Position = 0.0, Color = Color.Red });
                downSlider.AddThumb(new Thumb() { Position = 1.0, Color = Color.Blue });
            }

            // Renderer display a big square of our color
            var renderer = this.FindControl<RenderView>("Renderer");

            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = new int[(int)renderer.Bounds.Width];
                for (int x = 0; x < (int)renderer.Bounds.Width; x++)
                {
                    data[y][x] = GetColorFromPosition(x / renderer.Bounds.Width).ToArgb();
                }
            }

            renderer.RenderData = data;
            renderer.InvalidateVisual();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
