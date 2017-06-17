using Discord_UWP.API;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

namespace Discord_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page
    {
        private async void OnReady(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.Ready> e)
        {
            foreach (SharedModels.Presence presence in e.EventData.Presences)
            {
                if (Session.PrecenseDict.ContainsKey(presence.User.Id))
                {
                    Session.PrecenseDict.Remove(presence.User.Id);
                }
                Session.PrecenseDict.Add(presence.User.Id, presence);
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (ServerList.SelectedIndex > 0)
                    {
                        if (MemberList != null)
                        {
                            MemberList.Children.Clear();
                        };

                        #region Roles

                        List<ListView> listBuffer = new List<ListView>();
                        while (listBuffer.Count < 1000)
                        {
                            listBuffer.Add(new ListView());
                        }

                        await Session.GetGuildMembers((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                        await Session.GetGuild((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                        if (Session.Guild.Roles != null)
                        {
                            foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                            {
                                if (role.Hoist)
                                {
                                    ListView listview = new ListView();
                                    listview.Header = role.Name;
                                    listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                                    listview.SelectionMode = ListViewSelectionMode.None;

                                    foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                                    {
                                        if (member.Value.Raw.Roles.Contains<string>(role.Id))
                                        {
                                            ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                            listview.Items.Add(listviewitem);
                                        }
                                    }
                                    listBuffer.Insert(1000 - role.Position * 3, listview);
                                }
                            }
                        }

                        foreach (ListView listview in listBuffer)
                        {
                            if (listview.Items.Count != 0)
                            {
                                MemberList.Children.Add(listview);
                            }
                        }

                        ListView fulllistview = new ListView();
                        fulllistview.Header = "Everyone";

                        foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                        {
                            if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.Value.Raw.User.Id))
                            {
                                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.Value.Raw.User.Id, new CacheModels.Member(member.Value.Raw));
                            }
                            ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            fulllistview.Items.Add(listviewitem);
                        }
                        MemberList.Children.Add(fulllistview);

                        #endregion
                    }
                });

            Session.Friends = e.EventData.Friends;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (ServerList.SelectedIndex > 0)
                     {
                         if (MemberList != null)
                         {
                             MemberList.Children.Clear();
                         };

                         ListView fulllistview = new ListView();
                         foreach (SharedModels.Friend friend in Session.Friends)
                         {
                             fulllistview.Items.Add(FriendRender(friend));
                         }
                         MemberList.Children.Add(fulllistview);
                    }
                 });
        }

        private async void GuildChannelCreated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
                   if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == e.EventData.GuildId)
                   {
                       LoadGuild(null, null);
                   }
               });
        }

        private async void GuildChannelDeleted(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
                   if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == e.EventData.GuildId)
                   {
                       LoadGuild(null, null);
                   }
               });
        }

        private async void GuildChannelUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
                   if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == e.EventData.GuildId)
                   {
                       LoadGuild(null, null);
                   }
               });
        }

        private async void GuildCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;
                /*if (guild.Icon != null)
                {
                    Image icon = new Image();
                    icon.Source = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + guild.Id + "/" + guild.Icon + ".jpg"));
                    stack.Children.Add(icon); 
                } else
                {*/
                SharedModels.Guild guild = e.EventData;
                Grid grid = new Grid();
                grid.Background = GetSolidColorBrush("#FF738AD6");
                grid.Margin = new Thickness(-5, 0, 0, 0);
                grid.Height = 50;
                grid.Width = 45;
                TextBlock icon = new TextBlock();
                icon.Text = guild.Name.ToArray<char>()[0].ToString();
                icon.HorizontalAlignment = HorizontalAlignment.Center;
                icon.VerticalAlignment = VerticalAlignment.Center;
                grid.Children.Add(icon);
                stack.Children.Add(grid);
                //}
                TextBlock txtblock = new TextBlock();
                txtblock.Text = guild.Name;
                txtblock.VerticalAlignment = VerticalAlignment.Center;
                stack.Children.Add(txtblock);
                ListViewItem listviewitem = new ListViewItem();
                listviewitem.Height = 50;
                listviewitem.Content = stack;
                listviewitem.Tag = guild.Id;
                ServerList.Items.Add(listviewitem);
            });

            if (e.EventData.Presences != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() != "DMs")
                    {

                        if (MemberList != null)
                        {
                            MemberList.Children.Clear();
                        };

                        #region Roles

                        List<ListView> listBuffer = new List<ListView>();
                        while (listBuffer.Count < 1000)
                        {
                            listBuffer.Add(new ListView());
                        }

                        await Session.GetGuildMembers((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                        await Session.GetGuild((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                        if (Session.Guild.Roles != null)
                        {
                            foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                            {
                                if (role.Hoist)
                                {
                                    ListView listview = new ListView();
                                    listview.Header = role.Name;
                                    listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                                    listview.SelectionMode = ListViewSelectionMode.None;

                                    foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                                    {
                                        if (member.Value.Raw.Roles.Contains<string>(role.Id))
                                        {
                                            ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                            listview.Items.Add(listviewitem);
                                        }
                                    }
                                    listBuffer.Insert(1000 - role.Position * 3, listview);
                                }
                            }
                        }

                        foreach (ListView listview in listBuffer)
                        {
                            if (listview.Items.Count != 0)
                            {
                                MemberList.Children.Add(listview);
                            }
                        }

                        ListView fulllistview = new ListView();
                        fulllistview.Header = "Everyone";

                        foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                        {
                            if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.Value.Raw.User.Id))
                            {
                                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.Value.Raw.User.Id, new CacheModels.Member(member.Value.Raw));
                            }
                            ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            fulllistview.Items.Add(listviewitem);
                        }
                        MemberList.Children.Add(fulllistview);

                        #endregion
                    }
                });
            }
        }

        private async void GuildDeleted(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.GuildDelete> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
                   LoadGuilds();
               });
        }

        private async void GuildUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
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
                    if (TextChannels.SelectedIndex != -1 && e.EventData.ChannelId == ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id)
                    {
                        Messages.Children.Add(MessageRender(e.EventData));
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages.Add(e.EventData.Id, new CacheModels.Message(e.EventData));
                        Storage.SaveCache();
                    }
                }
                else
                {
                    if (DMs.SelectedIndex != -1 && e.EventData.ChannelId == (DMs.SelectedItem as ListViewItem).Tag.ToString())
                    {
                        Messages.Children.Add(MessageRender(e.EventData));
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
                if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
                {
                    GetDmData(null, null);
                }
                else
                {
                    if (TextChannels.SelectedItem != null && (TextChannels.SelectedItem as ListViewItem).Tag != null && ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id == e.EventData.ChannelId)
                    {
                        foreach (UIElement item in Messages.Children)
                        {
                            if (item is ListViewItem && (item as ListViewItem).Tag != null && ((item as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id == e.EventData.MessageId)
                            {
                                Messages.Children.Remove(item);
                            }
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
                if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
                {
                    GetDmData(null, null);
                }
                else
                {
                    if (TextChannels.SelectedItem != null && (TextChannels.SelectedItem as ListViewItem).Tag != null && ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id == e.EventData.ChannelId)
                    {
                        LoadChannelMessages(null, null);
                    }
                }
            });
        }

        private async void DirectMessageChannelCreated(object sender, Gateway.GatewayEventArgs<SharedModels.DirectMessageChannel> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
                   if (ServerList.SelectedIndex == 0)
                   {
                       SharedModels.DirectMessageChannel channel = e.EventData;
                       ListViewItem listviewitem = new ListViewItem();
                       StackPanel stack = new StackPanel();
                       stack.Orientation = Orientation.Horizontal;
                       Image image = new Image();
                       image.Height = 50;
                       image.Source = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + channel.User.Id + "/" + channel.User.Avatar + ".jpg"));
                       TextBlock txtblock = new TextBlock();
                       txtblock.Text = channel.User.Username;
                       txtblock.VerticalAlignment = VerticalAlignment.Center;
                       stack.Children.Add(image);
                       stack.Children.Add(txtblock);
                       listviewitem.Content = stack;
                       listviewitem.Tag = channel.Id;
                       DMs.Items.Add(listviewitem);
                   }
               });
        }

        private async void DirectMessageChannelDeleted(object sender, Gateway.GatewayEventArgs<SharedModels.DirectMessageChannel> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
                   if (ServerList.SelectedIndex == 0)
                   {
                       LoadGuild(null, null);
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
                async () =>
                {
                    if (ServerList.SelectedIndex != -1 && ServerList.SelectedIndex != 0)
                    {
                        if (ServerList.SelectedIndex == 0)
                        {
                            MemberList.Children.Clear();
                            #region Roles

                            List<ListView> listBuffer = new List<ListView>();
                            while (listBuffer.Count < 1000)
                            {
                                listBuffer.Add(new ListView());
                            }

                            await Session.GetGuildMembers((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                            await Session.GetGuild((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                            if (Session.Guild.Roles != null)
                            {
                                foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                                {
                                    if (role.Hoist)
                                    {
                                        ListView listview = new ListView();
                                        listview.Header = role.Name;
                                        listview.Foreground = GetSolidColorBrush("#FFFFFFFF");

                                        foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                                        {
                                            if (member.Value.Raw.Roles.Contains<string>(role.Id))
                                            {
                                                ListViewItem listviewitem = new ListViewItem();
                                                if (Session.PrecenseDict.ContainsKey(member.Value.Raw.User.Id))
                                                {
                                                    listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                                }
                                                else
                                                {
                                                    listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                                }
                                                listview.Items.Add(listviewitem);
                                            }
                                        }
                                        listBuffer.Insert(1000 - role.Position * 3, listview);
                                    }
                                }
                            }

                            foreach (ListView listview in listBuffer){
                                if (listview.Items.Count != 0)
                                {
                                    MemberList.Children.Add(listview);
                                }
                            }

                            ListView fulllistview = new ListView();
                            fulllistview.Header = "Everyone";

                            foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                            {
                                if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.Value.Raw.User.Id))
                                {
                                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.Value.Raw.User.Id, new CacheModels.Member(member.Value.Raw));
                                    ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                    fulllistview.Items.Add(listviewitem);
                                }
                            }
                            MemberList.Children.Add(fulllistview);
                            #endregion
                        }
                    }
                });
        }

        private async void TypingStarted(object sender, Gateway.GatewayEventArgs<SharedModels.TypingStart> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
               {
                   TypingList.Visibility = Visibility.Visible;
                   TypingList.Text = e.EventData.userId + " " + TypingList.Text;
               });
        }
    }
}
