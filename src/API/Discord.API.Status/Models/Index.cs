// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    public class Index
    {
        [JsonPropertyName("page")]
        public Page Page { get; set; }

        [JsonPropertyName("status")]
        public StatusClass Status { get; set; }

        [JsonPropertyName("components")]
        public Component[] Components { get; set; }

        [JsonPropertyName("incidents")]
        public Incident[] Incidents { get; set; }
    }
}
