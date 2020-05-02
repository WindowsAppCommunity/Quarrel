using Newtonsoft.Json;
using System;

namespace DiscordStatusAPI.Models
{
    public partial class Metric
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("metric_identifier")]
        public string MetricIdentifier { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("metrics_provider_id")]
        public string MetricsProviderId { get; set; }

        [JsonProperty("metrics_display_id")]
        public string MetricsDisplayId { get; set; }

        [JsonProperty("most_recent_data_at")]
        public DateTimeOffset MostRecentDataAt { get; set; }

        [JsonProperty("backfilled")]
        public bool Backfilled { get; set; }

        [JsonProperty("last_fetched_at")]
        public DateTimeOffset LastFetchedAt { get; set; }

        [JsonProperty("backfill_percentage")]
        public long BackfillPercentage { get; set; }
    }
}
