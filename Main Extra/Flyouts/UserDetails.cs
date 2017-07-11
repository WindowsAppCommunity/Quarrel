using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Discord_UWP.CacheModels;
using Discord_UWP.Controls;
using Discord_UWP.SharedModels;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private Flyout MakeUserDetailsFlyout(Member member)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new UserDetailsControl()
            {
                DisplayedMember = member
            };
            flyout.FlyoutPresenterStyle = (Style) App.Current.Resources["FlyoutPresenterStyle1"];
            return flyout;
        }
    }
}
