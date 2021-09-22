using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Event;
using ExtendedAvalonia.Impl;
using ExtendedAvalonia.Slider;
using System;

namespace ExtendedAvalonia
{
    public partial class GradientPicker : Window, IPicker<GradientPicker, PositionColor[]>
    {
        public static GradientPicker Show(Window parent, PositionColor[] defaultValue)
            => IPicker<GradientPicker, PositionColor[]>.Show(parent, defaultValue);

        public event EventHandler<DataEventArgs<PositionColor[]>> OnChange;
        public event EventHandler<DataEventArgs<PositionColor[]>> OnCompletion;
        public event EventHandler OnCancel;

        public GradientPicker()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnClose(object sender, EventArgs e)
        {
            OnCancel?.Invoke(sender, e);
        }

        void IPicker<GradientPicker, PositionColor[]>.Init(PositionColor[] defaultValue)
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
    }
}
