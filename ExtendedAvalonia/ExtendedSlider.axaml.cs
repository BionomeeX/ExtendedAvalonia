using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System;

namespace ExtendedAvalonia
{
    public partial class ExtendedSlider : UserControl
    {
        public double Value { private set; get; } = 0.0;

        public ExtendedSlider()
        {
            InitializeComponent();

            this.FindControl<Thumb>("Thumb").DragDelta += (sender, e) =>
            {
                // Value of the cursor
                Value += e.Vector.X;


                var thumb = this.FindControl<Thumb>("Thumb");
                var min = -Bounds.Width / 2f + thumb.Bounds.Width / 2f;
                var max = -min;

                // Make sure we aren't out of bounds
                if (Value < min) Value = min;
                else if (Value > max) Value = max;

                // Update position
                var measure = ((ILayoutable)thumb).PreviousMeasure;
                thumb.Arrange(new Rect(Value, 0, measure.Value.Width, measure.Value.Height));

                DragDelta?.Invoke(this, new());
            };
        }

        public (double min, double max) GetMinMax()
        {
            var thumb = this.FindControl<Thumb>("Thumb");
            var min = -Bounds.Width / 2f + thumb.Bounds.Width / 2f;
            return (min, -min);
        }

        public event EventHandler DragDelta;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
