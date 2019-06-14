using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("bot")]
        public bool Bot { get; set; }
        [JsonProperty("mfa_enabled")]
        public bool MfaEnabled { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("flags")]
        public int Flags { get; set; }
        [JsonProperty("premium")]
        public bool Premium { get; set; }
    }
}
