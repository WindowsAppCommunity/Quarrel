// Adam Dernis © 2022

using Discord.API.Models.Enums.Channels;
using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Channels
{
    internal class JsonThreadMetadata
    {
        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public ThreadArchiveDuration AutoArchiveDuration { get; set; }

        [JsonPropertyName("archive_timestamp")]
        public DateTimeOffset ArchiveTimestamp { get; set; }
        
        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }

        [JsonPropertyName("invitable")]
        public bool? Invitable { get; set; }

        [JsonPropertyName("create_timestamp")]
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
