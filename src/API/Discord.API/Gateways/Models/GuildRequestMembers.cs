// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models
{
    internal class GuildRequestMembers
    {
        [JsonPropertyName("guild_id")]
        public ulong[] GuildIds { get; set; }

        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("presences")]
        public bool? Presences { get; set; }

        [JsonPropertyName("user_ids")]
        public ulong[]? UserIds { get; set; }
    }
}
