using System;

namespace ExtendedAvalonia.Event
{
    public class DataEventArgs<T> : EventArgs
    {
        public T Data { init; get; }
    }
}
