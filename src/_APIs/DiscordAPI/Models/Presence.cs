using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    public class Presence
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }

        [JsonProperty("game")]
        public Game Game { get; set; }

        [JsonProperty("activities")]
        public IEnumerable<Game> Activities { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonIgnore]
        public bool IsOnline
        {
            get => Status == "online";
        }

        [JsonIgnore]
        public bool IsIdle
        {
            get => Status == "idle";
        }

        [JsonIgnore]
        public bool IsDnd
        {
            get => Status == "dnd";
        }

        [JsonIgnore]
        public bool IsOffline
        {
            get => Status == "offline" || Status == "invisible";
        }
    }
}
