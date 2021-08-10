using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace ExtendedAvalonia.Slider
{
    public partial class ExtendedThumb : UserControl
    {
        public ExtendedThumb()
        {
            InitializeComponent();

            this.FindControl<Thumb>("Thumb").DragDelta += (sender, e) =>
            {
                // Value of the cursor
                Value += e.Vector.X;
            };
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var thumb = this.FindControl<Thumb>("Thumb");
            var min = -Parent.Bounds.Width / 2f + thumb.Bounds.Width / 2f;
            var max = -min;

            // Make sure we aren't out of bounds
            if (Value < min) Value = min;
            else if (Value > max) Value = max;

            // Update position
            var measure = ((ILayoutable)thumb).PreviousMeasure;
            thumb.Arrange(new Rect(Value, 0, measure.Value.Width, measure.Value.Height));

            DragDelta?.Invoke(this, new());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public event EventHandler DragDelta;

        private double _value;
        public double Value
        {
            set
            {
                _value = value;
                InvalidateMeasure();
            }
            get
            {
                return _value;
            }
        }
    }
}
