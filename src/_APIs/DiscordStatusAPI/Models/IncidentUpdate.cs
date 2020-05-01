using Newtonsoft.Json;
using System;

namespace DiscordStatusAPI.Models
{
    public partial class IncidentUpdate
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("wants_twitter_update")]
        public bool WantsTwitterUpdate { get; set; }

        [JsonProperty("twitter_updated_at")]
        public object TwitterUpdatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("display_at")]
        public DateTimeOffset DisplayAt { get; set; }

        [JsonProperty("affected_components")]
        public AffectedComponent[] AffectedComponents { get; set; }

        [JsonProperty("custom_tweet")]
        public object CustomTweet { get; set; }

        [JsonProperty("deliver_notifications")]
        public bool DeliverNotifications { get; set; }

        [JsonProperty("tweet_id")]
        public object TweetId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("incident_id")]
        public string IncidentId { get; set; }
    }
}
