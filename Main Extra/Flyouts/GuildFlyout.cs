using Discord_UWP.API;
using Discord_UWP.API.Channel;
using Discord_UWP.API.Channel.Models;
using Discord_UWP.API.Gateway;
using Discord_UWP.API.Guild;
using Discord_UWP.API.Login;
using Discord_UWP.API.Login.Models;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Discord_UWP.Authentication;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Animation;
using Discord_UWP.CacheModels;
using Discord_UWP.Gateway.DownstreamEvents;
using Microsoft.Toolkit.Uwp;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private MenuFlyout MakeGuildMenu(Guild guild)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];
            MenuFlyoutItem editServer = new MenuFlyoutItem()
            {
                Text = "Edit Server",
                Tag = guild.RawGuild.Id,
                Icon = new SymbolIcon(Symbol.Edit),
                Margin = new Thickness(-26, 0, 0, 0)
            };
            editServer.Click += EditServer;
            menu.Items.Add(editServer);
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);
            ToggleMenuFlyoutItem mute = new ToggleMenuFlyoutItem()
            {
                Text = "Mute Server",
                Icon = new SymbolIcon(Symbol.Mute),
                Tag = guild.RawGuild.Id,
                Margin = new Thickness(-26, 0, 0, 0)
            };
            mute.IsChecked = Storage.MutedServers.Contains(guild.RawGuild.Id);
            mute.Click += MuteServer;
            menu.Items.Add(mute);
            MenuFlyoutItem markasread = new MenuFlyoutItem()
            {
                Text = "Mark as read",
                Tag = guild.RawGuild.Id,
                Icon = new SymbolIcon(Symbol.View),
                Margin = new Thickness(-26, 0, 0, 0),
                IsEnabled = (ServerList.Items.FirstOrDefault(x => (x as SimpleGuild).Id == guild.RawGuild.Id) as SimpleGuild).IsUnread
            };
            markasread.Click += MarkGuildasRead;
            menu.Items.Add(markasread);
            if (guild.RawGuild.OwnerId == Storage.Cache.CurrentUser.Raw.Id)
            {
                MenuFlyoutItem deleteServer = new MenuFlyoutItem()
                {
                    Text = "Delete Server",
                    Tag = guild.RawGuild.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.Delete),
                    Margin = new Thickness(-26, 0, 0, 0)
                };
                deleteServer.Click += DeleteServer;
                menu.Items.Add(deleteServer);
            }
            else
            {
                MenuFlyoutItem leaveServer = new MenuFlyoutItem()
                {
                    Text = "Leave Server",
                    Tag = guild.RawGuild.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.Remove),
                    Margin = new Thickness(-26, 0, 0, 0)
                };
                leaveServer.Click += LeaveServerClick;
                menu.Items.Add(leaveServer);
            }
            return menu;
        }

        private void LeaveServerClick(object sender, RoutedEventArgs e)
        {
            App.NavigateToLeaveServer((sender as MenuFlyoutItem).Tag.ToString());
        }

        private void DeleteServer(object sender, RoutedEventArgs e)
        {
            App.NavigateToDeleteServer((sender as MenuFlyoutItem).Tag.ToString());
        }

        private void MuteServer(object sender, RoutedEventArgs e)
        {
            if (Storage.MutedServers.Contains((sender as MenuFlyoutItem).Tag.ToString()))
            {
                Storage.MutedServers.Remove((sender as MenuFlyoutItem).Tag.ToString());
            } else
            {
                Storage.MutedServers.Add((sender as MenuFlyoutItem).Tag.ToString());
            }
            Storage.SaveMutedChannels();

            foreach (SimpleGuild sg in ServerList.Items)
            {
                if (sg.Id == (sender as MenuFlyoutItem).Tag.ToString())
                {
                    sg.IsMuted = Storage.MutedServers.Contains((sender as MenuFlyoutItem).Tag.ToString());
                }
            }

            UpdateGuildAndChannelUnread();
        }

        private void MarkGuildasRead(object sender, RoutedEventArgs e)
        {
            Session.AckGuild((sender as MenuFlyoutItem).Tag.ToString());
        }

        private void EditServer(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuildEdit((sender as MenuFlyoutItem).Tag.ToString());
        }
    }
}
