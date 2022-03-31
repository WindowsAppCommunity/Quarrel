// Adam Dernis © 2022

using Discord.API.Models.Enums.Users;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Users
{
    internal class JsonRelationship
    {
        [JsonPropertyName("id")]
        public ulong? Id { get; set; }

        [JsonPropertyName("user")]
        public JsonUser? User { get; set; }

        [JsonPropertyName("presence")]
        public JsonPresence? Presence { get; set; }

        [JsonPropertyName("type")]
        public RelationshipType Type { get; set; }
    }
}
