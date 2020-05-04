using Newtonsoft.Json;

namespace Quarrel.RichPresence.Models
{
    public class TimeStamps
    {
        [JsonProperty("start")]
        public long? Start;
        [JsonProperty("end")]
        public long? End;
    }
}
