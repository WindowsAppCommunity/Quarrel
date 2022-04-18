// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    public class AffectedComponent
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("old_status")]
        public string OldStatus { get; set; }

        [JsonPropertyName("new_status")]
        public string NewStatus { get; set; }
    }
}
