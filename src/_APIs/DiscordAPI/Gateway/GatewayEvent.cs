using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordAPI.Sockets
{
    public class SocketFrame
    {
        [JsonProperty("op")]
        public int? Operation { get; set; }
        [JsonProperty("d")]
        public object Payload { get; set; }
        [JsonProperty("s")]
        public int? SequenceNumber { get; set; }
        [JsonProperty("t")]
        public string Type { get; set; }

        public T GetData<T>()
        {
            if (Payload is JObject)
            {
                var dataAsJObject = Payload as JObject;
                return dataAsJObject.ToObject<T>();
            }
            if (Payload is JArray)
            {
                var dataAsJObject = Payload as JArray;
                return dataAsJObject.ToObject<T>();
            }
            return default(T);
        }
    }
}
