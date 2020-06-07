// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Channels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordAPI.Models.Guilds
{
    public class Guild
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("splash")]
        public string Splash { get; set; }
        [JsonProperty("banner")]
        public string Banner { get; set; }
        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("afk_channel_id")]
        public string AfkChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AfkTimeout { get; set; }
        [JsonProperty("embed_enabled")]
        public bool EmbedEnabled { get; set; }
        [JsonProperty("explicit_content_filter")]
        public int ExplicitContentFilter { get; set; }
        [JsonProperty("embed_channel_id")]
        public string EmbedChannelId { get; set; }
        [JsonProperty("verification_level")]
        public int VerificationLevel { get; set; }
        [JsonProperty("voice_states")]
        public IList<VoiceState> VoiceStates { get; set; }
        [JsonProperty("channels")]
        public IList<GuildChannel> Channels { get; set; }
        [JsonProperty("members")]
        public IList<GuildMember> Members { get; set; }
        [JsonProperty("roles")]
        public IList<Role> Roles { get; set; }
        [JsonProperty("emojis")]
        public IList<Emoji> Emojis { get; set; }
        [JsonProperty("features")]
        public IList<string> Features { get; set; }
        [JsonProperty("unavailable")]
        public bool Unavailable { get; set; }
        [JsonProperty("member_count")]
        public int MemberCount { get; set; }
        [JsonProperty("presences")]
        public IList<Presence> Presences { get; set; }

        public Uri SplashUri => SplashUrl != null ? new Uri(SplashUrl) : null;
        public string SplashUrl
        {
            get
            {
                if (Splash == null)
                    return null;
                else
                    return string.Format("https://cdn.discordapp.com/splashes/{0}/{1}.png", Id, Splash);
            }
        }

        public Uri BannerUri => BannerUrl != null ? new Uri(BannerUrl) : null;
        public string BannerUrl
        {
            get
            {
                if (Banner == null)
                {
                    return null;
                }

                return $"https://cdn.discordapp.com/banners/{Id}/{Banner}.png?size=512";
            }
        }
    }
}
