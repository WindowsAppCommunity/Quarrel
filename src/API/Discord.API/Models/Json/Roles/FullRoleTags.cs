// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Roles
{
    internal class FullRoleTags
    {
        [JsonPropertyName("bot_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong BotId { get; set; }

        [JsonPropertyName("integration_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? IntegrationId { get; set; }

        [JsonPropertyName("premium_subscriber")]
        public bool? IsPremiumSubscriber { get; set; }

    }
}
