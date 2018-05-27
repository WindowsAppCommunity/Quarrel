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
        public static MenuFlyout MakeTextChnMenu(LocalModels.GuildChannel chn)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];
            //MenuFlyoutItem PinChannel = new MenuFlyoutItem()
            //{
            //    Text = SecondaryTile.Exists(chn.Raw.Id) ? "Unpin From Start" : "Pin To Start",
            //    Tag = chn.Raw,
            //    Icon = SecondaryTile.Exists(chn.Raw.Id) ? new SymbolIcon(Symbol.UnPin) : new SymbolIcon(Symbol.Pin),
            //    Margin = new Thickness(-26, 0, 0, 0)
            //};
            //PinChannel.Click += PinChannelToStart;
            //menu.Items.Add(PinChannel);
            MenuFlyoutItem editchannel = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/EditChannel"),
                Tag = chn.raw.Id,
                Icon = new SymbolIcon(Symbol.Edit),
                Margin = new Thickness(-26, 0, 0, 0)
            };
            editchannel.Click += FlyoutManager.EditChannel;
            menu.Items.Add(editchannel);
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);
            ToggleMenuFlyoutItem mute = new ToggleMenuFlyoutItem()
            {
                Icon = new SymbolIcon(Symbol.Mute),
                Tag = chn.raw.Id,
                Margin = new Thickness(-26, 0, 0, 0)
            };
            //mute.IsChecked = LocalState.GuildSettings.Contains(chn.Raw.Id);

            if (LocalState.GuildSettings[App.CurrentGuildId].channelOverrides.ContainsKey(chn.raw.Id) && LocalState.GuildSettings[App.CurrentGuildId].channelOverrides[chn.raw.Id].Muted)
                mute.Text = App.GetString("/Flyouts/UnmuteChannel");
            else
                mute.Text = App.GetString("/Flyouts/MuteChannel");

            mute.Click += FlyoutManager.MuteChannel;
            menu.Items.Add(mute);
            MenuFlyoutItem markasread = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/MarkAsRead"),
                Tag = chn.raw.Id,
                Icon = new SymbolIcon(Symbol.View),
                Margin = new Thickness(-26, 0, 0, 0),
                //IsEnabled = false
                //IsEnabled = (TextChannels.Items.FirstOrDefault(x => (x as SimpleChannel).Id == chn.Raw.Id) as SimpleChannel).IsUnread
            };
            menu.Items.Add(markasread);
            markasread.Click += FlyoutManager.MarkChannelasRead;
            MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
            menu.Items.Add(sep2);
            MenuFlyoutItem deleteChannel = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/DeleteChannel"),
                Tag = chn.raw.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.Delete),
                Margin = new Thickness(-26, 0, 0, 0),
            };

            if (!chn.permissions.ManageChannels && !chn.permissions.Administrator && LocalState.Guilds[chn.raw.GuildId].Raw.OwnerId != LocalState.CurrentUser.Id)
            {
                deleteChannel.IsEnabled = false;
            }
            menu.Items.Add(deleteChannel);
            deleteChannel.Click += FlyoutManager.DeleteChannel;

            return menu;
        }
    }
}
