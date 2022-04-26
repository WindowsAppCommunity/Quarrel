// Quarrel © 2022

using Discord.API.Models.Enums.Channels;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    internal class JsonChannelMention
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("type")]
        public ChannelType ChannelType { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
