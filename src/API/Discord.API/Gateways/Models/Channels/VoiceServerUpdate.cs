﻿// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Channels
{
    internal class VoiceServerUpdate
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }
    }
}