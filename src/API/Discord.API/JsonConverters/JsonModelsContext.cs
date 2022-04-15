// Quarrel © 2022

using Discord.API.Gateways;
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
using System.Text.Json.Serialization;

namespace Discord.API.JsonConverters
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonSerializable(typeof(GatewayEvent))]
    [JsonSerializable(typeof(SocketFrame))]
    [JsonSerializable(typeof(UnknownEventSocketFrame))]
    [JsonSerializable(typeof(UnknownOperationSocketFrame))]
    [JsonSerializable(typeof(SocketFrame))]
    [JsonSerializable(typeof(SocketFrame<Hello>))]
    [JsonSerializable(typeof(SocketFrame<Ready>))]
    [JsonSerializable(typeof(SocketFrame<Resumed>))]
    [JsonSerializable(typeof(SocketFrame<JsonGuild>))]
    [JsonSerializable(typeof(SocketFrame<JsonGuild>))]
    [JsonSerializable(typeof(SocketFrame<GuildDeleted>))]
    [JsonSerializable(typeof(SocketFrame<GuildSync>))]
    [JsonSerializable(typeof(SocketFrame<GuildBanUpdate>))]
    [JsonSerializable(typeof(SocketFrame<GuildBanUpdate>))]
    [JsonSerializable(typeof(SocketFrame<JsonChannel>))]
    [JsonSerializable(typeof(SocketFrame<JsonChannel>))]
    [JsonSerializable(typeof(SocketFrame<JsonChannel>))]
    [JsonSerializable(typeof(SocketFrame<ChannelRecipientUpdate>))]
    [JsonSerializable(typeof(SocketFrame<ChannelRecipientUpdate>))]
    [JsonSerializable(typeof(SocketFrame<JsonMessageAck>))]
    [JsonSerializable(typeof(SocketFrame<JsonMessage>))]
    [JsonSerializable(typeof(SocketFrame<JsonMessage>))]
    [JsonSerializable(typeof(SocketFrame<JsonMessageDeleted>))]
    [JsonSerializable(typeof(SocketFrame<MessageReactionUpdated>))]
    [JsonSerializable(typeof(SocketFrame<MessageReactionUpdated>))]
    [JsonSerializable(typeof(SocketFrame<MessageReactionRemoveAll>))]
    [JsonSerializable(typeof(SocketFrame<JsonGuildMember>))]
    [JsonSerializable(typeof(SocketFrame<JsonGuildMember>))]
    [JsonSerializable(typeof(SocketFrame<GuildMemberRemoved>))]
    [JsonSerializable(typeof(SocketFrame<GuildMemberListUpdated>))]
    [JsonSerializable(typeof(SocketFrame<GuildMembersChunk>))]
    [JsonSerializable(typeof(SocketFrame<JsonRelationship>))]
    [JsonSerializable(typeof(SocketFrame<JsonRelationship>))]
    [JsonSerializable(typeof(SocketFrame<JsonRelationship>))]
    [JsonSerializable(typeof(SocketFrame<TypingStart>))]
    [JsonSerializable(typeof(SocketFrame<JsonPresence>))]
    [JsonSerializable(typeof(SocketFrame<UserNote>))]
    [JsonSerializable(typeof(SocketFrame<JsonUserSettings>))]
    [JsonSerializable(typeof(SocketFrame<JsonGuildSettings>))]
    [JsonSerializable(typeof(SocketFrame<JsonVoiceState>))]
    [JsonSerializable(typeof(SocketFrame<VoiceServerUpdate>))]
    [JsonSerializable(typeof(SocketFrame<SessionReplace[]>))]
    [JsonSerializable(typeof(SocketFrame<int>))]
    [JsonSerializable(typeof(SocketFrame<Identity>))]
    internal partial class JsonModelsContext : JsonSerializerContext
    {
    }
}
