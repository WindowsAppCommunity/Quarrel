// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models
{
    internal class UserNote
    {
        [JsonPropertyName("id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }
    }
}
