using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System.Drawing;

namespace ExtendedAvalonia
{
    // https://github.com/funwaywang/WpfRangeSlider
    public partial class GradientSlider : UserControl
    {
        public GradientSlider()
        {
            InitializeComponent();
            var renderer = this.FindControl<RenderView>("Renderer");
            this.FindControl<Thumb>("Thumb").DragDelta += (sender, e) =>
            {
                var thumb = (Thumb)sender;
                var measure = ((ILayoutable)thumb).PreviousMeasure;
                _value += e.Vector.X;

                thumb.Arrange(new Rect(_value, 0, measure.Value.Width, measure.Value.Height));

                int[][] data = new int[(int)measure.Value.Height][];
                for (int y = 0; y < (int)measure.Value.Height; y++)
                {
                    data[y] = new int[(int)measure.Value.Width];
                    for (int x = 0; x < (int)measure.Value.Width; x++)
                    {
                        data[y][x] = Color.Red.ToArgb();
                    }
                }

                renderer.RenderData = data;
                renderer.InvalidateVisual();
            };
            this.Initialized += (sender, e) =>
            {
                DisplayGradient();
            };
        }

        public void DisplayGradient()
        {
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

        private double _value;
    }
}
