// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Voice
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum VoiceEvent
    {
        READY,
        IDENTIFY,
        SPEAKING,
        HEARTBEAT,
        SELECT_PROTOCOL,
    }
}
