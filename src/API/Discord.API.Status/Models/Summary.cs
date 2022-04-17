// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Status.Models
{
    public partial class Summary
    {
        [JsonPropertyName("sum")]
        public double Sum { get; set; }

        [JsonPropertyName("mean")]
        public double Mean { get; set; }
    }
}
