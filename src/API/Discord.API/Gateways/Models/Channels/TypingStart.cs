﻿// Adam Dernis © 2022

using System;
using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Channels
{
    internal class TypingStart
    {
        [JsonPropertyName("channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("user_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("timestamp")]
        DateTimeOffset Timestamp { get; set; }
    }
}