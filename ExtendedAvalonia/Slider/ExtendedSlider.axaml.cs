using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;

namespace ExtendedAvalonia.Slider
{
    public partial class ExtendedSlider : UserControl
    {
        public double Value { private set; get; } = 0.0;

        public ExtendedSlider()
        {
            InitializeComponent();

            AddThumb();
        }

        public void AddThumb()
        {
            var t = new ExtendedThumb
            {
                Width = int.MaxValue
            };
            t.DragDelta += (sender, e) =>
            {
                DragDelta?.Invoke(sender, e);
            };
            this.FindControl<WrapPanel>("Slider").Children.Add(t);
        }

        public (double min, double max) GetMinMax()
        {
            var thumbSize = 20f;
            var min = -Bounds.Width / 2f + thumbSize / 2f;
            return (min, -min);
        }

        public event EventHandler DragDelta;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
