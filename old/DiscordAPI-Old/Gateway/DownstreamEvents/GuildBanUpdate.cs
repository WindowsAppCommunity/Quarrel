using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Gateway.DownstreamEvents
{
    public class GuildBanUpdate
    {
        [JsonProperty("user")]
        public SharedModels.User User { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
