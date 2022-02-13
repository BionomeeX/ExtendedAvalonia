using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtendedAvalonia.Slider;
using System.Drawing;

namespace ExtendedAvalonia
{
    public partial class DemoWindow : Window
    {
        public DemoWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.FindControl<Button>("ColorPicker").Click += (sender, e) =>
            {
                ColorPicker.Show(this, Color.Cyan);
            };
            this.FindControl<Button>("GradientPicker").Click += (sender, e) =>
            {
                GradientPicker.Show(this, new(
                    new PositionColor[]{
                        new() { Position = 0.0, Color = Color.Red },
                        new() { Position = 1.0, Color = Color.Blue }
                    }, 
                    0.0,
                    0.0
                ));
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
