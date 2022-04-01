// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Messages
{
    internal class MessageDeleted
    {
        [JsonPropertyName("id")]
        public string MessageId { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }
    }
}
