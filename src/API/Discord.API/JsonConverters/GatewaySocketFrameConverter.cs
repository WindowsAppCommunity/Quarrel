// Quarrel © 2022

using Discord.API.Gateways;
using System.Text.Json;

namespace Discord.API.JsonConverters
{
    internal class GatewaySocketFrameConverter : SocketFrameConverter<GatewaySocketFrame, GatewayOperation, GatewayEvent>
    {
        protected override GatewaySocketFrame ParseByEvent(GatewayOperation? op, GatewayEvent? eventName, ref Utf8JsonReader reader)
        {
            return op switch
            {
                GatewayOperation.Dispatch => eventName switch
                {
                    GatewayEvent.READY => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameReady)!,
                    GatewayEvent.RESUMED => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameResumed)!,

                    GatewayEvent.GUILD_CREATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonGuild)!,
                    GatewayEvent.GUILD_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonGuild)!,
                    GatewayEvent.GUILD_DELETE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameGuildDeleted)!,

                    GatewayEvent.GUILD_BAN_ADD => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameGuildBanUpdate)!,
                    GatewayEvent.GUILD_BAN_REMOVE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameGuildBanUpdate)!,

                    GatewayEvent.CHANNEL_CREATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonChannel)!,
                    GatewayEvent.CHANNEL_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonChannel)!,
                    GatewayEvent.CHANNEL_DELETE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonChannel)!,

                    GatewayEvent.CHANNEL_RECIPIENT_ADD => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameChannelRecipientUpdate)!,
                    GatewayEvent.CHANNEL_RECIPIENT_REMOVE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameChannelRecipientUpdate)!,

                    GatewayEvent.MESSAGE_ACK => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonMessageAck)!,
                    GatewayEvent.MESSAGE_CREATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonMessage)!,
                    GatewayEvent.MESSAGE_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonMessage)!,
                    GatewayEvent.MESSAGE_DELETE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonMessageDeleted)!,

                    GatewayEvent.MESSAGE_REACTION_ADD => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameMessageReactionUpdated)!,
                    GatewayEvent.MESSAGE_REACTION_REMOVE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameMessageReactionUpdated)!,
                    GatewayEvent.MESSAGE_REACTION_REMOVE_ALL => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameMessageReactionRemoveAll)!,

                    GatewayEvent.GUILD_MEMBER_ADD => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonGuildMember)!,
                    GatewayEvent.GUILD_MEMBER_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonGuildMember)!,
                    GatewayEvent.GUILD_MEMBER_REMOVE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameGuildMemberRemoved)!,
                    GatewayEvent.GUILD_MEMBER_LIST_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameGuildMemberListUpdated)!,
                    GatewayEvent.GUILD_MEMBERS_CHUNK => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameGuildMembersChunk)!,

                    GatewayEvent.RELATIONSHIP_ADD => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonRelationship)!,
                    GatewayEvent.RELATIONSHIP_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonRelationship)!,
                    GatewayEvent.RELATIONSHIP_REMOVE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonRelationship)!,

                    GatewayEvent.TYPING_START => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameTypingStart)!,
                    GatewayEvent.PRESENCE_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonPresence)!,

                    GatewayEvent.USER_NOTE_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameUserNote)!,
                    GatewayEvent.USER_SETTINGS_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonUserSettings)!,
                    GatewayEvent.USER_GUILD_SETTINGS_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonGuildSettings)!,

                    GatewayEvent.VOICE_STATE_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonVoiceState)!,
                    GatewayEvent.VOICE_SERVER_UPDATE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameJsonVoiceServerUpdate)!,

                    GatewayEvent.SESSIONS_REPLACE => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameSessionReplaceArray)!,

                    GatewayEvent.IDENTIFY => throw new JsonException("Server should not be sending us IDENTIFY"),
                    null => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.UnknownEventGatewaySocketFrame)!,
                    _ => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrame)!
                },
                GatewayOperation.Reconnect => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrame)!,
                GatewayOperation.InvalidSession => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrame)!,
                GatewayOperation.Hello => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrameHello)!,
                GatewayOperation.Heartbeat => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrame)!,
                GatewayOperation.HeartbeatAck => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrame)!,
                null => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.UnknownOperationGatewaySocketFrame)!,
                _ => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.GatewaySocketFrame)!
            };
        }
    }
}
