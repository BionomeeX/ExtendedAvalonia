using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Slider;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ExtendedAvalonia.Impl
{
    public partial class GradientPickerImpl : UserControl
    {
        public GradientPickerImpl()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private Action<PositionColor[]> _onChange;

        public void Init(Window me, PositionColor[] defaultValue, Action<PositionColor[]> onChange)
        {
            _onChange = onChange;

            var upSlider = this.FindControl<ExtendedSlider>("SliderUp");
            upSlider.AddThumb(new Thumb() { X = 0.0, Color = Color.Transparent });
            upSlider.AddThumb(new Thumb() { X = 1.0, Color = Color.Transparent });

            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            foreach (var pc in defaultValue)
            {
                downSlider.AddThumb(new Thumb() { X = pc.Position, Color = pc.Color });
            }

            downSlider.DragDelta += (sender, e) =>
            {
                UpdateDisplay();
            };

            downSlider.Click += (sender, e) =>
            {
                if (e.MouseButton == MouseButton.Left)
                {
                    if (e.Thumb == null) // Add new thumb
                    {
                        var picker = ColorPicker.Show(me, Color.White);
                        var newThumb = new Thumb() { X = e.X, Color = Color.White };
                        downSlider.AddThumb(newThumb);
                        UpdateDisplay();
                        picker.OnCompletion += (sender, c) =>
                        {
                            newThumb.Color = c.Data;
                            UpdateDisplay();
                        };
                        picker.OnChange += (sender, c) =>
                        {
                            newThumb.Color = c.Data;
                            UpdateDisplay();
                        };
                        picker.OnCancel += (sender, e) =>
                        {
                            downSlider.Thumbs.Remove(newThumb);
                        };
                    }
                    else // Change thumb color
                    {
                        var defaultColor = e.Thumb.Color;
                        var picker = ColorPicker.Show(me, e.Thumb.Color);
                        picker.OnCompletion += (sender, c) =>
                        {
                            e.Thumb.Color = c.Data;
                            UpdateDisplay();
                            downSlider.UpdateRender();
                        };
                        picker.OnCancel += (sender, c) =>
                        {
                            e.Thumb.Color = defaultColor;
                            UpdateDisplay();
                            downSlider.UpdateRender();
                        };
                        picker.OnChange += (sender, c) =>
                        {
                            e.Thumb.Color = c.Data;
                            UpdateDisplay();
                            downSlider.UpdateRender();
                        };
                    }
                }
                else if (e.MouseButton == MouseButton.Right)
                {
                    if (e.Thumb != null) // Delete thumb
                    {
                        downSlider.Thumbs.Remove(e.Thumb);
                        UpdateDisplay();
                        downSlider.UpdateRender();
                    }
                }
            };

            UpdateDisplay();
        }

        public PositionColor[] GetData()
        {
            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            return downSlider.Thumbs.Select(t => new PositionColor() { Position = t.X, Color = t.Color }).ToArray();
        }

        public static Color GetColorFromPosition(IEnumerable<PositionColor> thumbs, double position)
        {
            if (!thumbs.Any())
            {
                return Color.White;
            }

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

            _onChange?.Invoke(GetData());

            renderer.InvalidateVisual();
        }
    }
}
