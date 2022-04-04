// Adam Dernis © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models
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
