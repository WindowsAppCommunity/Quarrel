using Newtonsoft.Json;
using System;

namespace DiscordStatusAPI.Models
{
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
}
