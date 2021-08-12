using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
                ColorPicker.Show(this, (_) => { });
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
