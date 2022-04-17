// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Status.Models
{
    public partial class StatusClass
    {
        [JsonPropertyName("indicator")]
        public string Indicator { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
