// Quarrel © 2022

using Discord.API.Models.Enums.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Users
{
    internal class JsonUser
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("banner")]
        public string? Banner { get; set; }

        [JsonPropertyName("banner_color")]
        public string? BannerColor { get; set; }

        [JsonPropertyName("accent_color")]
        public uint? AccentColor { get; set; }

        // Bot
        [JsonPropertyName("bot")]
        public bool? Bot { get; set; }

        [JsonPropertyName("flags")]
        public UserProperties? Flags { get; set; }

        [JsonPropertyName("public_flags")]
        public UserProperties? PublicFlags { get; set; }

        // Self
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("verified")]
        public bool? Verified { get; set; }

        [JsonPropertyName("mfa_enabled")]
        public bool? MfaEnabled { get; set; }

        [JsonPropertyName("nsfw_allowed")]
        public bool? NSFWAllowed { get; set; }

        [JsonPropertyName("purchased_flags")]
        public PremiumType? PurchasedFlags { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
    }
}
