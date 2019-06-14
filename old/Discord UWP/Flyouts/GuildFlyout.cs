using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using DiscordAPI.SharedModels;
using Quarrel.LocalModels;
using Quarrel.Managers;

namespace Quarrel.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeGuildMenu(LocalModels.Guild guild)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Add "Edit Server" button
            MenuFlyoutItem editServer = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/EditServer"),
                Tag = guild.Raw.Id,
                Icon = new SymbolIcon(Symbol.Edit),
            };
            editServer.Click += FlyoutManager.EditServer;
            menu.Items.Add(editServer);


            // Add Seperator
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);

            // Create "Mute/Unmute" button
            MenuFlyoutItem mute = new MenuFlyoutItem()
            {
                Icon = new SymbolIcon(Symbol.Mute),
                Tag = guild.Raw.Id,
            };
            mute.Click += FlyoutManager.MuteServer;

            // If muted, unmute
            if (LocalState.GuildSettings.ContainsKey(guild.Raw.Id) && LocalState.GuildSettings[guild.Raw.Id].raw.Muted)
                mute.Text = App.GetString("/Flyouts/UnmuteServer");

            // If not muted, mute
            else
                mute.Text = App.GetString("/Flyouts/MuteServer");

            // Add "Mute/Unmute" button
            menu.Items.Add(mute);

            // Add "Mark as Read" button
            MenuFlyoutItem markasread = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/MarkAsRead"),
                Tag = guild.Raw.Id,
                Icon = new SymbolIcon(Symbol.View),
                //IsEnabled = (ServerList.Items.FirstOrDefault(x => (x as SimpleGuild).Id == guild.RawGuild.Id) as SimpleGuild).IsUnread //TODO: Disable Make as read if not unread
            };
            markasread.Click += FlyoutManager.MarkGuildasRead;
            menu.Items.Add(markasread);

            // If current user is owner
            if (guild.Raw.OwnerId == LocalState.CurrentUser.Id)
            {
                // Add "Delete Server" button
                MenuFlyoutItem deleteServer = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/DeleteServer"),
                    Tag = guild.Raw.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.Delete),
                };
                deleteServer.Click += FlyoutManager.DeleteServer;
                menu.Items.Add(deleteServer);
            }
            else
            {
                // Add "Leave Server" button
                MenuFlyoutItem leaveServer = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/LeaveServer"),
                    Tag = guild.Raw.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.Remove),
                };
                leaveServer.Click += FlyoutManager.LeaveServer;
                menu.Items.Add(leaveServer);
            }
             
            return menu;
        }
    }
}
