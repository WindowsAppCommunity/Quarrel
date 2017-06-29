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
using Discord_UWP.SharedModels;
#region CacheModels Overrule
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Message = Discord_UWP.CacheModels.Message;
using User = Discord_UWP.CacheModels.User;
using Guild = Discord_UWP.CacheModels.Guild;
#endregion

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private async void OnReady(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.Ready> e)
        {
            Storage.Cache.guildOrder.Clear();
            int pos = 0;
            foreach (string guild in e.EventData.Settings.GuildOrder)
            {
                pos++;
                Storage.Cache.guildOrder.Add(guild, pos);
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

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
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
                    if (TextChannels.SelectedIndex != -1 && e.EventData.ChannelId == ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id)
                    {
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Messages.Add(e.EventData.Id, new Message(e.EventData));
                        Storage.SaveCache();
                        Messages.Items.Add(NewMessageContainer(e.EventData, null, false, null));
                    }
                }
                else
                {
                    if (DirectMessageChannels.SelectedItem != null && e.EventData.ChannelId == ((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id)
                    {
                        Messages.Items.Add(NewMessageContainer(e.EventData, null, false, null));
                    }
                }
            });

            if (Storage.Settings.Toasts && !Storage.MutedChannels.Contains(e.EventData.ChannelId) && e.EventData.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                //In a real app, these would be initialized with actual data
                string toastTitle = e.EventData.User.Username + " sent a message on " + "(#" + Session.GetGuildChannel(e.EventData.ChannelId).Name + ")";
                string content = e.EventData.Content;
                //string imageurl = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";
                string userPhoto = "https://cdn.discordapp.com/avatars/" + e.EventData.User.Id + "/" + e.EventData.User.Avatar + ".jpg";
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

        private async void MessageDeleted(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.MessageDelete> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (ServerList.SelectedItem != null)
                {
                    if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs" && DirectMessageChannels.SelectedItem != null && ((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id == e.EventData.ChannelId)
                    {
                        LoadDmChannelMessages(null, null);
                    }
                    else
                    {
                        if (TextChannels.SelectedItem != null && (TextChannels.SelectedItem as ListViewItem).Tag != null && ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id == e.EventData.ChannelId)
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
                if (App.CurrentId == null && DirectMessageChannels.SelectedItem != null && ((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id == e.EventData.ChannelId)
                {
                    for (int x = 0; x < Messages.Items.Count; x++)
                    {
                        if ((Messages.Items[x] as MessageContainer).Message?.Id == e.EventData.Id)
                        {
                            Messages.Items[x] = NewMessageContainer(e.EventData, null, false, null);
                        }
                    }
                }
                else if(TextChannels.SelectedItem != null && (TextChannels.SelectedItem as ListViewItem).Tag != null && ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id == e.EventData.ChannelId)
                        for (int x = 0; x < Messages.Items.Count; x++)
                {
                    if ((Messages.Items[x] as MessageContainer).Message?.Id == e.EventData.Id)
                    {
                        Messages.Items[x] = NewMessageContainer(e.EventData, null, false, null);
                    }
                }
            });
        }

        private async void GuildCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            if (!Storage.Cache.Guilds.ContainsKey(e.EventData.Id))
            {
                Storage.Cache.Guilds.Add(e.EventData.Id, new Guild(e.EventData));
            } else
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
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId].Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId].Members.Add(e.EventData.User.Id, new Member(new GuildMember() { Deaf = e.EventData.Deaf, JoinedAt = e.EventData.JoinedAt, Mute = e.EventData.Mute, Nick = e.EventData.Nick, Roles = e.EventData.Roles, User = e.EventData.User }));
            }
        }

        private void GuildMemberRemoved(object sender, Gateway.GatewayEventArgs<GuildMemberRemove> e)
        {
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId].Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId].Members.Remove(e.EventData.User.Id);
            }
        }

        private void GuildMemberUpdated(object sender, Gateway.GatewayEventArgs<GuildMemberUpdate> e)
        {
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId].Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId].Members[e.EventData.User.Id].Raw = new GuildMember() { Nick = e.EventData.Nick, Roles = e.EventData.Roles };
            }
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

        private async void PresenceUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Presence> e)
        {
            if (Session.PrecenseDict.ContainsKey(e.EventData.User.Id))
            {
                Session.PrecenseDict.Remove(e.EventData.User.Id);
            }
            Session.PrecenseDict.Add(e.EventData.User.Id, e.EventData);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (ServerList.SelectedIndex != -1 && ServerList.SelectedIndex != 0)
                    {
                        if (ServerList.SelectedIndex == 0)
                        {
                            //MemberList.Children.Clear();
                            #region Roles
                            LoadMembers((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                            //List<ListView> listBuffer = new List<ListView>();
                            //while (listBuffer.Count < 1000)
                            //{
                            //    listBuffer.Add(new ListView());
                            //}
                            //
                            //if (Storage.Cache.Guilds[e.EventData.GuildId].Roles != null)
                            //{
                            //    foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                            //    {
                            //        if (role.Hoist)
                            //        {
                            //            ListView listview = new ListView();
                            //            listview.Header = role.Name;
                            //            listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                            //
                            //            foreach (KeyValuePair<string, Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                            //            {
                            //                if (member.Value.Raw.Roles.Contains<string>(role.Id))
                            //                {
                            //                    ListViewItem listviewitem = new ListViewItem();
                            //                    if (Session.PrecenseDict.ContainsKey(member.Value.Raw.User.Id))
                            //                    {
                            //                        listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            //                    }
                            //                    else
                            //                    {
                            //                        listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            //                    }
                            //                    listview.Items.Add(listviewitem);
                            //                }
                            //            }
                            //            listBuffer.Insert(1000 - role.Position * 3, listview);
                            //        }
                            //    }
                            //}
                            //
                            //foreach (ListView listview in listBuffer)
                            //{
                            //    if (listview.Items.Count != 0)
                            //    {
                            //        MemberList.Children.Add(listview);
                            //    }
                            //}
                            //
                            //ListView fulllistview = new ListView();
                            //fulllistview.Header = "Everyone";
                            //
                            //foreach (KeyValuePair<string, Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                            //{
                            //    if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.Value.Raw.User.Id))
                            //    {
                            //        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.Value.Raw.User.Id, new Member(member.Value.Raw));
                            //        ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            //        fulllistview.Items.Add(listviewitem);
                            //    }
                            //}
                            //MemberList.Children.Add(fulllistview);
                            #endregion
                        }
                    }
                });
        }

        private void TypingStarted(object sender, Gateway.GatewayEventArgs<SharedModels.TypingStart> e)
        {
            Debug.WriteLine("typing");
            Session.Typers.Add(e.EventData);
        }
    }
}
