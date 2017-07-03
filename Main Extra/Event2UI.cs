using Discord_UWP.API;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.CacheModels;
using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.Gateway;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;

#region CacheModels Overrule
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Message = Discord_UWP.CacheModels.Message;
using User = Discord_UWP.CacheModels.User;
using Guild = Discord_UWP.CacheModels.Guild;
using Friend = Discord_UWP.CacheModels.Friend;
#endregion

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private async void OnReady(object sender, GatewayEventArgs<Gateway.DownstreamEvents.Ready> e)
        {
            if (e.EventData.Notes != null)
                App.Notes = e.EventData.Notes;

            Storage.Cache.guildOrder.Clear();
            int pos = 0;

            Storage.Settings.DevMode = e.EventData.Settings.DevMode;

            if (e.EventData.Settings.Theme == "Light")
            {
                Storage.Settings.DiscordLightTheme = true;

                Storage.SaveAppSettings();
            }

            foreach (string guild in e.EventData.Settings.GuildOrder)
            {
                pos++;
                Storage.Cache.guildOrder.Add(guild, pos);
            }

            Storage.Cache.Friends.Clear();
            foreach (SharedModels.Friend friend in e.EventData.Friends)
            {
                Storage.Cache.Friends.Add(friend.Id, new Friend(friend));
            }

            Storage.Cache.DMs.Clear();
            foreach (DirectMessageChannel dm in e.EventData.PrivateChannels)
            {
                Storage.Cache.DMs.Add(dm.Id, new DmCache(dm));
            }

            Storage.Cache.Guilds.Clear();
            foreach (SharedModels.Guild guild in e.EventData.Guilds)
            {
                if (!Storage.Cache.Guilds.ContainsKey(guild.Id))
                {
                    Storage.Cache.Guilds.Add(guild.Id, new Guild(guild));
                }

                Storage.Cache.Guilds[guild.Id].Members.Clear();
                foreach (GuildMember member in guild.Members)
                {
                    Storage.Cache.Guilds[guild.Id].Members.Add(member.User.Id, new Member(member));
                }

                foreach (Presence status in guild.Presences)
                {
                    if (!Session.PrecenseDict.ContainsKey(status.User.Id))
                    {
                        Session.PrecenseDict.Add(status.User.Id, status);
                    }
                }

                Storage.Cache.Guilds[guild.Id].Channels.Clear();
                foreach (SharedModels.GuildChannel chn in guild.Channels)
                {
                    SharedModels.GuildChannel channel = chn;
                    channel.GuildId = guild.Id;
                    Storage.Cache.Guilds[guild.Id].Channels.Add(chn.Id, new GuildChannel(channel));
                }
            }

            foreach (Presence presence in e.EventData.Presences)
            {
                if (Session.PrecenseDict.ContainsKey(presence.User.Id))
                {
                    Session.PrecenseDict.Remove(presence.User.Id);
                }
                Session.PrecenseDict.Add(presence.User.Id, presence);
            }

            foreach (ReadState readstate in e.EventData.ReadStates)
            {
                Session.RPC.Add(readstate.Id, readstate);
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    switch (e.EventData.Settings.Status)
                    {
                        case "online":
                            UserStatusOnline.IsChecked = true;
                            break;
                        case "idle":
                            UserStatusIdle.IsChecked = true;
                            break;
                        case "dnd":
                            UserStatusDND.IsChecked = true;
                            break;
                        default:
                            UserStatusOnline.IsChecked = true;
                            break;
                    }

                    LoadingSplash.Hide(true);
                    LoadGuilds();
                });
        }

        private async void MessageCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Message> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (ServerList.SelectedIndex != 0)
                    {
                        if (TextChannels.SelectedIndex != -1 && e.EventData.ChannelId ==
                            ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id)
                        {
                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()]
                                .Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id]
                                .Messages.Add(e.EventData.Id, new Message(e.EventData));
                            Session.AckMessage(e.EventData.ChannelId, e.EventData.Id);
                            Storage.SaveCache();
                            Messages.Items.Add(NewMessageContainer(e.EventData, null, false, null));
                            try
                            {
                                var ToRemove = new List<TypingStart>();
                                for (int i = 0; i < Typers.Count; i++)
                                {
                                    var typer = Typers.ElementAt(i);
                                    if (typer.Key.userId == e.EventData.User.Id && typer.Key.channelId == e.EventData.ChannelId)
                                    {
                                        typer.Value.Stop();
                                        ToRemove.Add(Typers.ElementAt(i).Key);
                                    }
                                }
                                foreach (var key in ToRemove)
                                    Typers.Remove(key);
                                UpdateTypingUI();
                            }
                            catch (Exception exception) {}

                        }
                    }
                    else
                    {
                        if (DirectMessageChannels.SelectedItem != null && e.EventData.ChannelId ==
                            ((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id)
                        {
                            Session.AckMessage(e.EventData.ChannelId, e.EventData.Id);
                            Storage.SaveCache();
                            Messages.Items.Add(NewMessageContainer(e.EventData, null, false, null));
                            try
                            { Typers.Remove(Typers.FirstOrDefault(x => x.Key.userId == e.EventData.User.Id && x.Key.channelId == e.EventData.ChannelId).Key); }
                            catch (Exception exception) { }
                        }
                    }
                });

            if (Storage.Settings.Toasts && !Storage.MutedChannels.Contains(e.EventData.ChannelId) &&
                e.EventData.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                //In a real app, these would be initialized with actual data
                string toastTitle = e.EventData.User.Username + " sent a message on " + "(#" +
                                    Session.GetGuildChannel(e.EventData.ChannelId).Name + ")";
                string content = e.EventData.Content;
                //string imageurl = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";
                string userPhoto = "https://cdn.discordapp.com/avatars/" + e.EventData.User.Id + "/" +
                                   e.EventData.User.Avatar + ".jpg";
                string conversationId = e.EventData.ChannelId;
                // Construct the visuals of the toast
                ToastVisual visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = toastTitle
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            },
                            /*new AdaptiveImage()
                            {
                                Source = imageurl
                            }*/
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = userPhoto,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                };
                // Construct the actions for the toast (inputs and buttons)

                /*ToastTextBox replyContent = new ToastTextBox("Reply")
                {
                    PlaceholderContent = "Type a response"
                };

                ToastActionsCustom actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        replyContent
                    },
                    Buttons =
                    {
                        new ToastButton("Reply", "reply")
                        {
                            ActivationType = ToastActivationType.Background,
                            TextBoxId = replyContent.Id
                        }
                    }
                };
                */
                // Now we can construct the final toast content
                ToastContent toastContent = new ToastContent()
                {
                    Visual = visual,
                    //Actions = actions,
                    // Arguments when the user taps body of toast
                    /*Launch = new QueryString()
                    {
                        { "action", "reply" },
                        { "conversationId", conversationId },
                        {"message", replyContent.Id }
                    }.ToString()*/
                };
                // And create the toast notification
                ToastNotification notification = new ToastNotification(toastContent.GetXml());
                // And then send the toast
                ToastNotificationManager.CreateToastNotifier().Show(notification);

            }
        }

        private async void MessageDeleted(object sender,
            Gateway.GatewayEventArgs<Gateway.DownstreamEvents.MessageDelete> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (ServerList.SelectedItem != null)
                    {
                        if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs" &&
                            DirectMessageChannels.SelectedItem != null &&
                            ((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id ==
                            e.EventData.ChannelId)
                        {
                            LoadDmChannelMessages(null, null);
                        }
                        else
                        {
                            if (TextChannels.SelectedItem != null &&
                                (TextChannels.SelectedItem as ListViewItem).Tag != null &&
                                ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id ==
                                e.EventData.ChannelId)
                            {
                                if (Messages.Items != null)
                                    foreach (MessageContainer item in Messages.Items)
                                        if (item.Message.HasValue && item.Message.Value.Id == e.EventData.MessageId)
                                            Messages.Items.Remove(item);
                            }
                        }
                    }
                });
        }

        private async void MessageUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Message> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null && DirectMessageChannels.SelectedItem != null &&
                        ((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache)?.Raw.Id ==
                        e.EventData.ChannelId)
                    {
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message?.Id == e.EventData.Id)
                            {
                                Messages.Items[x] = NewMessageContainer(e.EventData, null, false, null);
                            }
                        }
                    }
                    else if (TextChannels.SelectedItem != null &&
                             (TextChannels.SelectedItem as ListViewItem)?.Tag != null &&
                             ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel)?.Raw.Id ==
                             e.EventData.ChannelId)
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message?.Id == e.EventData.Id)
                            {
                                Messages.Items[x] = NewMessageContainer(e.EventData, null, false, null);
                            }
                        }
                });
        }

        private async void MessageReactionRemovedAll(object sender,
            Gateway.GatewayEventArgs<MessageReactionRemoveAll> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null && DirectMessageChannels.SelectedItem != null &&
                        ((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache)?.Raw.Id ==
                        e.EventData.ChannelId)
                    {
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message != null &&
                                messageContainer.Message?.Id == e.EventData.MessageId)
                            {
                                //TODO REMOVE ALL REACTIONS
                            }
                        }
                    }
                    else if (TextChannels.SelectedItem != null &&
                             (TextChannels.SelectedItem as ListViewItem)?.Tag != null &&
                             ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel)?.Raw.Id ==
                             e.EventData.ChannelId)
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message?.Id == e.EventData.MessageId)
                            {
                                //TODO REMOVE ALL REACTIONS
                            }
                        }
                });
        }

        private async void MessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null && DirectMessageChannels.SelectedItem != null &&
                        ((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache)?.Raw.Id ==
                        e.EventData.ChannelId)
                    {
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message != null &&
                                messageContainer.Message?.Id == e.EventData.MessageId)
                            {
                                //TODO REMOVE REACTION
                            }
                        }
                    }
                    else if (TextChannels.SelectedItem != null &&
                             (TextChannels.SelectedItem as ListViewItem)?.Tag != null &&
                             ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel)?.Raw.Id ==
                             e.EventData.ChannelId)
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message?.Id == e.EventData.MessageId)
                            {
                                //TODO REMOVE REACTION
                            }
                        }
                });
        }

        private async void MessageReactionAdded(object sender, Gateway.GatewayEventArgs<MessageReactionUpdate> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null && DirectMessageChannels.SelectedItem != null &&
                        ((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache)?.Raw.Id ==
                        e.EventData.ChannelId)
                    {
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message != null &&
                                messageContainer.Message?.Id == e.EventData.MessageId)
                            {
                                //TODO ADD REACTION
                            }
                        }
                    }
                    else if (TextChannels.SelectedItem != null &&
                             (TextChannels.SelectedItem as ListViewItem)?.Tag != null &&
                             ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel)?.Raw.Id ==
                             e.EventData.ChannelId)
                        for (int x = 0; x < Messages.Items.Count; x++)
                        {
                            var messageContainer = Messages.Items[x] as MessageContainer;
                            if (messageContainer != null && messageContainer.Message?.Id == e.EventData.MessageId)
                            {
                                //TODO ADD REACTION
                            }
                        }
                });
        }

        private async void GuildCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            if (!Storage.Cache.Guilds.ContainsKey(e.EventData.Id))
            {
                Storage.Cache.Guilds.Add(e.EventData.Id, new Guild(e.EventData));
            }
            else
            {
                Storage.Cache.Guilds[e.EventData.Id] = new Guild(e.EventData);
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ServerList.Items.Add(GuildRender(new Guild(e.EventData)));
                });

            if (e.EventData.Presences != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
                        {

                            //if (MemberList != null)
                            //{
                            //    MemberList.Children.Clear();
                            //};

                            #region Roles

                            MembersCVS.Source = null;
                            LoadMembers((ServerList.SelectedItem as ListViewItem).Tag.ToString());

                            // List<ListView> listBuffer = new List<ListView>();
                            // while (listBuffer.Count < 1000)
                            // {
                            //     listBuffer.Add(new ListView());
                            // }
                            // 
                            // if (Storage.Cache.Guilds[e.EventData.Id].Roles != null)
                            // {
                            //     foreach (Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                            //     {
                            //         if (role.Hoist)
                            //         {
                            //             ListView listview = new ListView();
                            //             listview.Header = role.Name;
                            //             listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                            //             listview.SelectionMode = ListViewSelectionMode.None;
                            //
                            //             foreach (KeyValuePair<string, Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                            //             {
                            //                 if (member.Value.Raw.Roles.Contains<string>(role.Id))
                            //                 {
                            //                     ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            //                     listview.Items.Add(listviewitem);
                            //                 }
                            //             }
                            //             listBuffer.Insert(1000 - role.Position * 3, listview);
                            //         }
                            //     }
                            // }
                            //
                            // foreach (ListView listview in listBuffer)
                            // {
                            //     if (listview.Items.Count != 0)
                            //     {
                            //         MemberList.Children.Add(listview);
                            //     }
                            // }
                            //
                            // ListView fulllistview = new ListView();
                            // fulllistview.Header = "Everyone";
                            //
                            // foreach (KeyValuePair<string, Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                            // {
                            //     if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.Value.Raw.User.Id))
                            //     {
                            //         Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.Value.Raw.User.Id, new Member(member.Value.Raw));
                            //     }
                            //     ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            //     fulllistview.Items.Add(listviewitem);
                            // }
                            // MemberList.Children.Add(fulllistview);

                            #endregion
                        }
                    });
            }
        }

        private async void GuildDeleted(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.GuildDelete> e)
        {
            Storage.Cache.Guilds.Remove(e.EventData.MessageId);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    LoadGuilds();
                });
        }

        private async void GuildUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            Storage.Cache.Guilds[e.EventData.Id].RawGuild = e.EventData;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    LoadGuilds();
                });
        }

        private async void GuildChannelCreated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            Storage.Cache.Guilds[e.EventData.GuildId].Channels.Add(e.EventData.Id, new GuildChannel(e.EventData));
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == e.EventData.GuildId)
                    {
                        DownloadGuild(e.EventData.GuildId);
                    }
                });
        }

        private async void GuildChannelDeleted(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            Storage.Cache.Guilds[e.EventData.GuildId].Channels.Remove(e.EventData.Id);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == e.EventData.GuildId)
                    {
                        DownloadGuild(e.EventData.GuildId);
                    }
                });
        }

        private async void GuildChannelUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            Storage.Cache.Guilds[e.EventData.GuildId].Channels[e.EventData.Id].Raw = e.EventData;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == e.EventData.GuildId)
                    {
                        DownloadGuild(e.EventData.GuildId);
                    }
                });
        }

        private void GuildMemberAdded(object sender, Gateway.GatewayEventArgs<GuildMemberAdd> e)
        {
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId]
                    .Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId]
                    .Members.Add(e.EventData.User.Id,
                        new Member(new GuildMember()
                        {
                            Deaf = e.EventData.Deaf,
                            JoinedAt = e.EventData.JoinedAt,
                            Mute = e.EventData.Mute,
                            Nick = e.EventData.Nick,
                            Roles = e.EventData.Roles,
                            User = e.EventData.User
                        }));
            }
        }

        private void GuildMemberRemoved(object sender, Gateway.GatewayEventArgs<GuildMemberRemove> e)
        {
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId]
                    .Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId].Members.Remove(e.EventData.User.Id);
            }
        }

        private void GuildMemberUpdated(object sender, Gateway.GatewayEventArgs<GuildMemberUpdate> e)
        {
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId]
                    .Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId].Members[e.EventData.User.Id].Raw =
                    new GuildMember() {Nick = e.EventData.Nick, Roles = e.EventData.Roles};
            }
        }

        private async void GuildMemberChunked(object sender, Gateway.GatewayEventArgs<GuildMemberChunk> e)
        {
            foreach (GuildMember member in e.EventData.Members)
            {
                if (!Storage.Cache.Guilds[e.EventData.GuildId].Members.ContainsKey(member.User.Id))
                {
                    Storage.Cache.Guilds[e.EventData.GuildId].Members.Add(member.User.Id, new Member(member));
                }
                else
                {
                    Storage.Cache.Guilds[e.EventData.GuildId].Members[member.User.Id] = new Member(member);
                }
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    LoadMembers(e.EventData.GuildId);
                });
        }

        private async void DirectMessageChannelCreated(object sender, Gateway.GatewayEventArgs<DirectMessageChannel> e)
        {
            Storage.Cache.DMs.Add(e.EventData.Id, new DmCache(e.EventData));
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (ServerList.SelectedIndex == 0)
                    {
                        DirectMessageChannels.Items.Add(ChannelRender(new DmCache(e.EventData)));
                    }
                });
        }

        private async void DirectMessageChannelDeleted(object sender, Gateway.GatewayEventArgs<DirectMessageChannel> e)
        {
            Storage.Cache.DMs.Remove(e.EventData.Id);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (ServerList.SelectedIndex == 0)
                    {
                        DownloadDMs();
                    }
                });
        }

        private void RelationShipAdded(object sender, GatewayEventArgs<SharedModels.Friend> e)
        {
            if (!Storage.Cache.Friends.ContainsKey(e.EventData.Id))
            {
                Storage.Cache.Friends.Add(e.EventData.Id, new Friend(e.EventData));
            }
            else
            {
                Storage.Cache.Friends[e.EventData.Id] = new Friend(e.EventData);
            }
        }

        private void RelationShipRemoved(object sender, GatewayEventArgs<SharedModels.Friend> e)
        {
            if (Storage.Cache.Friends.ContainsKey(e.EventData.Id))
            {
                Storage.Cache.Friends.Remove(e.EventData.Id);
            }
        }

        private void RelationShipUpdated(object sender, GatewayEventArgs<SharedModels.Friend> e)
        {
            if (!Storage.Cache.Friends.ContainsKey(e.EventData.Id))
            {
                Storage.Cache.Friends.Add(e.EventData.Id, new Friend(e.EventData));
            } else
            {
                Storage.Cache.Friends[e.EventData.Id] = new Friend(e.EventData);
            }
        }

        private void UserNoteUpdated(object sender, GatewayEventArgs<UserNote> e)
        {
            if (App.Notes.ContainsKey(e.EventData.UserId))
                App.Notes[e.EventData.UserId] = e.EventData.Note;
            else
                App.Notes.Add(e.EventData.UserId, e.EventData.Note);
        }

        private async void GatewayOnUserSettingsUpdated(object sender, GatewayEventArgs<UserSettings> gatewayEventArgs)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //LocalStatusChangeEnabled = false;
                if (gatewayEventArgs.EventData.Status == "online")
                    UserStatusOnline.IsChecked = true;
                if (gatewayEventArgs.EventData.Status == "idle")
                    UserStatusIdle.IsChecked = true;
                if (gatewayEventArgs.EventData.Status == "dnd")
                    UserStatusDND.IsChecked = true;
                if (gatewayEventArgs.EventData.Status == "offline")
                    UserStatusInvisible.IsChecked = true;

            });
        }

        private async void PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
        {
            if (Session.PrecenseDict.ContainsKey(e.EventData.User.Id))
            {
                Session.PrecenseDict.Remove(e.EventData.User.Id);
            }
            Session.PrecenseDict.Add(e.EventData.User.Id, e.EventData);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id != null)
                    {
                        //TODO REPLACE WITH ADD/REMOVE
                        // LoadMembers(((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id);
                    }
                });
        }

        Dictionary<TypingStart, DispatcherTimer> Typers = new Dictionary<TypingStart, DispatcherTimer>();
        List<DispatcherTimer> TyperTimers = new List<DispatcherTimer>();

        private async void TypingStarted(object sender, GatewayEventArgs<TypingStart> e)
        {
            Debug.WriteLine("TYPING STARTED");
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DispatcherTimer timer = new DispatcherTimer();
                /*If the user was already typing in that channel before...*/
                if (Typers.Count > 0 && Typers.Any(x => x.Key.userId == e.EventData.userId &&
                                                        x.Key.channelId == e.EventData.channelId))
                {
                    /*...Reset the timer by calling Start again*/
                    Typers.First(x => x.Key.userId == e.EventData.userId && x.Key.channelId == e.EventData.channelId)
                        .Value.Start();
                }
                else
                {
                    /*...Otherwise, create a new timer and add it, with the EventData, to "Typers" */
                    timer.Interval = TimeSpan.FromSeconds(8);
                    timer.Tick += (sender2, o1) =>
                    {
                        timer.Stop();
                        Typers.Remove(Typers.First(t => t.Value == timer).Key);
                        UpdateTypingUI();
                    };
                    timer.Start();
                    Typers.Add(e.EventData, timer);
                    UpdateTypingUI();
                }
            });
        }

        private async void UpdateTypingUI()
        {
            //  try
            //      {
            string typingString = "";
            int DisplayedTyperCounter = 0;
            List<string> NamesTyping = new List<string>();
            for (int i = 0; i < Typers.Count; i++)
            {
                var typer = Typers.ElementAt(i);
                if (App.CurrentChannelId != null)
                {
                    if (App.CurrentGuildIsDM && App.CurrentChannelId != null)
                    {
                        try
                        {
                            NamesTyping.Add(Storage.Cache.DMs[App.CurrentChannelId].Raw.Users.First().Username);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        if (App.CurrentChannelId == typer.Key.channelId)
                        {
                            var member = Storage.Cache.Guilds[App.CurrentGuildId].Members[typer.Key.userId];
                            string DisplayedName = member.Raw.User.Username;
                            if (member.Raw.Nick != null) DisplayedName = member.Raw.Nick;
                            NamesTyping.Add(DisplayedName);
                        }
                        else
                        {
                            //TODO Display typing indicator on channel list
                        }
                        //TODO Display typing indicator on member list
                    }
                }
            }

            DisplayedTyperCounter = NamesTyping.Count();
            for (int i = 0; i < DisplayedTyperCounter; i++)
            {
                if (i == 0)
                    typingString += NamesTyping.ElementAt(i); //first element, no prefix
                else if (i == 2 && i == DisplayedTyperCounter)
                    typingString += " and " + NamesTyping.ElementAt(i); //last element out of 2, prefix = "and"
                else if (i == DisplayedTyperCounter)
                    typingString += ", and " + NamesTyping.ElementAt(i); //last element out of 2, prefix = "and" WITH OXFORD COMMA
                else
                    typingString += ", " + NamesTyping.ElementAt(i); //intermediary element, prefix = comma
            }
            if (DisplayedTyperCounter > 1)
                typingString += " are typing...";
            else
                typingString += " is typing...";

            if (DisplayedTyperCounter == 0)
            {
                TypingStackPanel.Fade(0, 200).Start();
            }
            else
            {
                TypingIndicator.Text = typingString;
                TypingStackPanel.Fade(1, 200).Start();
            }
        }
    }
}
