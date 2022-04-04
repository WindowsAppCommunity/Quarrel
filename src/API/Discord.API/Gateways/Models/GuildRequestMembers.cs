// Adam Dernis © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models
{
    internal class GuildRequestMembers
    {
        [JsonPropertyName("guild_id"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong[] GuildIds { get; set; }

        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("presences")]
        public bool? Presences { get; set; }

        [JsonPropertyName("user_ids"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong[]? UserIds { get; set; }
    }
}
