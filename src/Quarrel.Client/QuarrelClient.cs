// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Rest;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Users;
using Refit;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quarrel.Client
{
    /// <summary>
    /// A Discord client instance containing a Token, Gateway, and Cache.
    /// </summary>
    public partial class QuarrelClient
    {
        private string? _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuarrelClient"/> class.
        /// </summary>
        public QuarrelClient()
        {
            Channels = new QuarrelClientChannels(this);
            Guilds = new QuarrelClientGuilds(this);
            Members = new QuarrelClientMembers(this);
            Messages = new QuarrelClientMessages(this);
            Self = new QuarrelClientSelf(this);
            Users = new QuarrelClientUsers(this);
        }

        /// <summary>
        /// Gets the client's <see cref="QuarrelClientChannels"/>.
        /// </summary>
        public QuarrelClientChannels Channels { get; }

        /// <summary>
        /// Gets the client's <see cref="QuarrelClientGuilds"/>.
        /// </summary>
        public QuarrelClientGuilds Guilds { get; }

        /// <summary>
        /// Gets the client's <see cref="QuarrelClientMembers"/>.
        /// </summary>
        public QuarrelClientMembers Members { get; }

        /// <summary>
        /// Gets the client's <see cref="QuarrelClientMessages"/>.
        /// </summary>
        public QuarrelClientMessages Messages { get; }

        /// <summary>
        /// Gets the client's <see cref="QuarrelClientSelf"/>.
        /// </summary>
        public QuarrelClientSelf Self { get; }

        /// <summary>
        /// Gets the client's <see cref="QuarrelClientSelf"/>.
        /// </summary>
        public QuarrelClientUsers Users { get; }


        /// <summary>
        /// Gets the token used for authentication.
        /// </summary>
        public string? Token => _token;

        /// <summary>
        /// Gets the client's <see cref="Gateway"/>.
        /// </summary>
        private Gateway? Gateway { get; set; }

        private IChannelService? ChannelService { get; set; }

        private IGatewayService? GatewayService { get; set; }

        private IGuildService? GuildService { get; set; }

        private IUserService? UserService { get; set; }

        /// <summary>
        /// Initializes authenticated services and opens the gateway.
        /// </summary>
        /// <param name="token">The token used for authentication.</param>
        /// <exception cref="Exception">An exception will be thrown when connection to the gateway fails, but not when the handshake fails.</exception>
        public async Task LoginAsync(string token)
        {
            _token = token;
            InitializeServices(token);
            if(Gateway == null)
                await SetupGatewayAsync();
            await Gateway!.Connect(token);
        }

        private void InitializeServices(string token)
        {
            var restFactory = new DiscordRestFactory
            {
                Token = token
            };
            ChannelService = restFactory.GetChannelService();
            GatewayService = restFactory.GetGatewayService();
            GuildService = restFactory.GetGuildService();
            UserService = restFactory.GetUserService();
        }

        private async Task SetupGatewayAsync()
        {
            Guard.IsNotNull(GatewayService, nameof(GatewayService));
            var gatewayConfig = await MakeRefitRequest(() => GatewayService.GetGatewayConfig());
            Guard.IsNotNull(gatewayConfig, nameof(GatewayService));
            Gateway = new Gateway(gatewayConfig,
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
                gatewayStateChanged: OnGatewayStateChanged,

                guildCreated: e => { },
                guildUpdated: e => { },
                guildDeleted: e => { },

                guildBanAdded: e => { },
                guildBanRemoved: e => { },

                channelCreated: OnChannelCreated,
                channelUpdated: OnChannelUpdated,
                channelDeleted: OnChannelDeleted,

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
        }

        private void OnGatewayStateChanged(GatewayStatus newState)
        {
            switch (newState)
            {
                case GatewayStatus.Resuming:
                    Resuming?.Invoke();
                    break;

                case GatewayStatus.Reconnecting:
                    Resuming?.Invoke();
                    break;

                case GatewayStatus.Disconnected:
                    LoggedOut?.Invoke();
                    break;
            }
        }

        private async Task MakeRefitRequest(Func<Task> request)
        {
            try
            {
                await request();
            }
            catch (ApiException ex)
            {
                HttpExceptionHandled?.Invoke(this, ex);
            }
        }

        private async Task<T?> MakeRefitRequest<T>(Func<Task<T>> request)
        {
            try
            {
                return await request();
            }
            catch (ApiException ex)
            {
                HttpExceptionHandled?.Invoke(this, ex);
                return default;
            }
            catch { return default; }
        }
    }
}
