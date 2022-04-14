// Quarrel © 2022

using Discord.API.Models.Json.Users;
using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Voice
{
    internal class JsonVoiceState
    {
        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? ChannelId { get; set; }

        [JsonPropertyName("user_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("member")]
        public JsonGuildMember? Member { get; set; }

        [JsonPropertyName("session_id")]
        public string? SessionId { get; set; }

        [JsonPropertyName("deaf")]
        public bool? Deaf { get; set; }

        [JsonPropertyName("mute")]
        public bool? Mute { get; set; }

        [JsonPropertyName("self_deaf")]
        public bool? SelfDeaf { get; set; }

        [JsonPropertyName("self_mute")]
        public bool? SelfMute { get; set; }

        [JsonPropertyName("suppress")]
        public bool? Suppress { get; set; }

        [JsonPropertyName("self_stream")]
        public bool? SelfStream { get; set; }

        [JsonPropertyName("self_video")]
        public bool? SelfVideo { get; set; }

        [JsonPropertyName("request_to_speak_timestamp")]
        public DateTimeOffset? RequestToSpeakTimestamp { get; set; }
    }
}
