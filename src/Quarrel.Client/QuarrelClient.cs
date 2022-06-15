// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.Gateways;
using Discord.API.Rest;
using Discord.API.Sockets;
using Discord.API.Voice;
using Refit;
using System;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    /// <summary>
    /// A Discord client instance containing a Token, Gateway, and Cache.
    /// </summary>
    public partial class QuarrelClient
    {
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
            Voice = new QuarrelClientVoice(this);
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
        /// Gets the client's <see cref="QuarrelClientUsers"/>.
        /// </summary>
        public QuarrelClientUsers Users { get; }

        /// <summary>
        /// Gets the client's <see cref="QuarrelClientVoice"/>.
        /// </summary>
        public QuarrelClientVoice Voice { get; }

        /// <summary>
        /// Gets the token used for authentication.
        /// </summary>
        public string? Token { get; private set; }

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
            Token = token;
            InitializeServices(token);
            if (Gateway == null)
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
                unhandledMessageEncountered: OnUnhandledGatewayMessageEncountered,
                unknownEventEncountered: OnUnknownGatewayEventEncountered,
                unknownOperationEncountered: OnUnknownGatewayOperationEncountered,
                knownEventEncountered: OnKnownGatewayEventEncountered,
                unhandledOperationEncountered: OnUnhandledGatewayOperationEncountered,
                unhandledEventEncountered: OnUnhandledGatewayEventEncountered,

                ready: OnReady,
                messageCreated: OnMessageCreated,
                messageUpdated: OnMessageUpdated,
                messageDeleted: OnMessageDeleted,
                messageAck: OnMessageAck,

                resumed: _ => { },
                invalidSession: _ => { },
                gatewayStatusChanged: OnGatewayStateChanged,

                guildCreated: _ => { },
                guildUpdated: _ => { },
                guildDeleted: _ => { },

                guildBanAdded: _ => { },
                guildBanRemoved: _ => { },

                channelCreated: OnChannelCreated,
                channelUpdated: OnChannelUpdated,
                channelDeleted: OnChannelDeleted,

                channelRecipientAdded: _ => { },
                channelRecipientRemoved: _ => { },

                messageReactionAdded: _ => { },
                messageReactionRemoved: _ => { },
                messageReactionRemovedAll: _ => { },

                guildMemberAdded: _ => { },
                guildMemberUpdated: _ => { },
                guildMemberRemoved: _ => { },
                guildMemberListUpdated: _ => { },
                guildMembersChunk: _ => { },

                relationshipAdded: _ => { },
                relationshipUpdated: _ => { },
                relationshipRemoved: _ => { },

                typingStarted: _ => { },
                presenceUpdated: _ => { },

                userNoteUpdated: _ => { },
                userSettingsUpdated: _ => { },
                userGuildSettingsUpdated: _ => { },

                voiceStateUpdated: _ => { },
                voiceServerUpdated: OnVoiceServerUpdated,

                sessionReplaced: _ => { });
        }

        private void OnGatewayStateChanged(GatewayStatus newState)
        {
            switch (newState)
            {
                case GatewayStatus.Resuming:
                    Resuming?.Invoke();
                    break;

                case GatewayStatus.Reconnecting:
                    Reconnecting?.Invoke();
                    break;

                case GatewayStatus.Disconnected:
                    LoggedOut?.Invoke();
                    break;
            }
        }

        private void OnUnhandledGatewayMessageEncountered(SocketFrameException e)
            => GatewayExceptionHandled?.Invoke(this, e);

        private void OnUnknownGatewayEventEncountered(string e)
            => UnknownGatewayEventEncountered?.Invoke(this, e);

        private void OnUnknownGatewayOperationEncountered(int e)
            => UnknownGatewayOperationEncountered?.Invoke(this, e);

        private void OnKnownGatewayEventEncountered(string e)
            => KnownGatewayEventEncountered?.Invoke(this, e);

        private void OnUnhandledGatewayOperationEncountered(GatewayOperation e)
            => UnhandledGatewayOperationEncountered?.Invoke(this, (int)e);

        private void OnUnhandledGatewayEventEncountered(GatewayEvent e)
            => UnhandledGatewayEventEncountered?.Invoke(this, e.ToString());

        private void OnUnhandledVoiceMessageEncountered(SocketFrameException e)
            => VoiceExceptionHandled?.Invoke(this, e);

        private void OnUnknownVoiceEventEncountered(string e)
            => UnknownVoiceEventEncountered?.Invoke(this, e);

        private void OnUnknownVoiceOperationEncountered(int e)
            => UnknownVoiceOperationEncountered?.Invoke(this, e);

        private void OnKnownVoiceEventEncountered(string e)
            => KnownVoiceEventEncountered?.Invoke(this, e);

        private void OnUnhandledVoiceOperationEncountered(VoiceOperation e)
            => UnhandledVoiceOperationEncountered?.Invoke(this, (int)e);

        private void OnUnhandledVoiceEventEncountered(VoiceEvent e)
            => UnhandledVoiceEventEncountered?.Invoke(this, e.ToString());

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
