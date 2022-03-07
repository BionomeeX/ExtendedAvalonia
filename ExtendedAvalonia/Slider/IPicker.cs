using Avalonia.Controls;
using ExtendedAvalonia.Event;

namespace ExtendedAvalonia.Slider
{
    public interface IPicker<Class, Elem>
        where Class : Window, IPicker<Class, Elem>, new()
    {
        internal static Class Show(Window parent, Elem defaultValue)
        {
            var picker = new Class();
            if (parent == null)
            {
                picker.Show();
            }
            else
            {
                picker.Show(parent);
            }

            picker.Init(defaultValue);

            return picker;
        }

        internal void Init(Elem defaultValue);

        public event EventHandler<DataEventArgs<Elem>> OnChange;
        public event EventHandler<DataEventArgs<Elem>> OnCompletion;
        public event EventHandler OnCancel;
    }
}
