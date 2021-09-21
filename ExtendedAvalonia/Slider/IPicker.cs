using Avalonia.Controls;
using System;

namespace ExtendedAvalonia.Slider
{
    public interface IPicker<Class, Elem>
        where Class : Window, IPicker<Class, Elem>, new()
    {
        internal static void Show(Window parent, Action<Elem> onChange, Action<Elem> onCompletion, Elem defaultValue)
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

            picker.OnChange = onChange;
            picker.OnCompletion = onCompletion;

            picker.Init(defaultValue);
        }

        internal void Init(Elem defaultValue);

        internal Action<Elem> OnChange { set; get; }
        internal Action<Elem> OnCompletion { set; get; }
    }
}
