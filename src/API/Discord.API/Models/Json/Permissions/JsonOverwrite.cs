// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Permissions
{
    internal class JsonOverwrite
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("allow"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Allow { get; set; }

        [JsonPropertyName("deny"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Deny { get; set; }
    }
}
