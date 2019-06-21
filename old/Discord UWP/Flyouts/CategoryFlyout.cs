using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using Quarrel.Managers;

using GuildChannel = DiscordAPI.SharedModels.GuildChannel;

namespace Quarrel.Flyouts
{
    partial class FlyoutCreator
    {
        /// <summary>
        /// Make flyout for Category
        /// </summary>
        /// <param name="category">Category control</param>
        /// <param name="parentId">Guild Id</param>
        /// <returns>A MenuFlyout item to display</returns>
        public static MenuFlyout MakeCategoryMenu(GuildChannel category, string parentId)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Add "Mark As Read" button
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
