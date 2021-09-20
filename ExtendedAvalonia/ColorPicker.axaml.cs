using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Slider;
using System;
using System.Drawing;
using System.Linq;

namespace ExtendedAvalonia
{
    public partial class ColorPicker : Window
    {
        // Colors displayed by the small bar of the picker
        private readonly Color[] _colors = new[]
        {
            Color.Red,
            Color.Yellow,
            Color.Green,
            Color.Cyan,
            Color.Blue,
            Color.Magenta,
            Color.Red
        };
        internal Color CurrentColor { private set; get; }

        public static void Show(Window parent, Action<Color> OnCompletion, Color defaultValue)
        {
            var picker = new ColorPicker();
            if (parent == null)
            {
                picker.Show();
            }
            else
            {
                picker.Show(parent);
            }
            picker.FindControl<Button>("Validate").Click += (sender, e) =>
            {
                OnCompletion?.Invoke(picker.CurrentColor);
                picker.Close();
            };

            var slider = picker.FindControl<ExtendedSlider>("Slider");
            slider.AddThumb(new() { Position = 0.5, Color = Color.Transparent }); // TODO: Need to get the closest value to defaultValue
        }

        public ColorPicker()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var slider = this.FindControl<ExtendedSlider>("Slider");
            slider.DragDelta += (sender, e) =>
            {
                DisplayColor();
            };
            this.Opened += (sender, e) =>
            {
                DisplayColor();
            };
        }

        private void DisplayColor()
        {
            var slider = this.FindControl<ExtendedSlider>("Slider");

            var value = slider.Thumbs.Any() ? slider.Thumbs.ElementAt(0).Position : .5;

            // Get between what colors we are in the small bar
            var targetColor = value * (_colors.Length - 1);
            var minColor = _colors[(int)Math.Floor(targetColor)];
            var maxColor = _colors[(int)Math.Ceiling(targetColor)];

            // We are between 2 colors, we calculate where we are in it (0 is close to the left one and 1 to the right one)
            var subTargetColor = targetColor - (int)targetColor;

            // Get RGB value of the color we are in
            var red = GetColorValueBetween(minColor.R, maxColor.R, subTargetColor);
            var green = GetColorValueBetween(minColor.G, maxColor.G, subTargetColor);
            var blue = GetColorValueBetween(minColor.B, maxColor.B, subTargetColor);

            CurrentColor = Color.FromArgb(255, red, green, blue);

            // Renderer display a big square of our color
            var renderer = this.FindControl<ExtendedSlider>("Renderer");

            for (int y = 0; y < renderer.Background.Length; y++)
            {
                for (int x = 0; x < renderer.Background[y].Length; x++)
                {
                    renderer.Background[y][x] = CurrentColor.ToArgb();
                }
            }

            renderer.UpdateRender();

            this.FindControl<TextBlock>("RGBValues").Text = $"(R: {CurrentColor.R}, G: {CurrentColor.G}, B: {CurrentColor.B})";
        }

        /// <summary>
        /// Returns the value between the 2 given in parameter
        /// </summary>
        /// <param name="first">First bound</param>
        /// <param name="second">Second bound</param>
        /// <param name="value">Value between the 2 others, between 0 and 1</param>
        private static byte GetColorValueBetween(byte first, byte second, double value)
        {
            // Our both bounds are the same, no calculation to do
            if (first == second)
            {
                return first;
            }
            byte newValue = (byte)(value * Math.Max(first, second));
            if (Math.Min(first, second) == 0) // If min value if 0, nothing else to do
            {
                return first > second ? (byte)(255 - newValue) : newValue;
            }
            // Else we need to make our value between min and max
            newValue = (byte)((newValue * Math.Min(first, second) / 255) + Math.Min(first, second));
            return first > second ? (byte)(255 - newValue + Math.Min(first, second)) : newValue;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
