// Adam Dernis © 2022

using Discord.API.Models.Enums.Permissions;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Permissions
{
    internal class JsonOverwrite
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("allow")]
        public ChannelPermission Allow { get; set; }

        [JsonPropertyName("deny")]
        public ChannelPermission Deny { get; set; }
    }
}
