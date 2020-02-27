// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Quarrel.Xaml.Behaviors
{
    /// <summary>
    /// Action for opening the <see cref="TargetObject"/>'s Flyout.
    /// </summary>
    public class OpenFlyoutAction : DependencyObject, IAction
    {
        /// <summary>
        /// A property representing the object to open the flyout of.
        /// </summary>
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register(nameof(TargetObject), typeof(Control), typeof(OpenFlyoutAction), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the object to open the flyout of.
        /// </summary>
        public Control TargetObject
        {
            get => (Control)GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        /// <summary>
        /// Opens the <see cref="TargetObject"/>'s Flyout.
        /// </summary>
        /// <param name="sender">The object making the request (use if there is no TargetObject).</param>
        /// <param name="parameter">The <see cref="RoutedEventArgs"/> for tapping a <see cref="UIElement"/>.</param>
        /// <returns><see langword="null"/>.</returns>
        public object Execute(object sender, object parameter)
        {
            FlyoutBase.ShowAttachedFlyout(TargetObject ?? (FrameworkElement)sender);
            return null;
        }
    }
}
