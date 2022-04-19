﻿// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Rest;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Users;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    /// <summary>
    /// A Discord client instance containing a Token, Gateway, and Cache.
    /// </summary>
    public partial class QuarrelClient
    {
        private IChannelService? _channelService;
        private IGatewayService? _gatewayService;
        private Gateway? _gateway;
        private string? _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuarrelClient"/> class.
        /// </summary>
        public QuarrelClient()
        {
            _guildMap = new ConcurrentDictionary<ulong, Guild>();
            _channelMap = new ConcurrentDictionary<ulong, Channel>();
            _userMap = new ConcurrentDictionary<ulong, User>();
            _guildsMemberMap = new ConcurrentDictionary<(ulong GuildId, ulong UserId), GuildMember>();
        }

        /// <summary>
        /// Gets the token used for authentication.
        /// </summary>
        public string? Token => _token;

        /// <summary>
        /// Initializes authenticated services and opens the gateway.
        /// </summary>
        /// <param name="token">The token used for authentication.</param>
        /// <exception cref="Exception">An exception will be thrown when connection to the gateway fails, but not when the handshake fails.</exception>
        public async Task LoginAsync(string token)
        {
            _token = token;
            InitializeServices(token);
            await SetupGatewayAsync(token);
        }

        private void InitializeServices(string token)
        {
            var restFactory = new DiscordRestFactory
            {
                Token = token
            };
            _channelService = restFactory.GetChannelService();
            _gatewayService = restFactory.GetGatewayService();
        }

        private async Task SetupGatewayAsync(string token)
        {
            Guard.IsNotNull(_gatewayService, nameof(_gatewayService));
            var gatewayConfig = await MakeRefitRequest(() => _gatewayService.GetGatewayConfig());
            Guard.IsNotNull(gatewayConfig, nameof(_gatewayService));
            _gateway = new Gateway(gatewayConfig, token,
                unhandledMessageEncountered: (e) => GatewayExceptionHandled?.Invoke(this, e),
                unknownEventEncountered: e => UnknownGatewayEventEncountered?.Invoke(this, e),
                unknownOperationEncountered: e => UnknownGatewayOperationEncountered?.Invoke(this, e),
                knownEventEncountered: e => KnownGatewayEventEncountered?.Invoke(this, e),
                unhandledOperationEncountered: e => UnhandledGatewayOperationEncountered?.Invoke(this, (int)e),
                unhandledEventEncountered: e => UnhandledGatewayEventEncountered?.Invoke(this, e.ToString()),

                ready: OnReady,
                messageCreated: OnMessageCreated,
                messageUpdated: OnMessageUpdated,
                messageDeleted: e => MessageDeleted?.Invoke(this, new MessageDeleted(e, this)),
                messageAck: OnMessageAck,

                resumed: e => { },
                invalidSession: e => { },
                gatewayClosed: e => { },

                guildCreated: e => { },
                guildUpdated: e => { },
                guildDeleted: e => { },

                guildBanAdded: e => { },
                guildBanRemoved: e => { },

                channelCreated: e => { },
                channelUpdated: e => { },
                channelDeleted: e => { },

                channelRecipientAdded: e => { },
                channelRecipientRemoved: e => { },

                messageReactionAdded: e => { },
                messageReactionRemoved: e => { },
                messageReactionRemovedAll: e => { },

                guildMemberAdded: e => { },
                guildMemberUpdated: e => { },
                guildMemberRemoved: e => { },
                guildMemberListUpdated: e => { },
                guildMembersChunk: e => { },

                relationshipAdded: e => { },
                relationshipUpdated: e => { },
                relationshipRemoved: e => { },

                typingStarted: e => { },
                presenceUpdated: e => { },

                userNoteUpdated: e => { },
                userSettingsUpdated: e => { },
                userGuildSettingsUpdated: e => { },

                voiceStateUpdated: e => { },
                voiceServerUpdated: e => { },

                sessionReplaced: e => { });
            await _gateway.ConnectAsync();
        }
    }
}