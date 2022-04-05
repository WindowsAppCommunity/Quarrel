// Adam Dernis © 2022

using Discord.API.Models.Json.Users;
using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Guilds.Invites
{
    internal class JsonInvite
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("guild")]
        public JsonInviteGuild Guild { get; set; }

        [JsonPropertyName("inviter")]
        public JsonUser? Inviter { get; set; }

        [JsonPropertyName("approximate_presence_count")]
        public int? PresenceCount { get; set; }

        [JsonPropertyName("approximate_member_count")]
        public int? MemberCount { get; set; }

        [JsonPropertyName("target_user")]
        public JsonUser? TargetUser { get; set; }

        [JsonPropertyName("uses")]
        public int? Uses { get; set; }

        [JsonPropertyName("max_uses")]
        public int? MaxUses { get; set; }

        [JsonPropertyName("max_age")]
        public int? MaxAge { get; set; }

        [JsonPropertyName("temporary")]
        public bool Temporary { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
