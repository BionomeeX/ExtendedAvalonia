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
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
