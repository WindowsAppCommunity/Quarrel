// Adam Dernis © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

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
