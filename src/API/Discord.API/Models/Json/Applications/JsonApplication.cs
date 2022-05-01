// Quarrel © 2022

using Discord.API.Models.Json.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Applications
{
    internal class JsonApplication
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("rpc_origins")]
        public List<string>? RpcOrigins { get; set; }

        [JsonPropertyName("bot_public")]
        public bool BotPublic { get; set; }

        [JsonPropertyName("bot_require_code_grant")]
        public bool BotRequireCodeGrant { get; set; }

        [JsonPropertyName("terms_of_service_url")]
        public string? TermsOfServiceUrl { get; set; }

        [JsonPropertyName("privacy_policy_url")]
        public string? PrivacyPolicyUrl { get; set; }

        [JsonPropertyName("owner")]
        public JsonUser? Owner { get; set; }

        [JsonPropertyName("verify_key")]
        public string VerifyKey { get; set; }

        /*
        [JsonPropertyName("team")]
        public JsonTeam? Team { get; set; }
        */

        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("primary_sku_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? PrimarySkuId { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("cover_image")]
        public string? CoverImage { get; set; }

        [JsonPropertyName("flags")]
        public int? Flags { get; set; }

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        /*
        [JsonPropertyName("install_params")]
        public JsonInstallParams? InstallParams { get; set; }
        */

        [JsonPropertyName("custom_install_url")]
        public string? CustomInstallUrl { get; set; }
    }
}
