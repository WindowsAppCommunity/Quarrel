// Quarrel © 2022
using Discord.API.Exceptions;
using Discord.API.Gateways.Models;
using Discord.API.Gateways.Models.Channels;
using Discord.API.Gateways.Models.GuildMember;
using Discord.API.Gateways.Models.Guilds;
using Discord.API.Gateways.Models.Handshake;
using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Guilds;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Json.Settings;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Json.Voice;
using System;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private Action<Ready> Ready { get; }
        private Action<Resumed> Resumed { get; }
        private Action<InvalidSession> InvalidSession { get; }

        private Action<JsonGuild> GuildCreated { get; }
        private Action<JsonGuild> GuildUpdated { get; }
        private Action<GuildDeleted> GuildDeleted { get; }

        private Action<GuildBanUpdate> GuildBanAdded { get; }
        private Action<GuildBanUpdate> GuildBanRemoved { get; }

        private Action<JsonChannel> ChannelCreated { get; }
        private Action<JsonChannel> ChannelUpdated { get; }
        private Action<JsonChannel> ChannelDeleted { get; }

        private Action<ChannelRecipientUpdate> ChannelRecipientAdded { get; }
        private Action<ChannelRecipientUpdate> ChannelRecipientRemoved { get; }

        private Action<JsonMessageAck> MessageAck { get; }
        private Action<JsonMessage> MessageCreated { get; }
        private Action<JsonMessage> MessageUpdated { get; }
        private Action<JsonMessageDeleted> MessageDeleted { get; }

        private Action<MessageReactionUpdated> MessageReactionAdded { get; }
        private Action<MessageReactionUpdated> MessageReactionRemoved { get; }
        private Action<MessageReactionRemoveAll> MessageReactionRemovedAll { get; }

        private Action<JsonGuildMember> GuildMemberAdded { get; }
        private Action<JsonGuildMember> GuildMemberUpdated { get; }
        private Action<GuildMemberRemoved> GuildMemberRemoved { get; }
        private Action<GuildMemberListUpdated> GuildMemberListUpdated { get; }
        private Action<GuildMembersChunk> GuildMembersChunk { get; }

        private Action<JsonRelationship> RelationshipAdded { get; }
        private Action<JsonRelationship> RelationshipUpdated { get; }
        private Action<JsonRelationship> RelationshipRemoved { get; }

        private Action<TypingStart> TypingStarted { get; }
        private Action<JsonPresence> PresenceUpdated { get; }

        private Action<UserNote> UserNoteUpdated { get; }
        private Action<JsonUserSettings> UserSettingsUpdated { get; }
        private Action<JsonGuildSettings> UserGuildSettingsUpdated { get; }

        private Action<JsonVoiceState> VoiceStateUpdated { get; }
        private Action<VoiceServerUpdate> VoiceServerUpdated { get; }

        private Action<SessionReplace[]> SessionReplaced { get; }

        
        private static bool FireEvent<T>(SocketFrame frame, Action<T> eventHandler)
        {
            var eventArgs = ((SocketFrame<T>)frame).Payload;
            eventHandler(eventArgs);
            return true;
        }

        public static bool FireEvent<T>(T data, Action<T> eventHandler)
        {
            eventHandler(data);
            return true;
        }
        private bool OnReady(SocketFrame<Ready> frame)
        {
            var ready = frame.Payload;

            _sessionId = ready.SessionId;
            FireEvent(frame, Ready);
            return true;
        }

        protected override void ProcessEvents(SocketFrame frame)
        {
            bool succeeded = frame switch
            {
                UnknownOperationSocketFrame osf => FireEvent(osf.Operation, UnknownOperationEncountered),
                UnknownEventSocketFrame osf => FireEvent(osf.Event, UnknownEventEncountered),
                _ => frame.Operation switch
                {
                    GatewayOperation.Hello => OnHelloReceived((SocketFrame<Hello>)frame),
                    GatewayOperation.InvalidSession => OnInvalidSession(frame),
                    GatewayOperation.HeartbeatAck => OnHeartbeatAck(),
                    GatewayOperation.Dispatch => FireEvent(frame.Event.ToString(), KnownEventEncountered) && frame.Event switch
                    {
                        GatewayEvent.READY => OnReady((SocketFrame<Ready>)frame),
                        GatewayEvent.RESUMED => FireEvent(frame, Resumed),

                        GatewayEvent.GUILD_CREATE => FireEvent(frame, GuildCreated),
                        GatewayEvent.GUILD_UPDATE => FireEvent(frame, GuildUpdated),
                        GatewayEvent.GUILD_DELETE => FireEvent(frame, GuildDeleted),

                        GatewayEvent.GUILD_BAN_ADD => FireEvent(frame, GuildBanAdded),
                        GatewayEvent.GUILD_BAN_REMOVE => FireEvent(frame, GuildBanRemoved),

                        GatewayEvent.CHANNEL_CREATE => FireEvent(frame, ChannelCreated),
                        GatewayEvent.CHANNEL_UPDATE => FireEvent(frame, ChannelUpdated),
                        GatewayEvent.CHANNEL_DELETE => FireEvent(frame, ChannelDeleted),

                        GatewayEvent.CHANNEL_RECIPIENT_ADD => FireEvent(frame, ChannelRecipientAdded),
                        GatewayEvent.CHANNEL_RECIPIENT_REMOVE => FireEvent(frame, ChannelRecipientRemoved),

                        GatewayEvent.MESSAGE_ACK => FireEvent(frame, MessageAck),
                        GatewayEvent.MESSAGE_CREATE => FireEvent(frame, MessageCreated),
                        GatewayEvent.MESSAGE_UPDATE => FireEvent(frame, MessageUpdated),
                        GatewayEvent.MESSAGE_DELETE => FireEvent(frame, MessageDeleted),

                        GatewayEvent.MESSAGE_REACTION_ADD => FireEvent(frame, MessageReactionAdded),
                        GatewayEvent.MESSAGE_REACTION_REMOVE => FireEvent(frame, MessageReactionRemoved),
                        GatewayEvent.MESSAGE_REACTION_REMOVE_ALL => FireEvent(frame, MessageReactionRemovedAll),

                        GatewayEvent.GUILD_MEMBER_ADD => FireEvent(frame, GuildMemberAdded),
                        GatewayEvent.GUILD_MEMBER_UPDATE => FireEvent(frame, GuildMemberUpdated),
                        GatewayEvent.GUILD_MEMBER_REMOVE => FireEvent(frame, GuildMemberRemoved),
                        GatewayEvent.GUILD_MEMBER_LIST_UPDATE => FireEvent(frame, GuildMemberListUpdated),
                        GatewayEvent.GUILD_MEMBERS_CHUNK => FireEvent(frame, GuildMembersChunk),

                        GatewayEvent.RELATIONSHIP_ADD => FireEvent(frame, RelationshipAdded),
                        GatewayEvent.RELATIONSHIP_UPDATE => FireEvent(frame, RelationshipUpdated),
                        GatewayEvent.RELATIONSHIP_REMOVE => FireEvent(frame, RelationshipRemoved),

                        GatewayEvent.TYPING_START => FireEvent(frame, TypingStarted),
                        GatewayEvent.PRESENCE_UPDATE => FireEvent(frame, PresenceUpdated),

                        GatewayEvent.USER_NOTE_UPDATE => FireEvent(frame, UserNoteUpdated),
                        GatewayEvent.USER_SETTINGS_UPDATE => FireEvent(frame, UserSettingsUpdated),
                        GatewayEvent.USER_GUILD_SETTINGS_UPDATE => FireEvent(frame, UserGuildSettingsUpdated),

                        GatewayEvent.VOICE_STATE_UPDATE => FireEvent(frame, VoiceStateUpdated),
                        GatewayEvent.VOICE_SERVER_UPDATE => FireEvent(frame, VoiceServerUpdated),

                        GatewayEvent.SESSIONS_REPLACE => FireEvent(frame, SessionReplaced),

                        null => FireEvent(new SocketFrameException("Received null event despite dispatch opcode.", (int?)frame.Operation, frame.Event.ToString()), UnhandledMessageEncountered),
                        _ => FireEvent(frame.Event.Value, UnhandledEventEncountered),
                    },
                    _ => FireEvent(frame.Operation, UnhandledOperationEncountered),
                }
            };
            if (!succeeded) FireEvent(new SocketFrameException("Failed to handle socket frame.", (int?)frame.Operation, frame.Event.ToString()), UnhandledMessageEncountered);
        }
    }
}
