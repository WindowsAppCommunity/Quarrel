using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace Quarrel.Xaml.Behaviors
{
    public class KeyDownCommandAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var e = parameter as KeyRoutedEventArgs;
            if (e.Key == (VirtualKey) Enum.Parse(typeof(VirtualKey), Key))
            {
                KeyboardCapabilities keyboardCapabilities = new KeyboardCapabilities();
                if (keyboardCapabilities.KeyboardPresent > 0)
                {
                    if (ShiftCommand != null && CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                    {
                        ShiftCommand.Execute(parameter);
                        e.Handled = true;
                    }
                    else
                    {
                        Command.Execute(parameter);
                        e.Handled = true;
                    }
                }
                else if(ExecuteShiftCommandIfNoKeyboard)
                {
                    ShiftCommand.Execute(parameter);
                    e.Handled = true;
                }
                else
                {
                    Command.Execute(parameter);
                    e.Handled = true;
                }
                //e.Handled = true;
            }

            return null;
        }

        public string Key
        {
            get => (string)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(nameof(Key), typeof(string), typeof(KeyDownCommandAction), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(KeyDownCommandAction), new PropertyMetadata(null));

        public ICommand ShiftCommand
        {
            get => (ICommand)GetValue(ShiftCommandProperty);
            set => SetValue(ShiftCommandProperty, value);
        }
        public static readonly DependencyProperty ShiftCommandProperty =
            DependencyProperty.Register(nameof(ShiftCommand), typeof(ICommand), typeof(KeyDownCommandAction), new PropertyMetadata(null));

        public bool ExecuteShiftCommandIfNoKeyboard
        {
            get => (bool)GetValue(ExecuteShiftCommandIfNoKeyboardProperty);
            set => SetValue(ExecuteShiftCommandIfNoKeyboardProperty, value);
        }
        public static readonly DependencyProperty ExecuteShiftCommandIfNoKeyboardProperty =
            DependencyProperty.Register(nameof(ExecuteShiftCommandIfNoKeyboard), typeof(ICommand), typeof(KeyDownCommandAction), new PropertyMetadata(null));
    }
}
