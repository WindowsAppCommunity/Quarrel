using Newtonsoft.Json;

namespace Quarrel.RichPresence.Models
{
    /// <summary>
    /// Urls of Assets for the Activity
    /// </summary>
    public class Assets
    {
        [JsonProperty("small_image")]
        public string SmallImage { get; set; }

        [JsonProperty("large_image")]
        public string LargeImage { get; set; }

        [JsonProperty("small_text")]
        public string SmallText { get; set; }

        [JsonProperty("large_text")]
        public string LargeText { get; set; }
    }
}
