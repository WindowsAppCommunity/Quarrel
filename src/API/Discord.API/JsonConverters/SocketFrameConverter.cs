// Quarrel © 2022

using Discord.API.Exceptions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.API.JsonConverters
{
    internal abstract class SocketFrameConverter<T, TOperation, TEvent> : JsonConverter<T>
        where TOperation : struct, Enum
        where TEvent : struct, Enum
    {
        public override bool CanConvert(Type typeToConvert) => typeof(T).IsAssignableFrom(typeToConvert);

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new SocketFrameException();
            }

            TOperation? op = null;
            TEvent? eventName = null;
            int count = 0;

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
                        if (readerClone.TokenType != JsonTokenType.Number) throw new SocketFrameException("OP Code is not a number.", null, eventName?.ToString());
                        op = (TOperation)(object)readerClone.GetInt32();
                        if (count++ == 2) goto end;
                        break;
                    case "t":
                        readerClone.Read();
                        if (Enum.TryParse(readerClone.GetString(), out TEvent en))
                        {
                            eventName = en;
                        }

                        if (count++ == 2) goto end;
                        break;
                    case "s":
                        readerClone.Skip();
                        break;
                    case "d":
                        // TODO: Investigate skip alternative
                        goto end;
                    default:
                        throw new SocketFrameException($"Unexpected property {propertyName}");
                }
            }

            end:
            return ParseByEvent(op, eventName, ref reader);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        protected abstract T ParseByEvent(TOperation? op, TEvent? eventName, ref Utf8JsonReader reader);
    }
}
