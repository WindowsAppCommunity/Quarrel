using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    public class Connection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("revoked")]
        public bool Revoked { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("show_activity")]
        public bool ShowActivity { get; set; }
        [JsonProperty("friend_sync")]
        public bool FriendSync { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("integrations")]
        public IEnumerable<Integration> Integrations { get; set; }
    }
}
