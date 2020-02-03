using Newtonsoft.Json;

namespace Quarrel.RichPresence.Models
{
    public class Secrets
    {
        [JsonProperty("join")]
        public string Join { get; set; }
        [JsonProperty("spectate")]
        public string Spectate { get; set; }
        [JsonProperty("match")]
        public string Match { get; set; }
    }
}
