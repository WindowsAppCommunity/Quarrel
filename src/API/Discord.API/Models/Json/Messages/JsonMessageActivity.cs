// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    internal record JsonMessageActivity
    {
        [JsonPropertyName("type")]
        public MessageActivityType? Type { get; set; }

        [JsonPropertyName("party_id")]
        public string? PartyId { get; set; }
    }
}
