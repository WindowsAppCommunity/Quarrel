// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    internal class JsonCall
    {
        [JsonPropertyName("ended_timestamp")]
        public DateTimeOffset EndedTimestamp { get; set; }

        [JsonPropertyName("participants"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[] Participants { get; set; }
    }
}
