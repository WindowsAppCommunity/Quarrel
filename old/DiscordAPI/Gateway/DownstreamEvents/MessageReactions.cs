using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Gateway.DownstreamEvents
{
    public class MessageReactionUpdate
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }
    }

    public class MessageReactionRemoveAll
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
    }
}
