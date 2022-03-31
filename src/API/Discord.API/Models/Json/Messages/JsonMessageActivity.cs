// Adam Dernis © 2022

using Discord.API.Models.Managed.Enums.Messages;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Messages
{
    internal class JsonMessageActivity
    {
        [JsonPropertyName("type")]
        public MessageActivityType? Type { get; set; }

        [JsonPropertyName("party_id")]
        public string? PartyId { get; set; }
    }
}
