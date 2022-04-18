// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    public partial class Metric
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("metric_identifier")]
        public string MetricIdentifier { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("metrics_provider_id")]
        public string MetricsProviderId { get; set; }

        [JsonPropertyName("metrics_display_id")]
        public string MetricsDisplayId { get; set; }

        [JsonPropertyName("most_recent_data_at")]
        public DateTimeOffset MostRecentDataAt { get; set; }

        [JsonPropertyName("backfilled")]
        public bool Backfilled { get; set; }

        [JsonPropertyName("last_fetched_at")]
        public DateTimeOffset LastFetchedAt { get; set; }

        //[JsonPropertyName("backfill_percentage")]
        //public long BackfillPercentage { get; set; }
    }
}
