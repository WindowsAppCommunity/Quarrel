using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public class ActivityData
    {
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("application_id")]
        public int ApplicationId { get; set; }
        [JsonProperty("user_id")]
        public int UserId { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
    public class FeedSettings
    {
        [JsonProperty("subscribed_games")]
        public string[] SubscribedGames { get; set; }
        [JsonProperty("autosubscribed_games")]
        public string[] AutoSubscribedGames { get; set; }
        [JsonProperty("autosubscribed_games")]
        public string[] UnsubscribedGames { get; set; }
        [JsonProperty("subscribed_users")]
        public string[] SubscribedUsers { get; set; }
        [JsonProperty("unsubscribed_users")]
        public string[] UnsubscribedUsers { get; set; }
    }
}
