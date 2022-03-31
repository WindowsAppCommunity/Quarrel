// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Messages
{
    internal class JsonMessageReference
    {
        [JsonPropertyName("message_id")]
        public ulong? MessageId { get; set; }

        [JsonPropertyName("channel_id")]
        public ulong? ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public ulong? GuildId { get; set; }
    }
}
