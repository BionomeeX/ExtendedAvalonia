using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Event;
using ExtendedAvalonia.Impl;
using ExtendedAvalonia.Slider;
using System.Drawing;

namespace ExtendedAvalonia
{
    public partial class GradientPicker : Window, IPicker<GradientPicker, Gradient>
    {
        public static GradientPicker Show(Window parent, Gradient defaultValue)
            => IPicker<GradientPicker, Gradient>.Show(parent, defaultValue);

        public event EventHandler<DataEventArgs<Gradient>> OnChange;
        public event EventHandler<DataEventArgs<Gradient>> OnCompletion;
        public event EventHandler OnCancel;

        public GradientPicker()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnClose(object sender, EventArgs e)
        {
            OnCancel?.Invoke(sender, e);
        }

        void IPicker<GradientPicker, Gradient>.Init(Gradient defaultValue)
        {
            Closed += OnClose;

            var impl = this.FindControl<GradientPickerImpl>("GradientImpl");
            impl.Init(
                me: this,
                defaultValue: defaultValue,
                onChange: (pc) =>
                {
                    OnChange?.Invoke(this, new() { Data = pc });
                }
            );
            this.FindControl<Button>("Validate").Click += (sender, e) =>
            {
                OnCompletion?.Invoke(this, new() { Data = impl.GetData() });
                Closed -= OnClose;
                Close();
            };
        }

        public static Color GetColorFromPosition(Gradient gradient, double position)
        {
            var thumbs = gradient.PositionColors;

            if (!thumbs.Any())
            {
                return Color.White;
            }

            PositionColor min, max;

            if (thumbs.Any(t => t.Position <= position)) // We found a slider lower than our value
            {
                min = thumbs.Where(t => t.Position <= position).OrderByDescending(t => t.Position).ToArray()[0]; // Closest to our left
            }
            else
            {
                return thumbs.OrderBy(t => t.Position).ToArray()[0].Color; // Else we can just return the smaller one
            }

            // Then we do the same for max
            if (thumbs.Any(t => t.Position >= position))
            {
                max = thumbs.Where(t => t.Position >= position).OrderBy(t => t.Position).ToArray()[0];
            }
            else
            {
                return thumbs.OrderByDescending(t => t.Position).ToArray()[0].Color;
            }

            if (min.Color == max.Color) // Nothing to do since min and max are same color
            {
                return min.Color;
            }

            var percent = (position - min.Position) / (max.Position - min.Position); // Percent between 0 and 1

            return Color.FromArgb(
                alpha: 255,
                red: (int)(percent * max.Color.R + (1 - percent) * min.Color.R),
                green: (int)(percent * max.Color.G + (1 - percent) * min.Color.G),
                blue: (int)(percent * max.Color.B + (1 - percent) * min.Color.B)
            );
        }
    }
}
