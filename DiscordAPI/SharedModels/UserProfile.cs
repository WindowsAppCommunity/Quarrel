using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct UserProfile
    {
        [JsonProperty("premium_since")]
        public DateTime? PremiumSince { get; set; }
        [JsonProperty("mutual_guilds")]
        public IEnumerable<MutualGuild> MutualGuilds { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("connected_accounts")]
        public IEnumerable<ConnectedAccount> ConnectedAccount { get; set; }
    }
}
