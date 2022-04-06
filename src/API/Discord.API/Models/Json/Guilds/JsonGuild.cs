// Adam Dernis © 2022

using Discord.API.Models.Enums.Guilds;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Emojis;
using Discord.API.Models.Json.Roles;
using Discord.API.Models.Json.Stickers;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Json.Voice;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Guilds
{
    internal class JsonGuild
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("splash")]
        public string Splash { get; set; }

        [JsonPropertyName("discovery_splash")]
        public string DiscoverySplash { get; set; }

        [JsonPropertyName("owner_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong OwnerId { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("afk_channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? AFKChannelId { get; set; }

        [JsonPropertyName("afk_timeout")]
        public AFKTimeout? AFKTimeout { get; set; }

        [JsonPropertyName("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }

        [JsonPropertyName("default_message_notifications")]
        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }

        [JsonPropertyName("explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilter { get; set; }

        [JsonPropertyName("mfa_level")]
        public MfaLevel MfaLevel { get; set; }

        [JsonPropertyName("nsfw_level")]
        public NsfwLevel NsfwLevel { get; set; }

        [JsonPropertyName("channels")]
        public JsonChannel[] Channels { get; set; }

        [JsonPropertyName("threads")]
        public JsonChannel[] Threads { get; set; }

        [JsonPropertyName("members")]
        public JsonGuildMember[] Members { get; set; }

        [JsonPropertyName("voice_states")]
        public JsonVoiceState[] VoiceStates { get; set; }

        [JsonPropertyName("roles")]
        public JsonRole[] Roles { get; set; }

        [JsonPropertyName("emojis")]
        public JsonEmoji[] Emojis { get; set; }

        [JsonPropertyName("features")]
        public string[] Features { get; set; }

        [JsonPropertyName("application_id")]
        public string? ApplicationId { get; set; }

        [JsonPropertyName("widget_enabled")]
        public bool? IsWidgetEnabled { get; set; }

        [JsonPropertyName("widget_channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? WidgetChannelId { get; set; }

        [JsonPropertyName("system_channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? SystemChannelId { get; set; }

        [JsonPropertyName("premium_tier")]
        public PremiumTier PremiumTier { get; set; }

        [JsonPropertyName("vanity_url_code")]
        public string VanityURLCode { get; set; }

        [JsonPropertyName("banner")]
        public string Banner { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("system_channel_flags")]
        public SystemChannelMessageDeny SystemChannelFlags { get; set; }

        [JsonPropertyName("rules_channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? RulesChannelId { get; set; }

        [JsonPropertyName("max_presences")]
        public int? MaxPresences { get; set; }

        [JsonPropertyName("max_members")]
        public int? MaxMembers { get; set; }

        [JsonPropertyName("premium_subscription_count")]
        public int? PremiumSubscriptionCount { get; set; }

        [JsonPropertyName("preferred_locale")]
        public string PreferredLocale { get; set; }

        [JsonPropertyName("public_updates_channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? PublicUpdatesChannelId { get; set; }

        [JsonPropertyName("max_video_channel_users")]
        public int? MaxVideoChannelUsers { get; set; }

        [JsonPropertyName("approximate_member_count")]
        public int? ApproximateMemberCount { get; set; }

        [JsonPropertyName("approximate_presence_count")]
        public int? ApproximatePresenceCount { get; set; }

        [JsonPropertyName("stickers")]
        public JsonSticker[] Stickers { get; set; }

        [JsonPropertyName("premium_progress_bar_enabled")]
        public bool? IsBoostProgressBarEnabled { get; set; }
    }
}
