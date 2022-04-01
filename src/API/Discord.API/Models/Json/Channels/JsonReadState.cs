// Adam Dernis © 2022

using System;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Channels
{
    internal class JsonReadState
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
