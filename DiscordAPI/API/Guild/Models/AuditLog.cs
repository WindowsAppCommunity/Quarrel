using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;
using Newtonsoft.Json;

namespace DiscordAPI.API.Guild.Models
{
    public partial class AuditLog
    {
        [JsonProperty("webhooks")]
        public List<Webhook> Webhooks { get; set; }

        [JsonProperty("users")]
        public List<AuditLogUser> Users { get; set; }

        [JsonProperty("audit_log_entries")]
        public List<AuditLogEntry> AuditLogEntries { get; set; }
    }

    public partial class AuditLogEntry
    {
        [JsonProperty("target_id")]
        public string TargetId { get; set; }

        [JsonProperty("changes", NullValueHandling = NullValueHandling.Ignore)]
        public Change[] Changes { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("action_type")]
        public long ActionType { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public Options Options { get; set; }

        [JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; }
    }

    public partial class Change
    {
        [JsonProperty("new_value")]
        public object NewValue { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("old_value", NullValueHandling = NullValueHandling.Ignore)]
        public object OldValue { get; set; }
    }

    public partial class NewValueElement
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("deny", NullValueHandling = NullValueHandling.Ignore)]
        public long? Deny { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("allow", NullValueHandling = NullValueHandling.Ignore)]
        public long? Allow { get; set; }
    }

    public partial class Options
    {
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }

        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ChannelId { get; set; }

        [JsonProperty("role_name", NullValueHandling = NullValueHandling.Ignore)]
        public string RoleName { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }

    public partial class AuditLogUser
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("bot", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Bot { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}
