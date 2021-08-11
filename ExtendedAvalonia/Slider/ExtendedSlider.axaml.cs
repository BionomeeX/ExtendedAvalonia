using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedAvalonia.Slider
{
    public partial class ExtendedSlider : UserControl
    {
        public ExtendedSlider()
        {
            InitializeComponent();

            this.Initialized += (sender, e) =>
            {
                AddThumb(-20.0);
                AddThumb(500.0);
            };
        }

        public void AddThumb(double position)
        {
            _toAdd.Add(position);

            InvalidateVisual();
        }

        private const int _thumbSize = 20;

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            var (min, max) = GetMinMax();

            // We add thumbs there because window bounds might not be initialized in ctor
            if (_toAdd.Count > 0)
            {
                Thumbs.AddRange(_toAdd.Select(x =>
                {
                    if (x < min) x = min;
                    else if (x > max) x = max;
                    return x;
                }));
                _toAdd.Clear();
            }

            var renderer = this.FindControl<RenderView>("Renderer");

            // Initialize drawing array
            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = new int[(int)renderer.Bounds.Width];
            }

            var blackColor = System.Drawing.Color.Black.ToArgb();
            var halfThumbSize = _thumbSize / 2;

            // Write thumbs
            foreach (var pos in Thumbs)
            {
                for (int x = -halfThumbSize; x <= halfThumbSize; x++)
                {
                    data[0][(int)pos + x + (int)-min] = blackColor;
                    data[data.Length - 1][(int)pos + x + (int)-min] = blackColor;
                }
                for (int y = 0; y < data.Length; y++)
                {
                    data[y][(int)pos - halfThumbSize + (int)-min] = blackColor;
                    data[y][(int)pos + halfThumbSize + (int)-min] = blackColor;
                }
            }

            // Render data
            renderer.RenderData = data;
            renderer.InvalidateVisual();
        }

        public (double min, double max) GetMinMax()
        {
            var min = -Bounds.Width / 2f + _thumbSize / 2f;
            return (min, -min);
        }

        public event EventHandler DragDelta;

        private List<double> _toAdd = new();
        public List<double> Thumbs { get; } = new();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
