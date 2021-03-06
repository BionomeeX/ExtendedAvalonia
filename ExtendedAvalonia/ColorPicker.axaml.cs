using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Event;
using ExtendedAvalonia.Slider;
using System.Drawing;

namespace ExtendedAvalonia
{
    public partial class ColorPicker : Window, IPicker<ColorPicker, Color>
    {
        public static ColorPicker Show(Window parent, Color defaultValue)
            => IPicker<ColorPicker, Color>.Show(parent, defaultValue);

        public event EventHandler<DataEventArgs<Color>> OnChange;
        public event EventHandler<DataEventArgs<Color>> OnCompletion;
        public event EventHandler OnCancel;

        public ColorPicker()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnClose(object sender, EventArgs e)
        {
            OnCancel?.Invoke(sender, e);
        }

        void IPicker<ColorPicker, Color>.Init(Color defaultValue)
        {
            Closed += OnClose;

            var slider = this.FindControl<ExtendedSlider>("Slider");
            slider.DragDelta += (sender, e) =>
            {
                DisplayColor();
            };

            var renderer = this.FindControl<ExtendedSlider>("Renderer");
            renderer.DragDelta += (sender, e) =>
            {
                DisplayColor();
            };

            this.FindControl<Button>("Validate").Click += (sender, e) =>
            {
                OnCompletion?.Invoke(sender, new() { Data = CurrentColor });
                Closed -= OnClose;
                Close();
            };

            slider.AddThumb(new() { X = 0.5, Color = Color.Transparent }); // TODO: Need to get the closest value to defaultValue

            renderer.AddThumb(new() { X = 0.5, Color = Color.Transparent });

            DisplayColor();
        }

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

        private void DisplayColor()
        {
            var slider = this.FindControl<ExtendedSlider>("Slider");

            var value = slider.Thumbs.Any() ? slider.Thumbs.ElementAt(0).X : .5;

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

            var color = Color.FromArgb(255, red, green, blue);

            // Renderer display a big square of our color
            var renderer = this.FindControl<ExtendedSlider>("Renderer");

            for (int y = 0; y < renderer.Background.Length; y++)
            {
                var pY = (float)y / renderer.Background.Length;
                for (int x = 0; x < renderer.Background[y].Length; x++)
                {
                    var pX = (float)x / renderer.Background[y].Length;

                    renderer.Background[y][x] = Color.FromArgb(255,
                        red: (int)((1 - pY) * ((1 - pX) * 255 + pX * color.R)),
                        green: (int)((1 - pY) * ((1 - pX) * 255 + pX * color.G)),
                        blue: (int)((1 - pY) * ((1 - pX) * 255 + pX * color.B))
                    ).ToArgb();
                }
            }

            if (renderer.Thumbs.Any())
            {
                var thumb = renderer.Thumbs[0];
                var ccY = (int)(thumb.Y * (renderer.Background.Length - 1));
                var ccX = (int)(thumb.X * (renderer.Background[ccY].Length - 1));
                CurrentColor = Color.FromArgb(renderer.Background[ccY][ccX]);
            }
            else
            {
                CurrentColor = color;
            }

            OnChange?.Invoke(this, new() { Data = CurrentColor });

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
