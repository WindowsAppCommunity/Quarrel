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
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.API.JsonConverters
{
    internal class SocketFrameConverter : JsonConverter<SocketFrame>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(SocketFrame).IsAssignableFrom(typeToConvert);

        public override SocketFrame Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            OperationCode? op = null;
            string? eventName = null;
            
            while (true)
            {

                readerClone.Read();
                if (readerClone.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                if (readerClone.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string? propertyName = readerClone.GetString();
                switch (propertyName)
                {
                    case "op":
                        readerClone.Read();
                        if (readerClone.TokenType != JsonTokenType.Number) throw new JsonException();
                        op = (OperationCode)readerClone.GetInt32();
                        if (op != OperationCode.Dispatch) goto end;
                        break;
                    case "t":
                        readerClone.Read();
                        eventName = readerClone.GetString();
                        if (op != null) goto end;
                        break;
                    case "s":
                    case "d":
                        readerClone.Skip();
                        break;
                    default:
                        throw new JsonException();
                }
            }
            end:
            return op switch
            {
                OperationCode.Dispatch => eventName switch
                {
                    EventNames.READY => JsonSerializer.Deserialize<SocketFrame<Ready>>(ref reader, JsonModelsContext.Default.SocketFrameReady)!,
                    EventNames.RESUMED => JsonSerializer.Deserialize<SocketFrame<Resumed>>(ref reader, JsonModelsContext.Default.SocketFrameResumed)!,

                    EventNames.GUILD_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuild)!,
                    EventNames.GUILD_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuild)!,
                    EventNames.GUILD_DELETED => JsonSerializer.Deserialize<SocketFrame<GuildDeleted>>(ref reader, JsonModelsContext.Default.SocketFrameGuildDeleted)!,
                    EventNames.GUILD_SYNC => JsonSerializer.Deserialize<SocketFrame<GuildSync>>(ref reader, JsonModelsContext.Default.SocketFrameGuildSync)!,

                    EventNames.GUILD_BAN_ADDED => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameGuildBanUpdate)!,
                    EventNames.GUILD_BAN_REMOVED => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameGuildBanUpdate)!,

                    EventNames.CHANNEL_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, JsonModelsContext.Default.SocketFrameJsonChannel)!,
                    EventNames.CHANNEL_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, JsonModelsContext.Default.SocketFrameJsonChannel)!,
                    EventNames.CHANNEL_DELETED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, JsonModelsContext.Default.SocketFrameJsonChannel)!,

                    EventNames.CHANNEL_RECIPIENT_ADD => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameChannelRecipientUpdate)!,
                    EventNames.CHANNEL_RECIPIENT_REMOVE => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameChannelRecipientUpdate)!,

                    EventNames.MESSAGE_ACK => JsonSerializer.Deserialize<SocketFrame<JsonMessageAck>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessageAck)!,
                    EventNames.MESSAGE_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessage)!,
                    EventNames.MESSAGE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessage)!,
                    EventNames.MESSAGE_DELETED => JsonSerializer.Deserialize<SocketFrame<JsonMessageDeleted>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessageDeleted)!,

                    EventNames.MESSAGE_REACTION_ADD => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader, JsonModelsContext.Default.SocketFrameMessageReactionUpdated)!,
                    EventNames.MESSAGE_REACTION_REMOVE => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader, JsonModelsContext.Default.SocketFrameMessageReactionUpdated)!,
                    EventNames.MESSAGE_REACTION_REMOVE_ALL => JsonSerializer.Deserialize<SocketFrame<MessageReactionRemoveAll>>(ref reader, JsonModelsContext.Default.SocketFrameMessageReactionRemoveAll)!,

                    EventNames.GUILD_MEMBER_ADDED => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuildMember)!,
                    EventNames.GUILD_MEMBER_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuildMember)!,
                    EventNames.GUILD_MEMBER_REMOVED => JsonSerializer.Deserialize<SocketFrame<GuildMemberRemoved>>(ref reader, JsonModelsContext.Default.SocketFrameGuildMemberRemoved)!,
                    EventNames.GUILD_MEMBER_LIST_UPDATE => JsonSerializer.Deserialize<SocketFrame<GuildMemberListUpdated>>(ref reader, JsonModelsContext.Default.SocketFrameGuildMemberListUpdated)!,
                    EventNames.GUILD_MEMBERS_CHUNK => JsonSerializer.Deserialize<SocketFrame<GuildMembersChunk>>(ref reader, JsonModelsContext.Default.SocketFrameGuildMembersChunk)!,

                    EventNames.RELATIONSHIP_ADDED => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, JsonModelsContext.Default.SocketFrameJsonRelationship)!,
                    EventNames.RELATIONSHIP_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, JsonModelsContext.Default.SocketFrameJsonRelationship)!,
                    EventNames.RELATIONSHIP_REMOVED => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, JsonModelsContext.Default.SocketFrameJsonRelationship)!,

                    EventNames.TYPING_START => JsonSerializer.Deserialize<SocketFrame<TypingStart>>(ref reader, JsonModelsContext.Default.SocketFrameTypingStart)!,
                    EventNames.PRESENCE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonPresence>>(ref reader, JsonModelsContext.Default.SocketFrameJsonPresence)!,

                    EventNames.USER_NOTE_UPDATED => JsonSerializer.Deserialize<SocketFrame<UserNote>>(ref reader, JsonModelsContext.Default.SocketFrameUserNote)!,
                    EventNames.USER_SETTINGS_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonUserSettings>>(ref reader, JsonModelsContext.Default.SocketFrameJsonUserSettings)!,
                    EventNames.USER_GUILD_SETTINGS_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuildSettings>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuildSettings)!,

                    EventNames.VOICE_STATE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonVoiceState>>(ref reader, JsonModelsContext.Default.SocketFrameJsonVoiceState)!,
                    EventNames.VOICE_SERVER_UPDATED => JsonSerializer.Deserialize<SocketFrame<VoiceServerUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameVoiceServerUpdate)!,

                    EventNames.SESSIONS_REPLACE => JsonSerializer.Deserialize<SocketFrame<SessionReplace[]>>(ref reader, JsonModelsContext.Default.SocketFrameSessionReplaceArray)!,
                    _ => throw new JsonException()
                },
                OperationCode.Heartbeat => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                OperationCode.Reconnect => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                OperationCode.InvalidSession => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                OperationCode.Hello => JsonSerializer.Deserialize<SocketFrame<Hello>>(ref reader, JsonModelsContext.Default.SocketFrameHello)!,
                OperationCode.HeartbeatAck => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                _ => throw new JsonException()
            };

        }

        public override void Write(Utf8JsonWriter writer, SocketFrame value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

    }

    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
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