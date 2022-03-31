// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.Messages
{
    internal class MessageAck
    {
        [JsonPropertyName("message_id")]
        public ulong MessageId { get; set; }

        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
