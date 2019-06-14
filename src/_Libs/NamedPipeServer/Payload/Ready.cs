using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordPipeImpersonator.Payload
{
    class Ready
    {
        public class Config
        {
            [JsonProperty("cdn_host")]
            public string CdnHost { get; set; }
            [JsonProperty("api_endpoint")]
            public string APIendpoint { get; set; }
            [JsonProperty("environment")]
            public string Environment { get; set; }
        }

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
        }

        public class Data
        {
            [JsonProperty("v")]
            public int Version { get; set; }
            [JsonProperty("config")]
            public Config Config { get; set; }
            [JsonProperty("user")]
            public User User { get; set; }
        }

    }
}
