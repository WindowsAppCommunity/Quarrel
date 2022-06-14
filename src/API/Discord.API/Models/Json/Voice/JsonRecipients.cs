// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Voice
{
    internal class JsonRecipients
    {
        [JsonPropertyName("recipients")]
        public ulong[]? Recipients { get; set; }
    }
}
