// Quarrel © 2022

using Discord.API.Exceptions;
using Discord.API.Voice;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.API.JsonConverters
{
    internal class VoiceSocketFrameConverter : JsonConverter<VoiceSocketFrame>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(VoiceSocketFrame).IsAssignableFrom(typeToConvert);

        public override VoiceSocketFrame Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new SocketFrameException();
            }
            
            VoiceOperation? op = null;

            while (true)
            {
                readerClone.Read();
                if (readerClone.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                if (readerClone.TokenType != JsonTokenType.PropertyName)
                {
                    throw new SocketFrameException("Invalid JSON.");
                }

                string? propertyName = readerClone.GetString();
                switch (propertyName)
                {
                    case "op":
                        readerClone.Read();
                        if (readerClone.TokenType != JsonTokenType.Number) throw new SocketFrameException("OP Code is not a number.", null);
                        op = (VoiceOperation)(object)readerClone.GetInt32();
                        goto end;
                    case "d":
                        readerClone.Skip();
                        break;
                    default:
                        throw new SocketFrameException($"Unexpected property {propertyName}");
                }
            }

            end:
            return op switch
            {
                VoiceOperation.Hello => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameVoiceHello)!,
                VoiceOperation.Ready => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameVoiceReady)!,
                VoiceOperation.SessionDescription => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameVoiceSessionDescription)!,
                VoiceOperation.Heartbeat => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameInt32)!,
                VoiceOperation.HeartbeatAck => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameInt32)!,
                VoiceOperation.Speaking => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrameSpeaker)!,
                VoiceOperation.Identify => throw new JsonException("Server should not be sending us Identify"),
                null => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.UnknownOperationVoiceSocketFrame)!,
                _ => JsonSerializer.Deserialize(ref reader, JsonModelsContext.Default.VoiceSocketFrame)!
            };
        }

        public override void Write(Utf8JsonWriter writer, VoiceSocketFrame value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
