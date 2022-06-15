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
using Discord.API.Voice;
using Discord.API.Voice.Models.Handshake;
using System.Text.Json.Serialization;

namespace Discord.API.JsonConverters
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonSerializable(typeof(GatewayEvent))]
    [JsonSerializable(typeof(GatewaySocketFrame))]
    [JsonSerializable(typeof(UnknownEventGatewaySocketFrame))]
    [JsonSerializable(typeof(UnknownOperationGatewaySocketFrame))]
    [JsonSerializable(typeof(GatewaySocketFrame))]
    [JsonSerializable(typeof(GatewaySocketFrame<int>))]
    [JsonSerializable(typeof(GatewaySocketFrame<Hello>))]
    [JsonSerializable(typeof(GatewaySocketFrame<Identity>))]
    [JsonSerializable(typeof(GatewaySocketFrame<Ready>))]
    [JsonSerializable(typeof(GatewaySocketFrame<Resumed>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonGuild>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonGuild>))]
    [JsonSerializable(typeof(GatewaySocketFrame<GuildDeleted>))]
    [JsonSerializable(typeof(GatewaySocketFrame<GuildSync>))]
    [JsonSerializable(typeof(GatewaySocketFrame<GuildBanUpdate>))]
    [JsonSerializable(typeof(GatewaySocketFrame<GuildBanUpdate>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonChannel>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonChannel>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonChannel>))]
    [JsonSerializable(typeof(GatewaySocketFrame<ChannelRecipientUpdate>))]
    [JsonSerializable(typeof(GatewaySocketFrame<ChannelRecipientUpdate>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonMessageAck>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonMessage>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonMessage>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonMessageDeleted>))]
    [JsonSerializable(typeof(GatewaySocketFrame<MessageReactionUpdated>))]
    [JsonSerializable(typeof(GatewaySocketFrame<MessageReactionUpdated>))]
    [JsonSerializable(typeof(GatewaySocketFrame<MessageReactionRemoveAll>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonGuildMember>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonGuildMember>))]
    [JsonSerializable(typeof(GatewaySocketFrame<GuildMemberRemoved>))]
    [JsonSerializable(typeof(GatewaySocketFrame<GuildMemberListUpdated>))]
    [JsonSerializable(typeof(GatewaySocketFrame<GuildMembersChunk>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonRelationship>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonRelationship>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonRelationship>))]
    [JsonSerializable(typeof(GatewaySocketFrame<TypingStart>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonPresence>))]
    [JsonSerializable(typeof(GatewaySocketFrame<UserNote>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonUserSettings>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonGuildSettings>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonVoiceState>))]
    [JsonSerializable(typeof(GatewaySocketFrame<JsonVoiceServerUpdate>))]
    [JsonSerializable(typeof(GatewaySocketFrame<SessionReplace[]>))]

    [JsonSerializable(typeof(VoiceEvent))]
    [JsonSerializable(typeof(VoiceSocketFrame))]
    [JsonSerializable(typeof(UnknownEventVoiceSocketFrame))]
    [JsonSerializable(typeof(UnknownOperationVoiceSocketFrame))]
    [JsonSerializable(typeof(VoiceSocketFrame<int>))]
    [JsonSerializable(typeof(VoiceSocketFrame<VoiceHello>))]
    [JsonSerializable(typeof(VoiceSocketFrame<VoiceIdentity>))]
    [JsonSerializable(typeof(VoiceSocketFrame<VoiceReady>))]
    [JsonSerializable(typeof(VoiceSocketFrame<SessionDescription>))]
    internal partial class JsonModelsContext : JsonSerializerContext
    {
    }
}
