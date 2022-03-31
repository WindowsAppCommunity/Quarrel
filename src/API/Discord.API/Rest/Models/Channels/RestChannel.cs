// Adam Dernis © 2022

using Discord.API.Models.Channels.Enums;
using Discord.API.Rest.Models.Permissions;
using Discord.API.Rest.Models.Users;
using System;
using System.Text.Json.Serialization;

namespace Discord.API.Rest.Models.Channels
{
    internal class RestChannel
    {
        // Universal
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        public ChannelType Type { get; set; }

        // Shared

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("owner_id")]
        public ulong? OwnerId { get; set; }

        // Messages
        [JsonPropertyName("last_message_id")]
        public ulong? LastMessageId { get; set; }

        // Guild
        [JsonPropertyName("guild_id")]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("position")]
        public int? Position { get; set; }

        [JsonPropertyName("permission_overwrites")]
        public RestOverwrite[]? PermissionOverwrites { get; set; }

        [JsonPropertyName("parent_id")]
        public ulong? CategoryId { get; set; }

        // Text
        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        [JsonPropertyName("nsfw")]
        public bool? IsNSFW { get; set; }

        [JsonPropertyName("last_pin_timestamp")]
        public DateTimeOffset? LastPinTimestamp { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public int? SlowModeDelay { get; set; }

        // Voice
        [JsonPropertyName("bitrate")]
        public int? Bitrate { get; set; }

        [JsonPropertyName("user_limit")]
        public int? UserLimit { get; set; }

        [JsonPropertyName("rtc_region")]
        public string? RTCRegion { get; set; }

        // Direct
        [JsonPropertyName("recipient")]
        public RestUser? Recipient { get; set; }

        // Group
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        // Private
        [JsonPropertyName("recipients")]
        public RestUser[]? Recipients { get; set; }

        // Thread
        [JsonPropertyName("member")]
        public RestThreadMember? ThreadMember { get; set; }

        [JsonPropertyName("thread_metadata")]
        public RestThreadMetadata? ThreadMetadata { get; set; }

        [JsonPropertyName("message_count")]
        public int MessageCount { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }
    }
}
