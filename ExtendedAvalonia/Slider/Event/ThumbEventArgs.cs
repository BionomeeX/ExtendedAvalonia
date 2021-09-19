﻿using System;

namespace ExtendedAvalonia.Slider.Event
{
    public class ThumbEventArgs : EventArgs
    {
        public Thumb? Thumb { init; get; }
        public double Position { init; get; }
    }
}