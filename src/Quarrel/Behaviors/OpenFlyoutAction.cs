// Quarrel © 2022

using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Quarrel.Behaviors
{
    public class OpenFlyoutAction : DependencyObject, IAction
    {
        public object? Execute(object sender, object parameter)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            return null;
        }
    }
}
