// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Channels
{
    internal record JsonMarkRead
    {
        [JsonPropertyName("token")]
        public string? Token{ get; set; }
    }
}
