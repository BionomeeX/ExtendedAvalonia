using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ExtendedAvalonia
{
    public partial class RenderView : UserControl
    {
        public override void Render(DrawingContext context)
        {
            if (RenderData == null)
            {
                return;
            }

            // 2D to 1D array
            var newArray = new List<int>();
            foreach (var line in RenderData)
            {
                newArray.AddRange(line);
            }

            // Write data on the control
            using var bmp = new WriteableBitmap(new PixelSize(RenderData[0].Length, RenderData.Length), new Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, AlphaFormat.Unpremul);
            using var bmpLock = bmp.Lock();
            Marshal.Copy(newArray.ToArray(), 0, bmpLock.Address, newArray.Count);
            context?.DrawImage(bmp, new Rect(0, 0, RenderData[0].Length, RenderData.Length));
        }

        public int[][] RenderData { set; private get; }
    }
}
