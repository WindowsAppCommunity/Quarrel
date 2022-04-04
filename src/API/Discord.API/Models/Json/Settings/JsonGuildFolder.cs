// Adam Dernis © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Settings
{
    internal class JsonGuildFolder
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("color")]
        public uint? Color { get; set; }

        [JsonPropertyName("guild_ids"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[] GuildIds { get; set; }
    }
}
