// Adam Dernis © 2022

using Discord.API.Models.Json.Emojis;
using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Messages
{
    internal class MessageReactionUpdated
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }

        [JsonPropertyName("emoji")]
        public JsonEmoji Emoji { get; set; }
    }
}
