// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Users
{
    internal record JsonPresence
    {
        [JsonPropertyName("user")]
        public JsonUser? User { get; set; }

        [JsonPropertyName("roles"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[]? Roles { get; set; }

        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
