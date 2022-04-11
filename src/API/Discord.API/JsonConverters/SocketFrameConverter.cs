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
                    throw new JsonException();
                }

                string? propertyName = readerClone.GetString();
                switch (propertyName)
                {
                    case "op":
                        count++;
                        readerClone.Read();
                        if (readerClone.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }
                        op = (OperationCode)readerClone.GetInt32();
                        break;
                    case "t":
                        count++;
                        readerClone.Read();
                        eventName = readerClone.GetString();
                        break;
                    case "s":
                    case "d":
                        readerClone.Skip();
                        break;
                    default:
                        throw new JsonException();
                }

                if (count >= 2)
                {
                    break;
                }
            }

            return op switch
            {
                OperationCode.Dispatch => eventName switch
                {
                    EventNames.READY => JsonSerializer.Deserialize<SocketFrame<Ready>>(ref reader)!,
                    EventNames.RESUMED => JsonSerializer.Deserialize<SocketFrame<Resumed>>(ref reader)!,

                    EventNames.GUILD_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader)!,
                    EventNames.GUILD_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader)!,
                    EventNames.GUILD_DELETED => JsonSerializer.Deserialize<SocketFrame<GuildDeleted>>(ref reader)!,
                    EventNames.GUILD_SYNC => JsonSerializer.Deserialize<SocketFrame<GuildSync>>(ref reader)!,

                    EventNames.GUILD_BAN_ADDED => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader)!,
                    EventNames.GUILD_BAN_REMOVED => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader)!,

                    EventNames.CHANNEL_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader)!,
                    EventNames.CHANNEL_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader)!,
                    EventNames.CHANNEL_DELETED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader)!,

                    EventNames.CHANNEL_RECIPIENT_ADD => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader)!,
                    EventNames.CHANNEL_RECIPIENT_REMOVE => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader)!,

                    EventNames.MESSAGE_ACK => JsonSerializer.Deserialize<SocketFrame<JsonMessageAck>>(ref reader)!,
                    EventNames.MESSAGE_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader)!,
                    EventNames.MESSAGE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader)!,
                    EventNames.MESSAGE_DELETED => JsonSerializer.Deserialize<SocketFrame<JsonMessageDeleted>>(ref reader)!,

                    EventNames.MESSAGE_REACTION_ADD => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader)!,
                    EventNames.MESSAGE_REACTION_REMOVE => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader)!,
                    EventNames.MESSAGE_REACTION_REMOVE_ALL => JsonSerializer.Deserialize<SocketFrame<MessageReactionRemoveAll>>(ref reader)!,

                    EventNames.GUILD_MEMBER_ADDED => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader)!,
                    EventNames.GUILD_MEMBER_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader)!,
                    EventNames.GUILD_MEMBER_REMOVED => JsonSerializer.Deserialize<SocketFrame<GuildMemberRemoved>>(ref reader)!,
                    EventNames.GUILD_MEMBER_LIST_UPDATE => JsonSerializer.Deserialize<SocketFrame<GuildMemberListUpdated>>(ref reader)!,
                    EventNames.GUILD_MEMBERS_CHUNK => JsonSerializer.Deserialize<SocketFrame<GuildMembersChunk>>(ref reader)!,

                    EventNames.RELATIONSHIP_ADDED => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader)!,
                    EventNames.RELATIONSHIP_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader)!,
                    EventNames.RELATIONSHIP_REMOVED => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader)!,

                    EventNames.TYPING_START => JsonSerializer.Deserialize<SocketFrame<TypingStart>>(ref reader)!,
                    EventNames.PRESENCE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonPresence>>(ref reader)!,

                    EventNames.USER_NOTE_UPDATED => JsonSerializer.Deserialize<SocketFrame<UserNote>>(ref reader)!,
                    EventNames.USER_SETTINGS_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonUserSettings>>(ref reader)!,
                    EventNames.USER_GUILD_SETTINGS_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuildSettings>>(ref reader)!,

                    EventNames.VOICE_STATE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonVoiceState>>(ref reader)!,
                    EventNames.VOICE_SERVER_UPDATED => JsonSerializer.Deserialize<SocketFrame<VoiceServerUpdate>>(ref reader)!,

                    EventNames.SESSIONS_REPLACE => JsonSerializer.Deserialize<SocketFrame<SessionReplace[]>>(ref reader)!,
                    _ => throw new JsonException()
                },
                OperationCode.Heartbeat => JsonSerializer.Deserialize<SocketFrame>(ref reader)!,
                OperationCode.Reconnect => JsonSerializer.Deserialize<SocketFrame>(ref reader)!,
                OperationCode.InvalidSession => JsonSerializer.Deserialize<SocketFrame>(ref reader)!,
                OperationCode.Hello => JsonSerializer.Deserialize<SocketFrame<Hello>>(ref reader)!,
                OperationCode.HeartbeatAck => JsonSerializer.Deserialize<SocketFrame>(ref reader)!,
                _ => throw new JsonException()
            };

        }

        public override void Write(Utf8JsonWriter writer, SocketFrame value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize<SocketFrame>(writer, value, options);
        }

    }
}