using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace Quarrel.Xaml.Behaviors
{
    class OpenListViewItemFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            FlyoutBase.ShowAttachedFlyout(((sender as ListView).ContainerFromItem((parameter as ItemClickEventArgs).ClickedItem) as ListViewItem).ContentTemplateRoot as FrameworkElement);
            return null;
        }

    }
}
