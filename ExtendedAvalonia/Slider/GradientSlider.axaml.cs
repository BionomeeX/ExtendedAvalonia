using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Drawing;

namespace ExtendedAvalonia.Slider
{
    // https://github.com/funwaywang/WpfRangeSlider
    public partial class GradientSlider : UserControl
    {
        public GradientSlider()
        {
            InitializeComponent();
        }

        private bool _isInit;

        public override void Render(Avalonia.Media.DrawingContext context)
        {
            if (!_isInit)
            {
                _isInit = true;

                var upSlider = this.FindControl<ExtendedSlider>("SliderUp");
                upSlider.AddThumb(new Thumb() { Position = 0.0, Color = Color.Transparent });
                upSlider.AddThumb(new Thumb() { Position = 1.0, Color = Color.Transparent });

                var downSlider = this.FindControl<ExtendedSlider>("SliderDown");
                downSlider.AddThumb(new Thumb() { Position = 0.0, Color = Color.Transparent });
            }

            // Renderer display a big square of our color
            var renderer = this.FindControl<RenderView>("Renderer");

            int[][] data = new int[(int)renderer.Bounds.Height][];
            for (int y = 0; y < (int)renderer.Bounds.Height; y++)
            {
                data[y] = new int[(int)renderer.Bounds.Width];
                for (int x = 0; x < (int)renderer.Bounds.Width; x++)
                {
                    data[y][x] = Color.Red.ToArgb();
                }
            }

            renderer.RenderData = data;
            renderer.InvalidateVisual();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
