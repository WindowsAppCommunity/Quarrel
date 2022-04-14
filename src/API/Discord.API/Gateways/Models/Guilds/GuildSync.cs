// Quarrel © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models.Guilds
{
    internal class GuildSync
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("large")]
        public bool IsLarge { get; set; }

        [JsonPropertyName("members")]
        public JsonGuildMember[] Members { get; set; }

        [JsonPropertyName("presences")]
        public JsonPresence[] Presences { get; set; }
    }
}
