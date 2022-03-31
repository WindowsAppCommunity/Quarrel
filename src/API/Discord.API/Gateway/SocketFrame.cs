// Adam Dernis © 2022

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class SocketFrame
    {
        [JsonPropertyName("op")]
        public OperationCode? Operation { get; set; }

        [JsonPropertyName("d")]
        public object Payload { get; set; }

        [JsonPropertyName("s")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("t")]
        public string Type { get; set; }

        public T? GetData<T>()
        {
            if (Payload is JsonObject objPayload)
            {
                return objPayload.GetValue<T>();
            }

            if (Payload is JsonArray arrPayload)
            {
                return arrPayload.GetValue<T>();
            }

            return default(T);
        }
    }
}
