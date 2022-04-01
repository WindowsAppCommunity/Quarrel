// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models
{
    internal class StatusUpdate
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("since")]
        public long? IdleSince { get; set; }

        [JsonPropertyName("afk")]
        public bool IsAFK { get; set; }
    }
}
