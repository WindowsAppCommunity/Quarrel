// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Status.Models
{
    public partial class Period
    {
        [JsonPropertyName("count")]
        public long Count { get; set; }

        [JsonPropertyName("interval")]
        public long Interval { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }
    }
}
