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
        public event EventHandler<SocketFrameException?>? UnhandledMessageEncountered;
        public event EventHandler<string>? UnknownEventEncountered;
        public event EventHandler<int>? UnknownOperationEncountered;
        public event EventHandler<string>? KnownEventEncountered;
        public event EventHandler<GatewayOperation>? UnhandledOperationEncountered;
        public event EventHandler<GatewayEvent>? UnhandledEventEncountered;

        public event EventHandler<GatewayEventArgs<Ready>>? Ready;
        public event EventHandler<GatewayEventArgs<Resumed>>? Resumed;
        public event EventHandler<GatewayEventArgs<InvalidSession>>? InvalidSession;
        public event EventHandler<Exception>? GatewayClosed;

        public event EventHandler<GatewayEventArgs<JsonGuild>>? GuildCreated;
        public event EventHandler<GatewayEventArgs<JsonGuild>>? GuildUpdated;
        public event EventHandler<GatewayEventArgs<GuildDeleted>>? GuildDeleted;

        public event EventHandler<GatewayEventArgs<GuildBanUpdate>>? GuildBanAdded;
        public event EventHandler<GatewayEventArgs<GuildBanUpdate>>? GuildBanRemoved;

        public event EventHandler<GatewayEventArgs<JsonChannel>>? ChannelCreated;
        public event EventHandler<GatewayEventArgs<JsonChannel>>? ChannelUpdated;
        public event EventHandler<GatewayEventArgs<JsonChannel>>? ChannelDeleted;

        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>>? ChannelRecipientAdded;
        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>>? ChannelRecipientRemoved;

        public event EventHandler<GatewayEventArgs<JsonMessageAck>>? MessageAck;
        public event EventHandler<GatewayEventArgs<JsonMessage>>? MessageCreated;
        public event EventHandler<GatewayEventArgs<JsonMessage>>? MessageUpdated;
        public event EventHandler<GatewayEventArgs<JsonMessageDeleted>>? MessageDeleted;

        public event EventHandler<GatewayEventArgs<MessageReactionUpdated>>? MessageReactionAdded;
        public event EventHandler<GatewayEventArgs<MessageReactionUpdated>>? MessageReactionRemoved;
        public event EventHandler<GatewayEventArgs<MessageReactionRemoveAll>>? MessageReactionRemovedAll;

        public event EventHandler<GatewayEventArgs<JsonGuildMember>>? GuildMemberAdded;
        public event EventHandler<GatewayEventArgs<JsonGuildMember>>? GuildMemberUpdated;
        public event EventHandler<GatewayEventArgs<GuildMemberRemoved>>? GuildMemberRemoved;
        public event EventHandler<GatewayEventArgs<GuildMemberListUpdated>>? GuildMemberListUpdated;
        public event EventHandler<GatewayEventArgs<GuildMembersChunk>>? GuildMembersChunk;

        public event EventHandler<GatewayEventArgs<JsonRelationship>>? RelationshipAdded;
        public event EventHandler<GatewayEventArgs<JsonRelationship>>? RelationshipUpdated;
        public event EventHandler<GatewayEventArgs<JsonRelationship>>? RelationshipRemoved;

        public event EventHandler<GatewayEventArgs<TypingStart>>? TypingStarted;
        public event EventHandler<GatewayEventArgs<JsonPresence>>? PresenceUpdated;

        public event EventHandler<GatewayEventArgs<UserNote>>? UserNoteUpdated;
        public event EventHandler<GatewayEventArgs<JsonUserSettings>>? UserSettingsUpdated;
        public event EventHandler<GatewayEventArgs<JsonGuildSettings>>? UserGuildSettingsUpdated;

        public event EventHandler<GatewayEventArgs<JsonVoiceState>>? VoiceStateUpdated;
        public event EventHandler<GatewayEventArgs<VoiceServerUpdate>>? VoiceServerUpdated;

        public event EventHandler<GatewayEventArgs<SessionReplace[]>>? SessionReplaced;

        private bool FireEvent<T>(SocketFrame frame, EventHandler<GatewayEventArgs<T>>? eventHandler)
        {
            var eventArgs = new GatewayEventArgs<T>(((SocketFrame<T>)frame).Payload);
            eventHandler?.Invoke(this, eventArgs);
            return true;
        }
        private bool FireEvent<T>(T data, EventHandler<T>? eventHandler)
        {
            eventHandler?.Invoke(this, data);
            return true;
        }
        
        private bool OnReady(SocketFrame<Ready> frame)
        {
            var ready = frame.Payload;

            _sessionId = ready.SessionId;
            FireEvent(frame, Ready);
            return true;
        }
        
        private bool OnHeartbeatAck()
        {
            // TODO: Wait for ack after heartbeat.
            return true;
        }

        private void ProcessEvents(SocketFrame frame)
        {

            bool suceeded = frame switch
            {
                UnknownOperationSocketFrame osf => FireEvent(osf.Operation, UnknownOperationEncountered),
                UnknownEventSocketFrame osf => FireEvent(osf.Event, UnknownEventEncountered),
                _ => frame.Operation switch {
                    GatewayOperation.Hello => OnHelloReceived((SocketFrame<Hello>)frame),
                    GatewayOperation.InvalidSession => OnInvalidSession(frame),
                    GatewayOperation.Dispatch => FireEvent(frame.Event.ToString(), KnownEventEncountered) && frame.Event switch {
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
            if (!suceeded) FireEvent(new SocketFrameException("Failed to handle socket frame.", (int?)frame.Operation, frame.Event.ToString()), UnhandledMessageEncountered);
        }
    }
}
