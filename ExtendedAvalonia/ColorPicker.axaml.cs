using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System;
using System.Drawing;

namespace ExtendedAvalonia
{
    public partial class ColorPicker : Window
    {
        // Colors displayed by the small bar of the picker
        private Color[] _colors = new[]
        {
            Color.Red,
            Color.Yellow,
            Color.Green,
            Color.Cyan,
            Color.Blue,
            Color.Magenta,
            Color.Red
        };

        private double _value = 0.0;
        internal Color CurrentColor { private set; get; }

        public static void Show(Window parent, Action<Color> OnCompletion)
        {
            var picker = new ColorPicker();
            picker.Show(parent);
            picker.Closed += (sender, e) =>
            {
                OnCompletion?.Invoke(picker.CurrentColor);
            };
            picker.FindControl<Button>("Validate").Click += (sender, e) =>
            {
                OnCompletion?.Invoke(picker.CurrentColor);
                picker.Close();
            };
        }

        public ColorPicker()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.FindControl<Thumb>("Thumb").DragDelta += (sender, e) =>
            {
                // Value of the cursor
                _value += e.Vector.X;

                var (min, max) = GetMinMax();

                // Make sure we aren't out of bounds
                if (_value < min) _value = min;
                else if (_value > max) _value = max;

                // Update position
                var thumb = this.FindControl<Thumb>("Thumb");
                var measure = ((ILayoutable)thumb).PreviousMeasure;
                thumb.Arrange(new Rect(_value, 0, measure.Value.Width, measure.Value.Height));

                DisplayColor();
            };
            this.Opened += (sender, e) =>
            {
                DisplayColor();
            };
        }

        private (double min, double max) GetMinMax()
        {
            var thumb = this.FindControl<Thumb>("Thumb");
            var min = -Bounds.Width / 2f + thumb.Bounds.Width / 2f;
            return (min, -min);
        }

        private void DisplayColor()
        {
            var (min, max) = GetMinMax();

            // Get between what colors we are in the small bar
            var targetColor = (_value - min) * (_colors.Length - 1) / (max - min);
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
            var renderer = this.FindControl<RenderView>("Renderer");

            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = new int[(int)renderer.Bounds.Width];
                for (int x = 0; x < (int)renderer.Bounds.Width; x++)
                {
                    data[y][x] = CurrentColor.ToArgb();
                }
            }

            renderer.RenderData = data;
            renderer.InvalidateVisual();

            this.FindControl<TextBlock>("RGBValues").Text = $"(R: {CurrentColor.R}, G: {CurrentColor.G}, B: {CurrentColor.B})";
        }

        /// <summary>
        /// Returns the value between the 2 given in parameter
        /// </summary>
        /// <param name="first">First bound</param>
        /// <param name="second">Second bound</param>
        /// <param name="value">Value between the 2 others, between 0 and 1</param>
        private byte GetColorValueBetween(byte first, byte second, double value)
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
