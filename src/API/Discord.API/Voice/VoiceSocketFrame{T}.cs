// Quarrel © 2022

using Discord.API.Sockets;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice
{
    internal class VoiceSocketFrame<T> : VoiceSocketFrame
    {
        [JsonPropertyName("d")]
        public T Payload { get; set; }
    }
}
