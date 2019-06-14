using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class TypingStart
    {
        [JsonProperty("channel_id")]
        public string channelId { get; set; }
        [JsonProperty("user_id")]
        public string userId { get; set; }
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}
