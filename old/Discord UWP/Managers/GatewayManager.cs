using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Gateway.DownstreamEvents;
using GuildChannel = DiscordAPI.SharedModels.GuildChannel;
using Guild = DiscordAPI.SharedModels.Guild;

namespace Quarrel.Managers
{
    public class GatewayManager
    {
        public static async void StartGateway()
        {
            if (Gateway == null)
            {
                await RESTCalls.SetupToken();
            }
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
            //ChannelRecipientAdded is handled exclusively by MainPage
            //ChannelRecipientRemoved is handled exclusively by MainPage

            //Guild
            Gateway.GuildCreated += Gateway_GuildCreated;
            Gateway.GuildDeleted += Gateway_GuildDeleted;
            Gateway.GuildUpdated += Gateway_GuildUpdated;
            Gateway.GuildSynced += Gateway_GuildSynced;
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
            //Other
            Gateway.SessionReplaced += Gateway_SessionReplaced;
            await Gateway.ConnectAsync();
        }


        private static void Gateway_SessionReplaced(object sender, GatewayEventArgs<SessionReplace> e)
        {
            if (e.EventData != null)
            {
                session = e.EventData.SessionId;
            }
        }

        #region Ready
        //Aparently can contain nullref, (~2% of crashes)
        public static string session = "";
        private static async void Gateway_Ready(object sender, GatewayEventArgs<Ready> e)
        {
            session = e.EventData.SessionId;
            Storage.UNSdeferralStart(); //This improves performance, it means that every UpdateNotificationTask() won't save to disk (but UNSdeferralEnd() MUST be called to save the values!
            Storage.UNSclear(); //Purge it, no need to keep pointless shit in that dictionnary

            LocalState.CurrentUser = e.EventData.User;
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set LocalState.CurrentUser (ln 72)");});

            LocalState.Notes = e.EventData.Notes;
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set LocalState.Notes (ln 76)");});

            Storage.Settings.DiscordLightTheme = e.EventData.Settings.Theme == "Light" ? true : false;
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set Storage.Settings.DiscordLightTheme (ln 80)");});

            Storage.Settings.DevMode = e.EventData.Settings.DevMode;
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal,() => { App.StatusChanged("Succesfully set Storage.Settings.DevMode (ln 84)");});

            #region DMs
            foreach (var dm in e.EventData.PrivateChannels)
            {
                if (LocalState.DMs.ContainsKey(dm.Id))
                {
                    LocalState.DMs[dm.Id] = dm;
                }
                else
                {
                    LocalState.DMs.Add(dm.Id, dm);
                }
            }
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set all DMs (ln 104-113)"); });
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
                if (App.AslansBullshit)
                    await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully added guild with id" + guild.Id + "(ln 121-127)"); });

                if (guild.Members != null)
                {
                    foreach (var member in guild.Members)
                        LocalState.Guilds[guild.Id].members.TryAdd(member.User.Id, member);
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }
                if (App.AslansBullshit)
                    await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully added members of guild with id" + guild.Id + "(ln 131-147)"); });

                if (guild.Roles != null)
                {
                    foreach (var role in guild.Roles)
                            LocalState.Guilds[guild.Id].roles.TryAdd(role.Id, role);
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }
                if (App.AslansBullshit)
                    await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully added roles of guild with id" + guild.Id + "(ln 151-167)"); });
                

                if (guild.Channels != null)
                {
                    foreach (var channel in guild.Channels)
                            LocalState.Guilds[guild.Id].channels.TryAdd(channel.Id, new LocalModels.GuildChannel(channel, guild.Id));
                } else
                {
                    LocalState.Guilds[guild.Id].valid = false;
                }
                if (App.AslansBullshit)
                    await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully added channels of guild with id" + guild.Id + "(ln 173-189)"); });

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
                if (App.AslansBullshit)
                    await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully added presences of guild with id" + guild.Id + "(ln 193-209)"); });

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
                if (App.AslansBullshit)
                    await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully added voice states of guild with id" + guild.Id + "(ln 213-229)"); });
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
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set LocalSate.PresenceDict (ln 236-245)"); });
            #endregion

            #region ReadState (RPC)
            if (e.EventData.ReadStates != null)
            {

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
                    if (readstate.MentionCount > 0)
                        Storage.UpdateNotificationState("c" + readstate.Id, readstate.MentionCount.ToString());
                }
            }
            Storage.UNSdeferralEnd();
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set LocalState.RPC (ln 251-261)"); });
            #endregion


