using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{

    public class Friend
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("user")]
        public User user { get; set; }

        /// <summary>
        /// Friend=1, Blocked=2, Incoming=3, Outgoing=4
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
