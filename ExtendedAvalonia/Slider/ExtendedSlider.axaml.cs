using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Slider.Event;
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
                (double X, double Y) pos = (e.GetPosition(this).X / Max.X, e.GetPosition(this).Y / Max.Y);
                _movePos = pos;

                // Check if we clicked on a thumb
                for (int i = 0; i < Thumbs.Count; i++)
                {
                    var t = Thumbs[i];
                    if (pos.X > t.X && pos.X < t.X + _thumbsize / Max.X &&
                        pos.Y > t.Y && pos.Y < t.Y + _thumbsize / Max.Y)
                    {
                        _indexPressed = i;
                        break;
                    }
                }
            };
            this.PointerReleased += (sender, e) =>
            {
                if (_movePos.X == e.GetPosition(this).X / Max.X && _movePos.Y == e.GetPosition(this).Y / Max.Y) // Clicked
                {
                    OnClick?.Invoke(this, new ThumbEventArgs
                    {
                        Thumb = _indexPressed == -1 ? null : Thumbs[_indexPressed],
                        X = e.GetPosition(this).X / Max.X,
                        Y = e.GetPosition(this).Y / Max.Y
                    });
                }
                _indexPressed = -1;
            };
            this.PointerMoved += (sender, e) =>
            {
                if (_indexPressed != -1)
                {
                    { // X
                        var half = _thumbsize / 2f / Max.X;
                        // Pointer position will be the middle of the thumb
                        var pointerPos = e.GetPosition(this).X / Max.X - half;

                        if (pointerPos < 0) pointerPos = 0;
                        else if (pointerPos > 1) pointerPos = 1;

                        Thumbs[_indexPressed].X = pointerPos;
                    }
                    { // Y
                        var half = _thumbsize / 2f / Max.Y;
                        var pointerPos = e.GetPosition(this).Y / Max.Y - half;

                        if (pointerPos < 0) pointerPos = 0;
                        else if (pointerPos > 1) pointerPos = 1;

                        Thumbs[_indexPressed].Y = pointerPos;
                    }

                    DragDelta?.Invoke(sender, e);

                    UpdateRender();
                }
            };
        }

        // Index of the thumb currently pressed
        private int _indexPressed = -1;
        private (double X, double Y) _movePos; // Used to check if we dragged or clicked

        public void AddThumb(Thumb position)
        {
            _toAdd.Add(position);

            UpdateRender();
        }

        private const int _thumbsize = 20;

        public void UpdateRender()
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

            // https://stackoverflow.com/a/4671179/6663248
            var len = Background.Length;
            var data = new int[len][];
            for (var x = 0; x < len; x++)
            {
                var inner = Background[x];
                var ilen = inner.Length;
                var newer = new int[ilen];
                Array.Copy(inner, newer, ilen);
                data[x] = newer;
            }

            var blackColor = System.Drawing.Color.Black.ToArgb();

            // Write _thumbs
            foreach (var t in Thumbs)
            {
                var xPos = (int)(t.X * Max.X);
                var yPos = (int)(t.Y * Max.Y);
                for (int x = 0; x < _thumbsize; x++) // Draw first and last line
                {
                    data[yPos][xPos + x] = blackColor;
                    data[yPos + _thumbsize - 1][xPos + x] = blackColor;
                    if (t.Color.A != 0) // Transparent background, no need to bother
                    {
                        for (int y = 1; y < _thumbsize - 1; y++) // Fill
                        {
                            data[yPos + y][(int)(t.X * Max.X) + x] = t.Color.ToArgb();
                        }
                    }
                }
                for (int y = 0; y < _thumbsize; y++) // Draw first and last column
                {
                    data[yPos + y][xPos] = blackColor;
                    data[yPos + y][xPos + _thumbsize - 1] = blackColor;
                }
            }
            // Render data
            renderer.RenderData = data;
        }

        private (double X, double Y) Max
        {
            get
            {
                var x = Bounds.Width - _thumbsize;
                var y = Bounds.Height - _thumbsize;
                return (x == 0 ? .01 : x, y == 0 ? .01 : y);
            }
        }

        public event EventHandler DragDelta;
        public event EventHandler<ThumbEventArgs> OnClick;

        private readonly List<Thumb> _toAdd = new();
        public List<Thumb> Thumbs { get; } = new();

        private int[][] _background;
        public int[][] Background
        {
            get
            {
                if (_background == null)
                {
                    var renderer = this.FindControl<RenderView>("Renderer");
                    _background = new int[(int)renderer.Bounds.Height][];
                    for (int y = 0; y < (int)renderer.Bounds.Height; y++)
                    {
                        _background[y] = new int[(int)renderer.Bounds.Width];
                    }
                }
                return _background;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
