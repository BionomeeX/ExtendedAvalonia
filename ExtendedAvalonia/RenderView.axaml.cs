using Avalonia;
using Avalonia.Controls;
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
            if (_renderData == null)
            {
                return;
            }

            // 2D to 1D array
            var newArray = new List<int>();
            foreach (var line in _renderData)
            {
                newArray.AddRange(line);
            }

            // Write data on the control
            using var bmp = new WriteableBitmap(new PixelSize(_renderData[0].Length, _renderData.Length), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
            using var bmpLock = bmp.Lock();
            Marshal.Copy(newArray.ToArray(), 0, bmpLock.Address, newArray.Count);
            context?.DrawImage(bmp, new Rect(0, 0, _renderData[0].Length, _renderData.Length));
        }

        private int[][] _renderData;

        public int[][] RenderData
        {
            set
            {
                _renderData = value;
                InvalidateVisual();
            }
        }
    }
}
