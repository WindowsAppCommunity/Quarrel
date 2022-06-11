// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    internal record JsonMessageInteraction
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        public InterationType Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("user")]
        public JsonUser User { get; set; }

        [JsonPropertyName("member")]
        public JsonGuildMember? Mmber { get; set; }
    }
}
