using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Models
{
    public class DirectMessageChannel : Channel
    {
        [JsonProperty("is_private")]
        public bool Private { get; set; }

        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }

        [JsonProperty("recipients")]
        public List<User> Users { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
        public override string Name
        {
            get { return _name == null ? NameFromUsers() : _name; }
        }

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
            return string.IsNullOrEmpty(cache) ? "Unnamed" : cache;
        }
    }
}
