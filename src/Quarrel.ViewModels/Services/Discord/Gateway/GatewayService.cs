// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API;
using DiscordAPI.API.Gateway;
using DiscordAPI.Authentication;
using DiscordAPI.Gateway;
using DiscordAPI.Gateway.DownstreamEvents;
using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using DiscordAPI.Models.Guilds;
using DiscordAPI.Models.Messages;
using DiscordAPI.Sockets;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Channels;
using Quarrel.ViewModels.Messages.Gateway.Guild;
using Quarrel.ViewModels.Messages.Gateway.Relationships;
using Quarrel.ViewModels.Messages.Gateway.Voice;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Gateway
{
    /// <summary>
    /// Manages all events from the Discord Gateway.
    /// </summary>
    public class GatewayService : IGatewayService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ICacheService _cacheService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChannelsService _channelsService;
        private readonly IGuildsService _guildsService;
        private readonly IServiceProvider _serviceProvider;

        private string previousGuildId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayService"/> class.
        /// </summary>
        /// <param name="analyticsService">The app's analytics service.</param>
        /// <param name="cacheService">The app's cache service.</param>
        /// <param name="channelsService">The app's current user service.</param>
        /// <param name="currentUserService">The app's channels service.</param>
        /// <param name="guildsService">The app's guilds service.</param>
        /// <param name="serviceProvider">The app's service provider.</param>
        public GatewayService(
            IAnalyticsService analyticsService,
            ICacheService cacheService,
            IChannelsService channelsService,
            ICurrentUserService currentUserService,
            IGuildsService guildsService,
            IServiceProvider serviceProvider)
        {
            _analyticsService = analyticsService;
            _cacheService = cacheService;
            _channelsService = channelsService;
            _currentUserService = currentUserService;
            _guildsService = guildsService;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public DiscordAPI.Gateway.Gateway Gateway { get; private set; }

        /// <inheritdoc/>
        public async Task<bool> InitializeGateway([NotNull] string accessToken)
        {
            BasicRestFactory restFactory = new BasicRestFactory();
            IGatewayConfigService gatewayService = restFactory.GetGatewayConfigService();

            try
            {
                GatewayConfig gatewayConfig = await gatewayService.GetGatewayConfig();
                IAuthenticator authenticator = new DiscordAuthenticator(accessToken);
                Gateway = new DiscordAPI.Gateway.Gateway(_serviceProvider, gatewayConfig, authenticator);
            }
            catch
            {
                Messenger.Default.Send(new ConnectionStatusMessage(ConnectionStatus.Failed));
                return false;
            }

            Gateway.Ready += Gateway_Ready;
            Gateway.GuildMembersChunk += GatewayGuildMembersChunk;
            Gateway.GuildSynced += Gateway_GuildSynced;

            Gateway.GuildCreated += Gateway_GuildCreated;
            Gateway.GuildDeleted += Gateway_GuildDeleted;
            Gateway.GuildUpdated += Gateway_GuildUpdated;

            Gateway.MessageCreated += Gateway_MessageCreated;
            Gateway.MessageDeleted += Gateway_MessageDeleted;
            Gateway.MessageUpdated += Gateway_MessageUpdated;
            Gateway.MessageAck += Gateway_MessageAck;

            Gateway.MessageReactionAdded += Gateway_MessageReactionAdded;
            Gateway.MessageReactionRemoved += Gateway_MessageReactionRemoved;
            Gateway.MessageReactionRemovedAll += Gateway_MessageReactionRemovedAll;

            Gateway.GuildMemberListUpdated += Gateway_GuildMemberListUpdated;

            Gateway.ChannelCreated += Gateway_ChannelCreated;
            Gateway.ChannelDeleted += Gateway_ChannelDeleted;
            Gateway.GuildChannelUpdated += Gateway_GuildChannelUpdated;

            Gateway.TypingStarted += Gateway_TypingStarted;

            Gateway.PresenceUpdated += Gateway_PresenceUpdated;
            Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
            Gateway.UserGuildSettingsUpdated += Gateway_UserGuildSettingsUpdated;
            Gateway.UserSettingsUpdated += Gateway_UserSettingsUpdated;

            Gateway.VoiceServerUpdated += Gateway_VoiceServerUpdated;
            Gateway.VoiceStateUpdated += Gateway_VoiceStateUpdated;

            Gateway.RelationShipAdded += Gateway_RelationShipAdded;
            Gateway.RelationShipRemoved += Gateway_RelationShipRemoved;
            Gateway.RelationShipUpdated += Gateway_RelationShipUpdated;

            Gateway.SessionReplaced += Gateway_SessionReplaced;
            Gateway.InvalidSession += Gateway_InvalidSession;
            Gateway.GatewayClosed += Gateway_GatewayClosed;

            if (await ConnectWithRetryAsync(3))
            {
                _analyticsService.Log(Constants.Analytics.Events.Connected);
                Messenger.Default.Send(new ConnectionStatusMessage(ConnectionStatus.Connected));
                Messenger.Default.Register<ChannelNavigateMessage>(this, async m =>
                {
                    if (!m.Guild.IsDM)
                    {
                        await Gateway.SubscribeToGuildLazy(
                            m.Channel.GuildId,
                            new Dictionary<string, IEnumerable<int[]>>
                            {
                                {
                                    m.Channel.Model.Id,
                                    new List<int[]>
                                    {
                                        new[] { 0, 99 },
                                    }
                                },
                            });
                    }
                });
                Messenger.Default.Register<GuildNavigateMessage>(this, async m =>
                {
                    if (!m.Guild.IsDM)
                    {
                        if (previousGuildId != null)
                        {
                            await Gateway.SubscribeToGuildLazy(
                                previousGuildId,
                                new Dictionary<string, IEnumerable<int[]>> { });
                        }

                        previousGuildId = m.Guild.Model.Id;
                    }
                    else
                    {
                        previousGuildId = null;
                    }
                });
                Messenger.Default.Register<GatewayRequestGuildMembersMessage>(this, async m =>
                {
                    await Gateway.RequestGuildMembers(m.GuildIds, m.Query, m.Limit, m.Presences, m.UserIds);
                });
                Messenger.Default.Register<GatewayUpdateGuildSubscriptionsMessage>(this, async m =>
                {
                    await Gateway.SubscribeToGuildLazy(m.GuildId, m.Channels, m.Members);
                });
            }

            return true;
        }

        private async Task<bool> ConnectWithRetryAsync(int retries)
        {
            for (int i = 0; i < retries; i++)
            {
                _analyticsService.Log(Constants.Analytics.Events.ConnectionAttempt, ("attempt", (i + 1).ToString()));
                if (await Gateway.ConnectAsync())
                {
                    return true;
                }
            }

            return false;
        }

        private void Gateway_Ready(object sender, GatewayEventArgs<Ready> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayReadyMessage(e.EventData));
        }

        private void Gateway_InvalidSession(object sender, GatewayEventArgs<InvalidSession> e)
        {
            Messenger.Default.Send(new GatewayInvalidSessionMessage(e.EventData));
            _analyticsService.Log(Constants.Analytics.Events.InvalidSession);
        }

        private void Gateway_MessageCreated(object sender, GatewayEventArgs<Message> e)
        {
            var currentUser = _currentUserService.CurrentUser.Model;

            if (e.EventData.User == null)
            {
                e.EventData.User = currentUser;
            }

            Messenger.Default.Send(new GatewayMessageRecievedMessage(e.EventData));
        }

        private void Gateway_MessageDeleted(object sender, GatewayEventArgs<MessageDelete> e)
        {
            Messenger.Default.Send(new GatewayMessageDeletedMessage(e.EventData.ChannelId, e.EventData.MessageId));
        }

        private void Gateway_MessageUpdated(object sender, GatewayEventArgs<Message> e)
        {
            Messenger.Default.Send(new GatewayMessageUpdatedMessage(e.EventData));
        }

        private void Gateway_MessageAck(object sender, GatewayEventArgs<MessageAck> e)
        {
            Messenger.Default.Send(new GatewayMessageAckMessage(e.EventData.ChannelId, e.EventData.Id));
        }

        private void Gateway_MessageReactionAdded(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            Messenger.Default.Send(new GatewayReactionAddedMessage(e.EventData.MessageId, e.EventData.ChannelId, e.EventData.Emoji, e.EventData.UserId));
        }

        private void Gateway_MessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            Messenger.Default.Send(new GatewayReactionRemovedMessage(e.EventData.MessageId, e.EventData.ChannelId, e.EventData.Emoji));
        }

        private void Gateway_MessageReactionRemovedAll(object sender, GatewayEventArgs<MessageReactionRemoveAll> e)
        {
            Messenger.Default.Send(new GatewayReactionClearedMessage(e.EventData.MessageId, e.EventData.ChannelId));
        }

        private void Gateway_ChannelCreated(object sender, GatewayEventArgs<Channel> e)
        {
            Messenger.Default.Send(new GatewayChannelCreatedMessage(e.EventData));
        }

        private void Gateway_ChannelDeleted(object sender, GatewayEventArgs<Channel> e)
        {
            Messenger.Default.Send(new GatewayChannelDeletedMessage(e.EventData));
        }

        private void Gateway_GuildChannelUpdated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            Messenger.Default.Send(new GatewayGuildChannelUpdatedMessage(e.EventData));
        }

        private void Gateway_DirectMessageChannelCreated(object sender, GatewayEventArgs<DirectMessageChannel> e)
        {
            Messenger.Default.Send(new GatewayDirectMessageChannelCreatedMessage(e.EventData));
        }

        private void Gateway_GuildCreated(object sender, GatewayEventArgs<Guild> e)
        {
            Messenger.Default.Send(new GatewayGuildCreatedMessage(e.EventData));
        }

        private void Gateway_GuildDeleted(object sender, GatewayEventArgs<GuildDelete> e)
        {
            Messenger.Default.Send(new GatewayGuildDeletedMessage(e.EventData));
        }

        private void Gateway_GuildUpdated(object sender, GatewayEventArgs<Guild> e)
        {
            Messenger.Default.Send(new GatewayGuildUpdatedMessage(e.EventData));
        }

        private void Gateway_TypingStarted(object sender, GatewayEventArgs<TypingStart> e)
        {
            Messenger.Default.Send(new GatewayTypingStartedMessage(e.EventData));
        }

        private void GatewayGuildMembersChunk(object sender, GatewayEventArgs<GuildMembersChunk> e)
        {
            Messenger.Default.Send(new GatewayGuildMembersChunkMessage(e.EventData));
        }

        private void Gateway_GuildMemberListUpdated(object sender, GatewayEventArgs<GuildMemberListUpdated> e)
        {
            Messenger.Default.Send(new GatewayGuildMemberListUpdatedMessage(e.EventData));
        }

        private void Gateway_GuildSynced(object sender, GatewayEventArgs<GuildSync> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayGuildSyncMessage(e.EventData.GuildId, e.EventData.Members.ToList()));
        }

        private void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
        {
            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(e.EventData.User.Id, e.EventData));
        }

        private void Gateway_UserNoteUpdated(object sender, GatewayEventArgs<UserNote> e)
        {
            _cacheService.Runtime.SetValue(Constants.Cache.Keys.Note, e.EventData.Note, e.EventData.UserId);
            Messenger.Default.Send(new GatewayNoteUpdatedMessage(e.EventData.UserId, e.EventData.Note));
        }

        private void Gateway_UserGuildSettingsUpdated(object sender, GatewayEventArgs<GuildSetting> e)
        {
            _guildsService.AddOrUpdateGuildSettings(e.EventData.GuildId ?? "DM", e.EventData);

            foreach (var channel in e.EventData.ChannelOverrides)
            {
                _channelsService.AddOrUpdateChannelSettings(channel.ChannelId, channel);
            }

            Messenger.Default.Send(new GatewayUserGuildSettingsUpdatedMessage(e.EventData));
        }

        private void Gateway_UserSettingsUpdated(object sender, GatewayEventArgs<UserSettings> e)
        {
            Messenger.Default.Send(new GatewayUserSettingsUpdatedMessage(e.EventData));
        }

        private void Gateway_VoiceServerUpdated(object sender, GatewayEventArgs<VoiceServerUpdate> e)
        {
            Messenger.Default.Send(new GatewayVoiceServerUpdateMessage(e.EventData));
        }

        private void Gateway_VoiceStateUpdated(object sender, GatewayEventArgs<VoiceState> e)
        {
            Messenger.Default.Send(new GatewayVoiceStateUpdateMessage(e.EventData));
        }

        private void Gateway_RelationShipAdded(object sender, GatewayEventArgs<Friend> e)
        {
            Messenger.Default.Send(new GatewayRelationshipAddedMessage(e.EventData));
        }

        private void Gateway_RelationShipRemoved(object sender, GatewayEventArgs<Friend> e)
        {
            Messenger.Default.Send(new GatewayRelationshipRemovedMessage(e.EventData));
        }

        private void Gateway_RelationShipUpdated(object sender, GatewayEventArgs<Friend> e)
        {
            Messenger.Default.Send(new GatewayRelationshipUpdatedMessage(e.EventData));
        }

        private void Gateway_SessionReplaced(object sender, GatewayEventArgs<SessionReplace[]> e)
        {
            Messenger.Default.Send(new GatewaySessionReplacedMessage(e.EventData));
        }

        private void Gateway_GatewayClosed(object sender, Exception e)
        {
            _analyticsService.Log(Constants.Analytics.Events.Disconnected, (nameof(Exception), e.Message));
            if (e is WebSocketClosedException ex && ex.Reason == "Authentication failed.")
            {
                Messenger.Default.Send(new GatewayInvalidSessionMessage(new InvalidSession { ConnectedState = false }));
            }
            else
            {
                Messenger.Default.Send(new ConnectionStatusMessage(ConnectionStatus.Disconnected));
            }
        }
    }
}
