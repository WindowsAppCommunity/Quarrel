// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Reflection;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Quarrel.Helpers
{
    /// <summary>
    /// Adds ItemClick <see cref="ICommand"/> to a <see cref="DependencyObject"/>.
    /// </summary>
    public static class ItemClickCommandParameter
    {
        /// <summary>
        /// A property representing the command.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(ItemClickCommandParameter),
                new PropertyMetadata(null, OnCommandPropertyChanged));

        /// <summary>
        /// A property representing the command.
        /// </summary>
        public static readonly DependencyProperty CommanParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(ItemClickCommandParameter),
                new PropertyMetadata(null));

        /// <summary>
        /// Sets the <see cref="ICommand"/> on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="d">A <see cref="DependencyObject"/> to add an ItemClick command to.</param>
        /// <param name="value"><see cref="ICommand"/> to run on ItemClick.</param>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="d">A <see cref="DependencyObject"/> to add an ItemClick command to.</param>
        /// <returns><see cref="ICommand"/> to run on ItemClick.</returns>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the <see cref="object"/> on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="d">A <see cref="DependencyObject"/> to add an ItemClick command parameter to.</param>
        /// <param name="value"><see cref="object"/> parameter for a command.</param>
        public static void SetCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(CommanParameterProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="object"/> on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="d">A <see cref="DependencyObject"/> to add an ItemClick command parameter to.</param>
        /// <returns><see cref="object"/> parameter for a command.</returns>
        public static object GetCommandParameter(DependencyObject d)
        {
            return d.GetValue(CommanParameterProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListViewBase control)
            {
                control.ItemClick += OnItemClick;
            }
        }

        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            ListViewBase control = sender as ListViewBase;
            ICommand command = GetCommand(control);
            object parameter = GetCommandParameter(control);

            if (command != null && command.CanExecute(e.ClickedItem))
            {
                // Ugly hack to convert tuple to correct type
                var types = command.GetType().GetGenericArguments()[0].GetGenericArguments();
                var tupleType = typeof(ValueTuple<,>).MakeGenericType(types);
                var tuple = Activator.CreateInstance(tupleType, e.ClickedItem, parameter);
                command.Execute(tuple);
            }
        }
    }
}
