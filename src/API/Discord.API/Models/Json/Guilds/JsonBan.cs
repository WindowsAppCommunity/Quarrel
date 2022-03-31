// Adam Dernis © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Guilds
{
    internal class JsonBan
    {
        [JsonPropertyName("user")]
        public JsonUser User { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
