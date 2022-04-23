// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    /// <summary>
    /// An incident in the Discord API.
    /// </summary>
    public partial class Incident
    {
        /// <summary>
        /// Gets the name of the incident.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the status of the api.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets the time the incident began.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Gets the time of the last incident update.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Gets the time monitoring began.
        /// </summary>
        [JsonPropertyName("monitoring_at")]
        public DateTimeOffset? MonitoringAt { get; set; }

        /// <summary>
        /// Gets the time the incident was resolved at.
        /// </summary>
        [JsonPropertyName("resolved_at")]
        public DateTimeOffset? ResolvedAt { get; set; }

        /// <summary>
        /// Gets the impact of the incident.
        /// </summary>
        [JsonPropertyName("impact")]
        public string Impact { get; set; }

        /// <summary>
        /// Gets the short link url to the incident.
        /// </summary>
        [JsonPropertyName("shortlink")]
        public string Shortlink { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the post mortem of the incident is ignored.
        /// </summary>
        [JsonPropertyName("postmortem_ignored")]
        public bool PostmortemIgnored { get; set; }

        /// <summary>
        /// Gets the post mortem of the incident.
        /// </summary>
        [JsonPropertyName("postmortem_body")]
        public object PostmortemBody { get; set; }

        /// <summary>
        /// Gets the time the post mortem body was last updated.
        /// </summary>
        [JsonPropertyName("postmortem_body_last_updated_at")]
        public object PostmortemBodyLastUpdatedAt { get; set; }

        /// <summary>
        /// Gets the time when the post mortem was published.
        /// </summary>
        [JsonPropertyName("postmortem_published_at")]
        public object PostmortemPublishedAt { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not post mortem subscribers are notified.
        /// </summary>
        [JsonPropertyName("postmortem_notified_subscribers")]
        public bool PostmortemNotifiedSubscribers { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the post mortem was posted on Twitter.
        /// </summary>
        [JsonPropertyName("postmortem_notified_twitter")]
        public bool PostmortemNotifiedTwitter { get; set; }

        /// <summary>
        /// TODO: Investigate.
        /// </summary>
        [JsonPropertyName("scheduled_for")]
        public object ScheduledFor { get; set; }

        /// <summary>
        /// TODO: Investigate.
        /// </summary>
        [JsonPropertyName("scheduled_until")]
        public object ScheduledUntil { get; set; }

        /// <summary>
        /// TODO: Investigate.
        /// </summary>
        [JsonPropertyName("scheduled_remind_prior")]
        public bool ScheduledRemindPrior { get; set; }

        /// <summary>
        /// TODO: Investigate.
        /// </summary>
        [JsonPropertyName("scheduled_reminded_at")]
        public object ScheduledRemindedAt { get; set; }

        /// <summary>
        /// TODO: Investigate.
        /// </summary>
        [JsonPropertyName("impact_override")]
        public string ImpactOverride { get; set; }

        /// <summary>
        /// TODO: Investigate.
        /// </summary>
        [JsonPropertyName("scheduled_auto_in_progress")]
        public bool ScheduledAutoInProgress { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the incident is scheduled for auto complete.
        /// </summary>
        [JsonPropertyName("scheduled_auto_completed")]
        public bool ScheduledAutoCompleted { get; set; }

        /// <summary>
        /// Gets the incident id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets the page id for the incident.
        /// </summary>
        [JsonPropertyName("page_id")]
        public string PageId { get; set; }

        /// <summary>
        /// Gets a list of updates to the incident.
        /// </summary>
        [JsonPropertyName("incident_updates")]
        public IncidentUpdate[] IncidentUpdates { get; set; }
    }
}
