using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class DirectMessageChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("is_private")]
        public bool Private { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("name")]
        public string Name
        {
            get { return _name == null ? NameFromUsers() : _name; }
            set { _name = value; }
        }
        private string _name = null;
        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }
        //[JsonProperty("Icon")]
        [JsonProperty("recipients")]
        public List<User> Users { get; set; }
        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }

        private string NameFromUsers()
        {
            string cache = "";
            bool first = true;
            foreach (var user in Users)
            {
                if (first)
                {
                    cache += user.Username;
                    first = false;
                } else
                {
                    cache += ", " + user.Username;
                }
            }
            return cache;
        }

        public void UpdateLMID(string id)
        {
            LastMessageId = id;
        }
    }
}
