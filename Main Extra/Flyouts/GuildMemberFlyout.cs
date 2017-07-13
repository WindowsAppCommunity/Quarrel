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
        private MenuFlyout MakeGuildMemberMenu(Member member)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];
            MenuFlyoutItem profile = new MenuFlyoutItem()
            {
                Text = "Profile",
                Tag = member.Raw.User.Id,
                Icon = new SymbolIcon(Symbol.ContactInfo)
            };
            profile.Click += gotoProfile;
            menu.Items.Add(profile);
            MenuFlyoutItem message = new MenuFlyoutItem()
            {
                Text = "Message",
                Tag = member.Raw.User.Id,
                Icon = new SymbolIcon(Symbol.Message)
            };
            message.Click += mentionUser;
            menu.Items.Add(message);
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            MenuFlyoutSubItem InviteToServer = new MenuFlyoutSubItem()
            {
                Text = "Invite to Server",
                Tag = member.Raw.User.Id
                //Icon = new SymbolIcon(Symbol.),
            };
            foreach (KeyValuePair<string, Guild> guild in Storage.Cache.Guilds)
            {
                if (guild.Value.perms.EffectivePerms.Administrator || guild.Value.perms.EffectivePerms.CreateInstantInvite)
                {
                    MenuFlyoutItem item = new MenuFlyoutItem() { Text = guild.Value.RawGuild.Name, Tag = new Tuple<string, string>(guild.Value.Channels.FirstOrDefault().Value.Raw.Id, member.Raw.User.Id) };
                    item.Click += inviteToServer;
                    InviteToServer.Items.Add(item);
                }

            }
            menu.Items.Add(InviteToServer);
            return menu;
        }

        private void inviteToServer(object sender, RoutedEventArgs e)
        {
            //TODO: Generate single use temp invite, send to selected user 
            Tuple<string, string> data = (sender as MenuFlyoutItem).Tag as Tuple<string, string>;
            Task.Run(async () => 
            {
                SharedModels.Invite invite = await Session.CreateInvite(data.Item1, new SharedModels.CreateInvite() { MaxUses = 1, Unique = true });
                //TODO: Make more effiecient
                foreach (KeyValuePair<string, DmCache> dm in Storage.Cache.DMs)
                {
                    if (dm.Value.Raw.Users.Count() == 1 && dm.Value.Raw.Users.FirstOrDefault().Id == data.Item2)
                    {
                        await Session.CreateMessage(dm.Key, "https://discord.gg/" + invite.String);
                        App.NavigateToChannel(null, dm.Key);
                        return;
                    }
                }
            });
        }

        private void mentionUser(object sender, RoutedEventArgs e)
        {
            App.MentionUser((sender as MenuFlyoutItem).Tag.ToString());
        }

        private void gotoProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateToProfile((sender as MenuFlyoutItem).Tag.ToString());
        }
    }
}
