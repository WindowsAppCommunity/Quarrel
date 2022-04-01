// Adam Dernis © 2022

using System;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Channels
{
    internal class JsonThreadMember
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? Id { get; set; }

        [JsonPropertyName("user_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? UserId { get; set; }

        [JsonPropertyName("join_timestamp")]
        public DateTimeOffset JoinTimestamp { get; set; }
    }
}
