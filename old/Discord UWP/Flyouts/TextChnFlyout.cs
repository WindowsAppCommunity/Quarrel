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
        public static MenuFlyout MakeTextChnMenu(LocalModels.GuildChannel chn)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Add "Edit Channel" button
            MenuFlyoutItem editchannel = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/EditChannel"),
                Tag = chn.raw.Id,
                Icon = new SymbolIcon(Symbol.Edit),
                Margin = new Thickness(-26, 0, 0, 0)
            };
            editchannel.Click += FlyoutManager.EditChannel;
            menu.Items.Add(editchannel);


            // Add Separator
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);


            // Create "Mute/Unmute" button
            ToggleMenuFlyoutItem mute = new ToggleMenuFlyoutItem()
            {
                Icon = new SymbolIcon(Symbol.Mute),
                Tag = chn.raw.Id,
                Margin = new Thickness(-26, 0, 0, 0)
            };
            mute.Click += FlyoutManager.MuteChannel;
            //mute.IsChecked = LocalState.GuildSettings.Contains(chn.Raw.Id);

            // If muted, unmute
            if (LocalState.GuildSettings.ContainsKey(App.CurrentGuildId) && LocalState.GuildSettings[App.CurrentGuildId].channelOverrides.ContainsKey(chn.raw.Id) && LocalState.GuildSettings[App.CurrentGuildId].channelOverrides[chn.raw.Id].Muted)
                mute.Text = App.GetString("/Flyouts/UnmuteChannel");

            // If not muted, mute
            else
                mute.Text = App.GetString("/Flyouts/MuteChannel");

            // Add "Mute/Unmute" button
            menu.Items.Add(mute);

            // Add "Mark as Read" button
            MenuFlyoutItem markasread = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/MarkAsRead"),
                Tag = chn.raw.Id,
                Icon = new SymbolIcon(Symbol.View),
                Margin = new Thickness(-26, 0, 0, 0),
                //IsEnabled = false
                //IsEnabled = (TextChannels.Items.FirstOrDefault(x => (x as SimpleChannel).Id == chn.Raw.Id) as SimpleChannel).IsUnread
            };
            markasread.Click += FlyoutManager.MarkChannelasRead;
            menu.Items.Add(markasread);


            // Add Separator
            MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
            menu.Items.Add(sep2);


            // Create "Delete Channel" button
            MenuFlyoutItem deleteChannel = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/DeleteChannel"),
                Tag = chn.raw.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.Delete),
                Margin = new Thickness(-26, 0, 0, 0),
            };
            deleteChannel.Click += FlyoutManager.DeleteChannel;
            
            // If permissions to delete channel
            if (chn.permissions.ManageChannels || chn.permissions.Administrator || LocalState.Guilds[chn.raw.GuildId].Raw.OwnerId == LocalState.CurrentUser.Id)
            {
                // Add "Delete Channel" button
                menu.Items.Add(deleteChannel);
            }


            return menu;
        }
    }
}
