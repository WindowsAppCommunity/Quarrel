// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Xaml.Interactivity;
using System;
using System.Windows.Input;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Quarrel.Xaml.Behaviors
{
    /// <summary>
    /// Action for handling a Keydown event with a <see cref="ICommand"/>.
    /// </summary>
    public class KeyDownCommandAction : DependencyObject, IAction
    {
        /// <summary>
        /// A property representing the Key to handle an event on.
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
               DependencyProperty.Register(nameof(Key), typeof(string), typeof(KeyDownCommandAction), new PropertyMetadata(null));

        /// <summary>
        /// A property representing the Command to run when invoked.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(KeyDownCommandAction), new PropertyMetadata(null));

        /// <summary>
        /// A property representing the Command to run when invoked with shift key modifier.
        /// </summary>
        public static readonly DependencyProperty ShiftCommandProperty =
            DependencyProperty.Register(nameof(ShiftCommand), typeof(ICommand), typeof(KeyDownCommandAction), new PropertyMetadata(null));

        /// <summary>
        /// A property representing whether or not to use the <see cref="ShiftCommand"/> by default when the device has no Keyboard.
        /// </summary>
        public static readonly DependencyProperty ExecuteShiftCommandIfNoKeyboardProperty =
            DependencyProperty.Register(nameof(ExecuteShiftCommandIfNoKeyboard), typeof(ICommand), typeof(KeyDownCommandAction), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating which key was pressed.
        /// </summary>
        public string Key
        {
            get => (string)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to run when this command action is hit.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to run when this command action is hit with the Shift as key as a modifer.
        /// </summary>
        public ICommand ShiftCommand
        {
            get => (ICommand)GetValue(ShiftCommandProperty);
            set => SetValue(ShiftCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="ShiftCommand"/> should run by default when there's no keyboard on the device.
        /// </summary>
        public bool ExecuteShiftCommandIfNoKeyboard
        {
            get => (bool)GetValue(ExecuteShiftCommandIfNoKeyboardProperty);
            set => SetValue(ExecuteShiftCommandIfNoKeyboardProperty, value);
        }

        /// <summary>
        /// Executes key down Command.
        /// </summary>
        /// <param name="sender">The control invoking the Command.</param>
        /// <param name="parameter"><see cref="KeyRoutedEventArgs"/> parameters to KeyDown event.</param>
        /// <returns>The character represented by the key press.</returns>
        public object Execute(object sender, object parameter)
        {
            var e = parameter as KeyRoutedEventArgs;
            if (Key == null || e.Key == (VirtualKey)Enum.Parse(typeof(VirtualKey), Key))
            {
                KeyboardCapabilities keyboardCapabilities = new KeyboardCapabilities();
                if (keyboardCapabilities.KeyboardPresent > 0)
                {
                    if (ShiftCommand != null && CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                    {
                        ShiftCommand.Execute(KeyToChar(e.Key, true));
                    }
                    else
                    {
                        Command.Execute(KeyToChar(e.Key, false));
                    }
                }
                else if (ExecuteShiftCommandIfNoKeyboard)
                {
                    ShiftCommand.Execute(KeyToChar(e.Key, true));
                }
                else
                {
                    Command.Execute(KeyToChar(e.Key, false));
                }
            }

            return null;
        }

        private char KeyToChar(VirtualKey key, bool shift)
        {
            int c = (int)key;
            if (shift && char.IsLetter((char)c))
            {
                c += 32;
            }

            return (char)c;
        }
    }
}
