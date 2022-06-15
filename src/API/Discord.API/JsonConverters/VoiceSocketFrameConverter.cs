// Quarrel © 2022

using Discord.API.Voice;
using System.Text.Json;

namespace Discord.API.JsonConverters
{
    internal class VoiceSocketFrameConverter : SocketFrameConverter<VoiceSocketFrame, VoiceOperation, VoiceEvent>
    {
        protected override VoiceSocketFrame ParseByEvent(VoiceOperation? op, VoiceEvent? eventName, ref Utf8JsonReader reader)
        {
            return op switch
            {
                VoiceOperation.Hello => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameVoiceHello)!,
                VoiceOperation.Ready => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameVoiceReady)!,
                VoiceOperation.SessionDescription => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameSessionDescription)!,
                VoiceOperation.Heartbeat => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameInt32)!,
                VoiceOperation.HeartbeatAck => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameInt32)!,
                VoiceOperation.Identify => throw new JsonException("Server should not be sending us Identify"),
                null => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.UnknownOperationVoiceSocketFrame)!,
                _ => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrame)!
            };
        }
    }
}
