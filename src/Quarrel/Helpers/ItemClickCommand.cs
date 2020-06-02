// Copyright (c) Quarrel. All rights reserved.

using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Helpers
{
    /// <summary>
    /// Adds ItemClick <see cref="ICommand"/> to a <see cref="DependencyObject"/>.
    /// </summary>
    public static class ItemClickCommand
    {
        /// <summary>
        /// A property representing the command.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(ItemClickCommand),
                new PropertyMetadata(null, OnCommandPropertyChanged));

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

            if (command != null && command.CanExecute(e.ClickedItem))
            {
                command.Execute(e.ClickedItem);
            }
        }
    }
}