            #region GuildSettings (Notifications)
            if (e.EventData.GuildSettings != null)
            {
                foreach (DiscordAPI.SharedModels.GuildSetting guild in e.EventData.GuildSettings)
                {
                    if (guild.GuildId != null)
                    {
                        if (LocalState.GuildSettings.ContainsKey(guild.GuildId))
                        {
                            LocalState.GuildSettings[guild.GuildId] = new LocalModels.GuildSetting(guild);
                        }
                        else
                        {
                            LocalState.GuildSettings.Add(guild.GuildId, new LocalModels.GuildSetting(guild));
                        }
                    }
                }
            }
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set LocalState.GuildSettings (ln 267-276)"); });
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
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set guild positions (ln 282-290)"); });

            LocalState.Settings = e.EventData.Settings;
            #endregion

            #region CurrentUserPresence

            if (LocalState.PresenceDict.ContainsKey(e.EventData.User.Id))
            {
                LocalState.PresenceDict[e.EventData.User.Id].Status = e.EventData.Settings.Status;
            }
            else
            {
                LocalState.PresenceDict.Add(e.EventData.User.Id, new Presence() { User = e.EventData.User, Status = e.EventData.Settings.Status });
            }
            App.UserStatusChanged(e.EventData.Settings);
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set CurrentUserPresence (ln 296-304)"); });
            #endregion

            #region Spotify
            foreach(var account in e.EventData.ConnectedAccount)
            {
                if(account.Type == "spotify" && account.AccessToken!=null)
                {
                    //SpotifyManager.Start(account.AccessToken, account.Id);
                }
            }
            #endregion

            App.ReadyRecieved();
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully recieved Ready Packet (ln 309)"); });

            #region Friends
            //This improves performance, because we aren't saving the settings on every loop

