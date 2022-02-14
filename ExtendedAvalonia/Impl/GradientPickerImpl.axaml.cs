using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Slider;
using System;
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

        private Action<Gradient> _onChange;

        public void Init(Window me, Gradient defaultValue, Action<Gradient> onChange)
        {
            _onChange = onChange;

            var upSlider = this.FindControl<ExtendedSlider>("SliderUp");
            upSlider.AddThumb(new Thumb() { X = 0.0, Color = Color.Transparent });
            upSlider.AddThumb(new Thumb() { X = 1.0, Color = Color.Transparent });

            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            Reset(defaultValue);

            upSlider.DragDelta += (sender, e) =>
            {
                UpdateDisplay();
            };
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
                            downSlider.UpdateRender();
                        };
                        picker.OnChange += (sender, c) =>
                        {
                            newThumb.Color = c.Data;
                            UpdateDisplay();
                            downSlider.UpdateRender();
                        };
                        picker.OnCancel += (sender, e) =>
                        {
                            downSlider.Thumbs.Remove(newThumb);
                            UpdateDisplay();
                            downSlider.UpdateRender();
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
        }

        public void Reset(Gradient positions)
        {
            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            downSlider.Thumbs.Clear();
            foreach (var pc in positions.PositionColors)
            {
                downSlider.AddThumb(new Thumb() { X = pc.Position, Color = pc.Color });
            }

            UpdateDisplay();
        }

        public Gradient GetData()
        {
            var upSlider = this.FindControl<ExtendedSlider>("SliderUp").Thumbs.OrderBy(x => x.X);
            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            return new(downSlider.Thumbs.Select(t => new PositionColor() { Position = t.X, Color = t.Color }).ToArray(), upSlider.ElementAt(0).X, upSlider.ElementAt(1).X);
        }

        public void UpdateDisplay()
        {
            // Renderer display a big square of our color
            var renderer = this.FindControl<RenderView>("Renderer");

            var upSlider = this.FindControl<ExtendedSlider>("SliderUp").Thumbs.OrderBy(x => x.X);
            var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
            var rangeValue = Enumerable.Range(0, (int)renderer.Bounds.Width)
                .Select(x => GradientPicker.GetColorFromPosition(new(downSlider.Thumbs.Select(t => new PositionColor() { Position = t.X, Color = t.Color }).ToArray(), upSlider.ElementAt(0).X, upSlider.ElementAt(1).X), x / renderer.Bounds.Width).ToArgb()).ToArray();

            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = rangeValue;
            }

            renderer.RenderData = data;

            _onChange?.Invoke(GetData());
        }
    }
}
