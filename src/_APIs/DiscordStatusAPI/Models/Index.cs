using Newtonsoft.Json;

namespace DiscordStatusAPI.Models
{
    public class Index
    {
        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("status")]
        public StatusClass Status { get; set; }

        [JsonProperty("components")]
        public Component[] Components { get; set; }

        [JsonProperty("incidents")]
        public Incident[] Incidents { get; set; }
    }
}
