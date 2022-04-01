// Adam Dernis © 2022

using System;
using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Channels
{
    internal class TypingStart
    {
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("timestamp")]
        DateTimeOffset Timestamp { get; set; }
    }
}
