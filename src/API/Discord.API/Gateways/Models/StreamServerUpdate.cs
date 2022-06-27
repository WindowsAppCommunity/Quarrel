// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models
{
    internal record StreamServerUpdate
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("stream_key")]
        public string StreamKey { get; set; }

        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }
    }
}