            foreach (var friend in e.EventData.Friends)
            {
                if (LocalState.Friends.ContainsKey(friend.Id))
                {
                    LocalState.Friends[friend.Id] = friend;
                }
                else
                {
                    LocalState.Friends.Add(friend.Id, friend);
                }
                if (friend.Type == 2)
                {
                    if (LocalState.Blocked.ContainsKey(friend.Id))
                    {
                        LocalState.Blocked[friend.Id] = friend;
                    }
                    else
                    {
                        LocalState.Blocked.Add(friend.Id, friend);
                    }
                }
                else if (friend.Type == 3)
                {
                    Storage.UpdateNotificationState("r" + friend.user.Id, friend.Id);
                }
            }
            if (App.AslansBullshit)
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { App.StatusChanged("Succesfully set AllFriends (ln 89-98)"); });
            #endregion
            //DON'T ADD ANYTHING HERE, THE FRIENDS REGION MUST BE THE LAST
        }
        #endregion

        #region Message
        private static void Gateway_MessageCreated(object sender, GatewayEventArgs<Message> e)
        {
            if (App.CurrentGuildIsDM)
            {
                App.DMUpdate(e.EventData.ChannelId, e.EventData.Id);
            }
            bool IsDM = false;
            if(e.EventData.User.Id != LocalState.CurrentUser.Id)
            {
                if (App.CurrentChannelId == e.EventData.ChannelId || e.EventData.Type == 3 && App.IsFocused)
                {
                    if (Storage.Settings.SoundNotifications && App.Insider)
                    {
                        AudioManager.PlaySoundEffect(e.EventData.Type == 3 ? /*"inring"*/ "" :"message");
                    }
                   /* if (App.IsFocused)
                    {
                        App.MarkMessageAsRead(e.EventData.Id, e.EventData.ChannelId);
                    }
                    else
                    {
                        App.ReadWhenFocused(e.EventData.Id, e.EventData.ChannelId);
                    }*/
                    App.UpdateUnreadIndicators();
                }
                else
                {
                    if (LocalState.DMs.ContainsKey(e.EventData.ChannelId))
                    {
                        IsDM = true;
                        if (e.EventData.Type == 3 && e.EventData.User.Id != LocalState.CurrentUser.Id)
                        {
                            //TODO: Handle calls
                            NotificationManager.CreateCallNotification(e.EventData);
                        }
                        if (e.EventData.User.Id != LocalState.CurrentUser.Id)
                        {
                            LocalState.DMs[e.EventData.ChannelId].UpdateLMID(e.EventData.Id);
                        }

                        if (LocalState.RPC.ContainsKey(e.EventData.ChannelId))
                        {
                            var clone = LocalState.RPC[e.EventData.ChannelId];
                            clone.MentionCount += 1;
                            
                            LocalState.RPC[e.EventData.ChannelId] = clone;
                        }
                        else
                        {
                            LocalState.RPC.Add(e.EventData.ChannelId, new ReadState() { Id = e.EventData.ChannelId, LastMessageId = "0", MentionCount = 1 });
                        }

                        if (LocalState.DMs.ContainsKey(e.EventData.ChannelId))
                        {
                            var temp = LocalState.DMs[e.EventData.ChannelId];
                            temp.LastMessageId = e.EventData.Id;
                            LocalState.DMs[e.EventData.ChannelId] = temp;
                        }
                            
                    }
                    else
                    {
                        foreach (var guild in LocalState.Guilds)
                        {
                            if (guild.Value.channels.ContainsKey(e.EventData.ChannelId))
                            {
                                if (e.EventData.User.Id != LocalState.CurrentUser.Id)
                                {
                                    guild.Value.channels[e.EventData.ChannelId].raw.UpdateLMID(e.EventData.Id);
                                }

                                if (!LocalState.RPC.ContainsKey(e.EventData.ChannelId))
                                {
                                    var mentioncount = e.EventData.Mentions?.FirstOrDefault(x => x.Id == LocalState.CurrentUser?.Id)?.Id != null || e.EventData.MentionEveryone ? 1 : 0;
                                    LocalState.RPC.Add(e.EventData.ChannelId, new ReadState() { Id = e.EventData.ChannelId, LastMessageId = "0", MentionCount = mentioncount, LastPinTimestamp = null });
                                }
                                else
                                {
                                    if (e.EventData.Mentions.Any() && e.EventData.Mentions.FirstOrDefault(x => x.Id == LocalState.CurrentUser?.Id)?.Id != null || e.EventData.MentionEveryone)
                                    {
                                        var editReadState = LocalState.RPC[e.EventData.ChannelId];
                                        editReadState.MentionCount++;
                                        LocalState.RPC[e.EventData.ChannelId] = editReadState;
                                        if (Storage.Settings.GlowOnMention)
                                        {
                                            App.FlashMention();  
                                        }
                                        if (Storage.Settings.Toasts)
                                        {
                                            NotificationManager.CreateMentionNotification(e.EventData.User.Username,
                                                Common.AvatarString(e.EventData.User.Avatar, e.EventData.User.Id),
                                                guild.Value.Raw.Name,
                                                guild.Value.channels[e.EventData.ChannelId].raw.Name,
                                                e.EventData.Content, e.EventData.ChannelId, guild.Key,
                                                e.EventData.Id);
                                        }
                                        if (Storage.Settings.Badge)
                                        {
                                            // TODO: Update badge tile
                                        }
                                    }
                                }
                            }
                        }
                    }
                    App.UpdateUnreadIndicators();
                }
            }
            else
            {
                if (LocalState.RPC.ContainsKey(e.EventData.ChannelId))
                {
                    var clone = LocalState.RPC[e.EventData.ChannelId];
                    clone.LastMessageId = e.EventData.Id;
                    LocalState.RPC[e.EventData.ChannelId] = clone;
                }
                else
                {
                    LocalState.RPC.Add(e.EventData.ChannelId, new ReadState() { Id = e.EventData.ChannelId, LastMessageId = e.EventData.Id });
                }
            }
            App.MessageCreated(e.EventData);

            if (!IsDM)
            {
                //Update the last message ID in the LocalState.Guilds
                foreach (var guild in LocalState.Guilds)
                    if (guild.Value.channels.ContainsKey(e.EventData.ChannelId))
                        LocalState.Guilds[guild.Key].channels[e.EventData.ChannelId].raw.LastMessageId = e.EventData.Id;
            }
            else
            {
                Storage.UpdateNotificationState("c" + e.EventData.ChannelId, "0");
            }

//            await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal,
  //               () =>
    //             {
                     if (LocalState.Typers.ContainsKey(e.EventData.ChannelId))
                     {
                         if (LocalState.Typers[e.EventData.ChannelId].ContainsKey(e.EventData.User.Id))
                         {
                             LocalState.Typers[e.EventData.ChannelId].Remove(e.EventData.User.Id);
                             if (LocalState.Typers[e.EventData.ChannelId].Count == 0)
                             {
                                 LocalState.Typers.Remove(e.EventData.ChannelId);
                             }
                         }
                     }

                     App.UpdateTyping(e.EventData.User.Id, false, e.EventData.ChannelId);
            //           });
            
        }

        private static void Gateway_MessageDeleted(object sender, GatewayEventArgs<MessageDelete> e)
        {
            App.MessageDeleted(e.EventData.MessageId, e.EventData.ChannelId);
            if (App.CurrentChannelId == e.EventData.ChannelId)
            {
            } else
            {
                //TODO: Notifications (maybe)
            }
        }

        private static void Gateway_MessageUpdated(object sender, GatewayEventArgs<Message> e)
        {
            App.MessageEdited(e.EventData);
            if (App.CurrentChannelId == e.EventData.ChannelId)
            {
            } else
            {
                //TODO: Notifications (I'm actually really happy with this idea)
            }
        }

        private static void Gateway_MessageAck(object sender, GatewayEventArgs<MessageAck> e)
        {
            try
            {
                if (!LocalState.RPC.ContainsKey(e.EventData.ChannelId)) return;
                ReadState prevState = LocalState.RPC[e.EventData.ChannelId];
                LocalState.RPC[e.EventData.ChannelId] = new ReadState() { Id = e.EventData.ChannelId, LastMessageId = e.EventData.Id, LastPinTimestamp = prevState.LastPinTimestamp, MentionCount = 0 };
            }
            catch { }
             App.UpdateUnreadIndicators();
        }
        #endregion

        #region MessageReaction
        private static void Gateway_MessageReactionRemovedAll(object sender, GatewayEventArgs<MessageReactionRemoveAll> e)
        {
            //Managed in MessageControl
        }

        private static void Gateway_MessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            //Managed in MessageControl
        }

        private static void Gateway_MessageReactionAdded(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            //Managed in MessageControl
        }
        #endregion

        #region DMs
        private static void Gateway_DirectMessageChannelCreated(object sender, GatewayEventArgs<DirectMessageChannel> e)
        {
            if (!LocalState.DMs.ContainsKey(e.EventData.Id))
            {
                LocalState.DMs.Add(e.EventData.Id, e.EventData);
            }

            if (App.CurrentGuildIsDM)
            {
                App.DMCreated(e.EventData);
            }
        }

        private static void Gateway_DirectMessageChannelDeleted(object sender, GatewayEventArgs<DirectMessageChannel> e)
        {
            if (LocalState.DMs.ContainsKey(e.EventData.Id))
            {
                LocalState.DMs.Remove(e.EventData.Id);
            }

            if (App.CurrentGuildIsDM)
            {
                App.DMDeleted(e.EventData.Id);
            }
        }
        #endregion

        #region GuildChannel
        private static void Gateway_GuildChannelCreated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            LocalState.Guilds[e.EventData.GuildId].channels.TryAdd(e.EventData.Id, new LocalModels.GuildChannel(e.EventData));

            if (e.EventData.GuildId == App.CurrentGuildId)
            {
                App.GuildChannelCreated(e.EventData);
            }
        }

        private static void Gateway_GuildChannelDeleted(object sender, GatewayEventArgs<GuildChannel> e)
        {
            LocalModels.GuildChannel channel;
            LocalState.Guilds[e.EventData.GuildId].channels.TryRemove(e.EventData.Id, out channel);
            channel = null;
            if (e.EventData.GuildId == App.CurrentGuildId)
            {
                App.GuildChannelDeleted(e.EventData.GuildId, e.EventData.Id);
            }
        }

        private static void Gateway_GuildChannelUpdated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            if (LocalState.Guilds[e.EventData.GuildId].channels.ContainsKey(e.EventData.Id))
            {
                LocalState.Guilds[e.EventData.GuildId].channels[e.EventData.Id] = new LocalModels.GuildChannel(e.EventData);
            }

            if (e.EventData.GuildId == App.CurrentGuildId)
            {
                App.GuildChannelUpdated(e.EventData);
            }
        }
        #endregion

        #region Guild
        private static void Gateway_GuildCreated(object sender, GatewayEventArgs<Guild> e)
        {
            if (!LocalState.Guilds.ContainsKey(e.EventData.Id))
            {
                LocalState.Guilds.Add(e.EventData.Id, new LocalModels.Guild(e.EventData));
                foreach (var member in e.EventData.Members)
                    LocalState.Guilds[e.EventData.Id].members.TryAdd(member.User.Id, member);

                foreach (var role in e.EventData.Roles)
                    LocalState.Guilds[e.EventData.Id].roles.TryAdd(role.Id, role);
                

                foreach (var channel in e.EventData.Channels)
                    LocalState.Guilds[e.EventData.Id].channels.TryAdd(channel.Id, new LocalModels.GuildChannel(channel, e.EventData.Id));
                
                foreach (var presence in e.EventData.Presences)
                {
                    //TODO Optimize this, ContainsKey is running twice
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
                    //TODO Optimize this, ContainsKey is running twice
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
        private static void Gateway_GuildDeleted(object sender, GatewayEventArgs<GuildDelete> e)
        {
            //TODO: Deal with guild outages
            if (LocalState.Guilds.ContainsKey(e.EventData.GuildId))
            {
                App.GuildDeleted(e.EventData.GuildId);
            }
        }
        private static void Gateway_GuildUpdated(object sender, GatewayEventArgs<Guild> e)
        {
            if (LocalState.Guilds.ContainsKey(e.EventData.Id))
            {
                App.GuildUpdated(e.EventData);
            }
        }
        private static void Gateway_GuildSynced(object sender, GatewayEventArgs<GuildSync> e)
        {
            App.GuildSynced(e.EventData);
        }

        #endregion

        #region GuildMember
        private static void Gateway_GuildMemberAdded(object sender, GatewayEventArgs<GuildMemberAdd> e)
        {
            //TODO Optimize this, ContainsKey is running twice
            if (LocalState.Guilds[e.EventData.guildId].members.ContainsKey(e.EventData.User.Id))
            {
                LocalState.Guilds[e.EventData.guildId].members[e.EventData.User.Id] = new GuildMember() { Deaf = e.EventData.Deaf, JoinedAt = e.EventData.JoinedAt, Mute = e.EventData.Mute, Nick = e.EventData.Nick, Roles = e.EventData.Roles, User = e.EventData.User };
            } else
            {
                LocalState.Guilds[e.EventData.guildId].members.TryAdd(e.EventData.User.Id, new GuildMember() { Deaf = e.EventData.Deaf, JoinedAt = e.EventData.JoinedAt, Mute = e.EventData.Mute, Nick = e.EventData.Nick, Roles = e.EventData.Roles, User = e.EventData.User });
            }
        }

        private static void Gateway_GuildMemberRemoved(object sender, GatewayEventArgs<GuildMemberRemove> e)
        {
            LocalState.Guilds[e.EventData.guildId].members.TryRemove(e.EventData.User.Id, out GuildMember member);
            member = null;
        }

        private static void Gateway_GuildMemberUpdated(object sender, GatewayEventArgs<GuildMemberUpdate> e)
        {
            if (LocalState.Guilds[e.EventData.guildId].members.ContainsKey(e.EventData.User.Id))
            {
                var member = LocalState.Guilds[e.EventData.guildId].members[e.EventData.User.Id];
                member.Nick = e.EventData.Nick;
                member.Roles = e.EventData.Roles;
                LocalState.Guilds[e.EventData.guildId].members[e.EventData.User.Id] = member;
            }
        }

        private static void Gateway_GuildMemberChunk(object sender, GatewayEventArgs<GuildMemberChunk> e)
        {
            foreach (var member in e.EventData.Members)
            {
                var added = LocalState.Guilds[e.EventData.GuildId].members.TryAdd(member.User.Id, new GuildMember() { Deaf = member.Deaf, JoinedAt = member.JoinedAt, Mute = member.Mute, Nick = member.Nick, Roles = member.Roles, User = member.User });
                if(!added)
                    LocalState.Guilds[e.EventData.GuildId].members[member.User.Id] = new GuildMember() { Deaf = member.Deaf, JoinedAt = member.JoinedAt, Mute = member.Mute, Nick = member.Nick, Roles = member.Roles, User = member.User };
            }
            App.MembersUpdated();
        }
        #endregion

        #region GuildBan
        private static void Gateway_GuildBanRemoved(object sender, GatewayEventArgs<GuildBanUpdate> e)
        {

        }

        private static void Gateway_GuildBanAdded(object sender, GatewayEventArgs<GuildBanUpdate> e)
        {

        }
        #endregion

        #region Presence
        private static void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
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
        private static void Gateway_RelationShipAdded(object sender, GatewayEventArgs<Friend> e)
        {
            if (!LocalState.Friends.ContainsKey(e.EventData.Id))
            {
                LocalState.Friends.Add(e.EventData.Id, e.EventData);
            } else
            {
                LocalState.Friends[e.EventData.Id] = e.EventData;
            }
            if(e.EventData.Type == 3)
            {
                Storage.UpdateNotificationState("r" + e.EventData.user.Id, e.EventData.Id);
            }
        }

        private static void Gateway_RelationShipRemoved(object sender, GatewayEventArgs<Friend> e)
        {
            if (LocalState.Friends.ContainsKey(e.EventData.Id))
            {
                LocalState.Friends.Remove(e.EventData.Id);
            }
        }

        private static void Gateway_RelationShipUpdated(object sender, GatewayEventArgs<Friend> e)
        {
            if (LocalState.Friends.ContainsKey(e.EventData.Id))
            {
                LocalState.Friends[e.EventData.Id] = e.EventData;
            }
        }
        #endregion

        #region Typing
        private static async void Gateway_TypingStarted(object sender, GatewayEventArgs<TypingStart> e)
        {
            await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     /*If the user was already typing in that channel before...*/
                     if (LocalState.Typers.ContainsKey(e.EventData.channelId) && LocalState.Typers[e.EventData.channelId].ContainsKey(e.EventData.userId))
                     {
                         /*...Reset the timer by calling Start again*/
                         LocalState.Typers[e.EventData.channelId][e.EventData.userId].Start();
                     }
                     else
                     {
                         /*...Otherwise, create a new timer and add it, with the EventData, to "Typers" */
                         DispatcherTimer timer = new DispatcherTimer();
                         timer.Interval = TimeSpan.FromSeconds(8);
                         timer.Tick += (sender2, o1) =>
                         {
                             timer.Stop();
                             if (LocalState.Typers.ContainsKey(e.EventData.channelId) && LocalState.Typers[e.EventData.channelId].ContainsKey(e.EventData.userId))
                             {
                                 App.UpdateTyping(e.EventData.userId, false, e.EventData.channelId);
                                 LocalState.Typers[e.EventData.channelId].Remove(e.EventData.userId);
                                 if (LocalState.Typers[e.EventData.channelId].Count == 0)
                                 {
                                     LocalState.Typers.Remove(e.EventData.channelId);
                                 }
                                 App.UpdateTyping("", false, "");
                             }
                         };
                         timer.Start();
                         if (!LocalState.Typers.ContainsKey(e.EventData.channelId))
                         {
                             LocalState.Typers.Add(e.EventData.channelId, new Dictionary<string, DispatcherTimer>());
                         }
                         LocalState.Typers[e.EventData.channelId].Add(e.EventData.userId, timer);
                         App.UpdateTyping(e.EventData.userId, true, e.EventData.channelId);
                     }
                 });
        }
        #endregion

        #region Note
        private static void Gateway_UserNoteUpdated(object sender, GatewayEventArgs<UserNote> e)
        {
            LocalState.Notes[e.EventData.UserId] = e.EventData.Note;
        }
        #endregion

        #region User
        private static void Gateway_UserSettingsUpdated(object sender, GatewayEventArgs<UserSettings> e)
        {
            var temp = LocalState.CurrentUserPresence;
            temp.Status = e.EventData.Status;
            LocalState.PresenceDict[LocalState.CurrentUser.Id] = temp;
            App.UserStatusChanged(e.EventData);
            LocalState.Settings = e.EventData;
        }
        #endregion

        #region Voice

        private static async void Gateway_VoiceServerUpdated(object sender, GatewayEventArgs<VoiceServerUpdate> e)
        {
            await AudioManager.CreateAudioGraphs();
            VoiceManager.ConnectToVoiceChannel(e.EventData);
        }

        private static void Gateway_VoiceStateUpdated(object sender, GatewayEventArgs<VoiceState> e)
        {
            try
            {
                if (e.EventData.UserId == LocalState.CurrentUser.Id)
                {
                    LocalState.VoiceState = e.EventData;
                    if (String.IsNullOrEmpty(e.EventData.ChannelId))
                    {
                        AudioManager.PlaySoundEffect("voicedc");
                    }
                } else if (e.EventData.ChannelId == LocalState.VoiceState.ChannelId)
                {
                    AudioManager.PlaySoundEffect("userjoin");
                } else if (LocalState.VoiceDict.ContainsKey(e.EventData.UserId) && LocalState.VoiceDict[e.EventData.UserId].ChannelId == LocalState.VoiceState.ChannelId)
                {
                    AudioManager.PlaySoundEffect("userleave");
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


        public static Gateway Gateway;
    }
}
