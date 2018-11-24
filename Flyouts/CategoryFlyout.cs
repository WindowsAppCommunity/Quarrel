using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Discord_UWP.Managers;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeCategoryMenu(SharedModels.GuildChannel category, string parentId)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            MenuFlyoutItem markasread = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/MarkAsRead"),
                Icon = new SymbolIcon(Symbol.View),
                Tag = new Tuple<string, string>(category.Id, parentId)
            };
            markasread.Click += FlyoutManager.MarkCategoryAsRead;
            menu.Items.Add(markasread);

            return menu;
        }
    }
}
