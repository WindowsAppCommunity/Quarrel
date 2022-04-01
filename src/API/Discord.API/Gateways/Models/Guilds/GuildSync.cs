// Adam Dernis © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

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
