// Quarrel © 2022

using Discord.API.Models.Json.Emojis;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models.Messages
{
    internal class JsonReactionUpdated
    {
        [JsonPropertyName("user_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("message_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong MessageId { get; set; }

        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("emoji")]
        public JsonEmoji Emoji { get; set; }
    }
}
