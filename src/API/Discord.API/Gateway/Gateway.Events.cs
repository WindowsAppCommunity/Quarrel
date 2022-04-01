// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateway.Models;
using Discord.API.Gateway.Models.Channels;
using Discord.API.Gateway.Models.GuildMember;
using Discord.API.Gateway.Models.Guilds;
using Discord.API.Gateway.Models.Handshake;
using Discord.API.Gateway.Models.Messages;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Guilds;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Json.Settings;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Json.Voice;
using System;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    internal partial class Gateway
    {
        public event EventHandler<SocketFrame>? UnhandledMessageEncountered;

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

        public event EventHandler<GatewayEventArgs<MessageAck>>? MessageAck;
        public event EventHandler<GatewayEventArgs<JsonMessage>>? MessageCreated;
        public event EventHandler<GatewayEventArgs<JsonMessage>>? MessageUpdated;
        public event EventHandler<GatewayEventArgs<MessageDeleted>>? MessageDeleted;

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

        private void FireEventOnDelegate<T>(SocketFrame frame, EventHandler<GatewayEventArgs<T>>? eventHandler)
        {
            var eventArgs = new GatewayEventArgs<T>(frame.GetData<T>());
            eventHandler?.Invoke(this, eventArgs);
        }

        private void OnReady(SocketFrame frame)
        {
            var ready = frame.GetData<Ready>();
            Guard.IsNotNull(ready, nameof(ready));

            _sessionId = ready.SessionId;
            FireEventOnDelegate(frame, Ready);
        }

        private IReadOnlyDictionary<OperationCode, GatewayEventHandler> GetOperationHandlers()
        {
            return new Dictionary<OperationCode, GatewayEventHandler>()
            {
                { OperationCode.Hello, OnHelloReceived },
                { OperationCode.InvalidSession, OnInvalidSession },
            };
        }

        private IReadOnlyDictionary<string, GatewayEventHandler> GetEventHandlers()
        {
            return new Dictionary<string, GatewayEventHandler>()
            {
                { EventNames.READY, OnReady },
                { EventNames.RESUMED, (x) => FireEventOnDelegate(x, Resumed) },

                { EventNames.GUILD_CREATED, (x) => FireEventOnDelegate(x, GuildCreated) },
                { EventNames.GUILD_UPDATED, (x) => FireEventOnDelegate(x, GuildUpdated) },
                { EventNames.GUILD_DELETED, (x) => FireEventOnDelegate(x, GuildDeleted) },
                { EventNames.GUILD_SYNC, (x) => FireEventOnDelegate(x, GuildSynced) },

                { EventNames.GUILD_BAN_ADDED, (x) => FireEventOnDelegate(x, GuildBanAdded) },
                { EventNames.GUILD_BAN_REMOVED, (x) => FireEventOnDelegate(x, GuildBanRemoved) },

                { EventNames.CHANNEL_CREATED, (x) => FireEventOnDelegate(x, ChannelCreated) },
                { EventNames.CHANNEL_UPDATED, (x) => FireEventOnDelegate(x, ChannelUpdated) },
                { EventNames.CHANNEL_DELETED, (x) => FireEventOnDelegate(x, ChannelDeleted) },

                { EventNames.CHANNEL_RECIPIENT_ADD, (x) => FireEventOnDelegate(x, ChannelRecipientAdded) },
                { EventNames.CHANNEL_RECIPIENT_REMOVE, (x) => FireEventOnDelegate(x, ChannelRecipientRemoved) },

                { EventNames.MESSAGE_ACK, (x) => FireEventOnDelegate(x, MessageAck) },
                { EventNames.MESSAGE_CREATED, (x) => FireEventOnDelegate(x, MessageCreated) },
                { EventNames.MESSAGE_UPDATED, (x) => FireEventOnDelegate(x, MessageUpdated) },
                { EventNames.MESSAGE_DELETED, (x) => FireEventOnDelegate(x, MessageDeleted) },

                { EventNames.MESSAGE_REACTION_ADD, (x) => FireEventOnDelegate(x, MessageReactionAdded) },
                { EventNames.MESSAGE_REACTION_REMOVE, (x) => FireEventOnDelegate(x, MessageReactionRemoved) },
                { EventNames.MESSAGE_REACTION_REMOVE_ALL, (x) => FireEventOnDelegate(x, MessageReactionRemovedAll) },

                { EventNames.GUILD_MEMBER_ADDED, (x) => FireEventOnDelegate(x, GuildMemberAdded) },
                { EventNames.GUILD_MEMBER_UPDATED, (x) => FireEventOnDelegate(x, GuildMemberUpdated) },
                { EventNames.GUILD_MEMBER_REMOVED, (x) => FireEventOnDelegate(x, GuildMemberRemoved) },
                { EventNames.GUILD_MEMBER_LIST_UPDATE, (x) => FireEventOnDelegate(x, GuildMemberListUpdated) },
                { EventNames.GUILD_MEMBERS_CHUNK, (x) => FireEventOnDelegate(x, GuildMembersChunk) },

                { EventNames.RELATIONSHIP_ADDED, (x) => FireEventOnDelegate(x, RelationshipAdded) },
                { EventNames.RELATIONSHIP_UPDATE, (x) => FireEventOnDelegate(x, RelationshipUpdated) },
                { EventNames.RELATIONSHIP_REMOVED, (x) => FireEventOnDelegate(x, RelationshipRemoved) },

                { EventNames.TYPING_START, (x) => FireEventOnDelegate(x, TypingStarted) },
                { EventNames.PRESENCE_UPDATED, (x) => FireEventOnDelegate(x, PresenceUpdated) },

                { EventNames.USER_NOTE_UPDATED, (x) => FireEventOnDelegate(x, UserNoteUpdated) },
                { EventNames.USER_SETTINGS_UPDATED, (x) => FireEventOnDelegate(x, UserSettingsUpdated) },
                { EventNames.USER_GUILD_SETTINGS_UPDATED, (x) => FireEventOnDelegate(x, UserGuildSettingsUpdated) },

                { EventNames.VOICE_STATE_UPDATED, (x) => FireEventOnDelegate(x, VoiceStateUpdated) },
                { EventNames.VOICE_SERVER_UPDATED, (x) => FireEventOnDelegate(x, VoiceServerUpdated) },

                { EventNames.SESSIONS_REPLACE, (x) => FireEventOnDelegate(x, SessionReplaced) },
            };
        }
    }
}
