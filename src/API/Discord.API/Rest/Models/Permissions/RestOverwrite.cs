// Adam Dernis © 2022

using Discord.API.Models.Permissions;
using System.Text.Json.Serialization;

namespace Discord.API.Rest.Models.Permissions
{
    internal class RestOverwrite
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("allow")]
        public ChannelPermission Allow { get; set; }

        [JsonPropertyName("deny")]
        public ChannelPermission Deny { get; set; }
    }
}
