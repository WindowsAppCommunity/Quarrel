// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    internal class JsonMessageComponent
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; }

        [JsonPropertyName("components")]
        public List<JsonComponent> Components { get; set; }
    }
}
