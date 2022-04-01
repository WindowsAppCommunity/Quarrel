// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Users
{
    internal class JsonPresence
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
