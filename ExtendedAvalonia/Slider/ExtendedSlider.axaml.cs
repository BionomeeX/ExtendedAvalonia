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
            this.PointerPressed += (sender, e) =>
            {
                var pointerPos = e.GetPosition(this).X / Max;

                // Check if we clicked on a thumb
                for (int i = 0; i < Thumbs.Count; i++)
                {
                    var t = Thumbs[i];
                    if (pointerPos > t && pointerPos < t + _thumbsize / Max)
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
                    var half = _thumbsize / 2f / Max;
                    // Pointer position will be the middle of the thumb
                    var pointerPos = e.GetPosition(this).X / Max - half;

                    if (pointerPos < 0) pointerPos = 0;
                    else if (pointerPos > 1) pointerPos = 1;

                    Thumbs[_indexPressed] = pointerPos;

                    DragDelta?.Invoke(sender, e);

                    UpdateRender();
                }
            };
        }

        // Index of the thumb currently pressed
        private int _indexPressed = -1;

        public void AddThumb(double position)
        {
            _toAdd.Add(position);

            UpdateRender();
        }

        private const int _thumbsize = 20;

        private void UpdateRender()
        {
            // We add _thumbs there because window bounds might not be initialized in ctor
            if (_toAdd.Count > 0)
            {
                Thumbs.AddRange(_toAdd);
                _toAdd.Clear();
            }

            var renderer = this.FindControl<RenderView>("Renderer");

            if((int)renderer.Bounds.Width == 0) {
                return;
            }

            // Initialize drawing array
            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = new int[(int)renderer.Bounds.Width];
            }

            var blackColor = System.Drawing.Color.Black.ToArgb();

            // Write _thumbs
            foreach (var pos in Thumbs)
            {
                for (int x = 0; x < _thumbsize; x++) // Draw first and last line
                {
                    data[0][(int)(pos * Max) + x] = blackColor;
                    data[^1][(int)(pos * Max) + x] = blackColor;
                }
                for (int y = 0; y < data.Length; y++) // Draw first and last column
                {
                    data[y][(int)(pos * Max)] = blackColor;
                    data[y][(int)(pos * Max) + _thumbsize - 1] = blackColor;
                }
            }
            // Render data
            renderer.RenderData = data;
        }

        private double Max
        {
            get
            {
                return Bounds.Width - _thumbsize;
            }
        }

        public event EventHandler DragDelta;

        private readonly List<double> _toAdd = new();
        public List<double> Thumbs { get; } = new();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
