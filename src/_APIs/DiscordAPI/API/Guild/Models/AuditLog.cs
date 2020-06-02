using DiscordAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        public int ActionType { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public Options Options { get; set; }

        [JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; }

        // Todo: Move to a bindable
        [JsonIgnore]
        public IDictionary<string, (string, int)> Users { get; set; }

        [JsonIgnore]
        public IDictionary<string, (string, int)> Roles { get; set; }

        [JsonIgnore]
        public IDictionary<string, string> Channels { get; set; }
    }

    public enum AuditLogActionType
    {
        GuildUpdate = 1,
        ChannelCreate = 10,
        ChannelUpdate = 11,
        ChannelDelete = 12,
        ChannelOverwriteCreate = 13,
        ChannelOverwriteUpdate = 14,
        ChannelOverwriteDelete = 15,
        MemberKick = 20,
        MemberPrune = 21,
        MemberBanAdd = 22,
        MemberBanRemove = 23,
        MemberUpdate = 24,
        MemberRoleUpdate = 25,
        RoleCreate = 30,
        RoleUpdate = 31,
        RoleDelete = 32,
        InviteCreate = 40,
        InviteUpdate = 41,
        InviteDelete = 42,
        WebhookCreate = 50,
        WebhookUpdate = 51,
        WebhookDelete = 52,
        EmojiCreate = 60,
        EmojiUpdate = 61,
        EmojiDelete = 62,
        MessageDelete = 72
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

        [JsonProperty("delete_member_days", NullValueHandling = NullValueHandling.Ignore)]
        public string DeleteMemberDays { get; set; }

        [JsonProperty("members_removed", NullValueHandling = NullValueHandling.Ignore)]
        public string MembersRemoved { get; set; }
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
