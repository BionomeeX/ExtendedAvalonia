using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
                ColorPicker.Show(this, null, Color.Cyan);
            };
            this.FindControl<Button>("GradientPicker").Click += (sender, e) =>
            {
                GradientPicker.Show(this, null, Color.Cyan);
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
