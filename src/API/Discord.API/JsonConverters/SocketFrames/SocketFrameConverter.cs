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

namespace Discord.API.JsonConverters.SocketFrames
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
                    throw new SocketFrameException();
                }

                string? propertyName = readerClone.GetString();
                switch (propertyName)
                {
                    case "op":
                        readerClone.Read();
                        if (readerClone.TokenType != JsonTokenType.Number) throw new SocketFrameException("OP Code is not a number.", null, eventName);
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
                        throw new SocketFrameException($"Unexcepted property {propertyName}");
                }
            }
            end:
            return op switch
            {
                OperationCode.Dispatch => eventName switch
                {
                    EventNames.READY => JsonSerializer.Deserialize<SocketFrame<Ready>>(ref reader, SocketFrameContext.Default.SocketFrameReady)!,
                    EventNames.RESUMED => JsonSerializer.Deserialize<SocketFrame<Resumed>>(ref reader, SocketFrameContext.Default.SocketFrameResumed)!,

                    EventNames.GUILD_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader, SocketFrameContext.Default.SocketFrameJsonGuild)!,
                    EventNames.GUILD_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuild>>(ref reader, SocketFrameContext.Default.SocketFrameJsonGuild)!,
                    EventNames.GUILD_DELETED => JsonSerializer.Deserialize<SocketFrame<GuildDeleted>>(ref reader, SocketFrameContext.Default.SocketFrameGuildDeleted)!,
                    EventNames.GUILD_SYNC => JsonSerializer.Deserialize<SocketFrame<GuildSync>>(ref reader, SocketFrameContext.Default.SocketFrameGuildSync)!,

                    EventNames.GUILD_BAN_ADDED => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader, SocketFrameContext.Default.SocketFrameGuildBanUpdate)!,
                    EventNames.GUILD_BAN_REMOVED => JsonSerializer.Deserialize<SocketFrame<GuildBanUpdate>>(ref reader, SocketFrameContext.Default.SocketFrameGuildBanUpdate)!,

                    EventNames.CHANNEL_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, SocketFrameContext.Default.SocketFrameJsonChannel)!,
                    EventNames.CHANNEL_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, SocketFrameContext.Default.SocketFrameJsonChannel)!,
                    EventNames.CHANNEL_DELETED => JsonSerializer.Deserialize<SocketFrame<JsonChannel>>(ref reader, SocketFrameContext.Default.SocketFrameJsonChannel)!,

                    EventNames.CHANNEL_RECIPIENT_ADD => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader, SocketFrameContext.Default.SocketFrameChannelRecipientUpdate)!,
                    EventNames.CHANNEL_RECIPIENT_REMOVE => JsonSerializer.Deserialize<SocketFrame<ChannelRecipientUpdate>>(ref reader, SocketFrameContext.Default.SocketFrameChannelRecipientUpdate)!,

                    EventNames.MESSAGE_ACK => JsonSerializer.Deserialize<SocketFrame<JsonMessageAck>>(ref reader, SocketFrameContext.Default.SocketFrameJsonMessageAck)!,
                    EventNames.MESSAGE_CREATED => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader, SocketFrameContext.Default.SocketFrameJsonMessage)!,
                    EventNames.MESSAGE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonMessage>>(ref reader, SocketFrameContext.Default.SocketFrameJsonMessage)!,
                    EventNames.MESSAGE_DELETED => JsonSerializer.Deserialize<SocketFrame<JsonMessageDeleted>>(ref reader, SocketFrameContext.Default.SocketFrameJsonMessageDeleted)!,

                    EventNames.MESSAGE_REACTION_ADD => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader, SocketFrameContext.Default.SocketFrameMessageReactionUpdated)!,
                    EventNames.MESSAGE_REACTION_REMOVE => JsonSerializer.Deserialize<SocketFrame<MessageReactionUpdated>>(ref reader, SocketFrameContext.Default.SocketFrameMessageReactionUpdated)!,
                    EventNames.MESSAGE_REACTION_REMOVE_ALL => JsonSerializer.Deserialize<SocketFrame<MessageReactionRemoveAll>>(ref reader, SocketFrameContext.Default.SocketFrameMessageReactionRemoveAll)!,

                    EventNames.GUILD_MEMBER_ADDED => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader, SocketFrameContext.Default.SocketFrameJsonGuildMember)!,
                    EventNames.GUILD_MEMBER_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuildMember>>(ref reader, SocketFrameContext.Default.SocketFrameJsonGuildMember)!,
                    EventNames.GUILD_MEMBER_REMOVED => JsonSerializer.Deserialize<SocketFrame<GuildMemberRemoved>>(ref reader, SocketFrameContext.Default.SocketFrameGuildMemberRemoved)!,
                    EventNames.GUILD_MEMBER_LIST_UPDATE => JsonSerializer.Deserialize<SocketFrame<GuildMemberListUpdated>>(ref reader, SocketFrameContext.Default.SocketFrameGuildMemberListUpdated)!,
                    EventNames.GUILD_MEMBERS_CHUNK => JsonSerializer.Deserialize<SocketFrame<GuildMembersChunk>>(ref reader, SocketFrameContext.Default.SocketFrameGuildMembersChunk)!,

                    EventNames.RELATIONSHIP_ADDED => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, SocketFrameContext.Default.SocketFrameJsonRelationship)!,
                    EventNames.RELATIONSHIP_UPDATE => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, SocketFrameContext.Default.SocketFrameJsonRelationship)!,
                    EventNames.RELATIONSHIP_REMOVED => JsonSerializer.Deserialize<SocketFrame<JsonRelationship>>(ref reader, SocketFrameContext.Default.SocketFrameJsonRelationship)!,

                    EventNames.TYPING_START => JsonSerializer.Deserialize<SocketFrame<TypingStart>>(ref reader, SocketFrameContext.Default.SocketFrameTypingStart)!,
                    EventNames.PRESENCE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonPresence>>(ref reader, SocketFrameContext.Default.SocketFrameJsonPresence)!,

                    EventNames.USER_NOTE_UPDATED => JsonSerializer.Deserialize<SocketFrame<UserNote>>(ref reader, SocketFrameContext.Default.SocketFrameUserNote)!,
                    EventNames.USER_SETTINGS_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonUserSettings>>(ref reader, SocketFrameContext.Default.SocketFrameJsonUserSettings)!,
                    EventNames.USER_GUILD_SETTINGS_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonGuildSettings>>(ref reader, SocketFrameContext.Default.SocketFrameJsonGuildSettings)!,

                    EventNames.VOICE_STATE_UPDATED => JsonSerializer.Deserialize<SocketFrame<JsonVoiceState>>(ref reader, SocketFrameContext.Default.SocketFrameJsonVoiceState)!,
                    EventNames.VOICE_SERVER_UPDATED => JsonSerializer.Deserialize<SocketFrame<VoiceServerUpdate>>(ref reader, SocketFrameContext.Default.SocketFrameVoiceServerUpdate)!,

                    EventNames.SESSIONS_REPLACE => JsonSerializer.Deserialize<SocketFrame<SessionReplace[]>>(ref reader, SocketFrameContext.Default.SocketFrameSessionReplaceArray)!,
                    _ => throw new SocketFrameException($"Unexcepted event name {eventName}.", (int?)op, eventName)
                },
                OperationCode.Heartbeat => JsonSerializer.Deserialize<SocketFrame>(ref reader, SocketFrameContext.Default.SocketFrame)!,
                OperationCode.Reconnect => JsonSerializer.Deserialize<SocketFrame>(ref reader, SocketFrameContext.Default.SocketFrame)!,
                OperationCode.InvalidSession => JsonSerializer.Deserialize<SocketFrame>(ref reader, SocketFrameContext.Default.SocketFrame)!,
                OperationCode.Hello => JsonSerializer.Deserialize<SocketFrame<Hello>>(ref reader, SocketFrameContext.Default.SocketFrameHello)!,
                OperationCode.HeartbeatAck => JsonSerializer.Deserialize<SocketFrame>(ref reader, SocketFrameContext.Default.SocketFrame)!,
                _ => throw new SocketFrameException($"Unexcpeted OP Code {op}.", (int?)op, eventName)
            };
        }

        public override void Write(Utf8JsonWriter writer, SocketFrame value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
