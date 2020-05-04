using Newtonsoft.Json;

namespace Quarrel.RichPresence.Models
{
    public class Party
    {
        [JsonProperty("size")]
        public int?[] Size { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
