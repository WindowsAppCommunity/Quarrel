// Quarrel © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models.Channels
{
    internal class ChannelRecipientUpdate
    {
        [JsonPropertyName("channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("user")]
        public JsonUser User { get; set; }
    }
}
