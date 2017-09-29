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
        public Flyout MakeUserDetailsFlyout(Member member)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new UserDetailsControl()
            {
                DisplayedMember = member
            };
            flyout.FlyoutPresenterStyle = (Style) App.Current.Resources["FlyoutPresenterStyle1"];
            return flyout;
        }

        public Flyout MakeUserDetailsFlyout(SharedModels.User user)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new UserDetailsControl()
            {
                DisplayedMember = new Member() { Raw = new GuildMember() { User = user }}
            };
            flyout.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyle1"];
            return flyout;
        }
    }
}
