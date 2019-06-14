using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Models
{
    public abstract class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        internal string _name { get; set; }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 0 = Text,
        /// 1 = DM,
        /// 2 = Voice,
        /// 3 = Group DM,
        /// 4 = Category
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }

        public void UpdateLMID(string id)
        {
            LastMessageId = id;
        }
    }
}
