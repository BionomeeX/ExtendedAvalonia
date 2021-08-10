using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
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
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            AddThumb(0.0);
            AddThumb(20.0);
        }

        public void AddThumb(double position)
        {
            var t = new ExtendedThumb
            {
                Width = int.MaxValue,
                Value = position
            };
            t.DragDelta += (sender, e) =>
            {
                DragDelta?.Invoke(sender, e);
            };
            Thumbs.Add(t);
            this.FindControl<WrapPanel>("Slider").Children.Add(t);
        }

        public (double min, double max) GetMinMax()
        {
            var thumbSize = 20f;
            var min = -Bounds.Width / 2f + thumbSize / 2f;
            return (min, -min);
        }

        public event EventHandler DragDelta;

        public List<ExtendedThumb> Thumbs { get; } = new();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
