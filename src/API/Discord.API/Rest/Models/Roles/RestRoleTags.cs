// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Rest.Models.Roles
{
    internal class RestRoleTags
    {
        [JsonPropertyName("bot_id")]
        public ulong BotId { get; set; }

        [JsonPropertyName("integration_id")]
        public ulong? IntegrationId { get; set; }

        [JsonPropertyName("premium_subscriber")]
        public bool? IsPremiumSubscriber { get; set; }

    }
}
