using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class UserProfile
    {
        [JsonProperty("premium_since")]
        public DateTime? PremiumSince { get; set; }
        [JsonProperty("mutual_guilds")]
        public IEnumerable<MutualGuild> MutualGuilds { get; set; }
        public User user { get; set; }
        public IEnumerable<ConnectedAccount> connected_accounts { get; set; }
        public Friend Friend { get; set; }
    }
}
