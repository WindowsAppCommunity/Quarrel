using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Gateway;
using Quarrel.Services;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Posts.Requests;

namespace Quarrel.ViewModels
{
    public class GuildViewModel : ViewModelBase
    {
        public GuildViewModel()
        {
            Messenger.Default.Register<GatewayReadyMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    #region Settings

                    foreach (var gSettings in m.EventData.GuildSettings)
                    {
                        ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, gSettings, gSettings.GuildId);

                        foreach (var cSettings in gSettings.ChannelOverrides)
                        {
                            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, cSettings, cSettings.ChannelId);
                        }
                    }
                    #endregion

                    #region SortReadStates

                    Dictionary<string, ReadState> readStates = new Dictionary<string, ReadState>();
                    foreach (var state in m.EventData.ReadStates)
                    {
                        readStates.Add(state.Id, state);
                    }

                    #endregion

                    #region Guilds and Channels

                    List<BindableGuild> guildList = new List<BindableGuild>();

                    // Add DM
                    var dmGuild = new BindableGuild(new Guild() { Name = "DM", Id = "DM" });
                    guildList.Add(dmGuild);

                    // Add DM channels
                    if (m.EventData.PrivateChannels != null && m.EventData.PrivateChannels.Any())
                    {
                        foreach (var channel in m.EventData.PrivateChannels)
                        {
                            BindableChannel bChannel = new BindableChannel(channel);

                            dmGuild.Channels.Add(bChannel);

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];
                        }

                        // Sort by last message timestamp
                        dmGuild.Channels = dmGuild.Channels.OrderByDescending(x => Convert.ToUInt64(x.Model.LastMessageId)).ToList();
                    }


                    foreach (var guild in m.EventData.Guilds)
                    {
                        BindableGuild bGuild = new BindableGuild(guild);

                        // Handle guild settings
                        GuildSetting gSettings = ServicesManager.Cache.Runtime.TryGetValue<GuildSetting>(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, guild.Id);
                        if (gSettings != null)
                        {
                            bGuild.Muted = gSettings.Muted;
                        }

                        // Guild Order
                        bGuild.Position = m.EventData.Settings.GuildOrder.IndexOf(x => x == bGuild.Model.Id);

                        // Guild Channels
                        foreach (var channel in guild.Channels)
                        {
                            IEnumerable<VoiceState> state = guild.VoiceStates?.Where(x => x.ChannelId == channel.Id);
                            BindableChannel bChannel = new BindableChannel(channel, state);

                            // Handle channel settings
                            ChannelOverride cSettings = ServicesManager.Cache.Runtime.TryGetValue<ChannelOverride>(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, channel.Id);
                            if (cSettings != null)
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

                            // Find parent position
                            if (!string.IsNullOrEmpty(bChannel.ParentId))
                                bChannel.ParentPostion = guild.Channels.First(x => x.Id == bChannel.ParentId).Position;

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];

                            bGuild.Channels.Add(bChannel);
                        }

                        bGuild.Channels = bGuild.Channels.OrderBy(x => x.AbsolutePostion).ToList();

                        bGuild.Model.Channels = null;


                        //foreach (var user in guild.Members)
                        //{
                        //    BindableUser bgMember = new BindableUser(user);
                        //    bgMember.GuildId = guild.Id;
                        //    // TODO: Add to Memberlist
                        //}

                        // Guild Roles
                        foreach (var role in guild.Roles)
                        {
                            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildRole, role, guild.Id + role.Id);
                        }

                        // Guild Presences
                        foreach (var presence in guild.Presences)
                        {
                            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Presence, presence, presence.User.Id);
                        }

                        guildList.Add(bGuild);
                    }

                    guildList = guildList.OrderBy(x => x.Position).ToList();
                    #endregion

                    // Show guilds
                    foreach (var guild in guildList)
                    {
                        Source.Add(guild.Model.Id, guild);
                    }
                });
            });

            Messenger.Default.Register<BindableGuildRequestMessage>(this, m => m.ReportResult(Source[m.GuildId]));

            Messenger.Default.Register<BindableChannelRequestMessage>(this, m =>
            {
                foreach (var guild in Source.Values)
                {
                    var chn = guild.Channels.FirstOrDefault(x => x.Model.Id == m.ChannelId);
                    if (chn != null)
                        m.ReportResult(chn);
                }
            });
        }

        public ObservableHashedCollection<string, BindableGuild> Source { get; private set; } = new ObservableHashedCollection<string, BindableGuild>(new List<KeyValuePair<string, BindableGuild>>());

        private RelayCommand<BindableGuild> navigateGuildCommand;

        public RelayCommand<BindableGuild> NavigateGuildCommand => navigateGuildCommand ?? (navigateGuildCommand = new RelayCommand<BindableGuild>((guild) =>
        {
            Messenger.Default.Send(new GuildNavigateMessage(guild));
        }));
    }
}
