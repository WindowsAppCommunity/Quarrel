using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models
{
    public class Integration
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("syncing")]
        public bool Syncing { get; set; }
        [JsonProperty("role_id")]
        public string RoleId { get; set; }
        [JsonProperty("expire_behavior")]
        public int ExpireBehavior { get; set; }
        [JsonProperty("grace_period")]
        public int GracePeriod { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("account")]
        public IntegrationAccount Account { get; set; }
        [JsonProperty("synced_at")]
        public DateTime SyncedAt { get; set; }
    }

    public class IntegrationAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
