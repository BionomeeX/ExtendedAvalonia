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
                AddThumb(0);
            };
            this.PointerPressed += (sender, e) =>
            {
                var (min, max) = GetMinMax();

                var pointerPos = e.GetPosition(this).X + (int)min;

                for (int i = 0; i < Thumbs.Count; i++)
                {
                    var t = Thumbs[i];
                    if (pointerPos > t - _halfThumbSize && pointerPos < t + _halfThumbSize)
                    {
                        _indexPressed = i;
                        break;
                    }
                }
            };
            this.PointerReleased += (sender, e) =>
            {
                _indexPressed = -1;
            };
            this.PointerMoved += (sender, e) =>
            {
                if (_indexPressed != -1)
                {
                    var (min, max) = GetMinMax();

                    var pointerPos = MoveIntoBounds(e.GetPosition(this).X + (int)min, min, max);

                    Thumbs[_indexPressed] = pointerPos;
                    InvalidateVisual();
                }
            };
        }

        // Index of the thumb currently pressed
        private int _indexPressed = -1;

        public void AddThumb(double position)
        {
            _toAdd.Add(position);

            InvalidateVisual();
        }

        private const int _thumbSize = 20;
        private const int _halfThumbSize = _thumbSize / 2;

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            var (min, max) = GetMinMax();

            // We add thumbs there because window bounds might not be initialized in ctor
            if (_toAdd.Count > 0)
            {
                Thumbs.AddRange(_toAdd.Select(x =>
                {
                    return MoveIntoBounds(x, min, max);
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

            // Write thumbs
            foreach (var pos in Thumbs)
            {
                for (int x = -_halfThumbSize; x <= _halfThumbSize; x++)
                {
                    data[0][(int)pos + x + (int)-min] = blackColor;
                    data[data.Length - 1][(int)pos + x + (int)-min] = blackColor;
                }
                for (int y = 0; y < data.Length; y++)
                {
                    data[y][(int)pos - _halfThumbSize + (int)-min] = blackColor;
                    data[y][(int)pos + _halfThumbSize + (int)-min] = blackColor;
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

        public double MoveIntoBounds(double value, double min, double max)
        {
            if (value < min + _halfThumbSize) return min + _halfThumbSize;
            if (value > max + _halfThumbSize - 1) return max + _halfThumbSize - 1;
            return value;
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
