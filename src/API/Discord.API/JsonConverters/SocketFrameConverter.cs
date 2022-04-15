// Quarrel © 2022

using Discord.API.Exceptions;
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
                throw new SocketFrameException();
            }

            GatewayOperation? op = null;
            GatewayEvent? eventName = null;
            int count = 0;

            while (true)
            {

                readerClone.Read();
                if (readerClone.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                if (readerClone.TokenType != JsonTokenType.PropertyName)
                {
                    throw new SocketFrameException("Invalid JSON.");
                }

                string? propertyName = readerClone.GetString();
                switch (propertyName)
                {
                    case "op":
                        readerClone.Read();
                        if (readerClone.TokenType != JsonTokenType.Number) throw new SocketFrameException("OP Code is not a number.", null, eventName?.ToString());
                        op = (GatewayOperation)readerClone.GetInt32();
                        if (count++ == 2) goto end;
                        break;
                    case "t":
                        readerClone.Read();
                        if (Enum.TryParse(readerClone.GetString(), out GatewayEvent en))
                        {
                            eventName = en;
                        }

                        if (count++ == 2) goto end;
                        break;
                    case "s":
                    case "d":
                        readerClone.Skip();
                        break;
                    default:
                        throw new SocketFrameException($"Unexcepted property {propertyName}");
                }
            }
            end:
            return op switch
            {
                GatewayOperation.Dispatch => eventName switch
                {
                    GatewayEvent.READY => JsonSerializer.Deserialize<SocketFrame<Ready>>(ref reader, JsonModelsContext.Default.SocketFrameReady)!,
                    GatewayEvent.RESUMED => JsonSerializer.Deserialize<SocketFrame<Resumed>>(ref reader, JsonModelsContext.Default.SocketFrameResumed)!,

                    GatewayEvent.GUILD_CREATE => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuild)!,
                    GatewayEvent.GUILD_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuild)!,
                    GatewayEvent.GUILD_DELETE => JsonSerializer.Deserialize<SocketFrame<GuildDeleted>>(ref reader, JsonModelsContext.Default.SocketFrameGuildDeleted)!,

                    GatewayEvent.GUILD_BAN_ADD => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameGuildBanUpdate)!,
                    GatewayEvent.GUILD_BAN_REMOVE => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameGuildBanUpdate)!,

                    GatewayEvent.CHANNEL_CREATE => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, JsonModelsContext.Default.SocketFrameJsonChannel)!,
                    GatewayEvent.CHANNEL_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, JsonModelsContext.Default.SocketFrameJsonChannel)!,
                    GatewayEvent.CHANNEL_DELETE => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, JsonModelsContext.Default.SocketFrameJsonChannel)!,

                    GatewayEvent.CHANNEL_RECIPIENT_ADD => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameChannelRecipientUpdate)!,
                    GatewayEvent.CHANNEL_RECIPIENT_REMOVE => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameChannelRecipientUpdate)!,

                    GatewayEvent.MESSAGE_ACK => JsonSerializer.Deserialize<SocketFrame<JsonMessageAck>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessageAck)!,
                    GatewayEvent.MESSAGE_CREATE => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessage)!,
                    GatewayEvent.MESSAGE_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessage)!,
                    GatewayEvent.MESSAGE_DELETE => JsonSerializer.Deserialize<SocketFrame<JsonMessageDeleted>>(ref reader, JsonModelsContext.Default.SocketFrameJsonMessageDeleted)!,

                    GatewayEvent.MESSAGE_REACTION_ADD => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader, JsonModelsContext.Default.SocketFrameMessageReactionUpdated)!,
                    GatewayEvent.MESSAGE_REACTION_REMOVE => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader, JsonModelsContext.Default.SocketFrameMessageReactionUpdated)!,
                    GatewayEvent.MESSAGE_REACTION_REMOVE_ALL => JsonSerializer.Deserialize<SocketFrame<MessageReactionRemoveAll>>(ref reader, JsonModelsContext.Default.SocketFrameMessageReactionRemoveAll)!,

                    GatewayEvent.GUILD_MEMBER_ADD => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuildMember)!,
                    GatewayEvent.GUILD_MEMBER_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuildMember)!,
                    GatewayEvent.GUILD_MEMBER_REMOVE => JsonSerializer.Deserialize<SocketFrame<GuildMemberRemoved>>(ref reader, JsonModelsContext.Default.SocketFrameGuildMemberRemoved)!,
                    GatewayEvent.GUILD_MEMBER_LIST_UPDATE => JsonSerializer.Deserialize<SocketFrame<GuildMemberListUpdated>>(ref reader, JsonModelsContext.Default.SocketFrameGuildMemberListUpdated)!,
                    GatewayEvent.GUILD_MEMBERS_CHUNK => JsonSerializer.Deserialize<SocketFrame<GuildMembersChunk>>(ref reader, JsonModelsContext.Default.SocketFrameGuildMembersChunk)!,

                    GatewayEvent.RELATIONSHIP_ADD => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, JsonModelsContext.Default.SocketFrameJsonRelationship)!,
                    GatewayEvent.RELATIONSHIP_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, JsonModelsContext.Default.SocketFrameJsonRelationship)!,
                    GatewayEvent.RELATIONSHIP_REMOVE => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, JsonModelsContext.Default.SocketFrameJsonRelationship)!,

                    GatewayEvent.TYPING_START => JsonSerializer.Deserialize<SocketFrame<TypingStart>>(ref reader, JsonModelsContext.Default.SocketFrameTypingStart)!,
                    GatewayEvent.PRESENCE_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonPresence>>(ref reader, JsonModelsContext.Default.SocketFrameJsonPresence)!,

                    GatewayEvent.USER_NOTE_UPDATE => JsonSerializer.Deserialize<SocketFrame<UserNote>>(ref reader, JsonModelsContext.Default.SocketFrameUserNote)!,
                    GatewayEvent.USER_SETTINGS_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonUserSettings>>(ref reader, JsonModelsContext.Default.SocketFrameJsonUserSettings)!,
                    GatewayEvent.USER_GUILD_SETTINGS_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonGuildSettings>>(ref reader, JsonModelsContext.Default.SocketFrameJsonGuildSettings)!,

                    GatewayEvent.VOICE_STATE_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonVoiceState>>(ref reader, JsonModelsContext.Default.SocketFrameJsonVoiceState)!,
                    GatewayEvent.VOICE_SERVER_UPDATE => JsonSerializer.Deserialize<SocketFrame<VoiceServerUpdate>>(ref reader, JsonModelsContext.Default.SocketFrameVoiceServerUpdate)!,

                    GatewayEvent.SESSIONS_REPLACE => JsonSerializer.Deserialize<SocketFrame<SessionReplace[]>>(ref reader, JsonModelsContext.Default.SocketFrameSessionReplaceArray)!,

                    GatewayEvent.IDENTIFY => throw new JsonException("Server should not be sending us IDENTIFY"),
                    null => JsonSerializer.Deserialize<UnknownEventSocketFrame>(ref reader, JsonModelsContext.Default.UnknownEventSocketFrame)!,
                    _ => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!
                },
                GatewayOperation.Heartbeat => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                GatewayOperation.Reconnect => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                GatewayOperation.InvalidSession => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                GatewayOperation.Hello => JsonSerializer.Deserialize<SocketFrame<Hello>>(ref reader, JsonModelsContext.Default.SocketFrameHello)!,
                GatewayOperation.HeartbeatAck => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!,
                null => JsonSerializer.Deserialize<UnknownOperationSocketFrame>(ref reader, JsonModelsContext.Default.UnknownOperationSocketFrame)!,
                _ => JsonSerializer.Deserialize<SocketFrame>(ref reader, JsonModelsContext.Default.SocketFrame)!
            };
        }

        public override void Write(Utf8JsonWriter writer, SocketFrame value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
