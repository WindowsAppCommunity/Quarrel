using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class ActivityData
    {
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
    public class FeedSettings
    {
        [JsonProperty("subscribed_games")]
        public string[] SubscribedGames { get; set; }
        [JsonProperty("autosubscribed_games")]
        public string[] AutoSubscribedGames { get; set; }
        [JsonProperty("unsubscribed_games")]
        public string[] UnsubscribedGames { get; set; }
        [JsonProperty("subscribed_users")]
        public string[] SubscribedUsers { get; set; }
        [JsonProperty("unsubscribed_users")]
        public string[] UnsubscribedUsers { get; set; }
    }

    public class GameNews
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public Thumbnail Video { get; set; }
        [JsonProperty("game_id")]
        public string GameId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("thumbnail")]
        public Thumbnail Thumbnail { get; set; }
        public string GameTitle { get; set; }
    }
    public partial class Thumbnail
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public long Width { get; set; }
        [JsonProperty("proxy_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public long Height { get; set; }
    }
}
