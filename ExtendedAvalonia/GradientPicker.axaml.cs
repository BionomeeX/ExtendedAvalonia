using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Slider;
using System;
using System.Drawing;
using System.Linq;

namespace ExtendedAvalonia
{
    // https://github.com/funwaywang/WpfRangeSlider
    public partial class GradientPicker : Window
    {
        public GradientPicker()
        {
            InitializeComponent();
        }

        private bool _isInit;

        public static void Show(Window parent, Action<Color> OnCompletion, Color defaultValue)
        {
            var picker = new GradientPicker();
            if (parent == null)
            {
                picker.Show();
            }
            else
            {
                picker.Show(parent);
            }
        }

        private Color GetColorFromPosition(double position)
        {
            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            Thumb min, max;

            if (downSlider.Thumbs.Any(t => t.X <= position)) // We found a slider lower than our value
            {
                min = downSlider.Thumbs.Where(t => t.X <= position).OrderByDescending(t => t.X).ToArray()[0]; // Closest to our left
            }
            else
            {
                return downSlider.Thumbs.OrderBy(t => t.X).ToArray()[0].Color; // Else we can just return the smaller one
            }

            // Then we do the same for max
            if (downSlider.Thumbs.Any(t => t.X >= position))
            {
                max = downSlider.Thumbs.Where(t => t.X >= position).OrderBy(t => t.X).ToArray()[0];
            }
            else
            {
                return downSlider.Thumbs.OrderByDescending(t => t.X).ToArray()[0].Color;
            }

            if (min.Color == max.Color) // Nothing to do since min and max are same color
            {
                return min.Color;
            }

            var percent = (position - min.X) / (max.X - min.X); // Percent between 0 and 1

            return Color.FromArgb(
                alpha: 255,
                red: (int)(percent * max.Color.R + (1 - percent) * min.Color.R),
                green: (int)(percent * max.Color.G + (1 - percent) * min.Color.G),
                blue: (int)(percent * max.Color.B + (1 - percent) * min.Color.B)
            );
        }

        public override void Render(Avalonia.Media.DrawingContext context)
        {
            if (!_isInit)
            {
                _isInit = true;

                var upSlider = this.FindControl<ExtendedSlider>("SliderUp");
                upSlider.AddThumb(new Thumb() { X = 0.0, Color = Color.Transparent });
                upSlider.AddThumb(new Thumb() { X = 1.0, Color = Color.Transparent });

                var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
                downSlider.AddThumb(new Thumb() { X = 0.0, Color = Color.Red });
                downSlider.AddThumb(new Thumb() { X = 1.0, Color = Color.Blue });

                downSlider.DragDelta += (sender, e) =>
                {
                    UpdateDisplay();
                };

                downSlider.OnClick += (sender, e) =>
                {
                    if (e.Thumb == null)
                    {
                        downSlider.AddThumb(new Thumb() { X = e.X, Color = Color.White });
                        UpdateDisplay();
                    }
                    else
                    {
                        ColorPicker.Show(this, c => { e.Thumb.Color = c; UpdateDisplay(); downSlider.UpdateRender(); }, e.Thumb.Color);
                    }
                };
            }

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            // Renderer display a big square of our color
            var renderer = this.FindControl<RenderView>("Renderer");

            var rangeValue = Enumerable.Range(0, (int)renderer.Bounds.Width)
                .Select(x => GetColorFromPosition(x / renderer.Bounds.Width).ToArgb()).ToArray();

            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = rangeValue;
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
