// Adam Dernis © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.GuildMember
{
    internal class GuildMemberRemoved
    {
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }

        [JsonPropertyName("user")]
        public JsonUser User { get; set; }
    }
}
