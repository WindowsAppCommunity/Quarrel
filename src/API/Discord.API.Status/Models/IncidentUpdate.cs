// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    public partial class IncidentUpdate
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("wants_twitter_update")]
        public bool WantsTwitterUpdate { get; set; }

        [JsonPropertyName("twitter_updated_at")]
        public object TwitterUpdatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("display_at")]
        public DateTimeOffset DisplayAt { get; set; }

        [JsonPropertyName("affected_components")]
        public AffectedComponent[] AffectedComponents { get; set; }

        [JsonPropertyName("custom_tweet")]
        public object CustomTweet { get; set; }

        [JsonPropertyName("deliver_notifications")]
        public bool DeliverNotifications { get; set; }

        [JsonPropertyName("tweet_id")]
        public object TweetId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("incident_id")]
        public string IncidentId { get; set; }
    }
}
