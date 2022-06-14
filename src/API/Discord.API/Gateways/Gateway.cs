// Quarrel © 2022

using Discord.API.Exceptions;
using Discord.API.Gateways.Models;
using Discord.API.Gateways.Models.Channels;
using Discord.API.Gateways.Models.GuildMember;
using Discord.API.Gateways.Models.Guilds;
using Discord.API.Gateways.Models.Handshake;
using Discord.API.Gateways.Models.Messages;
using Discord.API.JsonConverters;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Gateway;
using Discord.API.Models.Json.Guilds;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Json.Settings;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Json.Voice;
using Discord.API.Sockets;
using System;
using System.Text.Json;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private readonly GatewayConfig _gatewayConfig;
        private string? _token;

        private GatewayStatus _gatewayStatus;

        private GatewayStatus GatewayStatus
        {
            get => _gatewayStatus;
            set
            {
                _gatewayStatus = value;
                GatewayStatusChanged(_gatewayStatus);
            }
        }
        
        private string? _connectionUrl;
        private string? _sessionId;
        private int _lastEventSequenceNumber;
        public Gateway(GatewayConfig config,
            Action<SocketFrameException> unhandledMessageEncountered,
            Action<string> unknownEventEncountered,
            Action<int> unknownOperationEncountered,
            Action<string> knownEventEncountered,
            Action<GatewayOperation> unhandledOperationEncountered,
            Action<GatewayEvent> unhandledEventEncountered,
            Action<Ready> ready,
            Action<Resumed> resumed,
            Action<InvalidSession> invalidSession,
            Action<GatewayStatus> gatewayStatusChanged,
            Action<JsonGuild> guildCreated,
            Action<JsonGuild> guildUpdated,
            Action<GuildDeleted> guildDeleted,
            Action<GuildBanUpdate> guildBanAdded,
            Action<GuildBanUpdate> guildBanRemoved,
            Action<JsonChannel> channelCreated,
            Action<JsonChannel> channelUpdated,
            Action<JsonChannel> channelDeleted,
            Action<ChannelRecipientUpdate> channelRecipientAdded,
            Action<ChannelRecipientUpdate> channelRecipientRemoved,
            Action<JsonMessageAck> messageAck,
            Action<JsonMessage> messageCreated,
            Action<JsonMessage> messageUpdated,
            Action<JsonMessageDeleted> messageDeleted,
            Action<MessageReactionUpdated> messageReactionAdded,
            Action<MessageReactionUpdated> messageReactionRemoved,
            Action<MessageReactionRemoveAll> messageReactionRemovedAll,
            Action<JsonGuildMember> guildMemberAdded,
            Action<JsonGuildMember> guildMemberUpdated,
            Action<GuildMemberRemoved> guildMemberRemoved,
            Action<GuildMemberListUpdated> guildMemberListUpdated,
            Action<GuildMembersChunk> guildMembersChunk,
            Action<JsonRelationship> relationshipAdded,
            Action<JsonRelationship> relationshipUpdated,
            Action<JsonRelationship> relationshipRemoved,
            Action<TypingStart> typingStarted,
            Action<JsonPresence> presenceUpdated,
            Action<UserNote> userNoteUpdated,
            Action<JsonUserSettings> userSettingsUpdated,
            Action<JsonGuildSettings> userGuildSettingsUpdated,
            Action<JsonVoiceState> voiceStateUpdated,
            Action<JsonVoiceServerUpdate> voiceServerUpdated,
            Action<SessionReplace[]> sessionReplaced)
        {
            _gatewayConfig = config;

            SessionReplaced = sessionReplaced;
            VoiceServerUpdated = voiceServerUpdated;
            VoiceStateUpdated = voiceStateUpdated;
            UserGuildSettingsUpdated = userGuildSettingsUpdated;
            UserSettingsUpdated = userSettingsUpdated;
            UserNoteUpdated = userNoteUpdated;
            PresenceUpdated = presenceUpdated;
            TypingStarted = typingStarted;
            RelationshipRemoved = relationshipRemoved;
            RelationshipUpdated = relationshipUpdated;
            RelationshipAdded = relationshipAdded;
            GuildMembersChunk = guildMembersChunk;
            GuildMemberListUpdated = guildMemberListUpdated;
            GuildMemberRemoved = guildMemberRemoved;
            GuildMemberUpdated = guildMemberUpdated;
            GuildMemberAdded = guildMemberAdded;
            MessageReactionRemovedAll = messageReactionRemovedAll;
            MessageReactionRemoved = messageReactionRemoved;
            MessageReactionAdded = messageReactionAdded;
            MessageDeleted = messageDeleted;
            MessageUpdated = messageUpdated;
            MessageCreated = messageCreated;
            MessageAck = messageAck;
            ChannelRecipientRemoved = channelRecipientRemoved;
            ChannelRecipientAdded = channelRecipientAdded;
            ChannelDeleted = channelDeleted;
            ChannelUpdated = channelUpdated;
            ChannelCreated = channelCreated;
            GuildBanRemoved = guildBanRemoved;
            GuildBanAdded = guildBanAdded;
            GuildDeleted = guildDeleted;
            GuildUpdated = guildUpdated;
            GuildCreated = guildCreated;
            InvalidSession = invalidSession;
            Resumed = resumed;
            Ready = ready;
            
            GatewayStatusChanged = gatewayStatusChanged;
            UnhandledEventEncountered = unhandledEventEncountered;
            UnhandledOperationEncountered = unhandledOperationEncountered;
            KnownEventEncountered = knownEventEncountered;
            UnknownOperationEncountered = unknownOperationEncountered;
            UnknownEventEncountered = unknownEventEncountered;
            UnhandledMessageEncountered = unhandledMessageEncountered;

            _gatewayStatus = GatewayStatus.Initialized;

            _serializeOptions = new JsonSerializerOptions();
            _serializeOptions.AddContext<JsonModelsContext>();

            _deserializeOptions = new JsonSerializerOptions { Converters = { new GatewaySocketFrameConverter() } };
        }
    }
}
