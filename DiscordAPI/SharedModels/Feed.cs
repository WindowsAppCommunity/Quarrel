using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class Feed
    {
        [JsonProperty("subscribed_games")]
        public IEnumerable<string> SubscribedGame { get; set; }
        [JsonProperty("autosubscribed_games")]
        public IEnumerable<string> AutoSubscribedGames { get; set; }
        [JsonProperty("subscribed_users")]
        public IEnumerable<string> SubscribedUsers { get; set; }
        [JsonProperty("unsubscribed_users")]
        public IEnumerable<string> UnsubscribedUsers { get; set; }
        [JsonProperty("unsubscribed_games")]
        public IEnumerable<string> UnsubscribedGames { get; set; }
    }

    public class FeedPatch
    {
        [JsonProperty("subscribe_users", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> UserSubscriptions { get; set; }
        [JsonProperty("subscribe_games", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> GameSubscriptions { get; set; }
    }
}
