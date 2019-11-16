using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DiscordAPI.Models
{

    public class Operator
    {
        [JsonProperty("range")]
        public int[] Range { get; set; }

        [JsonProperty("op")]
        public string Op { get; set; }

        [JsonProperty("items")]
        public IEnumerable<SyncItem> Items { get; set; }

        [JsonProperty("item")]
        public SyncItem Item { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }
    }
}
