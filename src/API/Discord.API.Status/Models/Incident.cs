// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

namespace Discord.API.Status.Models
{
    public partial class Incident
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonPropertyName("monitoring_at")]
        public DateTimeOffset? MonitoringAt { get; set; }

        [JsonPropertyName("resolved_at")]
        public DateTimeOffset? ResolvedAt { get; set; }

        [JsonPropertyName("impact")]
        public string Impact { get; set; }

        [JsonPropertyName("shortlink")]
        public string Shortlink { get; set; }

        [JsonPropertyName("postmortem_ignored")]
        public bool PostmortemIgnored { get; set; }

        [JsonPropertyName("postmortem_body")]
        public object PostmortemBody { get; set; }

        [JsonPropertyName("postmortem_body_last_updated_at")]
        public object PostmortemBodyLastUpdatedAt { get; set; }

        [JsonPropertyName("postmortem_published_at")]
        public object PostmortemPublishedAt { get; set; }

        [JsonPropertyName("postmortem_notified_subscribers")]
        public bool PostmortemNotifiedSubscribers { get; set; }

        [JsonPropertyName("postmortem_notified_twitter")]
        public bool PostmortemNotifiedTwitter { get; set; }

        [JsonPropertyName("scheduled_for")]
        public object ScheduledFor { get; set; }

        [JsonPropertyName("scheduled_until")]
        public object ScheduledUntil { get; set; }

        [JsonPropertyName("scheduled_remind_prior")]
        public bool ScheduledRemindPrior { get; set; }

        [JsonPropertyName("scheduled_reminded_at")]
        public object ScheduledRemindedAt { get; set; }

        [JsonPropertyName("impact_override")]
        public string ImpactOverride { get; set; }

        [JsonPropertyName("scheduled_auto_in_progress")]
        public bool ScheduledAutoInProgress { get; set; }

        [JsonPropertyName("scheduled_auto_completed")]
        public bool ScheduledAutoCompleted { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("page_id")]
        public string PageId { get; set; }

        [JsonPropertyName("incident_updates")]
        public IncidentUpdate[] IncidentUpdates { get; set; }
    }
}
