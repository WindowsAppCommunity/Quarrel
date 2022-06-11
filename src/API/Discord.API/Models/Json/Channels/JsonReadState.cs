// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Channels
{
    internal record JsonReadState
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("mention_count")]
        public int? MentionCount { get; set; }

        [JsonPropertyName("last_message_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? LastMessageId{ get; set; }

        [JsonPropertyName("last_pin_timestamp")]
        public DateTimeOffset? LastPinTimestamp { get; set; }
    }
}
