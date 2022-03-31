// Adam Dernis © 2022

using Discord.API.Gateway.Models;
using Discord.API.Gateway.Models.Channels;
using Discord.API.Gateway.Models.GuildMember;
using Discord.API.Gateway.Models.Guilds;
using Discord.API.Gateway.Models.Messages;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Guilds;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Json.Settings;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Json.Voice;
using System;

namespace Discord.API.Gateway
{
    internal partial class Gateway
    {
        public event EventHandler<GatewayEventArgs<InvalidSession>> InvalidSession;

        public event EventHandler<GatewayEventArgs<Ready>> Ready;
        public event EventHandler<GatewayEventArgs<Resumed>> Resumed;
        public event EventHandler<Exception> GatewayClosed;

        public event EventHandler<GatewayEventArgs<JsonGuild>> GuildCreated;
        public event EventHandler<GatewayEventArgs<JsonGuild>> GuildUpdated;
        public event EventHandler<GatewayEventArgs<GuildDeleted>> GuildDeleted;
        public event EventHandler<GatewayEventArgs<GuildSync>> GuildSynced;

        public event EventHandler<GatewayEventArgs<GuildBanUpdate>> GuildBanAdded;
        public event EventHandler<GatewayEventArgs<GuildBanUpdate>> GuildBanRemoved;

        public event EventHandler<GatewayEventArgs<JsonChannel>> ChannelCreated;
        public event EventHandler<GatewayEventArgs<JsonChannel>> GuildChannelUpdated;
        public event EventHandler<GatewayEventArgs<JsonChannel>> ChannelDeleted;
        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>> ChannelRecipientAdded;
        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>> ChannelRecipientRemoved;

        public event EventHandler<GatewayEventArgs<JsonMessage>> MessageCreated;
        public event EventHandler<GatewayEventArgs<JsonMessage>> MessageUpdated;
        public event EventHandler<GatewayEventArgs<MessageDeleted>> MessageDeleted;
        public event EventHandler<GatewayEventArgs<MessageReactionUpdated>> MessageReactionAdded;
        public event EventHandler<GatewayEventArgs<MessageReactionUpdated>> MessageReactionRemoved;
        public event EventHandler<GatewayEventArgs<MessageReactionRemoveAll>> MessageReactionRemovedAll;
        public event EventHandler<GatewayEventArgs<MessageAck>> MessageAck;

        public event EventHandler<GatewayEventArgs<JsonGuildMember>> GuildMemberAdded;
        public event EventHandler<GatewayEventArgs<GuildMemberRemoved>> GuildMemberRemoved;
        public event EventHandler<GatewayEventArgs<JsonGuildMember>> GuildMemberUpdated;
        public event EventHandler<GatewayEventArgs<GuildMemberListUpdated>> GuildMemberListUpdated;
        public event EventHandler<GatewayEventArgs<GuildMembersCheck>> GuildMembersChunk;

        public event EventHandler<GatewayEventArgs<JsonRelationship>> RelationShipAdded;
        public event EventHandler<GatewayEventArgs<JsonRelationship>> RelationShipRemoved;
        public event EventHandler<GatewayEventArgs<JsonRelationship>> RelationShipUpdated;

        public event EventHandler<GatewayEventArgs<JsonPresence>> PresenceUpdated;
        public event EventHandler<GatewayEventArgs<TypingStart>> TypingStarted;

        public event EventHandler<GatewayEventArgs<UserNote>> UserNoteUpdated;
        public event EventHandler<GatewayEventArgs<JsonUserSettings>> UserSettingsUpdated;
        public event EventHandler<GatewayEventArgs<JsonGuildSettings>> UserGuildSettingsUpdated;

        public event EventHandler<GatewayEventArgs<JsonVoiceState>> VoiceStateUpdated;
        public event EventHandler<GatewayEventArgs<VoiceServerUpdate>> VoiceServerUpdated;

        public event EventHandler<GatewayEventArgs<SessionReplace[]>> SessionReplaced;
    }
}
