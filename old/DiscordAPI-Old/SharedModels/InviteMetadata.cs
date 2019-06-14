using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class InviteMetadata
    {
        [JsonProperty("inviter")]
        public User Inviter { get; set; }
        [JsonProperty("uses")]
        public int Uses { get; set; }
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }
        [JsonProperty("max_age")]
        public int MaxAge { get; set; }
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("revoked")]
        public bool Revoked { get; set; }
    }
}
