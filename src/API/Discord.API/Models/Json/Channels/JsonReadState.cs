// Adam Dernis © 2022

using System;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Channels
{
    internal class JsonReadState
    {
        [JsonPropertyName("id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("mention_count")]
        public int? MentionCount { get; set; }

        [JsonPropertyName("last_pin_timestamp")]
        public ulong? LastMessageId{ get; set; }

        [JsonPropertyName("last_pin_timestamp")]
        public DateTimeOffset? LastPinTimestamp { get; set; }
    }
}
