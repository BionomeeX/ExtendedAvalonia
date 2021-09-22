using Avalonia.Input;
using ExtendedAvalonia.Slider;
using System;

namespace ExtendedAvalonia.Event
{
    public class ThumbEventArgs : EventArgs
    {
        public Thumb? Thumb { init; get; }
        public double X { init; get; }
        public double Y { init; get; }
        public MouseButton MouseButton { init; get; }
    }
}
