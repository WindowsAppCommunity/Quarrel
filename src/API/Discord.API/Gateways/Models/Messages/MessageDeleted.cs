// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Messages
{
    public class MessageDeleted
    {
        [JsonPropertyName("id"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong MessageId { get; set; }

        [JsonPropertyName("channel_id"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong ChannelId { get; set; }
    }
}
