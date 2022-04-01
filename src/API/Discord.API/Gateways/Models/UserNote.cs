// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models
{
    internal class UserNote
    {
        [JsonPropertyName("id"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("note"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong Note { get; set; }
    }
}
