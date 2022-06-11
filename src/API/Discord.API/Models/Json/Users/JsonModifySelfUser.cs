// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Users
{
    internal record JsonModifySelfUser
    {
        [JsonPropertyName("bio")]
        public string? AboutMe { get; set; }
    }
}
