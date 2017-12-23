using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Managers
{
    public class GatewayManager
    {
        public async static void StartGateway()
        {
            //Ready
            Gateway.Ready += Gateway_Ready;
            //Message
            Gateway.MessageCreated += Gateway_MessageCreated;
            Gateway.MessageDeleted += Gateway_MessageDeleted;
            Gateway.MessageUpdated += Gateway_MessageUpdated;
            Gateway.MessageAck += Gateway_MessageAck;
            //MessageReaction
            Gateway.MessageReactionAdded += Gateway_MessageReactionAdded;
            Gateway.MessageReactionRemoved += Gateway_MessageReactionRemoved;
            Gateway.MessageReactionRemovedAll += Gateway_MessageReactionRemovedAll;
            //DMs
            Gateway.DirectMessageChannelCreated += Gateway_DirectMessageChannelCreated;
            Gateway.DirectMessageChannelDeleted += Gateway_DirectMessageChannelDeleted;
            //GuildChannel
            Gateway.GuildChannelCreated += Gateway_GuildChannelCreated;
            Gateway.GuildChannelDeleted += Gateway_GuildChannelDeleted;
            Gateway.GuildChannelUpdated += Gateway_GuildChannelUpdated;
            //Guild
            Gateway.GuildCreated += Gateway_GuildCreated;
            Gateway.GuildDeleted += Gateway_GuildDeleted;
            Gateway.GuildUpdated += Gateway_GuildUpdated;
            //GuildMember
            Gateway.GuildMemberAdded += Gateway_GuildMemberAdded;
            Gateway.GuildMemberChunk += Gateway_GuildMemberChunk;
            Gateway.GuildMemberRemoved += Gateway_GuildMemberRemoved;
            Gateway.GuildMemberUpdated += Gateway_GuildMemberUpdated;
            //GuildBan
            Gateway.GuildBanAdded += Gateway_GuildBanAdded;
            Gateway.GuildBanRemoved += Gateway_GuildBanRemoved;
            //Presence
            Gateway.PresenceUpdated += Gateway_PresenceUpdated;
            //RelationShip
            Gateway.RelationShipAdded += Gateway_RelationShipAdded;
            Gateway.RelationShipRemoved += Gateway_RelationShipRemoved;
            Gateway.RelationShipUpdated += Gateway_RelationShipUpdated;
            //Typing
            Gateway.TypingStarted += Gateway_TypingStarted;
            //Note
            Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
            //UserSettings
            Gateway.UserSettingsUpdated += Gateway_UserSettingsUpdated;
            //Voice
            Gateway.VoiceServerUpdated += Gateway_VoiceServerUpdated;
            Gateway.VoiceStateUpdated += Gateway_VoiceStateUpdated;

            await Gateway.ConnectAsync();
        }

        #region Ready
        //Aparently can contain nullref, (~2% of crashes)
        private static void Gateway_Ready(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.Ready> e)
        {
            LocalState.CurrentUser = e.EventData.User;

            LocalState.Notes = e.EventData.Notes;

            Storage.Settings.DiscordLightTheme = e.EventData.Settings.Theme == "Light" ? true : false;
            Storage.Settings.DevMode = e.EventData.Settings.DevMode;
            
            #region Friends
            foreach (var friend in e.EventData.Friends)
            {
                if (LocalState.Friends.ContainsKey(friend.Id))
                {
                    LocalState.Friends[friend.Id] = friend;
                } else
                {
                    LocalState.Friends.Add(friend.Id, friend);
                }
            }
            #endregion
            
            #region DMs
            foreach (var dm in e.EventData.PrivateChannels)
            {
                if (LocalState.DMs.ContainsKey(dm.Id))
                {
                    LocalState.DMs[dm.Id] = dm;
                } else
                {
                    LocalState.DMs.Add(dm.Id, dm);
                }
            }
            #endregion

            #region Guild
            foreach (var guild in e.EventData.Guilds)
            {
                if (LocalState.Guilds.ContainsKey(guild.Id))
                {
                    LocalState.Guilds[guild.Id] = new LocalModels.Guild(guild);
                } else
                {
                    LocalState.Guilds.Add(guild.Id, new LocalModels.Guild(guild));
                }

                if (guild.Members != null)
                {
                    foreach (var member in guild.Members)
                    {
                        if (LocalState.Guilds[guild.Id].members.ContainsKey(member.User.Id))
                        {
                            LocalState.Guilds[guild.Id].members[member.User.Id] = member;
                        }
                        else
                        {
                            LocalState.Guilds[guild.Id].members.Add(member.User.Id, member);
                        }
                    }
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }

                if (guild.Roles != null)
                {
                    foreach (var role in guild.Roles)
                    {
                        if (LocalState.Guilds[guild.Id].roles.ContainsKey(role.Id))
                        {
                            LocalState.Guilds[guild.Id].roles[role.Id] = role;
                        }
                        else
                        {
                            LocalState.Guilds[guild.Id].roles.Add(role.Id, role);
                        }
                    }
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }

                LocalState.Guilds[guild.Id].GetPermissions();

                if (guild.Channels != null)
                {
                    foreach (var channel in guild.Channels)
                    {
                        if (LocalState.Guilds[guild.Id].channels.ContainsKey(channel.Id))
                        {
                            LocalState.Guilds[guild.Id].channels[channel.Id] = new LocalModels.GuildChannel(channel, guild.Id);
                        }
                        else
                        {
                            LocalState.Guilds[guild.Id].channels.Add(channel.Id, new LocalModels.GuildChannel(channel, guild.Id));
                        }
                    }
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }

                if (guild.Presences != null)
                {
                    foreach (var presence in guild.Presences)
                    {
                        if (LocalState.PresenceDict.ContainsKey(presence.User.Id))
                        {
                            LocalState.PresenceDict[presence.User.Id] = presence;
                        }
                        else
                        {
                            LocalState.PresenceDict.Add(presence.User.Id, presence);
                        }
                    }
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }

                if (guild.VoiceStates != null)
                {
                    foreach (var voiceState in guild.VoiceStates)
                    {
                        if (LocalState.VoiceDict.ContainsKey(voiceState.UserId))
                        {
                            LocalState.VoiceDict[voiceState.UserId] = voiceState;
                        }
                        else
                        {
                            LocalState.VoiceDict.Add(voiceState.UserId, voiceState);
                        }
                    }
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }
            }
            #endregion

            #region Presence
            foreach (Presence presence in e.EventData.Presences)
            {
                if (LocalState.PresenceDict.ContainsKey(presence.User.Id))
                {
                    LocalState.PresenceDict[presence.User.Id] = presence;
                } else
                {
                    LocalState.PresenceDict.Add(presence.User.Id, presence);
                }
            }
            #endregion

            #region ReadState (RPC)
            foreach (ReadState readstate in e.EventData.ReadStates)
            {
                if (LocalState.RPC.ContainsKey(readstate.Id))
                {
                    LocalState.RPC[readstate.Id] = readstate;
                }
                else
                {
                    LocalState.RPC.Add(readstate.Id, readstate);
                }
            }
            #endregion

            #region GuildSettings (Notifications)
            foreach (SharedModels.GuildSetting guild in e.EventData.GuildSettings)
            {
                if (LocalState.GuildSettings.ContainsKey(guild.GuildId))
                {
                    LocalState.GuildSettings[guild.GuildId] = new LocalModels.GuildSetting(guild);
                } else
                {
                    LocalState.GuildSettings.Add(guild.GuildId, new LocalModels.GuildSetting(guild));
                }
            }
            #endregion

            #region GuildOrder
            int pos = 0;
            foreach (string guild in e.EventData.Settings.GuildOrder)
            {
                if (LocalState.Guilds.ContainsKey(guild))
                {
                    LocalState.Guilds[guild].Position = pos;
                }
                pos++;
            }
            #endregion

            #region CurrentUserPresence
            if (LocalState.PresenceDict.ContainsKey(e.EventData.User.Id))
            {
                LocalState.PresenceDict[e.EventData.User.Id] = new Presence() { User = e.EventData.User, Status = e.EventData.Settings.Status };
            }
            else
            {
                LocalState.PresenceDict.Add(e.EventData.User.Id, new Presence() { User = e.EventData.User, Status = e.EventData.Settings.Status });
            }
            App.UserStatusChanged(e.EventData.Settings);
            #endregion

            App.ReadyRecieved();
        }
        #endregion

        #region Message
        private static void Gateway_MessageCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Message> e)
        {
            if (App.CurrentChannelId == e.EventData.ChannelId)
            {
                App.MessageCreated(e.EventData);
                App.MarkChannelAsRead(e.EventData.ChannelId);
                App.UpdateUnreadIndicators();
            } else
            {
                foreach (var guild in LocalState.Guilds) //TODO: Check efficiency
                {
                    if (guild.Value.channels.ContainsKey(e.EventData.ChannelId))
                    {
                        var editableRawChn = guild.Value.channels[e.EventData.ChannelId].raw;
                        editableRawChn.LastMessageId = e.EventData.Id;
                        guild.Value.channels[e.EventData.ChannelId].raw = editableRawChn;
                        if (!LocalState.RPC.ContainsKey(e.EventData.ChannelId))
                        {
                            LocalState.RPC.Add(e.EventData.ChannelId, new ReadState() { Id = e.EventData.ChannelId, LastMessageId = editableRawChn.LastMessageId, MentionCount = e.EventData.Mentions.FirstOrDefault( x => x.Id == LocalState.CurrentUser.Id).Id != null || e.EventData.MentionEveryone ? 1 : 0 , LastPinTimestamp = null });
                        } 
                    }
                }
                App.UpdateUnreadIndicators();

                if (Storage.Settings.Toasts)
                {
                    NotifcationManager.CreateMessageCreatedNotifcation(e.EventData);
                }
            }
        }

        private static void Gateway_MessageDeleted(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.MessageDelete> e)
        {
            if (App.CurrentChannelId == e.EventData.ChannelId)
            {
                App.MessageDeleted(e.EventData.MessageId);
            } else
            {
                //TODO: Notifications (maybe)
            }
        }

        private static void Gateway_MessageUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Message> e)
        {
            if (App.CurrentChannelId == e.EventData.ChannelId)
            {
                App.MessageEdited(e.EventData);
            } else
            {
                //TODO: Notifications (I'm actually really happy with this idea)
            }
        }

        private static void Gateway_MessageAck(object sender, Gateway.GatewayEventArgs<SharedModels.MessageAck> e)
        {
            try
            {
                ReadState prevState = LocalState.RPC[e.EventData.ChannelId];
                LocalState.RPC[e.EventData.ChannelId] = new ReadState() { Id = e.EventData.ChannelId, LastMessageId = e.EventData.Id, LastPinTimestamp = prevState.LastPinTimestamp, MentionCount = 0 };
            }
            catch (Exception) { }
             App.UpdateUnreadIndicators();
        }
        #endregion

        #region MessageReaction
        private static void Gateway_MessageReactionRemovedAll(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.MessageReactionRemoveAll> e)
        {
            //Managed in MessageControl
        }

        private static void Gateway_MessageReactionRemoved(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.MessageReactionUpdate> e)
        {
            //Managed in MessageControl
        }

        private static void Gateway_MessageReactionAdded(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.MessageReactionUpdate> e)
        {
            //Managed in MessageControl
        }
        #endregion

        #region DMs
        private static void Gateway_DirectMessageChannelCreated(object sender, Gateway.GatewayEventArgs<SharedModels.DirectMessageChannel> e)
        {

        }

        private static void Gateway_DirectMessageChannelDeleted(object sender, Gateway.GatewayEventArgs<SharedModels.DirectMessageChannel> e)
        {

        }
        #endregion

        #region GuildChannel
        private static void Gateway_GuildChannelCreated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            if (!LocalState.Guilds[e.EventData.GuildId].channels.ContainsKey(e.EventData.Id))
            {
                LocalState.Guilds[e.EventData.GuildId].channels.Add(e.EventData.Id, new LocalModels.GuildChannel(e.EventData));
            }

            if (e.EventData.GuildId == App.CurrentGuildId)
            {
                App.GuildChannelCreated(e.EventData);
            }
        }

        private static void Gateway_GuildChannelDeleted(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            if (LocalState.Guilds[e.EventData.GuildId].channels.ContainsKey(e.EventData.Id))
            {
                LocalState.Guilds[e.EventData.GuildId].channels.Remove(e.EventData.Id);
            }

            if (e.EventData.GuildId == App.CurrentGuildId)
            {
                App.GuildChannelDeleted(e.EventData.GuildId, e.EventData.Id);
            }
        }

        private static void Gateway_GuildChannelUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildChannel> e)
        {
            if (LocalState.Guilds[e.EventData.GuildId].channels.ContainsKey(e.EventData.Id))
            {
                LocalState.Guilds[e.EventData.GuildId].channels[e.EventData.Id] = new LocalModels.GuildChannel(e.EventData);
            }
        }
        #endregion

        #region Guild
        private static void Gateway_GuildCreated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {
            if (!LocalState.Guilds.ContainsKey(e.EventData.Id))
            {
                LocalState.Guilds.Add(e.EventData.Id, new LocalModels.Guild(e.EventData));
                foreach (var member in e.EventData.Members)
                {
                    if (LocalState.Guilds[e.EventData.Id].members.ContainsKey(member.User.Id))
                    {
                        LocalState.Guilds[e.EventData.Id].members[member.User.Id] = member;
                    }
                    else
                    {
                        LocalState.Guilds[e.EventData.Id].members.Add(member.User.Id, member);
                    }
                }

                foreach (var role in e.EventData.Roles)
                {
                    LocalState.Guilds[e.EventData.Id].roles.Add(role.Id, role);
                }

                LocalState.Guilds[e.EventData.Id].GetPermissions();

                foreach (var channel in e.EventData.Channels)
                {
                    if (LocalState.Guilds[e.EventData.Id].channels.ContainsKey(channel.Id))
                    {
                        LocalState.Guilds[e.EventData.Id].channels[channel.Id] = new LocalModels.GuildChannel(channel, e.EventData.Id);
                    }
                    else
                    {
                        LocalState.Guilds[e.EventData.Id].channels.Add(channel.Id, new LocalModels.GuildChannel(channel, e.EventData.Id));
                    }
                }
                foreach (var presence in e.EventData.Presences)
                {
                    if (LocalState.PresenceDict.ContainsKey(presence.User.Id))
                    {
                        LocalState.PresenceDict[presence.User.Id] = presence;
                    }
                    else
                    {
                        LocalState.PresenceDict.Add(presence.User.Id, presence);
                    }
                }
                foreach (var voiceState in e.EventData.VoiceStates)
                {
                    if (LocalState.VoiceDict.ContainsKey(voiceState.UserId))
                    {
                        LocalState.VoiceDict[voiceState.UserId] = voiceState;
                    }
                    else
                    {
                        LocalState.VoiceDict.Add(voiceState.UserId, voiceState);
                    }
                }
                App.GuildCreated(e.EventData);
            }
        }
        private static void Gateway_GuildDeleted(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.GuildDelete> e)
        {
            //TODO: Deal with guild outages
            if (LocalState.Guilds.ContainsKey(e.EventData.GuildId))
            {
                
            }
        }
        private static void Gateway_GuildUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Guild> e)
        {

        }
        #endregion

        #region GuildMember
        private static void Gateway_GuildMemberAdded(object sender, Gateway.GatewayEventArgs<SharedModels.GuildMemberAdd> e)
        {
            if (LocalState.Guilds[e.EventData.guildId].members.ContainsKey(e.EventData.User.Id))
            {
                LocalState.Guilds[e.EventData.guildId].members[e.EventData.User.Id] = new GuildMember() { Deaf = e.EventData.Deaf, JoinedAt = e.EventData.JoinedAt, Mute = e.EventData.Mute, Nick = e.EventData.Nick, Roles = e.EventData.Roles, User = e.EventData.User };
            } else
            {
                LocalState.Guilds[e.EventData.guildId].members.Add(e.EventData.User.Id, new GuildMember() { Deaf = e.EventData.Deaf, JoinedAt = e.EventData.JoinedAt, Mute = e.EventData.Mute, Nick = e.EventData.Nick, Roles = e.EventData.Roles, User = e.EventData.User });
            }
        }

        private static void Gateway_GuildMemberRemoved(object sender, Gateway.GatewayEventArgs<SharedModels.GuildMemberRemove> e)
        {
            if (LocalState.Guilds[e.EventData.guildId].members.ContainsKey(e.EventData.User.Id))
            {
                LocalState.Guilds[e.EventData.guildId].members.Remove(e.EventData.User.Id);
            }
        }

        private static void Gateway_GuildMemberUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.GuildMemberUpdate> e)
        {
            if (LocalState.Guilds[e.EventData.guildId].members.ContainsKey(e.EventData.User.Id))
            {
                var member = LocalState.Guilds[e.EventData.guildId].members[e.EventData.User.Id];
                member.Nick = e.EventData.Nick;
                member.Roles = e.EventData.Roles;
                LocalState.Guilds[e.EventData.guildId].members[e.EventData.User.Id] = member;
            }
        }

        private static void Gateway_GuildMemberChunk(object sender, Gateway.GatewayEventArgs<SharedModels.GuildMemberChunk> e)
        {
            foreach (var member in e.EventData.Members)
            {
                if (!LocalState.Guilds[e.EventData.GuildId].members.ContainsKey(member.User.Id))
                {
                    LocalState.Guilds[e.EventData.GuildId].members.Add(member.User.Id, new GuildMember() { Deaf = member.Deaf, JoinedAt = member.JoinedAt, Mute = member.Mute, Nick = member.Nick, Roles = member.Roles, User = member.User });
                }
                else
                {
                    LocalState.Guilds[e.EventData.GuildId].members[member.User.Id] = new GuildMember() { Deaf = member.Deaf, JoinedAt = member.JoinedAt, Mute = member.Mute, Nick = member.Nick, Roles = member.Roles, User = member.User };
                }
            }
            App.MembersUpdated();
        }
        #endregion

        #region GuildBan
        private static void Gateway_GuildBanRemoved(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.GuildBanUpdate> e)
        {

        }

        private static void Gateway_GuildBanAdded(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.GuildBanUpdate> e)
        {

        }
        #endregion

        #region Presence
        private static void Gateway_PresenceUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Presence> e)
        {
            if (LocalState.PresenceDict.ContainsKey(e.EventData.User.Id))
            {
                LocalState.PresenceDict[e.EventData.User.Id] = e.EventData;
            } else
            {
                LocalState.PresenceDict.Add(e.EventData.User.Id, e.EventData);
            }
            App.PresenceUpdated(e.EventData.User.Id, e.EventData);
        }
        #endregion

        #region RelationShip
        private static void Gateway_RelationShipAdded(object sender, Gateway.GatewayEventArgs<SharedModels.Friend> e)
        {
            if (!LocalState.Friends.ContainsKey(e.EventData.Id))
            {
                LocalState.Friends.Add(e.EventData.Id, e.EventData);
            }
        }

        private static void Gateway_RelationShipRemoved(object sender, Gateway.GatewayEventArgs<SharedModels.Friend> e)
        {
            if (LocalState.Friends.ContainsKey(e.EventData.Id))
            {
                LocalState.Friends.Remove(e.EventData.Id);
            }
        }

        private static void Gateway_RelationShipUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Friend> e)
        {
            if (LocalState.Friends.ContainsKey(e.EventData.Id))
            {
                LocalState.Friends[e.EventData.Id] = e.EventData;
            }
        }
        #endregion

        #region Typing
        private static async void Gateway_TypingStarted(object sender, Gateway.GatewayEventArgs<SharedModels.TypingStart> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     DispatcherTimer timer = new DispatcherTimer();
                     /*If the user was already typing in that channel before...*/
                     if (LocalState.Typers.Count > 0 && LocalState.Typers.Any(x => x.Key.userId == e.EventData.userId &&
                                                             x.Key.channelId == e.EventData.channelId))
                     {
                         /*...Reset the timer by calling Start again*/
                         LocalState.Typers.First(x => x.Key.userId == e.EventData.userId && x.Key.channelId == e.EventData.channelId)
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
                                 App.UpdateTyping(LocalState.Typers.First(t => t.Value == timer).Key.userId, false, e.EventData.channelId);
                                 LocalState.Typers.Remove(LocalState.Typers.First(t => t.Value == timer).Key);
                             }
                             catch
                             {

                             }
                         };
                         timer.Start();
                         LocalState.Typers.Add(e.EventData, timer);
                         App.UpdateTyping(e.EventData.userId, true, e.EventData.channelId);
                     }
                 });
        }
        #endregion

        #region Note
        private static void Gateway_UserNoteUpdated(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.UserNote> e)
        {
            LocalState.Notes[e.EventData.UserId] = e.EventData.Note;
        }
        #endregion

        #region User
        private static void Gateway_UserSettingsUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.UserSettings> e)
        {
            var temp = LocalState.PresenceDict[LocalState.CurrentUser.Id];
            temp.Status = e.EventData.Status;
            LocalState.PresenceDict[LocalState.CurrentUser.Id] = temp;
            App.UserStatusChanged(e.EventData);
        }
        #endregion

        #region Voice
        private static async void Gateway_VoiceServerUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.VoiceServerUpdate> e)
        {
            await AudioTrig.CreateAudioGraph();
            VoiceManager.VoiceConnection = new Voice.VoiceConnection(e.EventData, LocalState.VoiceState);
            await VoiceManager.VoiceConnection.ConnectAsync();
        }

        private static void Gateway_VoiceStateUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.VoiceState> e)
        {
            try
            {
                if (e.EventData.UserId == LocalState.CurrentUser.Id)
                {
                    LocalState.VoiceState = e.EventData;
                    
                }
                if (LocalState.VoiceDict.ContainsKey(e.EventData.UserId))
                {
                    LocalState.VoiceDict[e.EventData.UserId] = e.EventData;
                }
                else
                {
                    LocalState.VoiceDict.Add(e.EventData.UserId, e.EventData);
                }
            }
            catch
            {
                //Huh, Weird
            }
        }
        #endregion

        public static Gateway.Gateway Gateway;
    }
}
