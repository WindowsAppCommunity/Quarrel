// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.Guilds
{
    internal class GuildDeleted
    {
        [JsonPropertyName("id")]
        public string GuildId { get; set; }

        [JsonPropertyName("unavailable")]
        public bool Unavailable { get; set; }
    }
}
