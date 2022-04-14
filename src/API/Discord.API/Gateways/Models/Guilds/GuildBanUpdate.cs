// Quarrel © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models.Guilds
{
    internal class GuildBanUpdate
    {
        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("user")]
        public JsonUser User { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
