// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Quarrel.Xaml.Behaviors
{
    /// <summary>
    /// Action on a <see cref="ListView"/> to open a <see cref="ListViewItem"/>'s Flyout.
    /// </summary>
    public class OpenListViewItemFlyoutAction : DependencyObject, IAction
    {
        /// <summary>
        /// Opens a <see cref="ListViewItem"/>'s Flyout.
        /// </summary>
        /// <param name="sender">The <see cref="ListView"/> the <see cref="ListViewItem"/> is in.</param>
        /// <param name="parameter">The <see cref="ItemClickEventArgs"/> containing the <see cref="ListViewItem"/>.</param>
        /// <returns><see langword="null"/>.</returns>
        public object Execute(object sender, object parameter)
        {
            if (((sender as ListView)?
                .ContainerFromItem((parameter as ItemClickEventArgs)?.ClickedItem) as ListViewItem)?
                .ContentTemplateRoot is FrameworkElement flyout && FlyoutBase.GetAttachedFlyout(flyout) != null)
            {
                FlyoutBase.ShowAttachedFlyout(flyout);
            }

            return null;
        }
    }
}
