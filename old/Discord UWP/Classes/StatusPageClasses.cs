using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;

namespace Quarrel.Classes
{
    public class StatusPageClasses
    {

        public partial class AllMetrics
        {
            [JsonProperty("period")]
            public Period Period { get; set; }

            [JsonProperty("metrics")]
            public MetricElement[] Metrics { get; set; }

            [JsonProperty("summary")]
            public Summary Summary { get; set; }
        }

        public partial class MetricElement
        {
            [JsonProperty("metric")]
            public MetricMetric Metric { get; set; }

            [JsonProperty("summary")]
            public Summary Summary { get; set; }

            [JsonProperty("data")]
            public Datum[] Data { get; set; }
        }

        public partial class Datum
        {
            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("value")]
            public ushort Value { get; set; }
        }

        public partial class MetricMetric
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

        public partial class Summary
        {
            [JsonProperty("sum")]
            public double Sum { get; set; }

            [JsonProperty("mean")]
            public double Mean { get; set; }
        }

        public partial class Period
        {
            [JsonProperty("count")]
            public long Count { get; set; }

            [JsonProperty("interval")]
            public long Interval { get; set; }

            [JsonProperty("identifier")]
            public string Identifier { get; set; }
        }

        public partial class Index
        {
            [JsonProperty("page")]
            public Page Page { get; set; }

            [JsonProperty("status")]
            public StatusClass Status { get; set; }

            [JsonProperty("components")]
            public Component[] Components { get; set; }

            [JsonProperty("incidents")]
            public Incident[] Incidents { get; set; }
        }

        public partial class Component
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("created_at")]
            public DateTimeOffset CreatedAt { get; set; }

            [JsonProperty("updated_at")]
            public DateTimeOffset UpdatedAt { get; set; }

            [JsonProperty("position")]
            public long Position { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("showcase")]
            public bool Showcase { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("page_id")]
            public string PageId { get; set; }

            [JsonProperty("group_id")]
            public object GroupId { get; set; }

            [JsonProperty("components", NullValueHandling = NullValueHandling.Ignore)]
            public string[] Components { get; set; }
        }

        public partial class Incident
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("created_at")]
            public DateTimeOffset? CreatedAt { get; set; }

            [JsonProperty("updated_at")]
            public DateTimeOffset? UpdatedAt { get; set; }

            [JsonProperty("monitoring_at")]
            public DateTimeOffset? MonitoringAt { get; set; }

            [JsonProperty("resolved_at")]
            public DateTimeOffset? ResolvedAt { get; set; }

            [JsonProperty("impact")]
            public string Impact { get; set; }

            [JsonProperty("shortlink")]
            public string Shortlink { get; set; }

            [JsonProperty("postmortem_ignored")]
            public bool PostmortemIgnored { get; set; }

            [JsonProperty("postmortem_body")]
            public object PostmortemBody { get; set; }

            [JsonProperty("postmortem_body_last_updated_at")]
            public object PostmortemBodyLastUpdatedAt { get; set; }

            [JsonProperty("postmortem_published_at")]
            public object PostmortemPublishedAt { get; set; }

            [JsonProperty("postmortem_notified_subscribers")]
            public bool PostmortemNotifiedSubscribers { get; set; }

            [JsonProperty("postmortem_notified_twitter")]
            public bool PostmortemNotifiedTwitter { get; set; }

            [JsonProperty("scheduled_for")]
            public object ScheduledFor { get; set; }

            [JsonProperty("scheduled_until")]
            public object ScheduledUntil { get; set; }

            [JsonProperty("scheduled_remind_prior")]
            public bool ScheduledRemindPrior { get; set; }

            [JsonProperty("scheduled_reminded_at")]
            public object ScheduledRemindedAt { get; set; }

            [JsonProperty("impact_override")]
            public string ImpactOverride { get; set; }

            [JsonProperty("scheduled_auto_in_progress")]
            public bool ScheduledAutoInProgress { get; set; }

            [JsonProperty("scheduled_auto_completed")]
            public bool ScheduledAutoCompleted { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("page_id")]
            public string PageId { get; set; }

            [JsonProperty("incident_updates")]
            public IncidentUpdate[] IncidentUpdates { get; set; }
        }

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
        public partial class AffectedComponent
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("old_status")]
            public string OldStatus { get; set; }

            [JsonProperty("new_status")]
            public string NewStatus { get; set; }
        }

        public partial class Page
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public partial class StatusClass
        {
            [JsonProperty("indicator")]
            public string Indicator { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }

    }
}
