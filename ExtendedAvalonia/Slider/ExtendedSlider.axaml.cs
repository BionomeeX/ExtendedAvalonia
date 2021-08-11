using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace ExtendedAvalonia.Slider
{
    public partial class ExtendedSlider : UserControl
    {
        public ExtendedSlider()
        {
            InitializeComponent();

            AddThumb(20.0);
            AddThumb(0.0);
        }

        private const int _thumbSize = 20;

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var renderer = this.FindControl<RenderView>("Renderer");

            // Initialize drawing array
            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = new int[(int)renderer.Bounds.Width];
            }

            var blackColor = System.Drawing.Color.Black.ToArgb();
            var halfThumbSize = _thumbSize / 2;
            var (min, max) = GetMinMax();

            // Write thumbs
            foreach (var pos in Thumbs)
            {
                for (int x = -halfThumbSize; x <= halfThumbSize; x++)
                {
                    data[0][(int)pos + x + (int)min] = blackColor;
                    data[data.Length - 1][(int)pos + x + (int)min] = blackColor;
                }
                for (int y = 0; y < data.Length; y++)
                {
                    data[y][(int)pos - halfThumbSize] = blackColor;
                    data[y][(int)pos + halfThumbSize] = blackColor;
                }
            }

            // Render data
            renderer.RenderData = data;
            renderer.InvalidateVisual();
        }

        public void AddThumb(double position)
        {
            var (min, max) = GetMinMax();

            if (position < min) position = min;
            else if (position > max) position = max;
            Thumbs.Add(position);

            InvalidateVisual();
        }

        public (double min, double max) GetMinMax()
        {
            var min = -Bounds.Width / 2f + _thumbSize / 2f;
            return (min, -min);
        }

        public event EventHandler DragDelta;

        public List<double> Thumbs { get; } = new();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
