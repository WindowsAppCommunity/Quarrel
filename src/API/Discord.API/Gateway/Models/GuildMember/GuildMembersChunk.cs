// Adam Dernis © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.GuildMember
{
    internal class GuildMembersChunk
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("members")]
        public JsonGuildMember[] Members { get; set; }

        [JsonPropertyName("presences")]
        public JsonPresence[] Presences { get; set; }
    }
}
