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

        public event EventHandler<GatewayEventArgs<Ready>>? Ready;
        public event EventHandler<GatewayEventArgs<Resumed>>? Resumed;
        public event EventHandler<GatewayEventArgs<InvalidSession>>? InvalidSession;
        public event EventHandler<Exception>? GatewayClosed;

        public event EventHandler<GatewayEventArgs<JsonGuild>>? GuildCreated;
        public event EventHandler<GatewayEventArgs<JsonGuild>>? GuildUpdated;
        public event EventHandler<GatewayEventArgs<GuildDeleted>>? GuildDeleted;
        public event EventHandler<GatewayEventArgs<GuildSync>>? GuildSynced;

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

        private bool FireEventOnDelegate<T>(SocketFrame frame, EventHandler<GatewayEventArgs<T>>? eventHandler)
        {
            var eventArgs = new GatewayEventArgs<T>(((SocketFrame<T>)frame).Payload);
            eventHandler?.Invoke(this, eventArgs);
            return true;
        }
        
        private bool OnReady(SocketFrame<Ready> frame)
        {
            var ready = frame.Payload;

            _sessionId = ready.SessionId;
            FireEventOnDelegate(frame, Ready);
            return true;
        }
        
        private bool OnHeartbeatAck()
        {
            // TODO: Wait for ack after heartbeat.
            return true;
        }

        private void ProcessEvents(SocketFrame frame)
        {
            bool suceeded = frame.Operation switch
            {
                OperationCode.Hello => OnHelloReceived((SocketFrame<Hello>)frame),
                OperationCode.InvalidSession => OnInvalidSession(frame),
                OperationCode.HeartbeatAck => OnHeartbeatAck(),
                OperationCode.Dispatch => frame.Type switch {
                    EventNames.READY => OnReady((SocketFrame<Ready>)frame),
                    EventNames.RESUMED => FireEventOnDelegate(frame, Resumed),

                    EventNames.GUILD_CREATED => FireEventOnDelegate(frame, GuildCreated),
                    EventNames.GUILD_UPDATED => FireEventOnDelegate(frame, GuildUpdated),
                    EventNames.GUILD_DELETED => FireEventOnDelegate(frame, GuildDeleted),
                    EventNames.GUILD_SYNC => FireEventOnDelegate(frame, GuildSynced),

                    EventNames.GUILD_BAN_ADDED => FireEventOnDelegate(frame, GuildBanAdded),
                    EventNames.GUILD_BAN_REMOVED => FireEventOnDelegate(frame, GuildBanRemoved),

                    EventNames.CHANNEL_CREATED => FireEventOnDelegate(frame, ChannelCreated),
                    EventNames.CHANNEL_UPDATED => FireEventOnDelegate(frame, ChannelUpdated),
                    EventNames.CHANNEL_DELETED => FireEventOnDelegate(frame, ChannelDeleted),

                    EventNames.CHANNEL_RECIPIENT_ADD => FireEventOnDelegate(frame, ChannelRecipientAdded),
                    EventNames.CHANNEL_RECIPIENT_REMOVE => FireEventOnDelegate(frame, ChannelRecipientRemoved),

                    EventNames.MESSAGE_ACK => FireEventOnDelegate(frame, MessageAck),
                    EventNames.MESSAGE_CREATED => FireEventOnDelegate(frame, MessageCreated),
                    EventNames.MESSAGE_UPDATED => FireEventOnDelegate(frame, MessageUpdated),
                    EventNames.MESSAGE_DELETED => FireEventOnDelegate(frame, MessageDeleted),

                    EventNames.MESSAGE_REACTION_ADD => FireEventOnDelegate(frame, MessageReactionAdded),
                    EventNames.MESSAGE_REACTION_REMOVE => FireEventOnDelegate(frame, MessageReactionRemoved),
                    EventNames.MESSAGE_REACTION_REMOVE_ALL => FireEventOnDelegate(frame,
                        MessageReactionRemovedAll),

                    EventNames.GUILD_MEMBER_ADDED => FireEventOnDelegate(frame, GuildMemberAdded),
                    EventNames.GUILD_MEMBER_UPDATED => FireEventOnDelegate(frame, GuildMemberUpdated),
                    EventNames.GUILD_MEMBER_REMOVED => FireEventOnDelegate(frame, GuildMemberRemoved),
                    EventNames.GUILD_MEMBER_LIST_UPDATE => FireEventOnDelegate(frame, GuildMemberListUpdated),
                    EventNames.GUILD_MEMBERS_CHUNK => FireEventOnDelegate(frame, GuildMembersChunk),

                    EventNames.RELATIONSHIP_ADDED => FireEventOnDelegate(frame, RelationshipAdded),
                    EventNames.RELATIONSHIP_UPDATE => FireEventOnDelegate(frame, RelationshipUpdated),
                    EventNames.RELATIONSHIP_REMOVED => FireEventOnDelegate(frame, RelationshipRemoved),

                    EventNames.TYPING_START => FireEventOnDelegate(frame, TypingStarted),
                    EventNames.PRESENCE_UPDATED => FireEventOnDelegate(frame, PresenceUpdated),

                    EventNames.USER_NOTE_UPDATED => FireEventOnDelegate(frame, UserNoteUpdated),
                    EventNames.USER_SETTINGS_UPDATED => FireEventOnDelegate(frame, UserSettingsUpdated),
                    EventNames.USER_GUILD_SETTINGS_UPDATED => FireEventOnDelegate(frame,
                        UserGuildSettingsUpdated),

                    EventNames.VOICE_STATE_UPDATED => FireEventOnDelegate(frame, VoiceStateUpdated),
                    EventNames.VOICE_SERVER_UPDATED => FireEventOnDelegate(frame, VoiceServerUpdated),

                    EventNames.SESSIONS_REPLACE => FireEventOnDelegate(frame, SessionReplaced),
                    _ => false
                },
                _ => false
            };
            if (!suceeded) UnhandledMessageEncountered?.Invoke(this, new SocketFrameException("Socket frame parsed, but unhandled.", (int?)frame.Operation, frame.Type));
        }
    }
}
