// Adam Dernis © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Users
{
    internal class JsonGuildMember
    {
        [JsonPropertyName("user")]
        public JsonUser User { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("nick")]
        public string Nickname { get; set; }

        [JsonPropertyName("hoisted_role"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? HoistedRole { get; set; }

        [JsonPropertyName("roles"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[] Roles { get; set; }

        [JsonPropertyName("joined_at")]
        public DateTimeOffset? JoinedAt { get; set; }

        [JsonPropertyName("deaf")]
        public bool? Deaf { get; set; }

        [JsonPropertyName("mute")]
        public bool? Mute { get; set; }

        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }
    }
}
