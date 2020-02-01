using Newtonsoft.Json;
using Quarrel.RichPresence.Models.Enums;

namespace Quarrel.RichPresence.Models
{
    /// <summary>
    /// Detailed game state
    /// </summary>
    public class RichGame : Game
    {
        public RichGame(string name, ActivityType type) : base(name, type)
        {

        }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("timestamps")]
        public TimeStamps TimeStamps { get; set; }

        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("party")]
        public Party Party { get; set; }

        [JsonProperty("assets")]
        public Assets Assets { get; set; }

        [JsonProperty("secrets")]
        public Secrets Secrets { get; set; }

        [JsonProperty("flags")]
        public int Flags { get; set; }

        [JsonProperty("instance")]
        public bool Instance { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}
