using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct DirectMessageChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("is_private")]
        public bool Private { get; set; }
        [JsonProperty("recipient")]
        public User User { get; set; }
        [JsonProperty("recipients")]
        public IEnumerable<User> Users { get; set; }
        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }
    }
}
