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
            AvaloniaXamlLoader.Load(this);
        }

        public static void Show(Window parent, Action<PositionColor[]> OnCompletion, PositionColor[] defaultValue)
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

            var upSlider = picker.FindControl<ExtendedSlider>("SliderUp");
            upSlider.AddThumb(new Thumb() { X = 0.0, Color = Color.Transparent });
            upSlider.AddThumb(new Thumb() { X = 1.0, Color = Color.Transparent });

            var downSlider = picker.FindControl<ExtendedSlider>("SliderDown");
            foreach (var pc in defaultValue)
            {
                downSlider.AddThumb(new Thumb() { X = pc.Position, Color = pc.Color });
            }

            downSlider.DragDelta += (sender, e) =>
            {
                picker.UpdateDisplay();
            };

            downSlider.Click += (sender, e) =>
            {
                if (e.Thumb == null)
                {
                    downSlider.AddThumb(new Thumb() { X = e.X, Color = Color.White });
                    picker.UpdateDisplay();
                }
                else
                {
                    ColorPicker.Show(picker, c => { e.Thumb.Color = c; picker.UpdateDisplay(); downSlider.UpdateRender(); }, e.Thumb.Color);
                }
            };
            picker.FindControl<Button>("Validate").Click += (sender, e) =>
            {
                OnCompletion?.Invoke(downSlider.Thumbs.Select(t => new PositionColor() { Position = t.X, Color = t.Color }).ToArray());
                picker.Close();
            };

            picker.UpdateDisplay();
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
    }
}
