using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class ReadState
    {
        [JsonProperty("last_pin_timestamp")]
        public string LastPinTimestamp { get; set; }
        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("mention_count")]
        public int MentionCount { get; set; }
    }
}
