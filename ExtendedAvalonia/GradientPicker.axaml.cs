using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Slider;
using System;
using System.Collections.Generic;
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
                    ColorPicker.Show(picker, c =>
                    {
                        downSlider.AddThumb(new Thumb() { X = e.X, Color = c });
                        picker.UpdateDisplay();
                    }, Color.White);
                }
                else
                {
                    ColorPicker.Show(picker, c =>
                    {
                        e.Thumb.Color = c;
                        picker.UpdateDisplay();
                        downSlider.UpdateRender();
                    }, e.Thumb.Color);
                }
            };
            picker.FindControl<Button>("Validate").Click += (sender, e) =>
            {
                OnCompletion?.Invoke(downSlider.Thumbs.Select(t => new PositionColor() { Position = t.X, Color = t.Color }).ToArray());
                picker.Close();
            };

            picker.UpdateDisplay();
        }

        public static Color GetColorFromPosition(IEnumerable<PositionColor> thumbs, double position)
        {
            PositionColor min, max;

            if (thumbs.Any(t => t.Position <= position)) // We found a slider lower than our value
            {
                min = thumbs.Where(t => t.Position <= position).OrderByDescending(t => t.Position).ToArray()[0]; // Closest to our left
            }
            else
            {
                return thumbs.OrderBy(t => t.Position).ToArray()[0].Color; // Else we can just return the smaller one
            }

            // Then we do the same for max
            if (thumbs.Any(t => t.Position >= position))
            {
                max = thumbs.Where(t => t.Position >= position).OrderBy(t => t.Position).ToArray()[0];
            }
            else
            {
                return thumbs.OrderByDescending(t => t.Position).ToArray()[0].Color;
            }

            if (min.Color == max.Color) // Nothing to do since min and max are same color
            {
                return min.Color;
            }

            var percent = (position - min.Position) / (max.Position - min.Position); // Percent between 0 and 1

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

            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            var rangeValue = Enumerable.Range(0, (int)renderer.Bounds.Width)
                .Select(x => GetColorFromPosition(downSlider.Thumbs.Select(t => new PositionColor() { Position = t.X, Color = t.Color }), x / renderer.Bounds.Width).ToArgb()).ToArray();

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
