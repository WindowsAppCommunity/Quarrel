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
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }
        //[JsonProperty("Icon")]
        [JsonProperty("recipients")]
        public List<User> Users { get; set; }
        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }

        public void UpdateLMID(string id)
        {
            LastMessageId = id;
        }
    }
}
