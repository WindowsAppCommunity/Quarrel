// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models
{
    internal class UserNote
    {
        [JsonPropertyName("id"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("note"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong Note { get; set; }
    }
}
