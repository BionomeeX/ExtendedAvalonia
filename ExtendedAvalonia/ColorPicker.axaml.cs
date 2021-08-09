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

        public ColorPicker()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.FindControl<Thumb>("Thumb").DragDelta += (sender, e) =>
            {
                var thumb = (Thumb)sender;
                var measure = ((ILayoutable)thumb).PreviousMeasure;
                _value += e.Vector.X;

                var min = -Bounds.Width / 2f + thumb.Bounds.Width / 2f;
                var max = -min;
                if (_value < min) _value = min;
                else if (_value > max) _value = max;

                var targetColor = (_value - min) * _colors.Length / (max - min);

                var minColor = _colors[(int)Math.Floor(targetColor)];
                var maxColor = _colors[(int)Math.Ceiling(targetColor)];

                var subTargetColor = targetColor - (int)targetColor;

                Color color = Color.FromArgb(255,
                    red: (int)(subTargetColor * Math.Max(minColor.R, maxColor.R)),
                    green: (int)(subTargetColor * Math.Max(minColor.G, maxColor.G)),
                    blue: (int)(subTargetColor * Math.Max(minColor.B, maxColor.B))
                    );

                thumb.Arrange(new Rect(_value, 0, measure.Value.Width, measure.Value.Height));

                var renderer = this.FindControl<RenderView>("Renderer");

                int[][] data = new int[(int)renderer.Bounds.Height][];
                for (int y = 0; y < (int)renderer.Bounds.Height; y++)
                {
                    data[y] = new int[(int)renderer.Bounds.Width];
                    for (int x = 0; x < (int)renderer.Bounds.Width; x++)
                    {
                        data[y][x] = color.ToArgb();
                    }
                }

                renderer.RenderData = data;
                renderer.InvalidateVisual();
            };
        }

        double _value = 0.0;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
