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
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Media.SpeechSynthesis;
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
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private async void OnReady(object sender, GatewayEventArgs<Gateway.DownstreamEvents.Ready> e)
        {
            if (e.EventData.Notes != null)
                App.Notes = e.EventData.Notes;
            int pos = 0;

            Storage.Settings.DevMode = e.EventData.Settings.DevMode;

            if (e.EventData.Settings.Theme == "Light")
            {
                Storage.Settings.DiscordLightTheme = true;

                Storage.SaveAppSettings();
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

                if (guild.Members != null)
                {
                    Storage.Cache.Guilds[guild.Id].Members.Clear();
                    foreach (GuildMember member in guild.Members)
                    {
                        Storage.Cache.Guilds[guild.Id].Members.Add(member.User.Id, new Member(member));
                    }
                }

                if (guild.Roles != null)
                {
                    Storage.Cache.Guilds[guild.Id].Roles.Clear();
                    Storage.Cache.Guilds[guild.Id].perms = new Common.Permissions();
                    Storage.Cache.Guilds[guild.Id].perms.Perms.Permissions = 0;
                    foreach (Role role in guild.Roles)
                    {
                        Storage.Cache.Guilds[guild.Id].Roles.Add(role.Id, role);
                        if (Storage.Cache.Guilds[guild.Id].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Contains(role.Id) || role.Name == "@everyone")
                        {
                            if (Storage.Cache.Guilds[guild.Id].Members[Storage.Cache.CurrentUser.Raw.Id].HighRole.Position < role.Position)
                            {
                                Storage.Cache.Guilds[guild.Id].Members[Storage.Cache.CurrentUser.Raw.Id].HighRole = role;
                            }
                            Storage.Cache.Guilds[guild.Id].perms.Perms.Permissions = Storage.Cache.Guilds[guild.Id].perms.Perms.Permissions | Convert.ToInt32(role.Permissions);
                        }
                    }
                }

                if (guild.Presences != null)
                {
                    foreach (Presence status in guild.Presences)
                    {
                        if (!Session.PrecenseDict.ContainsKey(status.User.Id))
                        {
                            Session.PrecenseDict.Add(status.User.Id, status);
                        }
                    }
                }

                if (guild.Channels != null)
                {
                    Storage.Cache.Guilds[guild.Id].Channels.Clear();
                    foreach (SharedModels.GuildChannel chn in guild.Channels)
                    {
                        SharedModels.GuildChannel channel = chn;
                        channel.GuildId = guild.Id;
                        Storage.Cache.Guilds[guild.Id].Channels.Add(chn.Id, new GuildChannel(channel));

                        foreach (var Channel in Storage.Cache.Guilds[guild.Id].Channels)
                        {
                            Storage.Cache.Guilds[guild.Id].Channels[Channel.Key].chnPerms = new Common.Permissions() { Perms = Storage.Cache.Guilds[guild.Id].perms.Perms};
                            Storage.Cache.Guilds[guild.Id].Channels[Channel.Key].chnPerms.AddOverwrites(Channel.Value.Raw.PermissionOverwrites, guild.Id);
                        }
                    }
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

            foreach (GuildSetting guild in e.EventData.GuildSettings)
            {
                Session.GuildSettings.Add(guild.GuildId, guild);
            }

            foreach (string guild in e.EventData.Settings.GuildOrder)
            {
                if (Storage.Cache.Guilds.ContainsKey(guild))
                {
                    Storage.Cache.Guilds[guild].Postition = pos;
                }
                pos++;
            }

            Storage.Cache.CurrentUser = new User(e.EventData.User);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    Session.PrecenseDict.Add(e.EventData.User.Id, new Presence() { User = e.EventData.User, Status = e.EventData.Settings.Status});
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
                    //CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
                    friendPanel.Load();
                    LoadGuilds();
                    Storage.SaveCache();
                });
        }
        
        private TimeSpan VibrationDuration = TimeSpan.FromMilliseconds(100);
        private async void MessageCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Message> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (ServerList.SelectedIndex != 0)
                    {
                        if (TextChannels.SelectedIndex != -1 && e.EventData.ChannelId == App.CurrentChannelId)
                        {
                            Storage.Cache.Guilds[App.CurrentGuildId]
                                .Channels[App.CurrentChannelId].Messages.Add(e.EventData.Id, new Message(e.EventData));
                            await Task.Run(() => Session.AckMessage(e.EventData.ChannelId, e.EventData.Id));
                            Storage.SaveCache();
                            Messages.Items.Add(NewMessageContainer(e.EventData, null, false, null));
                            if (e.EventData.TTS)
                            {
                                MediaElement mediaplayer = new MediaElement();
                                using (var speech = new SpeechSynthesizer())
                                {
                                    speech.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Female);
                                    string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" + e.EventData.User.Username + "said" + e.EventData.Content + "</speak>";
                                    SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
                                    mediaplayer.SetSource(stream, stream.ContentType);
                                    mediaplayer.Play();
                                }
                            }
                            if (VibrationEnabled)
                                Windows.Phone.Devices.Notification.VibrationDevice.GetDefault().Vibrate(VibrationDuration);
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

                        var guild = Storage.Cache.Guilds.FirstOrDefault(
                            x => x.Value.Channels.ContainsKey(e.EventData.ChannelId));
                        if (guild.Value != null)
                            Storage.Cache.Guilds[guild.Key].Channels[e.EventData.ChannelId].Raw.LastMessageId = e.EventData.Id;
                    }
                    else
                    {
                        if (DirectMessageChannels.SelectedItem != null && e.EventData.ChannelId ==
                            (DirectMessageChannels.SelectedItem as SimpleChannel).Id)
                        {
                            await Task.Run(() => Session.AckMessage(e.EventData.ChannelId, e.EventData.Id));
                            Storage.SaveCache();
                            Messages.Items.Add(NewMessageContainer(e.EventData, null, false, null));
                            if (e.EventData.TTS)
                            {
                                MediaElement mediaplayer = new MediaElement();
                                using (var speech = new SpeechSynthesizer())
                                {
                                    speech.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Male);
                                    string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" + e.EventData.User.Username + "said" + e.EventData.Content + "</speak>";
                                    SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
                                    mediaplayer.SetSource(stream, stream.ContentType);
                                    mediaplayer.Play();
                                }
                            }
                            try
                            { Typers.Remove(Typers.FirstOrDefault(x => x.Key.userId == e.EventData.User.Id && x.Key.channelId == e.EventData.ChannelId).Key); }
                            catch (Exception exception) { }
                            Storage.Cache.DMs[e.EventData.ChannelId].Raw.LastMessageId = e.EventData.Id;
                        }
                    }
                });

            if (Storage.Settings.Toasts && !Storage.MutedChannels.Contains(e.EventData.ChannelId) &&
                e.EventData.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                //In a real app, these would be initialized with actual data
                string toastTitle = e.EventData.User.Username + " " + App.GetString("/Main/Notifications_sentMessageOn") + " " + " " + "(#" +
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

            UpdateGuildAndChannelUnread();
        }

        private async void MessageDeleted(object sender,
            GatewayEventArgs<Gateway.DownstreamEvents.MessageDelete> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentChannelId != null &&
                                App.CurrentChannelId ==
                                e.EventData.ChannelId)
                    {
                        if (Messages.Items != null)
                            foreach (MessageContainer item in Messages.Items)
                                if (item.Message.HasValue && item.Message.Value.Id == e.EventData.MessageId)
                                    Messages.Items.Remove(item);
                    }
                });
        }

        private async void MessageUpdated(object sender, GatewayEventArgs<SharedModels.Message> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null && App.CurrentChannelId != null &&
                        App.CurrentChannelId ==
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
                             (TextChannels.SelectedItem as SimpleChannel).Id ==
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
            GatewayEventArgs<MessageReactionRemoveAll> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null &&
                        App.CurrentChannelId ==
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
                });
        }

        private async void MessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null && App.CurrentChannelId != null &&
                        App.CurrentChannelId ==
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
                });
        }

        private async void MessageReactionAdded(object sender, Gateway.GatewayEventArgs<MessageReactionUpdate> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (App.CurrentGuildId == null && App.CurrentChannelId != null &&
                        App.CurrentChannelId ==
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
                });
        }

        private void OnMessageAck(object sender, GatewayEventArgs<MessageAck> e)
        {
            if (Session.RPC.ContainsKey(e.EventData.ChannelId))
            {
                ReadState item = Session.RPC[e.EventData.ChannelId];
                item.Id = e.EventData.ChannelId;
                item.LastMessageId = e.EventData.Id;
                item.MentionCount = 0;
                Session.RPC[e.EventData.ChannelId] = item;
            } else
            {
                ReadState item = new ReadState();
                item.Id = e.EventData.ChannelId;
                item.LastMessageId = e.EventData.Id;
                item.MentionCount = 0;
                Session.RPC.Add(e.EventData.ChannelId, item);
            }

            UpdateGuildAndChannelUnread();
        }

        private async void GuildCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            if (!Storage.Cache.Guilds.ContainsKey(e.EventData.Id))
            {
                Storage.Cache.Guilds.Add(e.EventData.Id, new Guild(e.EventData));

                if (!Storage.Cache.Guilds.ContainsKey(e.EventData.Id))
                {
                    Storage.Cache.Guilds.Add(e.EventData.Id, new Guild(e.EventData));
                }

                if (e.EventData.Members != null)
                {
                    Storage.Cache.Guilds[e.EventData.Id].Members.Clear();
                    foreach (GuildMember member in e.EventData.Members)
                    {
                        Storage.Cache.Guilds[e.EventData.Id].Members.Add(member.User.Id, new Member(member));
                    }
                }

                if (e.EventData.Roles != null)
                {
                    Storage.Cache.Guilds[e.EventData.Id].Roles.Clear();
                    Storage.Cache.Guilds[e.EventData.Id].perms = new Common.Permissions();
                    Storage.Cache.Guilds[e.EventData.Id].perms.Perms.Permissions = 0;
                    foreach (Role role in e.EventData.Roles)
                    {
                        Storage.Cache.Guilds[e.EventData.Id].Roles.Add(role.Id, role);
                        if (Storage.Cache.Guilds[e.EventData.Id].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Contains(role.Id))
                        {
                            Storage.Cache.Guilds[e.EventData.Id].perms.Perms.Permissions = Storage.Cache.Guilds[e.EventData.Id].perms.Perms.Permissions | Convert.ToInt32(role.Permissions);
                        }
                    }
                }

                if (e.EventData.Presences != null)
                {
                    foreach (Presence status in e.EventData.Presences)
                    {
                        if (!Session.PrecenseDict.ContainsKey(status.User.Id))
                        {
                            Session.PrecenseDict.Add(status.User.Id, status);
                        }
                    }
                }

                if (e.EventData.Channels != null)
                {
                    Storage.Cache.Guilds[e.EventData.Id].Channels.Clear();
                    foreach (SharedModels.GuildChannel chn in e.EventData.Channels)
                    {
                        SharedModels.GuildChannel channel = chn;
                        channel.GuildId = e.EventData.Id;
                        Storage.Cache.Guilds[e.EventData.Id].Channels.Add(chn.Id, new GuildChannel(channel));

                        foreach (var Channel in Storage.Cache.Guilds[e.EventData.Id].Channels)
                        {
                            Storage.Cache.Guilds[e.EventData.Id].Channels[Channel.Key].chnPerms = new Common.Permissions() { Perms = Storage.Cache.Guilds[e.EventData.Id].perms.Perms };
                            Storage.Cache.Guilds[e.EventData.Id].Channels[Channel.Key].chnPerms.AddOverwrites(Channel.Value.Raw.PermissionOverwrites, e.EventData.Id);
                        }
                    }
                }
            }
            else
            {
                Storage.Cache.Guilds[e.EventData.Id] = new Guild(e.EventData);

                if (!Storage.Cache.Guilds.ContainsKey(e.EventData.Id))
                {
                    Storage.Cache.Guilds.Add(e.EventData.Id, new Guild(e.EventData));
                }

                if (e.EventData.Members != null)
                {
                    Storage.Cache.Guilds[e.EventData.Id].Members.Clear();
                    foreach (GuildMember member in e.EventData.Members)
                    {
                        Storage.Cache.Guilds[e.EventData.Id].Members.Add(member.User.Id, new Member(member));
                    }
                }

                if (e.EventData.Roles != null)
                {
                    Storage.Cache.Guilds[e.EventData.Id].Roles.Clear();
                    Storage.Cache.Guilds[e.EventData.Id].perms = new Common.Permissions();
                    Storage.Cache.Guilds[e.EventData.Id].perms.Perms.Permissions = 0;
                    foreach (Role role in e.EventData.Roles)
                    {
                        Storage.Cache.Guilds[e.EventData.Id].Roles.Add(role.Id, role);
                        if (Storage.Cache.Guilds[e.EventData.Id].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Contains(role.Id))
                        {
                            Storage.Cache.Guilds[e.EventData.Id].perms.Perms.Permissions = Storage.Cache.Guilds[e.EventData.Id].perms.Perms.Permissions | Convert.ToInt32(role.Permissions);
                        }
                    }
                }

                if (e.EventData.Presences != null)
                {
                    foreach (Presence status in e.EventData.Presences)
                    {
                        if (!Session.PrecenseDict.ContainsKey(status.User.Id))
                        {
                            Session.PrecenseDict.Add(status.User.Id, status);
                        }
                    }
                }

                if (e.EventData.Channels != null)
                {
                    Storage.Cache.Guilds[e.EventData.Id].Channels.Clear();
                    foreach (SharedModels.GuildChannel chn in e.EventData.Channels)
                    {
                        SharedModels.GuildChannel channel = chn;
                        channel.GuildId = e.EventData.Id;
                        Storage.Cache.Guilds[e.EventData.Id].Channels.Add(chn.Id, new GuildChannel(channel));

                        foreach (var Channel in Storage.Cache.Guilds[e.EventData.Id].Channels)
                        {
                            Storage.Cache.Guilds[e.EventData.Id].Channels[Channel.Key].chnPerms = new Common.Permissions() { Perms = Storage.Cache.Guilds[e.EventData.Id].perms.Perms };
                            Storage.Cache.Guilds[e.EventData.Id].Channels[Channel.Key].chnPerms.AddOverwrites(Channel.Value.Raw.PermissionOverwrites, e.EventData.Id);
                        }
                    }
                }
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ServerList.Items.Clear();
                    LoadGuildList();
                });

            if (e.EventData.Presences != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (ServerList.SelectedItem != null)
                        {
                            if ((ServerList.SelectedItem as SimpleGuild).Id != "DMs")
                            {
                                #region Roles

                                MembersCvs.Source = null;
                                LoadMembers((ServerList.SelectedItem as SimpleGuild).Id);

                                #endregion
                            }
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
                    if ((ServerList.SelectedItem as SimpleGuild).Id == e.EventData.GuildId)
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
                    if ((ServerList.SelectedItem as SimpleGuild).Id == e.EventData.GuildId)
                    {
                        TextChannels.Items.Remove(
                            TextChannels.Items.FirstOrDefault(x => (x as SimpleChannel).Id == e.EventData.Id));
                    }
                });
        }

        private async void GuildChannelUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            Storage.Cache.Guilds[e.EventData.GuildId].Channels[e.EventData.Id].Raw = e.EventData;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if ((ServerList.SelectedItem as SimpleGuild).Id == e.EventData.GuildId)
                    {
                        DownloadGuild(e.EventData.GuildId);
                    }
                });
        }

        private async void GuildMemberAdded(object sender, Gateway.GatewayEventArgs<GuildMemberAdd> e)
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
            //TODO: Update list more efficiently
            if (!App.CurrentGuildIsDM)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        LoadMembers(App.CurrentGuildId);
                    });
            }
        }

        private async void GuildMemberRemoved(object sender, Gateway.GatewayEventArgs<GuildMemberRemove> e)
        {
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId]
                    .Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId].Members.Remove(e.EventData.User.Id);
            }
            //TODO: Update list more efficiently
            if (!App.CurrentGuildIsDM)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        LoadMembers(App.CurrentGuildId);
                    });
            }
        }

        private async void GuildMemberUpdated(object sender, Gateway.GatewayEventArgs<GuildMemberUpdate> e)
        {
            if (Storage.Cache.Guilds.ContainsKey(e.EventData.guildId) && Storage.Cache.Guilds[e.EventData.guildId]
                    .Members.ContainsKey(e.EventData.User.Id))
            {
                Storage.Cache.Guilds[e.EventData.guildId].Members[e.EventData.User.Id].Raw.Nick = e.EventData.Nick;
                Storage.Cache.Guilds[e.EventData.guildId].Members[e.EventData.User.Id].Raw.Roles = e.EventData.Roles;
                Storage.Cache.Guilds[e.EventData.guildId].Members[e.EventData.User.Id].Raw.User = e.EventData.User;
            }
            //TODO: Update list more efficiently
            if (!App.CurrentGuildIsDM)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        LoadMembers(App.CurrentGuildId);
                    });
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
                        LoadDMs(); //TODO: Don't reload all for one
                        if (SelectChannel)
                        {
                            foreach (SimpleChannel channel in DirectMessageChannels.Items)
                            {
                                if (channel.Type == 1 && Storage.Cache.DMs[channel.Id].Raw.Users.FirstOrDefault().Id == SelectChannelId)
                                {
                                    DirectMessageChannels.SelectedItem = channel;
                                    SelectChannel = false;
                                }
                            }
                            if (SelectChannel)
                            {
                                Session.CreateDM(new CreateDM() { Recipients = new List<string>() { SelectChannelId } });
                            }
                        }
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
                        foreach (SimpleChannel chn in DirectMessageChannels.Items)
                        {
                            if (chn.Id == e.EventData.Id)
                            {
                                DirectMessageChannels.Items.Remove(chn);
                            }
                        }
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

        private async void GatewayOnUserSettingsUpdated(object sender, GatewayEventArgs<UserSettings> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //LocalStatusChangeEnabled = false;
                Session.PrecenseDict[Storage.Cache.CurrentUser.Raw.Id] = new Presence() { User = Storage.Cache.CurrentUser.Raw, Status = e.EventData.Status };
                if (e.EventData.Status == "online")
                {
                    UserStatusOnline.IsChecked = true;
                    AnimateStatusColor((SolidColorBrush)App.Current.Resources["online"]);
                }
                if (e.EventData.Status == "idle")
                {
                    UserStatusIdle.IsChecked = true;
                    AnimateStatusColor((SolidColorBrush)App.Current.Resources["idle"]);
                }
                if (e.EventData.Status == "dnd")
                {
                    UserStatusDND.IsChecked = true;
                    AnimateStatusColor((SolidColorBrush)App.Current.Resources["dnd"]);
                }
                if (e.EventData.Status == "invisible")
                {
                    UserStatusInvisible.IsChecked = true;
                    AnimateStatusColor((SolidColorBrush)App.Current.Resources["offline"]);
                }
            });
        }

        Storyboard sb = new Storyboard() { Duration = TimeSpan.FromMilliseconds(300) };
        ColorAnimation ca = new ColorAnimation();
        private void AnimateStatusColor(SolidColorBrush brush)
        {
            StatusColorAnimation.To = brush.Color;
            ChangeStatusColor.Begin();
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
                    if (App.CurrentGuildIsDM)
                    {
                        foreach (SimpleChannel chn in DirectMessageChannels.Items)
                        {
                            if (chn.Type == 1 && Storage.Cache.DMs[chn.Id].Raw.Users.FirstOrDefault().Id == e.EventData.User.Id)
                            {
                                chn.UserStatus = e.EventData.Status;
                                if (e.EventData.Game.HasValue)
                                {
                                    chn.Playing = new Game() { Name = e.EventData.Game.Value.Name, Type = e.EventData.Game.Value.Type, Url = e.EventData.Game.Value.Url };
                                }
                            }
                        }
                    } else
                    {
                        //TODO REPLACE WITH ADD/REMOVE
                        //LoadMembers((TextChannels.SelectedItem as SimpleChannel).Id);
                    }
                });
        }

        private void OnVoiceStateUpdated(object sender, GatewayEventArgs<VoiceState> e)
        {

        }

        private void OnVoiceServerUpdated(object sender, GatewayEventArgs<VoiceState> e)
        {

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
                        try
                        {
                            Typers.Remove(Typers.First(t => t.Value == timer).Key);
                            App.UpdateTyping(Typers.First(t => t.Value == timer).Key.userId, false, e.EventData.channelId);
                        }
                        catch
                        {
                        }
                        UpdateTypingUI();
                    };
                    timer.Start();
                    Typers.Add(e.EventData, timer);
                    App.UpdateTyping(e.EventData.userId, true, e.EventData.channelId);
                    UpdateTypingUI();
                }
            });
        }

        private async void UpdateTypingUI()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    string typingString = "";
                    int DisplayedTyperCounter = 0;
                    List<string> NamesTyping = new List<string>();
                    foreach (var channel in TextChannels.Items)
                        (channel as SimpleChannel).IsTyping = false;
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
                                if (App.CurrentChannelId == typer.Key.channelId &&
                                    App.GuildMembers.ContainsKey(typer.Key.userId))
                                {
                                    var member = App.GuildMembers[typer.Key.userId];
                                    string DisplayedName = member.Raw.User.Username;
                                    if (member.Raw.Nick != null) DisplayedName = member.Raw.Nick;
                                    NamesTyping.Add(DisplayedName);
                                }
                                try
                                {
                                    (TextChannels.Items.FirstOrDefault(
                                            x => (x as SimpleChannel).Id == typer.Key.channelId) as SimpleChannel)
                                        .IsTyping = true;
                                }
                                catch (Exception)
                                {
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
                            typingString += " "+ App.GetString("/Main/TypingAnd") + " " + " " + NamesTyping.ElementAt(i); //last element out of 2, prefix = "and"
                        else if (i == DisplayedTyperCounter)
                            typingString +=
                                ", " + App.GetString("/Main/TypingAnd") + " " +
                                NamesTyping.ElementAt(i); //last element out of 2, prefix = "and" WITH OXFORD COMMA
                        else
                            typingString += ", " + NamesTyping.ElementAt(i); //intermediary element, prefix = comma
                    }
                    if (DisplayedTyperCounter > 1)
                        typingString += " " + App.GetString("/Main/TypingPlural");
                    else
                        typingString += " "+ App.GetString("/Main/TypingSingular");

                    if (DisplayedTyperCounter == 0)
                    {
                        TypingStackPanel.Fade(0, 200).Start();
                    }
                    else
                    {
                        TypingIndicator.Text = typingString;
                        TypingStackPanel.Fade(1, 200).Start();
                    }
                });
        }

        private async void UpdateGuildAndChannelUnread()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => 
                {
                    int Fullcount = 0;
                    foreach (SimpleGuild guild in ServerList.Items)
                    {
                        SimpleGuild gclone = guild.Clone();
                        gclone.NotificationCount = 0; //Will Change if true
                        gclone.IsUnread = false; //Will change if true
                        if (gclone.Id == "DMs")
                        {
                            if (App.FriendNotifications > 0 && Storage.Settings.FriendsNotifyFriendRequest)
                            {
                                gclone.NotificationCount += App.FriendNotifications;
                            }
                                
                            foreach (var chn in Storage.Cache.DMs.Values)
                                if (Session.RPC.ContainsKey(chn.Raw.Id))
                                {
                                    ReadState readstate = Session.RPC[chn.Raw.Id];
                                    if (Storage.Settings.FriendsNotifyDMs)
                                    {
                                        gclone.NotificationCount += readstate.MentionCount;
                                        Fullcount += readstate.MentionCount;
                                    }
                                    var StorageChannel = Storage.Cache.DMs[chn.Raw.Id];
                                    if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null && readstate.LastMessageId != StorageChannel.Raw.LastMessageId)
                                        gclone.IsUnread = true;
                            }
                        }
                        else
                        {
                            foreach (var chn in Storage.Cache.Guilds[gclone.Id].Channels.Values)
                                if (Session.RPC.ContainsKey(chn.Raw.Id))
                                {
                                    ReadState readstate = Session.RPC[chn.Raw.Id];
                                    gclone.NotificationCount += readstate.MentionCount;
                                    Fullcount += readstate.MentionCount;
                                    var StorageChannel = Storage.Cache.Guilds[gclone.Id].Channels[chn.Raw.Id];
                                    if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null && readstate.LastMessageId != StorageChannel.Raw.LastMessageId && !Storage.MutedChannels.Contains(chn.Raw.Id))
                                        gclone.IsUnread = true;
                            }
                        }
                        guild.Id = gclone.Id;
                        guild.ImageURL = gclone.ImageURL;
                        guild.IsDM = gclone.IsDM;
                        guild.IsMuted = gclone.IsMuted;
                        guild.IsUnread = gclone.IsUnread;
                        guild.Name = gclone.Name;
                        guild.NotificationCount = gclone.NotificationCount;
                    }
                    if (App.CurrentGuildIsDM)
                    {
                        foreach (SimpleChannel sc in DirectMessageChannels.Items)
                            if (Session.RPC.ContainsKey(sc.Id))
                            {
                                ReadState readstate = Session.RPC[sc.Id];
                                sc.NotificationCount = readstate.MentionCount;
                                var StorageChannel = Storage.Cache.DMs[sc.Id];
                                if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null &&
                                    readstate.LastMessageId != StorageChannel.Raw.LastMessageId)
                                    sc.IsUnread = true;
                                else
                                    sc.IsUnread = false;
                            }
                    }
                    else
                    {
                        foreach (SimpleChannel sc in TextChannels.Items)
                            if (Session.RPC.ContainsKey(sc.Id))
                            {
                                ReadState readstate = Session.RPC[sc.Id];
                                sc.NotificationCount = readstate.MentionCount;
                                var StorageChannel = Storage.Cache.Guilds[App.CurrentGuildId].Channels[sc.Id];
                                if (StorageChannel != null && StorageChannel.Raw.LastMessageId != null &&
                                    readstate.LastMessageId != StorageChannel.Raw.LastMessageId)
                                    sc.IsUnread = true;
                                else
                                    sc.IsUnread = false;
                                
                            }
                    }

                    if (Storage.Settings.FriendsNotifyFriendRequest)
                    {
                        Fullcount += App.FriendNotifications;
                    }

                    if (App.FriendNotifications > 0)
                    {
                        FriendsNotificationCounter.Text = App.FriendNotifications.ToString();
                        ShowFriendsBadge.Begin();
                    }
                    else
                    {
                        HideFriendsBadge.Begin();
                    }

                    if (Fullcount > 0)
                    {
                        ShowBadge.Begin();
                        BurgerNotificationCounter.Text = Fullcount.ToString();
                    }
                        
                });
        }
    }
}
