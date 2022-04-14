// Quarrel © 2022

using Discord.API.Models.Enums.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Users
{
    internal class JsonRelationship
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? Id { get; set; }

        [JsonPropertyName("user")]
        public JsonUser? User { get; set; }

        [JsonPropertyName("presence")]
        public JsonPresence? Presence { get; set; }

        [JsonPropertyName("type")]
        public RelationshipType Type { get; set; }
    }
}
