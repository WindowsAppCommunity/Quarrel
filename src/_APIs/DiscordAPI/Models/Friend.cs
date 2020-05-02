using Newtonsoft.Json;

namespace DiscordAPI.Models
{

    public class Friend
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// Friend=1, Blocked=2, Incoming=3, Outgoing=4
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
