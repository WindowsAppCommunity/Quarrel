// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Rest;
using Quarrel.Client.Logger;
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
        public QuarrelClient(IClientLogger logger)
        {
            Logger = logger;

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

        /// <summary>
        /// Gets the client's logger.
        /// </summary>
        public IClientLogger Logger { get; }

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
                unhandledMessageEncountered: e => LogException(ClientLogEvent.GatewayExceptionHandled, e),
                unknownEventEncountered: e => LogEvent(ClientLogEvent.UnknownGatewayEventEncountered, e),
                unknownOperationEncountered: e => LogOperation(ClientLogEvent.UnknownGatewayOperationEncountered, e),
                knownEventEncountered: e => LogEvent(ClientLogEvent.KnownGatewayEventEncountered, $"{e}"),
                unhandledOperationEncountered: e => LogOperation(ClientLogEvent.UnhandledGatewayOperationEncountered, (int)e),
                unhandledEventEncountered: e => LogEvent(ClientLogEvent.UnhandledGatewayEventEncountered, $"{e}"),

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

                voiceStateUpdated: OnVoiceStateUpdated,
                voiceServerUpdated: OnVoiceServerUpdated,

                streamCreate: OnStreamCreate,
                streamServerUpdate: OnStreamServerUpdate,

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

        internal void Log(ClientLogEvent logEvent)
        {
            Logger.Log(logEvent);
        }

        internal void LogOperation(ClientLogEvent logEvent, int op)
        {
            Logger.Log(logEvent,
                ("Operation", $"{op}"));
        }

        internal void LogEvent(ClientLogEvent logEvent, string eventName)
        {
            Logger.Log(logEvent,
                ("Event", eventName));
        }

        internal void LogException(ClientLogEvent logEvent, Exception e)
        {
            Logger.Log(logEvent,
                ("Exception Type", e.GetType().FullName),
                ("Exception Message", e.Message));
        }

        private async Task MakeRefitRequest(Func<Task> request)
        {
            try
            {
                await request();
            }
            catch (ApiException e)
            {
                LogException(ClientLogEvent.HttpExceptionHandled, e);
            }
        }

        private async Task<T?> MakeRefitRequest<T>(Func<Task<T>> request)
        {
            try
            {
                return await request();
            }
            catch (ApiException e)
            {
                LogException(ClientLogEvent.HttpExceptionHandled, e);
                return default;
            }
            catch { return default; }
        }
    }
}
