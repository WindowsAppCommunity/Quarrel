// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Messages
{
    internal record JsonComponent
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; }
    }
}
