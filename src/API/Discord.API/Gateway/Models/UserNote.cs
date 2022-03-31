// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models
{
    internal class UserNote
    {
        [JsonPropertyName("id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }
    }
}
